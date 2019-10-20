#include "stdafx.h"	// DB매니저를 PCH에 넣을지 일단 아직 애매함
#include "DatabaseManager.h"

DatabaseManager* DatabaseManager::instance;

DatabaseManager* DatabaseManager::GetInstance()
{
	if (instance == nullptr)
		instance = new DatabaseManager();

	return instance;
}

void DatabaseManager::Destroy()
{
	delete instance;
}

void DatabaseManager::Init()
{
	mysql_init(&conn);	// mysql 서버 접속 전 초기화

	// 접속시도(실패시 nullptr리턴)
	connPtr = mysql_real_connect(&conn,
		HOSTIP,
		USERID,
		PASSWORD,
		DBNAME,
		PORT,
		nullptr,
		0);

	// 서버 연결에 실패했다면 에러내용 + 메시지박스 띄우고 종료함
	if (connPtr == nullptr)
		LogManager::GetInstance()->ErrQuitMsgBox(mysql_error(&conn));

	// 연결 성공했다면
	else
		_tprintf(TEXT("[MySQL 서버] 접속 성공!\n"));
}

void DatabaseManager::End()
{
	mysql_close(instance->connPtr);
}

bool DatabaseManager::QueryToMySQL(const char* _query)
{
	stat = mysql_query(connPtr, _query);
	if (stat != 0)
	{
		LogManager::GetInstance()->ErrorPrintf(mysql_error(&conn));
		//LogManager::GetInstance()->ErrQuitMsgBox(mysql_error(&conn));

		return false;
	}

	result = mysql_store_result(connPtr);

	return true;
}

UserInfo* DatabaseManager::LoadUserInfo()
{
	static bool isLoad = false;

	IC_CS cs;

	if (isLoad == false)
	{
		// mysql에 테이블 정보를 모두 가져오라고 요청한다.
		if (QueryToMySQL("SELECT * FROM tbl_userinfo") == true)
		{
			isLoad = true;
			numOfUserInfoRows = (int)mysql_num_rows(result);	// 유저 행 정보 얻음
		}
	}

	row = mysql_fetch_row(result);

	if (row == nullptr)
	{
		mysql_free_result(result);	// 결과 메모리 반납

		return nullptr;
	}

	// 아직 row가 존재하면 user정보 셋팅
	else
	{
		UserInfo info;
		memset(&info, 0, sizeof(UserInfo));

		// UTF8 -> Unicode로 변환 후 저장
		UtilityManager::GetInstance()->UTF8ToUnicode(row[1], info.id);
		UtilityManager::GetInstance()->UTF8ToUnicode(row[2], info.pw);
		UtilityManager::GetInstance()->UTF8ToUnicode(row[3], info.nickname);

		// 동적 할당 후 리턴
		UserInfo* ptr = new UserInfo(info);

		return ptr;
	}
}

void DatabaseManager::InsertUserInfo(UserInfo* _userInfo)
{
	IC_CS cs;

	wchar_t wcharQuery[MSGSIZE] = { 0, };

	// 1번째 값은 기본키(자동증가) 이므로 생략한후 TCHAR로 쿼리 만듦
	wsprintf(wcharQuery, TEXT("INSERT INTO tbl_userinfo(UserID, UserPW, UserNickName) VALUES "
		"('%s', '%s', '%s')"), _userInfo->id, _userInfo->pw, _userInfo->nickname);

	// utf-8로 변환 시킴
	char utfQuery[MSGSIZE] = { 0, };
	UtilityManager::GetInstance()->UnicodeToUTF8(wcharQuery, utfQuery);

	// 쿼리 실행
	stat = mysql_query(connPtr, utfQuery);
	if (stat != 0)
		LogManager::GetInstance()->ErrorPrintf(mysql_error(&conn));
		//LogManager::GetInstance()->ErrQuitMsgBox(mysql_error(&conn));
}

GameInfo* DatabaseManager::LoadGameInfo()
{
	static bool isLoad = false;

	IC_CS cs;

	if (isLoad == false)
	{
		// mysql에 테이블 정보를 모두 가져오라고 요청한다.
		if (QueryToMySQL("SELECT * FROM tbl_gameinfo") == true)
		{
			isLoad = true;
			numOfGameInfoRows = (int)mysql_num_rows(result);	// 게임 행 정보 얻음
		}
	}

	row = mysql_fetch_row(result);

	if (row == nullptr)
	{
		mysql_free_result(result);	// 결과 메모리 반납

		return nullptr;
	}

	// 아직 row가 존재하면 무기정보 셋팅
	else
	{
		GameInfo info;
		memset(&info, 0, sizeof(GameInfo));

		info.gameType = atoi(row[0]);
		info.maxSpeed = (float)atof(row[1]);
		info.maxHealth = (float)atof(row[2]);
		info.responTime = atoi(row[3]);
		info.gameTime = atoi(row[4]);

		/*printf("[게임정보] %d\n", info.gameType);
		printf("최대속도:%f\n", info.gameType);
		printf("최대체력:%f\n", info.maxHealth);
		printf("리스폰 :%d\n", info.responTime);
		printf("주어진 게임시간:%d\n\n", info.gameTime);*/

		// 동적 할당 후 리턴
		GameInfo* ptr = new GameInfo(info);

		return ptr;
	}
}

WeaponInfo* DatabaseManager::LoadWeaponInfo()
{
	static bool isLoad = false;

	IC_CS cs;

	if (isLoad == false)
	{
		// mysql에 테이블 정보를 모두 가져오라고 요청한다.
		if (QueryToMySQL("SELECT * FROM tbl_weaponinfo") == true)
		{
			isLoad = true;
			numOfWeaponRows = (int)mysql_num_rows(result);	// 무기 행 정보 얻음
			printf("무기정보 행 갯수 : %d\n", numOfWeaponRows);
		}
	}

	row = mysql_fetch_row(result);

	if (row == nullptr)
	{
		mysql_free_result(result);	// 결과 메모리 반납

		return nullptr;
	}

	// 아직 row가 존재하면 무기정보 셋팅
	else
	{
		WeaponInfo info;
		memset(&info, 0, sizeof(WeaponInfo));

		info.num           = atoi(row[0]);
		info.numOfPattern  = atoi(row[1]);
		info.bulletPerShot = atoi(row[2]);
		info.maxAmmo       = atoi(row[3]);
		info.fireRate      = (float)atof(row[4]);
		info.damage        = (float)atof(row[5]);
		info.accuracy      = (float)atof(row[6]);
		info.range         = (float)atof(row[7]);
		info.speed         = (float)atof(row[8]);
		UtilityManager::GetInstance()->UTF8ToUnicode(row[9], info.weaponName);

		/*printf("[무기정보] %ls\n", info.weaponName);
		printf("무기번호:%d\n", info.num);
		printf("패턴갯수:%d\n", info.numOfPattern);
		printf("최대총알:%d\n", info.maxAmmo);
		printf("발사주기:%f\n", info.fireRate);
		printf("데미지 :%f\n", info.damage);
		printf("정확도 :%f\n", info.accuracy);
		printf("사정거리:%f\n", info.range);
		printf("탄속    :%f\n\n", info.speed);*/

		// 동적 할당 후 리턴
		WeaponInfo* ptr = new WeaponInfo(info);

		return ptr;
	}
}

