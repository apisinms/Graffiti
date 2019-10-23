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

	bool QueryToMySQL(const char* _query);	// 파라미터로 넘어온 쿼리문으로 mysql에 질의하고, 결과를 result에 저장시킴 

public:
	static DatabaseManager* GetInstance();
	static void Destroy();

private:
	int numOfUserInfoRows;	// 유저정보 행 수
	int numOfGameInfoRows;	// 게임정보 행 수
	int numOfWeaponRows;	// 무기정보 행 수

public:
	int GetNumOfUserInfoRows() { return numOfUserInfoRows; }
	int GetNumOfGameRows() { return numOfGameInfoRows; }
	int GetNumOfWeaponRows() { return numOfWeaponRows; }

public:
	void Init();
	void End();
	
	UserInfo* LoadUserInfo();					// 넘어온 list에 데이터를 load해서 저장하는 함수
	void InsertUserInfo(UserInfo* _userInfo);	// id, pw, nickname의 유저정보로 

	 WeaponInfo* LoadWeaponInfo();
	 GameInfo* LoadGameInfo();
	 RespawnInfo* LoadRespawnInfo();
};