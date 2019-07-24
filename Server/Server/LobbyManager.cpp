#include "LobbyManager.h"
#include "LoginManager.h"
#include "LogManager.h"
#include "MatchManager.h"
#include "C_ClientInfo.h"


int RoomInfo::incRoomNum = 0;

LobbyManager* LobbyManager::instance;

LobbyManager* LobbyManager::GetInstance()
{
	if (instance == nullptr)
		instance = new LobbyManager();

	return instance;
}
void LobbyManager::Destroy()
{
	delete instance;
}

void LobbyManager::Init()
{
	roomList = new C_List<RoomInfo*>();
}

void LobbyManager::End()
{
	delete roomList;
}

void LobbyManager::PackPacket(char* _setptr, TCHAR* _str1, int& _size)
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
void LobbyManager::UnPackPacket(char* _getBuf, TCHAR* _str1)
{
	int str1size;
	char* ptr = _getBuf + sizeof(PROTOCOL_LOBBY);

	memcpy(&str1size, ptr, sizeof(str1size));
	ptr = ptr + sizeof(str1size);

	// 문자열 1 받음
	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;
}

void LobbyManager::GetProtocol(PROTOCOL_LOBBY& _protocol)
{
	// major state를 제외한(클라는 state를 안보내니까(혹시나 추후에 보내게되면 이부분을 수정)) protocol을 가져오기 위해서 상위 10비트 위치에 마스크를 만듦
	__int64 mask = ((__int64)0x1f << (64 - 10));

	// 마스크에 걸러진 1개의 프로토콜이 저장된다. 
	PROTOCOL_LOBBY protocol = (PROTOCOL_LOBBY)(_protocol & (PROTOCOL_LOBBY)mask);

	// 아웃풋용 인자이므로 저장해준다.
	// 나중에 한번더 저장해주는 이유는 나중에 추가로 받을 수 있는 result 에 대해서 protocol 을 살려놓기 위해 
	_protocol = protocol;
}
LobbyManager::PROTOCOL_LOBBY LobbyManager::SetProtocol(STATE_PROTOCOL _state, PROTOCOL_LOBBY _protocol, RESULT_LOBBY _result)
{
	// 완성된 프로토콜을 리턴 
	PROTOCOL_LOBBY protocol = (PROTOCOL_LOBBY)0;
	protocol = (PROTOCOL_LOBBY)(_state | _protocol | _result);
	return protocol;
}

LobbyManager::PROTOCOL_LOBBY LobbyManager::GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf)
{
	__int64 bitProtocol;
	_ptr->GetPacket(bitProtocol, _buf);	// 우선 걸러지지않은 프로토콜을 가져온다.

	// 진짜 프로토콜을 가져와 준다.(안에서 프로토콜 AND 검사)
	PROTOCOL_LOBBY realProtocol = (PROTOCOL_LOBBY)bitProtocol;
	GetProtocol(realProtocol);

	return realProtocol;
}


