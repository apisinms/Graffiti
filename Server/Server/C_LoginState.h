#pragma once
#include "C_State.h"
#include "LoginManager.h"

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
		// �̵��ؾ��ϴ� ���� ��������� �� ���·� �����Ѵ�.
		if (state == STATE_LOBBY)
			_ptr->SetState(_ptr->GetLobbyState());

		// ���� ���·� ���ư��� �ڵ�
		{
			//C_State* prevState = _ptr->PopState();		// Ŭ���� ���� ���ÿ��� ���� ���¸� ���´�.

			//// ������ �����ߴ� ���°� �����ٸ�
			//if (prevState == nullptr)
			//	_ptr->SetState(_ptr->GetLobbyState());		// �κ� ���·� �̵��Ѵ�.

			//// ���� ���·� �̵��Ѵ�.
			//else
			//	_ptr->SetState(prevState);					// ���� ���·� �̵��Ѵ�.
		}
	}
};