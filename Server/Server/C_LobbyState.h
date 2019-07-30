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
		// ��ΰ� �ٰ��� �ΰ������� �Ѿ�� �� ���������� �޾��� ������ INGAME���� �Ѿ�ߵȴ�.
		if (LobbyManager::GetInstance()->CanIStart(_ptr) == true)
			_ptr->SetState(_ptr->GetInGameState());		// �ΰ��� ���·� �̵��Ѵ�.

		// ��Ī�� ��������
		if (LobbyManager::GetInstance()->CanIMatch(_ptr) == true)
			pos = LOBBY;
			//pos = INGAME;

		// �κ� ������ �α��� â���� ���°� ��������
		if (LobbyManager::GetInstance()->CanILeaveLobby(_ptr) == true)
			pos = LOGIN;
			
	}

	void Write(C_ClientInfo* _ptr) override
	{
		// �ٲ����ϴ� ���¿����� ���¸� �����Ѵ�.
		switch (pos)
		{
		//case INGAME:
		//	_ptr->SetState(_ptr->GetInGameState());		// �ΰ��� ���·� �̵��Ѵ�.
		//	break;

		//case CHAT:
		//	_ptr->SetState(_ptr->GetChatState());		// ä�� ���·� �̵��Ѵ�.
		//	break;

		case LOGIN:
			//_ptr->PushState(_ptr->GetCurrentState());	// ���� ���¸� ���ÿ� �����ϰ�
			_ptr->SetState(_ptr->GetLoginState());		// �α��� ���·� �̵��Ѵ�.
			break;

		default:
			break;
		}
	}
};