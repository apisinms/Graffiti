#pragma once
#include "C_List.h"
#include "C_Global.h"
#include <tchar.h>

#define MAX_ROOM_SIZE	200
#define ROOM_TITLE_LEN	255
#define ROOM_NUM_LEN	20

class C_ClientInfo;

class InGameManager
{
	enum PROTOCOL_INGAME : __int64
	{
		ITEMSELECT_PROTOCOL = ((__int64)0x1 << 58),
	};

	enum RESULT_INGAME : __int64
	{
		INGAME_SUCCESS = ((__int64)0x1 << 53),
		INGAME_FAIL = ((__int64)0x1 << 52),

		NODATA = ((__int64)0x1 << 49)
	};
	struct Weapon
	{
		char mainW;
		char subW;
	}weapon;

private:
	InGameManager() {}
	~InGameManager() {}
	static InGameManager* instance;

public:
	void Init();
	void End();
	static InGameManager* GetInstance();
	static void Destroy();

private:
	void PackPacket(char* _setptr, TCHAR* _str1, int& _size);	// 문자열 1개를 Pack하는 함수
	void UnPackPacket(char* _getBuf, Weapon& _struct);
	void UnPackPacket(char* _getBuf, int& _num1, int& _num2);				// 문자열 1개를 UnPack하는 함수

	void GetProtocol(PROTOCOL_INGAME& _protocol);								// 프로토콜을 얻음
	PROTOCOL_INGAME SetProtocol(STATE_PROTOCOL _state, PROTOCOL_INGAME _protocol, RESULT_INGAME _result);	// 프로토콜 + result(있다면)을 설정함

	PROTOCOL_INGAME GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf와 Protocol을 동시에 얻는 함수
	bool ItemSelctProcess(C_ClientInfo* _ptr, char* _buf);
public:
	bool CanIItemSelect(C_ClientInfo* _ptr);	// 아이템 선택
};