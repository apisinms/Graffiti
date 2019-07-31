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
		if (InGameManager::GetInstance()->CanIItemSelect(_ptr) == true)
			state = STATE_INGAME;
	}

	void Write(C_ClientInfo* _ptr) override
	{

	}
};