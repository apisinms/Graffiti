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

	// 문자열 길이
	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	// 문자열(유니코드)
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

	// 문자열 1 받음
	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;
}

void ChatManager::GetProtocol(PROTOCOL_CHAT& _protocol)
{
	// major state를 제외한(클라는 state를 안보내니까(혹시나 추후에 보내게되면 이부분을 수정)) protocol을 가져오기 위해서 상위 10비트 위치에 마스크를 만듦
	__int64 mask = ((__int64)PROTOCOL_OFFSET << (64 - PROTOCOL_MASK));

	// 마스크에 걸러진 1개의 프로토콜이 저장된다. 
	PROTOCOL_CHAT protocol = (PROTOCOL_CHAT)(_protocol & (PROTOCOL_CHAT)mask);

	// 나중에 한번더 저장해주는 이유는 나중에 추가로 받을 수 있는 result 에 대해서 protocol 을 살려놓기 위해 
	_protocol = protocol;
}
ChatManager::PROTOCOL_CHAT ChatManager::SetProtocol(STATE_PROTOCOL _state, PROTOCOL_CHAT _protocol, RESULT_CHAT _result)
{
	// 완성된 프로토콜을 리턴 
	PROTOCOL_CHAT protocol = (PROTOCOL_CHAT)0;
	protocol = (PROTOCOL_CHAT)(_state | _protocol | _result);
	return protocol;
}

ChatManager::PROTOCOL_CHAT ChatManager::GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf)
{
	__int64 bitProtocol = 0;
	_ptr->GetPacket(bitProtocol, _buf);	// 우선 걸러지지않은 프로토콜을 가져온다.

	// 진짜 프로토콜을 가져와 준다.(안에서 프로토콜 AND 검사)
	PROTOCOL_CHAT realProtocol = (PROTOCOL_CHAT)bitProtocol;
	GetProtocol(realProtocol);

	return realProtocol;
}

bool ChatManager::LeaveRoomProcess(C_ClientInfo* _ptr, char* _buf)
{
	RESULT_CHAT leaveResult = RESULT_CHAT::NODATA;
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_CHAT protocol;

	char buf[BUFSIZE];		// 버퍼
	int packetSize = 0;     // 총 사이즈

	//// 로비로 돌아간 것에 성공하면 result 변경
	//if (LobbyManager::GetInstance()->CheckLeaveRoom(_ptr) == true)
	//{
	//	leaveResult = RESULT_CHAT::LEAVE_ROOM_SUCCESS;
	//	_tcscpy_s(msg, MSGSIZE, GOTO_LOBBY_SUCCESS_MSG);

	//	_ptr->SetRoomNum(-1);	// 소속된 방이 이제 없음
	//}

	//else
	//{
	//	leaveResult = RESULT_CHAT::LEAVE_ROOM_FAIL;
	//	_tcscpy_s(msg, MSGSIZE, GOTO_LOBBY_SUCCESS_MSG);
	//}

	// 프로토콜 세팅 
	protocol = SetProtocol(CHAT_STATE, PROTOCOL_CHAT::LEAVE_ROOM_PROTOCOL, leaveResult);
	ZeroMemory(buf, sizeof(BUFSIZE));
	// 패킹 및 전송
	PackPacket(buf, msg, packetSize);
	_ptr->SendPacket(protocol, buf, packetSize);

	if (leaveResult == RESULT_CHAT::LEAVE_ROOM_SUCCESS)
		return true;

	return false;
}

bool ChatManager::CanILeaveRoom(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_CHAT protocol = GetBufferAndProtocol(_ptr, buf);

	if (protocol == LEAVE_ROOM_PROTOCOL)
		return LeaveRoomProcess(_ptr, buf);

	return false;
}

bool ChatManager::CheckChattingMessage(C_ClientInfo* _ptr)
{
	int packetSize = 0;
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_CHAT protocol = GetBufferAndProtocol(_ptr, buf);	// 프로토콜 얻고

	// 채팅 내용을 얻고 [id:내용] 형식으로 셋팅시켜준다.
	TCHAR chatMsg[MSGSIZE] = { 0, };
	TCHAR compleChatMsg[IDSIZE + MSGSIZE] = { 0, };
	UnPackPacket(buf, chatMsg);
	wsprintf(compleChatMsg, TEXT("%s:%s"), _ptr->GetUserInfo()->id, chatMsg);

	if (protocol == CHAT_PROTOCOL)
	{
		//while (1)
		//{
		//	// 방에있는 클라의 정보를 하나씩얻어옴
		//	C_ClientInfo* ptr = LobbyManager::GetInstance()->GetRoomClient(_ptr->GetRoomNum());

		//	// 자신에게는 안보낸다.
		//	if (ptr == _ptr)
		//		continue;

		//	// 클라 정보가 없으면 빠져나간다.
		//	if (ptr == nullptr)
		//		break;



		//	// 그 외에 경우에는 채팅 메시지를 보내준다.

		//	// 프로토콜 세팅 
		//	protocol = SetProtocol(CHAT_STATE, PROTOCOL_CHAT::CHAT_PROTOCOL, (RESULT_CHAT)0);
		//	ZeroMemory(buf, sizeof(BUFSIZE));
		//	// 패킹 및 전송
		//	PackPacket(buf, compleChatMsg, packetSize);
		//	ptr->SendPacket(protocol, buf, packetSize);
		//}

		return true;
	}
	return false;
}