//bool LobbyManager::CreateRoomProcess(C_ClientInfo* _ptr, char* _buf)
//{
//	RESULT_LOBBY createResult = RESULT_LOBBY::NODATA;
//	TCHAR msg[MSGSIZE] = { 0, };
//	PROTOCOL_LOBBY protocol;
//
//	char buf[BUFSIZE];		// 버퍼
//	int packetSize = 0;     // 총 사이즈
//
//	// 넘겨오는 정보 : 방 제목을 얻는다.
//	TCHAR tmpRoomTitle[ROOM_TITLE_LEN] = { 0, };
//	UnPackPacket(_buf, tmpRoomTitle);
//
//	// 방이 생성 가능한 최대갯수를 넘지않았다면 생성할 수 있다.
//	if (roomList->GetCount() < MAX_ROOM_SIZE)
//		createResult = RESULT_LOBBY::CREATE_ROOM_SUCCESS;
//	else
//		createResult = RESULT_LOBBY::CREATE_ROOM_FAIL;
//
//	switch (createResult)
//	{
//	case RESULT_LOBBY::CREATE_ROOM_FAIL:
//		_tcscpy_s(msg, MSGSIZE, CREATE_ROOM_FAIL_MSG);
//		break;
//
//
//	case RESULT_LOBBY::CREATE_ROOM_SUCCESS:
//	{
//		// 성공 메시지 조립 후
//		_tcscpy_s(msg, MSGSIZE, CREATE_ROOM_SUCCESS_MSG);
//
//		// 룸 리스트에 추가한다.
//		RoomInfo* ptr = new RoomInfo(_ptr, tmpRoomTitle);
//		roomList->Insert(ptr);
//		_ptr->SetRoomNum(ptr->roomNum);	// 방 번호 설정
//	}
//	break;
//	}
//
//	// 프로토콜 세팅 
//	protocol = SetProtocol(LOBBY_STATE, PROTOCOL_LOBBY::CREATE_ROOM_PROTOCOL, createResult);
//	ZeroMemory(buf, sizeof(BUFSIZE));
//
//	// 패킹 및 전송
//	PackPacket(buf, msg, packetSize);
//	_ptr->SendPacket(protocol, buf, packetSize);
//
//	if (createResult == RESULT_LOBBY::CREATE_ROOM_SUCCESS)
//		return true;
//
//	return false;
//}
//
//bool LobbyManager::EnterRoomProcess(C_ClientInfo* _ptr, char* _buf)
//{
//	RESULT_LOBBY enterResult = RESULT_LOBBY::NODATA;
//	TCHAR msg[MSGSIZE] = { 0, };
//	PROTOCOL_LOBBY protocol;
//
//	char buf[BUFSIZE];		// 버퍼
//	int packetSize = 0;     // 총 사이즈
//
//	// 넘겨오는 정보 : 방 번호를 얻는다.
//	TCHAR tmpRoomNum[ROOM_NUM_LEN] = { 0, };
//	UnPackPacket(_buf, tmpRoomNum);
//
//	// 방 번호가 방 리스트에 있으면 들어갈 수 있다.(리턴값은 입장가능한 그 방 포인터)
//	RoomInfo* room = CheckEnterRoom(_tstoi(tmpRoomNum), enterResult);
//
//	switch (enterResult)
//	{
//	case RESULT_LOBBY::ENTER_ROOM_FAIL:
//		_tcscpy_s(msg, MSGSIZE, ENTER_ROOM_FAIL_MSG);
//		break;
//
//
//	case RESULT_LOBBY::ENTER_ROOM_SUCCESS:
//	{
//		// 성공 메시지 조립 후
//		_tcscpy_s(msg, MSGSIZE, ENTER_ROOM_SUCCESS_MSG);
//
//		// 룸 리스트에 추가한다.
//		room->userList->Insert(_ptr);
//		_ptr->SetRoomNum(_ttoi(tmpRoomNum));	// 방 번호 설정
//	}
//	break;
//	}
//
//	// 프로토콜 세팅 
//	protocol = SetProtocol(LOBBY_STATE, PROTOCOL_LOBBY::ENTER_ROOM_PROTOCOL, enterResult);
//	ZeroMemory(buf, sizeof(BUFSIZE));
//
//	// 패킹 및 전송
//	PackPacket(buf, msg, packetSize);
//	_ptr->SendPacket(protocol, buf, packetSize);
//
//	if (enterResult == RESULT_LOBBY::ENTER_ROOM_SUCCESS)
//		return true;
//
//	return false;
//}
//RoomInfo* LobbyManager::CheckEnterRoom(int _roomNum, RESULT_LOBBY& _result)
//{
//	_result = RESULT_LOBBY::NODATA;
//	RoomInfo* info = nullptr;	// SearchData로 리턴받아서 리스트를 순회하는 용도의 변수
//
//	// 만약 방 리스트가 검색중이 아니라면
//	if (roomList->SearchCheck() == false)
//	{
//		roomList->SearchStart();	// 검색 시작
//		while (1)
//		{
//			info = roomList->SearchData();	// 데이터를 받아서
//			if (info == nullptr)	// 데이터가 nullptr이면 리스트에 내용이 더이상 없다.
//			{
//				// 이전에 발견한게 아무것도 없다면, 방 입장에 실패한다.
//				if (_result == RESULT_LOBBY::NODATA)
//					_result = RESULT_LOBBY::ENTER_ROOM_FAIL;
//
//				break;
//			}
//
//			// 방 번호가 일치하는 방이 현재 방 리스트에 있다면 입장 성공이다.
//			if (info->roomNum == _roomNum)
//			{
//				_result = RESULT_LOBBY::ENTER_ROOM_SUCCESS;
//				break;
//			}
//		}
//		roomList->SearchEnd();	// 검색 종료
//	}
//
//	return info;
//}
//
//bool LobbyManager::RoomListProcess(C_ClientInfo* _ptr, char* _buf)
//{
//	TCHAR msg[HALF_BUFSIZE] = { 0, };
//	PROTOCOL_LOBBY protocol;
//
//	char buf[BUFSIZE];		// 버퍼
//	int packetSize = 0;     // 총 사이즈
//
//	// 모든 방 제목을 붙여서 보냄
//	if (GetRoomList(msg) == false)
//	{
//		// 개설된 방이 없을시 문자열을 만듦
//		_tcscpy_s(msg, HALF_BUFSIZE, ROOMLIST_NOEXIST_MSG);
//	}
//
//	// 프로토콜 세팅 
//	protocol = SetProtocol(LOBBY_STATE, PROTOCOL_LOBBY::ROOMLIST_PROTOCOL, (RESULT_LOBBY)0);
//	ZeroMemory(buf, sizeof(BUFSIZE));
//
//	// 패킹 및 전송
//	PackPacket(buf, msg, packetSize);
//	_ptr->SendPacket(protocol, buf, packetSize);
//
//	return true;	// 방이 생성된게 있든 없든 모두 성공
//}
//
//bool LobbyManager::GetRoomList(TCHAR* _msg)
//{
//	// 만약 방 리스트가 검색중이 아니라면
//	if (roomList->SearchCheck() == false)
//	{
//		RoomInfo* info = nullptr;	// 리스트를 순회하면서 받아올 방 정보 포인터
//
//		roomList->SearchStart();	// 검색 시작
//		while (1)
//		{
//			info = roomList->SearchData();	// 데이터를 받아서
//			if (info == nullptr)	// 데이터가 nullptr이면 리스트에 내용이 더이상 없다.
//				break;
//
//			else
//			{
//				if (lstrlen(_msg) == 0)
//					_tcscat_s(_msg, HALF_BUFSIZE, TEXT("<<<방 목록>>>\n"));
//
//				TCHAR frame[MSGSIZE] = { 0, };
//				wsprintf(frame, TEXT("[%d번방]\t%s\n"), info->roomNum, info->roomTitle);
//				_tcscat_s(_msg, HALF_BUFSIZE, frame);
//			}
//		}
//		roomList->SearchEnd();	// 검색 종료
//	}
//
//	// 생성된 방 정보가 있다면 true리턴
//	if (lstrlen(_msg) > 0)
//		return true;
//
//	else
//		return false;
//}
//
//
//bool LobbyManager::CheckLeaveRoom(C_ClientInfo* _ptr)
//{
//	bool retFlag = false;
//
//	// 만약 방 리스트가 검색중이 아니라면
//	if (roomList->SearchCheck() == false)
//	{
//		RoomInfo* roomInfo = nullptr;	// SearchData로 리턴받아서 리스트를 순회하는 용도의 변수
//
//		roomList->SearchStart();	// 검색 시작
//		while (1)
//		{
//			roomInfo = roomList->SearchData();	// 데이터를 받아서
//			if (roomInfo == nullptr)	// 데이터가 nullptr이면 리스트에 내용이 더이상 없다.
//				break;
//
//
//			// 이 클라가 속한 방(방 번호로찾음)의 유저리스트에서 없앤다
//			if (roomInfo->roomNum == _ptr->GetRoomNum())
//			{
//				// 우선 해당 방에 속한 유저의 정보를 제거한다.
//				roomInfo->userList->Delete(_ptr);
//
//				// 근데 만약 방금 나간 유저가 방의 마지막 사람이었다면 그 방도 제거한다.
//				if (roomInfo->userList->GetCount() == 0)
//					roomList->Delete(roomInfo);
//
//				retFlag = true;
//			}
//
//			if (retFlag == true)
//				break;
//		}
//		roomList->SearchEnd();	// 검색 종료
//	}
//
//	return retFlag;
//}
//
////POSITION LobbyManager::RecvProcess(C_ClientInfo* _ptr)
////{
////	LogManager::GetInstance()->ErrorPrintf("로비매니저 리시브 프로세스\n");
////
////	__int64 bitProtocol;
////	char buf[BUFSIZE]; // 암호화가 끝난 패킷을 가지고 있을 버프 
////	_ptr->GetPacket(bitProtocol, buf);	// 우선 걸러지지않은 프로토콜을 가져온다.
////
////	// 진짜 프로토콜을 가져와 준다.(안에서 프로토콜 AND 검사)
////	PROTOCOL_LOBBY realProtocol = (PROTOCOL_LOBBY)bitProtocol;
////	GetProtocol(realProtocol);
////	switch (realProtocol)
////	{
////	case ENTER_ROOM_PROTOCOL:
////		if(EnterRoomProcess(_ptr, buf) == true)
////			return ROOM;
////
////	case CREATE_ROOM_PROTOCOL:
////		if(CreateRoomProcess(_ptr, buf) == true)
////			return ROOM;
////
////	case ROOMLIST_PROTOCOL:
////		if (RoomListProcess(_ptr, buf) == true)
////			return LOBBY;
////
////	case LOGOUT_PROTOCOL:
////		if(LoginManager::GetInstance()->LogoutProcess(_ptr) == true)
////		return LOGIN;
////	}
////
////	return LOBBY;
////}
////bool LobbyManager::SendProcess(C_ClientInfo* _ptr)
////{
////	LogManager::GetInstance()->ErrorPrintf("로비매니저 센드 프로세스\n");
////
////	return true;
////}
//
//
//
//bool LobbyManager::CanIEnterRoom(C_ClientInfo* _ptr)
//{
//	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
//	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);
//
//	if (protocol == ENTER_ROOM_PROTOCOL)
//		return EnterRoomProcess(_ptr, buf);
//
//	return false;
//}
//
//bool LobbyManager::CanICreateRoom(C_ClientInfo* _ptr)
//{
//	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
//	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);
//
//	if (protocol == CREATE_ROOM_PROTOCOL)
//		return CreateRoomProcess(_ptr, buf);
//
//	return false;
//}
//
//bool LobbyManager::CanIGetRoomList(C_ClientInfo* _ptr)
//{
//	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
//	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);
//
//	if (protocol == ROOMLIST_PROTOCOL)
//		return RoomListProcess(_ptr, buf);
//
//	return false;
//}
//
//bool LobbyManager::CanILeaveLobby(C_ClientInfo* _ptr)
//{
//	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
//	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);
//
//	// 로비에서 Logout을 요청했다면, LoginList를 관리하는 LoginManager의 CanILogout()을 호출해서 검사받아야한다.
//	if (protocol == LOGOUT_PROTOCOL)
//		return LoginManager::GetInstance()->CanILogout(_ptr);
//
//	return false;
//}
//
//
//C_ClientInfo* LobbyManager::GetRoomClient(int _roomNum)
//{
//	static C_ClientInfo* ptr = nullptr;
//
//	// 만약 방 리스트가 검색중이 아니라면
//	if (roomList->SearchCheck() == false)
//	{
//		RoomInfo* roomInfo = nullptr;	// SearchData로 리턴받아서 리스트를 순회하는 용도의 변수
//
//		roomList->SearchStart();	// 검색 시작
//		while (1)
//		{
//			roomInfo = roomList->SearchData();	// 데이터를 받아서
//			if (roomInfo == nullptr)	// 데이터가 nullptr이면 리스트에 내용이 더이상 없다.
//				break;
//
//
//			// 같은 방을 찾았다면 해당 방의 유저정보를 리턴한다.
//			if (roomInfo->roomNum == _roomNum)
//			{
//				// 처음 받는거면 일단 검색 노드를 푸쉬해놓는다.
//				if(ptr == nullptr)
//					roomInfo->userList->SearchStart();
//
//				ptr = roomInfo->userList->SearchData();
//
//				// 모두 다 돌았으면 다시 검색노드를 팝한다.
//				if(ptr == nullptr)
//					roomInfo->userList->SearchEnd();
//
//				break;
//			}
//
//		}
//		roomList->SearchEnd();	// 검색 종료
//	}
//
//	return ptr;
//}


bool LobbyManager::CanIMatch(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// 로비에서 매칭버튼을 눌렀다면, 매칭매니저에서 처리해야한다.
	if (protocol == MATCH_PROTOCOL)
		return MatchManager::GetInstance()->MatchProcess(_ptr);

	return false;
}
