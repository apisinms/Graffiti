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
		if (LobbyManager::GetInstance()->CanIGotoInGame(_ptr) == true)
			_ptr->SetState(_ptr->GetInGameState());		// 인게임 상태로 이동한다.(Send하는게 없으므로 Read에서 처리해줘야함)

		// 매칭이 가능한지
		if (LobbyManager::GetInstance()->CanIMatch(_ptr) == true)
			state = STATE_LOBBY;

		// 매칭 취소가 가능한지
		if (LobbyManager::GetInstance()->CanICancelMatch(_ptr) == true)
			state = STATE_LOBBY;
	}

	void Write(C_ClientInfo* _ptr) override
	{
		// 바뀌어야하는 상태에따라 상태를 변경한다.
		switch (state)
		{
		case STATE_LOGIN:
			_ptr->SetState(_ptr->GetLoginState());		// 로그인 상태로 이동한다.
			break;

		case STATE_LOBBY:
			_ptr->SetState(_ptr->GetLobbyState());		// 로비 상태로 이동한다.
			break;

		case STATE_INGAME:
			_ptr->SetState(_ptr->GetInGameState());		// 인게임 상태로 이동한다.
			break;


		default:
			break;
		}
	}
};