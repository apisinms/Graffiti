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
	PlayerInfo* playerInfo;	// 인게임에서 사용하는 플레이어 정보
	RoomInfo* room;			// 소속된 방 정보(매칭 잡혔을 시)

	UserInfo* userInfo;		// 이 클라의 회원정보
	C_State* state;			// 이 클라의 현재 상태
	C_Stack<C_State*, STACK_SIZE>* stateStack;	// Undo 기능을 구현할 스택

	// 미리 상태를 정의해둔다.
	C_LoginState* loginState;
	C_LobbyState* lobbyState;
	//C_ChatState*  chatState;
	C_InGameState*  inGameState;
	
public:
	C_ClientInfo(UserInfo* _userInfo, SOCKET _sock, SOCKADDR_IN _addr);
	~C_ClientInfo();

	void SetState(C_State* _state);
	C_State* GetCurrentState();
	C_State* GetLobbyState();
	C_State* GetLoginState();
	//C_State* GetChatState();
	C_State* GetInGameState();
	void PushState(C_State* _state);
	C_State* PopState();
	void SetUserInfo(UserInfo* _userInfo);
	UserInfo* GetUserInfo();

	void SetRoom(RoomInfo* _room);
	RoomInfo* GetRoom();

	// 모든 정보가 다 포함된 얘를 리턴해주면 됨(어차피 인터페이스 있으니까)
	void SetPlayerInfo(PlayerInfo* _playerInfo);
	PlayerInfo* GetPlayerInfo();
};