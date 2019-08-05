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
		// 로그인을 할 수 있는지
		if (LoginManager::GetInstance()->CanILogin(_ptr) == true)
			state = STATE_LOBBY;

		// 회원가입을 할 수 있는지
		if (LoginManager::GetInstance()->CanIJoin(_ptr) == true)
			state = STATE_LOGIN;
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

		default:
			break;
		}

		// 이전 상태로 돌아가는 코드
		{
			//C_State* prevState = _ptr->PopState();		// 클라의 상태 스택에서 이전 상태를 얻어온다.

			//// 이전에 진행했던 상태가 없었다면
			//if (prevState == nullptr)
			//	_ptr->SetState(_ptr->GetLobbyState());		// 로비 상태로 이동한다.

			//// 이전 상태로 이동한다.
			//else
			//	_ptr->SetState(prevState);					// 이전 상태로 이동한다.
		}
	}
};