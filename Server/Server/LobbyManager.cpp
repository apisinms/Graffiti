#include "LobbyManager.h"
#include "LoginManager.h"
#include "LogManager.h"
#include "MatchManager.h"
#include "RoomManager.h"
#include "C_ClientInfo.h"

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

			// 모든 플레이어들에게 게임을 시작하라는 프로토콜을 보내서 인게임상태로 넘어감
			protocol = SetProtocol(LOBBY_STATE, PROTOCOL_LOBBY::START_PROTOCOL, RESULT_LOBBY::MATCH_SUCCESS);
			ZeroMemory(buf, sizeof(BUFSIZE));

			// 패킹 및 전송
			int packetSize = 0;
			//PackPacket(buf, nullptr, packetSize);	// 이 부분이 null로 넘겼을 때 처리가 될지??
			_ptr->GetRoom()->team1->player1->SendPacket(protocol, buf, packetSize);
			_ptr->GetRoom()->team1->player2->SendPacket(protocol, buf, packetSize);
			_ptr->GetRoom()->team2->player1->SendPacket(protocol, buf, packetSize);
			_ptr->SendPacket(protocol, buf, packetSize);

			wprintf(L"1팀 : %s, %s\n2팀 : %s, %s\n"
				, _ptr->GetRoom()->team1->player1->GetUserInfo()->id
				, _ptr->GetRoom()->team1->player2->GetUserInfo()->id
				, _ptr->GetRoom()->team2->player1->GetUserInfo()->id
				, _ptr->GetRoom()->team2->player2->GetUserInfo()->id);

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
bool LobbyManager::CanIStart(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// 만약 4인 매칭이 성공하여 성공했던 클라가 나에게 시작 프로토콜을 보낸다면 인게임으로 들어가야한다.
	if (protocol == START_PROTOCOL)
		return true;

	return false;
}
