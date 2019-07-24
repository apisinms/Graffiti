#pragma once
#include "C_State.h"
#include "LoginManager.h"

class C_LoginState : public C_State
{
private:
	POSITION pos = LOGIN;
public:
	void Init()override {}
	void End()override {}

	void Read(C_ClientInfo* _ptr) override
	{
		// 로그인을 할 수 있는지
		if (LoginManager::GetInstance()->CanILogin(_ptr) == true)
			pos = LOBBY;

		// 회원가입을 할 수 있는지
		else if (LoginManager::GetInstance()->CanIJoin(_ptr) == true)
			pos = LOGIN;
	}

	void Write(C_ClientInfo* _ptr) override
	{
		// 이동해야하는 곳이 어딘지보고 그 상태로 변경한다.
		if (pos == LOBBY)
			_ptr->SetState(_ptr->GetLobbyState());

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