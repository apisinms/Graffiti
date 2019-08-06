#pragma once
class C_ClientInfo;

#define GOTO_LOBBY_SUCCESS_MSG		TEXT("�κ� �̵��� �����Ͽ����ϴ�.\n")
#define GOTO_LOBBY_FAIL_MSG			TEXT("�κ� �̵��� �����Ͽ����ϴ�.\n")

class ChatManager
{

	enum PROTOCOL_CHAT : __int64
	{
		LEAVE_ROOM_PROTOCOL = ((__int64)0x1 << 58),
		CHAT_PROTOCOL = ((__int64)0x1 << 57),
	};

	enum RESULT_CHAT : __int64
	{
		LEAVE_ROOM_SUCCESS = ((__int64)0x1 << 53),
		LEAVE_ROOM_FAIL = ((__int64)0x1 << 52),

		NODATA = ((__int64)0x1 << 49)
	};


private:
	ChatManager() {}
	~ChatManager() {}
	static ChatManager* instance;
public:
	static ChatManager* GetInstance();
	static void Destroy();

private:
	PROTOCOL_CHAT GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);
	void PackPacket(char* _setptr, TCHAR* _str1, int& _size);
	void UnPackPacket(char* _getBuf, TCHAR* _str1);
	void GetProtocol(PROTOCOL_CHAT& _protocol);
	PROTOCOL_CHAT SetProtocol(STATE_PROTOCOL _state, PROTOCOL_CHAT _protocol, RESULT_CHAT _result);

private:
	bool LeaveRoomProcess(C_ClientInfo* _ptr, char* _buf);

public:
	void Init();
	void End();

	bool CanILeaveRoom(C_ClientInfo* _ptr);			// ���� ���� �� �ִ���
	bool CheckChattingMessage(C_ClientInfo* _ptr);	// ä�� �޽����� üũ�ؼ� �ٸ� Ŭ��鿡�� �����Ѵ�.

	//POSITION RecvProcess(C_ClientInfo* _ptr);
	//bool SendProcess(C_ClientInfo* _ptr);
};