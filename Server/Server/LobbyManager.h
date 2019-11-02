#pragma once
#include "C_Global.h"

//#define ENTER_ROOM_SUCCESS_MSG		TEXT("�� ���忡 �����Ͽ����ϴ�.\n")
//#define ENTER_ROOM_FAIL_MSG			TEXT("�� ���忡 �����Ͽ����ϴ�.\n")
//#define CREATE_ROOM_SUCCESS_MSG		TEXT("���� �����Ͽ����ϴ�.\n")
//#define CREATE_ROOM_FAIL_MSG		TEXT("���̻� ���� ������ �� �����ϴ�.\n")
//#define ROOMLIST_NOEXIST_MSG		TEXT("������ ���� �����ϴ�.\n")
//
//#define MAX_ROOM_SIZE	200
//#define ROOM_TITLE_LEN	255
//#define ROOM_NUM_LEN	20

class C_ClientInfo;

class LobbyManager
{
	// 53~34
	enum PROTOCOL_LOBBY : __int64
	{
		MATCH_PROTOCOL        = ((__int64)0x1 << 53),
		MATCH_CANCEL_PROTOCOL = ((__int64)0x1 << 52),
		GOTO_INGAME_PROTOCOL  = ((__int64)0x1 << 51),      // �ΰ��� ���·� ���� ��������
		LOGOUT_PROTOCOL       = ((__int64)0x1 << 50),

		//LOGOUT_PROTOCOL = ((__int64)0x1 << 56),			// LOGIN �Ŵ������� ���Ǳ� ������
	};

	// 33~24
	enum RESULT_LOBBY : __int64
	{
		LOBBY_SUCCESS = ((__int64)0x1 << 33),		 // �κ񿡼� ���� ó��
		LOBBY_FAIL    = ((__int64)0x1 << 32),        // �κ񿡼� ���� ó��

		NODATA = ((__int64)0x1 << 10)
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
	void PackPacket(char* _setptr, int _num, int& _size);		// ���� 1���� Pack�ϴ� �Լ�
	void PackPacket(char* _setptr, TCHAR* _str1, int& _size);	// ���ڿ� 1���� Pack�ϴ� �Լ�
	void UnPackPacket(char* _getBuf, TCHAR* _str1);				// ���ڿ� 1���� UnPack�ϴ� �Լ�
	void UnPackPacket(char* _getBuf, int& _gameType);

	void GetProtocol(PROTOCOL_LOBBY& _protocol);								// ���������� ����
	PROTOCOL_LOBBY SetProtocol(STATE_PROTOCOL _state, PROTOCOL_LOBBY _protocol, RESULT_LOBBY _result);	// �������� + result(�ִٸ�)�� ������

	PROTOCOL_LOBBY GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// buf�� Protocol�� ���ÿ� ��� �Լ�
	void SendPacket_Room(C_ClientInfo* _ptr, char* buf, PROTOCOL_LOBBY protocol);

public:
	bool CanIMatch(C_ClientInfo* _ptr);			// ��Ī�� �� �� �ִ°�
	bool CanICancelMatch(C_ClientInfo* _ptr);	// ��Ī�� ��� �� �� �ִ°�
	bool CanILeaveLobby(C_ClientInfo* _ptr);	// �α׾ƿ� �� �� �ִ°�
	bool CanIGotoInGame(C_ClientInfo* _ptr);	// �ΰ��� ���·� �� �� �ִ°�
};