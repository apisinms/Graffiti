#include "C_ClientInfo.h"

#include "C_LoginState.h"
#include "C_LobbyState.h"
#include "C_ChatState.h"
#include "C_InGameState.h"

C_ClientInfo::C_ClientInfo(UserInfo* _userInfo, SOCKET _sock, SOCKADDR_IN _addr)
{
	userInfo = _userInfo;
	this->sock = _sock;
	this->addr = _addr;


	this->recvData.compRecvBytes = 0;
	this->recvData.recvBytes = 0;
	this->recvData.rSizeFlag = false;
	this->sendData.compSendBytes = 0;
	this->sendData.sendBytes = 0;

	this->room = nullptr;

	ZeroMemory(&rOverlapped, sizeof(rOverlapped));
	ZeroMemory(&sOverlapped, sizeof(sOverlapped));

	this->rOverlapped.ptr = this;
	this->rOverlapped.type = IO_TYPE::IO_RECV;

	this->sOverlapped.ptr = this;
	this->sOverlapped.type = IO_TYPE::IO_SEND;

	memset(this->recvData.recvBuf, 0, BUFSIZE);
	memset(this->sendData.sendBuf, 0, BUFSIZE);

	loginState = new C_LoginState();
	lobbyState = new C_LobbyState();
	//chatState = new C_ChatState();
	inGameState = new C_InGameState();

	weapon = new Weapon();	// -1로 초기화

	state = loginState;	// 초기 상태는 로그인 상태
}
C_ClientInfo::~C_ClientInfo()
{
	delete loginState;
	delete lobbyState;
	//delete chatState;
	delete inGameState;
}
void C_ClientInfo::SetState(C_State* _state)
{ 
	state = _state; 
}
C_State* C_ClientInfo::GetCurrentState() { return state; }
C_State* C_ClientInfo::GetLobbyState() { return (C_State*)lobbyState; }
C_State* C_ClientInfo::GetLoginState() { return (C_State*)loginState; }
//C_State* C_ClientInfo::GetChatState() { return (C_State*)chatState; }
C_State* C_ClientInfo::GetInGameState() { return (C_State*)inGameState; }
void C_ClientInfo::SetUserInfo(UserInfo* _userInfo) { userInfo = _userInfo; }
UserInfo* C_ClientInfo::GetUserInfo() { return userInfo; }
void C_ClientInfo::PushState(C_State* _state) { stateStack->push(_state); }
C_State* C_ClientInfo::PopState() 
{ 
	C_State* ptr;
	if(stateStack->pop(ptr) == true)
		return ptr;

	return nullptr;
}

void C_ClientInfo::SetRoom(RoomInfo* _room) { room = _room; }
RoomInfo* C_ClientInfo::GetRoom() { return room; }
Weapon* C_ClientInfo::GetWeapon() { return weapon; }
void C_ClientInfo::SetWeapon(Weapon* _weapon) { weapon = _weapon; }