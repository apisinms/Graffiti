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
		// 채팅방에 입장이 가능한지
		if (LobbyManager::GetInstance()->CanIEnterRoom(_ptr) == true)
			pos = CHAT;

		// 채팅방 생성이 가능한지
		else if (LobbyManager::GetInstance()->CanICreateRoom(_ptr) == true)
			pos = CHAT;

		// 로비를 떠나서 로그인 창으로 가는게 가능한지
		else if (LobbyManager::GetInstance()->CanILeaveLobby(_ptr) == true)
			pos = LOGIN;

		// 방 리스트를 얻어올 수 있는지
		else if (LobbyManager::GetInstance()->CanIGetRoomList(_ptr) == true)
			pos = LOBBY;
	}

	void Write(C_ClientInfo* _ptr) override
	{
		// 바뀌어야하는 상태에따라 상태를 변경한다.
		switch (pos)
		{
		case CHAT:
			_ptr->SetState(_ptr->GetChatState());		// 채팅 상태로 이동한다.
			break;

		case LOGIN:
			//_ptr->PushState(_ptr->GetCurrentState());	// 현재 상태를 스택에 저장하고
			_ptr->SetState(_ptr->GetLoginState());		// 로그인 상태로 이동한다.
			break;

		default:
			break;
		}
	}
};