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


struct RoomInfo
{
	static int incRoomNum;

	C_List<C_ClientInfo*>* userList;
	int roomNum;
	TCHAR roomTitle[ROOM_TITLE_LEN];

	// ������
	RoomInfo(C_ClientInfo* _user, TCHAR *_roomTitle)
	{
		userList = new C_List<C_ClientInfo*>();
		userList->Insert(_user);
		roomNum = ++incRoomNum;
		_tcscpy_s(roomTitle, ROOM_TITLE_LEN, _roomTitle);
	}

	~RoomInfo()
	{
		delete userList;
	}
};

class LobbyManager
{
	enum PROTOCOL_LOBBY : __int64
	{
		ENTER_ROOM_PROTOCOL = ((__int64)0x1 << 58),
		CREATE_ROOM_PROTOCOL = ((__int64)0x1 << 57),
		LOGOUT_PROTOCOL = ((__int64)0x1 << 56),
		ROOMLIST_PROTOCOL = ((__int64)0x1 << 55),
	};

	enum RESULT_LOBBY : __int64
	{
		ENTER_ROOM_SUCCESS = ((__int64)0x1 << 53),
		ENTER_ROOM_FAIL = ((__int64)0x1 << 52),

		CREATE_ROOM_SUCCESS = ((__int64)0x1 << 53),
		CREATE_ROOM_FAIL = ((__int64)0x1 << 52),

		NODATA = ((__int64)0x1 << 49)
	};


private:
	LobbyManager() {}
	~LobbyManager() {}
	static LobbyManager* instance;

	C_List<RoomInfo*>* roomList;

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

private:
	bool EnterRoomProcess(C_ClientInfo* _ptr, char* _buf);			// �濡 ���� ó��
	RoomInfo* CheckEnterRoom(int _roomNum, RESULT_LOBBY& _result);	// �濡 ���� �ִ��� �˻�

	bool CreateRoomProcess(C_ClientInfo* _ptr, char* _buf);			// ���� �����ϴ� ó��

	bool RoomListProcess(C_ClientInfo* _ptr, char* _buf);			// �� ����Ʈ�� �� Ŭ�󿡰� �����ִ� ó��
	bool GetRoomList(TCHAR* _msg);									// �� ����Ʈ�� ��� _msg�� ����

public:
	bool CanIEnterRoom(C_ClientInfo* _ptr);		// �濡 ���� �����Ѱ�
	bool CanICreateRoom(C_ClientInfo* _ptr);	// ���� ������ �� �ִ°�
	bool CanIGetRoomList(C_ClientInfo* _ptr);	// �� ����� ���� �� �ִ°�
	bool CanILeaveLobby(C_ClientInfo* _ptr);	// �κ�� �� �� �ִ°�
	bool CheckLeaveRoom(C_ClientInfo* _ptr);	// ���� ���� �� �ִ°�(CHAT_STATE���� ȣ���ϰ� ��)

	C_ClientInfo* GetRoomClient(int _roomNum);	// _roomNum ��ȣ�� �ش��ϴ� �濡 �ִ� �������� ������ ����

	//POSITION RecvProcess(C_ClientInfo* _ptr);
	//bool SendProcess(C_ClientInfo* _ptr);
};