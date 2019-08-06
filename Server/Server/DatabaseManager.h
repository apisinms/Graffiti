#pragma once
#include "C_SyncCS.h"
#include <mysql.h>
struct UserInfo;
class C_ClientInfo;
template <typename T>
class C_List;

class DatabaseManager : public C_SyncCS<DatabaseManager> 
{
private:
	const char* USERID = "root";
	const char* PASSWORD = "1234";
	const char* HOSTIP = "localhost";
	const char* DBNAME = "db_userinfo";
	const unsigned int PORT = 3306;

	MYSQL conn;
	MYSQL* connPtr;
	MYSQL_RES* result;
	MYSQL_ROW row;
	int stat;

private:
	DatabaseManager() {}
	~DatabaseManager() {}
	static DatabaseManager* instance;

public:
	static DatabaseManager* GetInstance();
	static void Destroy();

public:
	void Init();
	void End();
	UserInfo* LoadData();	// �Ѿ�� list�� �����͸� load�ؼ� �����ϴ� �Լ�
	void InsertData(UserInfo* _userInfo);	// id, pw, nickname�� ���������� 
};