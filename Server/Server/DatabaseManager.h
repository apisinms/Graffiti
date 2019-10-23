#pragma once
#include "C_SyncCS.h"
#include <mysql.h>

//struct UserInfo;
//struct WeaponInfo;
//struct GameInfo;

class DatabaseManager : public C_SyncCS<DatabaseManager> 
{
private:
	const char* USERID = "root";
	const char* PASSWORD = "1234";
	const char* HOSTIP = "localhost";
	const char* DBNAME = "db_graffiti";
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

	bool QueryToMySQL(const char* _query);	// �Ķ���ͷ� �Ѿ�� ���������� mysql�� �����ϰ�, ����� result�� �����Ŵ 

public:
	static DatabaseManager* GetInstance();
	static void Destroy();

private:
	int numOfUserInfoRows;	// �������� �� ��
	int numOfGameInfoRows;	// �������� �� ��
	int numOfWeaponRows;	// �������� �� ��

public:
	int GetNumOfUserInfoRows() { return numOfUserInfoRows; }
	int GetNumOfGameRows() { return numOfGameInfoRows; }
	int GetNumOfWeaponRows() { return numOfWeaponRows; }

public:
	void Init();
	void End();
	
	UserInfo* LoadUserInfo();					// �Ѿ�� list�� �����͸� load�ؼ� �����ϴ� �Լ�
	void InsertUserInfo(UserInfo* _userInfo);	// id, pw, nickname�� ���������� 

	 WeaponInfo* LoadWeaponInfo();
	 GameInfo* LoadGameInfo();
	 RespawnInfo* LoadRespawnInfo();
};