#pragma once
#include "C_List.h"
#include "C_Global.h"
#include <tchar.h>

#define ENTER_ROOM_SUCCESS_MSG		TEXT("�� ���忡 �����Ͽ����ϴ�.\n")
#define ENTER_ROOM_FAIL_MSG			TEXT("�� ���忡 �����Ͽ����ϴ�.\n")
#define CREATE_ROOM_SUCCESS_MSG		TEXT("���� �����Ͽ����ϴ�.\n")
#define CREATE_ROOM_FAIL_MSG		TEXT("���̻� ���� ������ �� �����ϴ�.\n")
#define ROOMLIST_NOEXIST_MSG		TEXT("������ ���� �����ϴ�.\n")

#define MAX_ROOM_SIZE	200
#define ROOM_TITLE_LEN	255
#define ROOM_NUM_LEN	20

class C_ClientInfo;

class LobbyManager
{
	enum PROTOCOL_LOBBY : __int64
	{
		MATCH_PROTOCOL = ((__int64)0x1 << 58),		// ��Ī ��������
		START_PROTOCOL = ((__int64)0x1 << 57),		// ���ӽ��� ��������

		LOGOUT_PROTOCOL = ((__int64)0x1 << 56),			// LOGIN �Ŵ������� ���Ǳ� ������
	};

	enum RESULT_LOBBY : __int64
	{
		MATCH_SUCCESS = ((__int64)0x1 << 53),
		MATCH_FAIL = ((__int64)0x1 << 52),

		NODATA = ((__int64)0x1 << 49)
	};


private:
	LobbyManager() {}
	~LobbyManager() {}
	static LobbyManager* instance;

public:
	void Init();
	void End();
	static LobbyManager* GetInstance();
	static void Destroy();

private:
	void PackPacket(char* _setptr, TCHAR* _str1, int& _size);	// ���ڿ� 1���� Pack�ϴ� �Լ�
	void UnPackPacket(char* _getBuf, TCHAR* _str1);				// ���ڿ� 1���� UnPack�ϴ� �Լ�

	void GetProtocol(PROTOCOL_LOBBY& _protocol);								// ���������� ����
	PROTOCOL_LOBBY SetProtocol(STATE_PROTOCOL _state, PROTOCOL_LOBBY _protocol, RESULT_LOBBY _result);	// �������� + result(�ִٸ�)�� ������

	PROTOCOL_LOBBY GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf�� Protocol�� ���ÿ� ��� �Լ�

public:
	bool CanIMatch(C_ClientInfo* _ptr);			// ��Ī�� �� �� �ִ°�
	bool CanILeaveLobby(C_ClientInfo* _ptr);	// �α׾ƿ� �� �� �ִ°�
	bool CanIStart(C_ClientInfo* _ptr);			// ���� �� �� �ִ°�
};