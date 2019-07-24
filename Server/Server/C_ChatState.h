#pragma once
#include "C_State.h"
#include "ChatManager.h"
#include "C_Global.h"

class C_ChatState : public C_State
{
private:
	POSITION pos = CHAT;
public:
	void Init()override {}
	void End()override {}

	void Read(C_ClientInfo* _ptr) override
	{
		// ä�� �޽����� �濡 �Ҽӵ� �ٸ� Ŭ��鿡�� �����ϵ��� �˻��Ѵ�.
		if (ChatManager::GetInstance()->CheckChattingMessage(_ptr) == true)
			pos = CHAT;
	}

	void Write(C_ClientInfo* _ptr) override
	{
		switch (pos)
		{
		case LOBBY:
			_ptr->SetState(_ptr->GetLobbyState());		// �κ� ���·� �̵��Ѵ�.
			break;

		default:
			break;
		}
	}
};