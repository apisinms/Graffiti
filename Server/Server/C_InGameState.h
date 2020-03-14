#pragma once
#include "C_State.h"
#include "InGameManager.h"

class C_InGameState : public C_State
{
private:
	STATE state = STATE_INGAME;
public:
	void Init()override {}
	void End()override {}

	void Read(C_ClientInfo* _ptr) override
	{
		if (InGameManager::GetInstance()->IngameProtocolChecker(_ptr) == true)
			state = STATE_INGAME;

		if (InGameManager::GetInstance()->CanIGotoLobby(_ptr) == true)
			_ptr->SetState(_ptr->GetLobbyState());		// 로비 상태로 이동한다.
	}

	void Write(C_ClientInfo* _ptr) override
	{
	}
};