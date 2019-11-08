#include "stdafx.h"	// DB�Ŵ����� PCH�� ������ �ϴ� ���� �ָ���
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
	mysql_init(&conn);	// mysql ���� ���� �� �ʱ�ȭ

	// ���ӽõ�(���н� nullptr����)
	connPtr = mysql_real_connect(&conn,
		HOSTIP,
		USERID,
		PASSWORD,
		DBNAME,
		PORT,
		nullptr,
		0);

	// ���� ���ῡ �����ߴٸ� �������� + �޽����ڽ� ���� ������
	if (connPtr == nullptr)
		LogManager::GetInstance()->ErrQuitMsgBox(mysql_error(&conn));

	// ���� �����ߴٸ�
	else
		_tprintf(TEXT("[MySQL ����] ���� ����!\n"));
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
		// �ҷ��������� �Խ�Ʈ ������ ��� �����Ѵ� ������ ��ȸ���̴ϱ�
		QueryToMySQL("Delete FROM tbl_userinfo WHERE UserID Like 'Guest%'");

		// mysql�� ���̺� ������ ��� ��������� ��û�Ѵ�.
		if (QueryToMySQL("SELECT * FROM tbl_userinfo") == true)
		{
			isLoad = true;
			numOfUserInfoRows = (int)mysql_num_rows(result);	// ���� �� ���� ����
		}
	}

	row = mysql_fetch_row(result);

	if (row == nullptr)
	{
		mysql_free_result(result);	// ��� �޸� �ݳ�

		return nullptr;
	}

	// ���� row�� �����ϸ� user���� ����
	else
	{
		UserInfo info;
		memset(&info, 0, sizeof(UserInfo));

		// UTF8 -> Unicode�� ��ȯ �� ����
		UtilityManager::GetInstance()->UTF8ToUnicode(row[1], info.id);
		UtilityManager::GetInstance()->UTF8ToUnicode(row[2], info.pw);
		UtilityManager::GetInstance()->UTF8ToUnicode(row[3], info.nickname);

		// ���� �Ҵ� �� ����
		UserInfo* ptr = new UserInfo(info);

		return ptr;
	}
}

void DatabaseManager::InsertUserInfo(UserInfo* _userInfo)
{
	IC_CS cs;

	wchar_t wcharQuery[MSGSIZE] = { 0, };

	// 1��° ���� �⺻Ű(�ڵ�����) �̹Ƿ� �������� TCHAR�� ���� ����
	wsprintf(wcharQuery, TEXT("INSERT INTO tbl_userinfo(UserID, UserPW, UserNickName) VALUES "
		"('%s', '%s', '%s')"), _userInfo->id, _userInfo->pw, _userInfo->nickname);

	// utf-8�� ��ȯ ��Ŵ
	char utfQuery[MSGSIZE] = { 0, };
	UtilityManager::GetInstance()->UnicodeToUTF8(wcharQuery, utfQuery);

	// ���� ����
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
		// mysql�� ���̺� ������ ��� ��������� ��û�Ѵ�.
		if (QueryToMySQL("SELECT * FROM tbl_gameinfo") == true)
		{
			isLoad = true;
			numOfGameInfoRows = (int)mysql_num_rows(result);	// ���� �� ���� ����
		}
	}

	row = mysql_fetch_row(result);

	if (row == nullptr)
	{
		mysql_free_result(result);	// ��� �޸� �ݳ�

		return nullptr;
	}

	// ���� row�� �����ϸ� �������� ����
	else
	{
		GameInfo info;
		memset(&info, 0, sizeof(GameInfo));

		info.gameType         = atoi(row[0]);
		info.maxPlayer        = atoi(row[1]);
		info.maxSpeed         = (float)atof(row[2]);
		info.maxHealth        = (float)atof(row[3]);
		info.respawnTime      = (float)atoi(row[4]);
		info.subSprayingTime  = (float)atof(row[5]);
		info.mainSprayingTime = (float)atof(row[6]);
		info.gameTime         = atoi(row[7]);
		info.killPoint        = atoi(row[8]);
		info.capturePoint     = atoi(row[9]);

		// ���� �Ҵ� �� ����
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
		// mysql�� ���̺� ������ ��� ��������� ��û�Ѵ�.
		if (QueryToMySQL("SELECT * FROM tbl_weaponinfo") == true)
		{
			isLoad = true;
			numOfWeaponRows = (int)mysql_num_rows(result);	// ���� �� ���� ����
		}
	}

	row = mysql_fetch_row(result);

	if (row == nullptr)
	{
		mysql_free_result(result);	// ��� �޸� �ݳ�

		return nullptr;
	}

	// ���� row�� �����ϸ� �������� ����
	else
	{
		WeaponInfo info;
		memset(&info, 0, sizeof(WeaponInfo));

		info.num                = atoi(row[0]);
		info.numOfPattern       = atoi(row[1]);
		info.bulletPerShot      = atoi(row[2]);
		info.maxAmmo            = atoi(row[3]);
		info.fireRate           = (float)atof(row[4]);
		info.damage             = (float)atof(row[5]);
		info.accuracy           = (float)atof(row[6]);
		info.range              = (float)atof(row[7]);
		info.speed              = (float)atof(row[8]);
		info.reloadTime         = (float)atof(row[9]);
		UtilityManager::GetInstance()->UTF8ToUnicode(row[10], info.weaponName);

		// ���� �Ҵ� �� ����
		WeaponInfo* ptr = new WeaponInfo(info);

		return ptr;
	}
}

LocationInfo* DatabaseManager::LoadLocationInfo()
{
	static bool isLoad = false;

	IC_CS cs;

	if (isLoad == false)
	{
		// mysql�� ���̺� ������ ��� ��������� ��û�Ѵ�.
		if (QueryToMySQL("SELECT * FROM tbl_locationinfo") == true)
		{
			isLoad = true;
		}
	}

	row = mysql_fetch_row(result);

	if (row == nullptr)
	{
		mysql_free_result(result);	// ��� �޸� �ݳ�

		return nullptr;
	}

	// ���� row�� �����ϸ� ���������� ����
	else
	{
		LocationInfo info;
		memset(&info, 0, sizeof(LocationInfo));

		// ������ ����
		info.respawnInfo.gameType      = atoi(row[1]);
		info.respawnInfo.playerNum     = atoi(row[2]);
		info.respawnInfo.posX          = (float)atof(row[3]);
		info.respawnInfo.posZ          = (float)atof(row[4]);

		// �ʱ� ��ġ
		info.firstPosInfo.gameType      = atoi(row[1]);
		info.firstPosInfo.playerNum     = atoi(row[2]);
		info.firstPosInfo.posX          = (float)atof(row[5]);
		info.firstPosInfo.posZ          = (float)atof(row[6]);

		// ���� �Ҵ� �� ����
		LocationInfo* ptr = new LocationInfo(info);

		return ptr;
	}
}

