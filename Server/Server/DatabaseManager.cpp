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

UserInfo* DatabaseManager::LoadData()
{
	static bool isLoad = false;

	IC_CS cs;

	if (isLoad == false)
	{
		// �������� mysql�� �����Ѵ�.(���ϰ��� 0�̾ƴϸ� ����)
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
		mysql_free_result(result);	// ��� �޸� �ݳ�

		return nullptr;
	}

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

void DatabaseManager::InsertData(UserInfo* _userInfo)
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