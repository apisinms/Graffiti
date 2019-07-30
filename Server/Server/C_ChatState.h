#pragma once
#include "C_State.h"
#include "ChatManager.h"
#include "C_Global.h"

class C_ChatState : public C_State
{
private:
	POSITION pos = LOBBY;
public:
	void Init()override {}
	void End()override {}

	void Read(C_ClientInfo* _ptr) override
	{
		// 방을 떠날 수 있다면 모두 Write한 뒤에 LOBBY로 상태로 이동하게 한다.
		if (ChatManager::GetInstance()->CanILeaveRoom(_ptr) == true)
			pos = LOBBY;

		//// 채팅 메시지를 방에 소속된 다른 클라들에게 전송하도록 검사한다.
		//else if (ChatManager::GetInstance()->CheckChattingMessage(_ptr) == true)
		//	pos = CHAT;
	}

	void Write(C_ClientInfo* _ptr) override
	{
		switch (pos)
		{
		case LOBBY:
			_ptr->SetState(_ptr->GetLobbyState());		// 로비 상태로 이동한다.
			break;

		default:
			break;
		}
	}
};