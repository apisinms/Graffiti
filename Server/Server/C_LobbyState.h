#pragma once
#include "C_State.h"
#include "LobbyManager.h"
#include "LoginManager.h"

class C_LobbyState : public C_State
{
private:
	STATE state = STATE_LOBBY;
public:
	void Init()override {}
	void End()override {}

	void Read(C_ClientInfo* _ptr) override
	{
		// ��ΰ� �ٰ��� �ΰ������� �Ѿ�� �� ���������� �޾��� ������ INGAME���� �Ѿ�ߵȴ�.
		if (LobbyManager::GetInstance()->CanIGotoInGame(_ptr) == true)
			_ptr->SetState(_ptr->GetInGameState());		// �ΰ��� ���·� �̵��Ѵ�.(Send�ϴ°� �����Ƿ� Read���� ó���������)

		// ��Ī�� ��������
		if (LobbyManager::GetInstance()->CanIMatch(_ptr) == true)
			state = STATE_LOBBY;

		// ��Ī ��Ұ� ��������
		if (LobbyManager::GetInstance()->CanICancelMatch(_ptr) == true)
			state = STATE_LOBBY;
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

		case STATE_INGAME:
			_ptr->SetState(_ptr->GetInGameState());		// �ΰ��� ���·� �̵��Ѵ�.
			break;


		default:
			break;
		}
	}
};