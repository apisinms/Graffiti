#include "C_ClientInfo.h"

#include "C_LoginState.h"
#include "C_LobbyState.h"
#include "C_ChatState.h"
#include "C_InGameState.h"

C_ClientInfo::C_ClientInfo(UserInfo* _userInfo, C_State* _state, SOCKET _sock, SOCKADDR_IN _addr)
{
	userInfo = _userInfo;
	state = _state;
	this->sock = _sock;
	this->addr = _addr;
	this->recvData.compRecvBytes = 0;
	this->recvData.recvBytes = 0;
	this->recvData.rSizeFlag = false;
	this->sendData.compSendBytes = 0;
	this->sendData.sendBytes = 0;

	this->roomNum = -1;

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
	chatState = new C_ChatState();
	inGameState = new C_InGameState();
}
C_ClientInfo::~C_ClientInfo()
{
	delete loginState;
	delete lobbyState;
	delete chatState;
	delete inGameState;
}
void C_ClientInfo::SetState(C_State* _state)
{ 
	state = _state; 
}
C_State* C_ClientInfo::GetCurrentState() { return state; }
C_State* C_ClientInfo::GetLobbyState() { return (C_State*)lobbyState; }
C_State* C_ClientInfo::GetLoginState() { return (C_State*)loginState; }
C_State* C_ClientInfo::GetChatState() { return (C_State*)chatState; }
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

void C_ClientInfo::SetRoomNum(int _roomNum) { roomNum = _roomNum; }
int C_ClientInfo::GetRoomNum() { return roomNum; }