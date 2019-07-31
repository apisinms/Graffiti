#pragma once
#include "C_State.h"
#include "LobbyManager.h"
#include "LoginManager.h"

class C_LobbyState : public C_State
{
private:
	STATE state = STATE_LOBBY;
public:
	void Init()override {}
	void End()override {}

	void Read(C_ClientInfo* _ptr) override
	{
		// 모두가 다같이 인게임으로 넘어가는 이 프로토콜을 받았을 때만이 INGAME으로 넘어가야된다.
		if (LobbyManager::GetInstance()->CanISelectWeapon(_ptr) == true)
			_ptr->SetState(_ptr->GetInGameState());		// 인게임 상태로 이동한다.

		// 매칭이 가능한지
		if (LobbyManager::GetInstance()->CanIMatch(_ptr) == true)
			state = STATE_LOBBY;
			//pos = INGAME;

		// 로비를 떠나서 로그인 창으로 가는게 가능한지
		if (LobbyManager::GetInstance()->CanILeaveLobby(_ptr) == true)
			state = STATE_LOGIN;
			
	}

	void Write(C_ClientInfo* _ptr) override
	{
		// 바뀌어야하는 상태에따라 상태를 변경한다.
		switch (state)
		{
		//case INGAME:
		//	_ptr->SetState(_ptr->GetInGameState());		// 인게임 상태로 이동한다.
		//	break;

		//case CHAT:
		//	_ptr->SetState(_ptr->GetChatState());		// 채팅 상태로 이동한다.
		//	break;

		case STATE_LOGIN:
			//_ptr->PushState(_ptr->GetCurrentState());	// 현재 상태를 스택에 저장하고
			_ptr->SetState(_ptr->GetLoginState());		// 로그인 상태로 이동한다.
			break;

		default:
			break;
		}
	}
};