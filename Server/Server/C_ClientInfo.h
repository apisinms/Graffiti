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

	void SetPlayerInfo(PlayerInfo* _playerInfo);
	PlayerInfo* GetPlayerInfo();

	///////// 이 밑에 메서드들은 처리하는 방식을 좀 바꿔야될듯.. 멤버 늘어나면 계속 늘어나게됨

	PositionPacket* GetPosition();
	void SetPosition(PositionPacket* _position);

	INDEX GetIndex();
	void SetIndex(INDEX _index);

	Weapon* GetWeapon();
	void SetWeapon(Weapon* _weapon);

	float GetHealth();
	void SetHealth(float _health);

	float GetSpeed();
	void SetSpeed(float _speed);

	int GetBullet();
	void SetBullet(int _bullet);
};