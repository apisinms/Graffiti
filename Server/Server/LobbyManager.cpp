#include "LobbyManager.h"
#include "LoginManager.h"
#include "LogManager.h"
#include "MatchManager.h"
#include "RoomManager.h"
#include "C_ClientInfo.h"
#include "InGameManager.h"
#include <process.h>

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
}

void LobbyManager::End()
{
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
#ifdef __64BIT__
	__int64 mask = ((__int64)0x1f << (64 - 10));
#endif

#ifdef __32BIT__
	int mask = ((int)0x1f << (32 - 10));
#endif

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
#ifdef __64BIT__
	__int64 bitProtocol = 0;
#endif

#ifdef __32BIT__
	int bitProtocol = 0;
#endif

	_ptr->GetPacket(bitProtocol, _buf);	// 우선 걸러지지않은 프로토콜을 가져온다.

	// 진짜 프로토콜을 가져와 준다.(안에서 프로토콜 AND 검사)
	PROTOCOL_LOBBY realProtocol = (PROTOCOL_LOBBY)bitProtocol;
	GetProtocol(realProtocol);

	return realProtocol;
}

// 매칭할 수 있는지
bool LobbyManager::CanIMatch(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// 로비에서 매칭버튼을 눌렀다면, 매칭매니저에서 처리해야한다.
	if (protocol == MATCH_PROTOCOL)
	{
		if (MatchManager::GetInstance()->MatchProcess(_ptr) == true)
		{

			// 모든 플레이어들에게 인게임 상태로 진입하라는 프로토콜을 보내버림
			protocol = SetProtocol(LOBBY_STATE, PROTOCOL_LOBBY::GOTO_INGAME_PROTOCOL, RESULT_LOBBY::MATCH_SUCCESS);
			ZeroMemory(buf, sizeof(BUFSIZE));

			// 패킹 및 전송(매칭이 완료되었고, 게임을 시작해도 좋음)
			int packetSize = 0;

			// 같은 방에 있는 모든 플레이어에게 시작 프로토콜을 전송함.
			_ptr->GetRoom()->team1->player1->SendPacket(protocol, buf, packetSize);
			_ptr->GetRoom()->team1->player2->SendPacket(protocol, buf, packetSize);
			_ptr->GetRoom()->team2->player1->SendPacket(protocol, buf, packetSize);
			_ptr->SendPacket(protocol, buf, packetSize);

			//wprintf(L"1팀:%s, %s\n2팀:%s, %s\n"
			//	,_ptr->GetRoom()->team1->player1->GetUserInfo()->id
			//	,_ptr->GetRoom()->team1->player2->GetUserInfo()->id
			//	,_ptr->GetRoom()->team2->player1->GetUserInfo()->id
			//	,_ptr->GetRoom()->team2->player2->GetUserInfo()->id);

			return true;
		}
		
	}

	return false;
}

bool LobbyManager::CanILeaveLobby(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// 로비에서 Logout을 요청했다면, LoginList를 관리하는 LoginManager의 CanILogout()을 호출해서 검사받아야한다.
	if (protocol == LOGOUT_PROTOCOL)
		return LoginManager::GetInstance()->CanILogout(_ptr);

	return false;
}

// 게임을 시작할 수 있는지
bool LobbyManager::CanISelectWeapon(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// 만약 4인 매칭이 성공하여 성공했던 클라가 나에게 시작 프로토콜을 보낸다면 인게임의 무기선택 창으로 들어가야한다.
	if (protocol == GOTO_INGAME_PROTOCOL)
	{
		// 만약 타이머 쓰레드가 아직 생성이 안됐다면
		if (_ptr->GetRoom()->timerHandle == NULL)
		{
			// InGameManager에게 타이머 쓰레드를 생성해 30초를 세도록 부탁한다.
			_ptr->GetRoom()->timerHandle = (HANDLE)_beginthreadex(NULL, 0, (_beginthreadex_proc_type)InGameManager::TimerThread, (void*)_ptr, 0, NULL);
			if (_ptr->GetRoom()->timerHandle == NULL)
				LogManager::GetInstance()->ErrorPrintf("_beginthreadex() in CanIStart()");

			_ptr->GetRoom()->roomStatus = ROOMSTATUS::ROOM_ITELSEL;	// 이제 아이템 선택 상태로
		}
		return true;
	}

	return false;
}
