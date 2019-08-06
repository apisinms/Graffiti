#pragma once
#include "C_State.h"
#include "ChatManager.h"
#include "C_Global.h"

class C_ChatState : public C_State
{
private:
	STATE state = STATE_LOBBY;
public:
	void Init()override {}
	void End()override {}

	void Read(C_ClientInfo* _ptr) override
	{
		// ���� ���� �� �ִٸ� ��� Write�� �ڿ� LOBBY�� ���·� �̵��ϰ� �Ѵ�.
		if (ChatManager::GetInstance()->CanILeaveRoom(_ptr) == true)
			state = STATE_LOBBY;

		//// ä�� �޽����� �濡 �Ҽӵ� �ٸ� Ŭ��鿡�� �����ϵ��� �˻��Ѵ�.
		//else if (ChatManager::GetInstance()->CheckChattingMessage(_ptr) == true)
		//	pos = CHAT;
	}

	void Write(C_ClientInfo* _ptr) override
	{
		switch (state)
		{
		case STATE_LOBBY:
			_ptr->SetState(_ptr->GetLobbyState());		// �κ� ���·� �̵��Ѵ�.
			break;

		default:
			break;
		}
	}
};