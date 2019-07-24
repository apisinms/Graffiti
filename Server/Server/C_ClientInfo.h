#pragma once
#include "C_Packet.h"
#include "C_Global.h"
#include "C_State.h"
#include "C_Stack.h"

#define STACK_SIZE 5

class C_LoginState;
class C_LobbyState;
class C_ChatState;
class C_InGameState;

struct RoomInfo;

class C_ClientInfo : public C_Packet
{
private:
	RoomInfo* room;
	UserInfo* userInfo;
	C_State* state;
	C_Stack<C_State*, STACK_SIZE>* stateStack;	// Undo 기능을 구현할 스택

	// 미리 상태를 정의해둔다.
	C_LoginState* loginState;
	C_LobbyState* lobbyState;
	C_ChatState*  chatState;
	C_InGameState*  inGameState;
	
public:
	C_ClientInfo(UserInfo* _userInfo, C_State* _state, SOCKET _sock, SOCKADDR_IN _addr);
	~C_ClientInfo();

	void SetState(C_State* _state);
	C_State* GetCurrentState();
	C_State* GetLobbyState();
	C_State* GetLoginState();
	C_State* GetChatState();
	C_State* GetInGameState();
	void PushState(C_State* _state);
	C_State* PopState();
	void SetUserInfo(UserInfo* _userInfo);
	UserInfo* GetUserInfo();

	void SetRoom(RoomInfo* _room);
	RoomInfo* GetRoom();
};