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

UserInfo* DatabaseManager::LoadData()
{
	static bool isLoad = false;

	IC_CS cs;

	if (isLoad == false)
	{
		// 쿼리문을 mysql에 전송한다.(리턴값이 0이아니면 오류)
		stat = mysql_query(connPtr, "SELECT * FROM tbl_userinfo");
		if (stat != 0)
			LogManager::GetInstance()->ErrorPrintf(mysql_error(&conn));
		//LogManager::GetInstance()->ErrQuitMsgBox(mysql_error(&conn));

		result = mysql_store_result(connPtr);

		isLoad = true;
	}

	row = mysql_fetch_row(result);

	if (row == nullptr)
	{
		mysql_free_result(result);	// 결과 메모리 반납

		return nullptr;
	}

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

void DatabaseManager::InsertData(UserInfo* _userInfo)
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