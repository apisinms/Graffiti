#pragma once
#include "C_State.h"
#include "LoginManager.h"
#include "MainManager.h"

class C_LoginState : public C_State
{
private:
	STATE state = STATE_LOGIN;
public:
	void Init()override {}
	void End()override {}

	void Read(C_ClientInfo* _ptr) override
	{
		// �α����� �� �� �ִ���
		if (LoginManager::GetInstance()->CanILogin(_ptr) == true)
			state = STATE_LOBBY;

		// ȸ�������� �� �� �ִ���
		if (LoginManager::GetInstance()->CanIJoin(_ptr) == true)
			state = STATE_LOGIN;
	}

	void Write(C_ClientInfo* _ptr) override
	{
		// �ٲ����ϴ� ���¿����� ���¸� �����Ѵ�.
		switch (state)
		{
		case STATE_LOGIN:
			_ptr->SetState(_ptr->GetLoginState());		// �α��� ���·� �̵��Ѵ�.
			break;

		case STATE_LOBBY:
			_ptr->SetState(_ptr->GetLobbyState());		// �κ� ���·� �̵��Ѵ�.
			break;

		default:
			break;
		}
	}
};