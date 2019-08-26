#include "stdafx.h"
#include "LobbyManager.h"
#include "ChatManager.h"
#include "C_ClientInfo.h"

ChatManager* ChatManager::instance;

ChatManager* ChatManager::GetInstance()
{
	if (instance == nullptr)
		instance = new ChatManager();

	return instance;
}
void ChatManager::Destroy()
{
	delete instance;
}

void ChatManager::Init()
{
}

void ChatManager::End()
{
}

void ChatManager::PackPacket(char* _setptr, TCHAR* _str1, int& _size)
{
	char* ptr = _setptr;
	int strsize1 = _tcslen(_str1) * sizeof(TCHAR);
	_size = 0;

	// ���ڿ� ����
	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	// ���ڿ�(�����ڵ�)
	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	_size = _size + strsize1;
}
void ChatManager::UnPackPacket(char* _getBuf, TCHAR* _str1)
{
	int str1size;
	char* ptr = _getBuf + sizeof(PROTOCOL_CHAT);

	memcpy(&str1size, ptr, sizeof(str1size));
	ptr = ptr + sizeof(str1size);

	// ���ڿ� 1 ����
	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;
}

void ChatManager::GetProtocol(PROTOCOL_CHAT& _protocol)
{
	// major state�� ������(Ŭ��� state�� �Ⱥ����ϱ�(Ȥ�ó� ���Ŀ� �����ԵǸ� �̺κ��� ����)) protocol�� �������� ���ؼ� ���� 10��Ʈ ��ġ�� ����ũ�� ����
	__int64 mask = ((__int64)PROTOCOL_OFFSET << (64 - PROTOCOL_MASK));

	// ����ũ�� �ɷ��� 1���� ���������� ����ȴ�. 
	PROTOCOL_CHAT protocol = (PROTOCOL_CHAT)(_protocol & (PROTOCOL_CHAT)mask);

	// ���߿� �ѹ��� �������ִ� ������ ���߿� �߰��� ���� �� �ִ� result �� ���ؼ� protocol �� ������� ���� 
	_protocol = protocol;
}
ChatManager::PROTOCOL_CHAT ChatManager::SetProtocol(STATE_PROTOCOL _state, PROTOCOL_CHAT _protocol, RESULT_CHAT _result)
{
	// �ϼ��� ���������� ���� 
	PROTOCOL_CHAT protocol = (PROTOCOL_CHAT)0;
	protocol = (PROTOCOL_CHAT)(_state | _protocol | _result);
	return protocol;
}

ChatManager::PROTOCOL_CHAT ChatManager::GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf)
{
	__int64 bitProtocol = 0;
	_ptr->GetPacket(bitProtocol, _buf);	// �켱 �ɷ��������� ���������� �����´�.

	// ��¥ ���������� ������ �ش�.(�ȿ��� �������� AND �˻�)
	PROTOCOL_CHAT realProtocol = (PROTOCOL_CHAT)bitProtocol;
	GetProtocol(realProtocol);

	return realProtocol;
}

bool ChatManager::LeaveRoomProcess(C_ClientInfo* _ptr, char* _buf)
{
	RESULT_CHAT leaveResult = RESULT_CHAT::NODATA;
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_CHAT protocol;

	char buf[BUFSIZE];		// ����
	int packetSize = 0;     // �� ������

	//// �κ�� ���ư� �Ϳ� �����ϸ� result ����
	//if (LobbyManager::GetInstance()->CheckLeaveRoom(_ptr) == true)
	//{
	//	leaveResult = RESULT_CHAT::LEAVE_ROOM_SUCCESS;
	//	_tcscpy_s(msg, MSGSIZE, GOTO_LOBBY_SUCCESS_MSG);

	//	_ptr->SetRoomNum(-1);	// �Ҽӵ� ���� ���� ����
	//}

	//else
	//{
	//	leaveResult = RESULT_CHAT::LEAVE_ROOM_FAIL;
	//	_tcscpy_s(msg, MSGSIZE, GOTO_LOBBY_SUCCESS_MSG);
	//}

	// �������� ���� 
	protocol = SetProtocol(CHAT_STATE, PROTOCOL_CHAT::LEAVE_ROOM_PROTOCOL, leaveResult);
	ZeroMemory(buf, sizeof(BUFSIZE));
	// ��ŷ �� ����
	PackPacket(buf, msg, packetSize);
	_ptr->SendPacket(protocol, buf, packetSize);

	if (leaveResult == RESULT_CHAT::LEAVE_ROOM_SUCCESS)
		return true;

	return false;
}

bool ChatManager::CanILeaveRoom(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_CHAT protocol = GetBufferAndProtocol(_ptr, buf);

	if (protocol == LEAVE_ROOM_PROTOCOL)
		return LeaveRoomProcess(_ptr, buf);

	return false;
}

bool ChatManager::CheckChattingMessage(C_ClientInfo* _ptr)
{
	int packetSize = 0;
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_CHAT protocol = GetBufferAndProtocol(_ptr, buf);	// �������� ���

	// ä�� ������ ��� [id:����] �������� ���ý����ش�.
	TCHAR chatMsg[MSGSIZE] = { 0, };
	TCHAR compleChatMsg[IDSIZE + MSGSIZE] = { 0, };
	UnPackPacket(buf, chatMsg);
	wsprintf(compleChatMsg, TEXT("%s:%s"), _ptr->GetUserInfo()->id, chatMsg);

	if (protocol == CHAT_PROTOCOL)
	{
		//while (1)
		//{
		//	// �濡�ִ� Ŭ���� ������ �ϳ�������
		//	C_ClientInfo* ptr = LobbyManager::GetInstance()->GetRoomClient(_ptr->GetRoomNum());

		//	// �ڽſ��Դ� �Ⱥ�����.
		//	if (ptr == _ptr)
		//		continue;

		//	// Ŭ�� ������ ������ ����������.
		//	if (ptr == nullptr)
		//		break;



		//	// �� �ܿ� ��쿡�� ä�� �޽����� �����ش�.

		//	// �������� ���� 
		//	protocol = SetProtocol(CHAT_STATE, PROTOCOL_CHAT::CHAT_PROTOCOL, (RESULT_CHAT)0);
		//	ZeroMemory(buf, sizeof(BUFSIZE));
		//	// ��ŷ �� ����
		//	PackPacket(buf, compleChatMsg, packetSize);
		//	ptr->SendPacket(protocol, buf, packetSize);
		//}

		return true;
	}
	return false;
}