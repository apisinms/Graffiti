#pragma once
#include "C_State.h"
#include "LobbyManager.h"
#include "LoginManager.h"

class C_LobbyState : public C_State
{
private:
	POSITION pos = LOBBY;
public:
	void Init()override {}
	void End()override {}

	void Read(C_ClientInfo* _ptr) override
	{
		// ä�ù濡 ������ ��������
		if (LobbyManager::GetInstance()->CanIEnterRoom(_ptr) == true)
			pos = CHAT;

		// ä�ù� ������ ��������
		else if (LobbyManager::GetInstance()->CanICreateRoom(_ptr) == true)
			pos = CHAT;

		// �κ� ������ �α��� â���� ���°� ��������
		else if (LobbyManager::GetInstance()->CanILeaveLobby(_ptr) == true)
			pos = LOGIN;

		// �� ����Ʈ�� ���� �� �ִ���
		else if (LobbyManager::GetInstance()->CanIGetRoomList(_ptr) == true)
			pos = LOBBY;
	}

	void Write(C_ClientInfo* _ptr) override
	{
		// �ٲ����ϴ� ���¿����� ���¸� �����Ѵ�.
		switch (pos)
		{
		case CHAT:
			_ptr->SetState(_ptr->GetChatState());		// ä�� ���·� �̵��Ѵ�.
			break;

		case LOGIN:
			//_ptr->PushState(_ptr->GetCurrentState());	// ���� ���¸� ���ÿ� �����ϰ�
			_ptr->SetState(_ptr->GetLoginState());		// �α��� ���·� �̵��Ѵ�.
			break;

		default:
			break;
		}
	}
};