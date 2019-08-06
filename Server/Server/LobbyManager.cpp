#include "LobbyManager.h"
#include "LoginManager.h"
#include "LogManager.h"
#include "MatchManager.h"
#include "RoomManager.h"
#include "C_ClientInfo.h"
#include "InGameManager.h"
#include <process.h>

LobbyManager* LobbyManager::instance;

LobbyManager* LobbyManager::GetInstance()
{
	if (instance == nullptr)
		instance = new LobbyManager();

	return instance;
}
void LobbyManager::Destroy()
{
	delete instance;
}

void LobbyManager::Init()
{
}

void LobbyManager::End()
{
}

void LobbyManager::PackPacket(char* _setptr, int& _num, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// ���ڿ�(�����ڵ�)
	memcpy(ptr, &_num, sizeof(int));
	ptr = ptr + sizeof(int);
	_size = _size + sizeof(int);
}
void LobbyManager::PackPacket(char* _setptr, TCHAR* _str1, int& _size)
{
	char* ptr = _setptr;
	int strsize1 = _tcslen(_str1) * sizeof(TCHAR);
	_size = 0;

	// ���ڿ� ����
	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	// ���ڿ�(�����ڵ�)
	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	_size = _size + strsize1;
}
void LobbyManager::UnPackPacket(char* _getBuf, TCHAR* _str1)
{
	int str1size;
	char* ptr = _getBuf + sizeof(PROTOCOL_LOBBY);

	memcpy(&str1size, ptr, sizeof(str1size));
	ptr = ptr + sizeof(str1size);

	// ���ڿ� 1 ����
	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;
}

void LobbyManager::GetProtocol(PROTOCOL_LOBBY& _protocol)
{
	// major state�� ������(Ŭ��� state�� �Ⱥ����ϱ�(Ȥ�ó� ���Ŀ� �����ԵǸ� �̺κ��� ����)) protocol�� �������� ���ؼ� ���� 10��Ʈ ��ġ�� ����ũ�� ����
#ifdef __64BIT__
	__int64 mask = ((__int64)0x1f << (64 - 10));
#endif

#ifdef __32BIT__
	int mask = ((int)0x1f << (32 - 10));
#endif

	// ����ũ�� �ɷ��� 1���� ���������� ����ȴ�. 
	PROTOCOL_LOBBY protocol = (PROTOCOL_LOBBY)(_protocol & (PROTOCOL_LOBBY)mask);

	// �ƿ�ǲ�� �����̹Ƿ� �������ش�.
	// ���߿� �ѹ��� �������ִ� ������ ���߿� �߰��� ���� �� �ִ� result �� ���ؼ� protocol �� ������� ���� 
	_protocol = protocol;
}
LobbyManager::PROTOCOL_LOBBY LobbyManager::SetProtocol(STATE_PROTOCOL _state, PROTOCOL_LOBBY _protocol, RESULT_LOBBY _result)
{
	// �ϼ��� ���������� ���� 
	PROTOCOL_LOBBY protocol = (PROTOCOL_LOBBY)0;
	protocol = (PROTOCOL_LOBBY)(_state | _protocol | _result);
	return protocol;
}

LobbyManager::PROTOCOL_LOBBY LobbyManager::GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf)
{
#ifdef __64BIT__
	__int64 bitProtocol = 0;
#endif

#ifdef __32BIT__
	int bitProtocol = 0;
#endif

	_ptr->GetPacket(bitProtocol, _buf);	// �켱 �ɷ��������� ���������� �����´�.

	// ��¥ ���������� ������ �ش�.(�ȿ��� �������� AND �˻�)
	PROTOCOL_LOBBY realProtocol = (PROTOCOL_LOBBY)bitProtocol;
	GetProtocol(realProtocol);

	return realProtocol;
}

// ��Ī�� �� �ִ���
bool LobbyManager::CanIMatch(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// �κ񿡼� ��Ī��ư�� �����ٸ�, ��Ī�Ŵ������� ó���ؾ��Ѵ�.
	if (protocol == MATCH_PROTOCOL)
	{
		if (MatchManager::GetInstance()->MatchProcess(_ptr) == true)
		{

			// ��� �÷��̾�鿡�� �ΰ��� ���·� �����϶�� ���������� ��������
			protocol = SetProtocol(LOBBY_STATE, PROTOCOL_LOBBY::GOTO_INGAME_PROTOCOL, RESULT_LOBBY::LOBBY_SUCCESS);
			ZeroMemory(buf, sizeof(BUFSIZE));

			SendPacket_Room(_ptr, buf, protocol);	// ������������ + ��� �÷��̾����� ��ŷ �� ����

			return true;
		}
		
	}

	return false;
}

// ��Ī ��� �� �� �ִ���
bool LobbyManager::CanICancelMatch(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// ��Ī ��� ���������̶��
	if (protocol == MATCH_CANCEL_PROTOCOL)
	{
		// �� Ŭ�� ������ ��⸮��Ʈ���� �����ϰ�
		MatchManager::GetInstance()->WaitListDelete(_ptr);


		// ��Ī ��� ���� �������� ����
		protocol = SetProtocol(LOBBY_STATE, PROTOCOL_LOBBY::MATCH_CANCEL_PROTOCOL, RESULT_LOBBY::LOBBY_SUCCESS);
		ZeroMemory(buf, sizeof(BUFSIZE));

		// ��ŷ �� ����(��Ī�� �Ϸ�Ǿ���, ������ �����ص� ����)
		int packetSize = 0;

		// ���������� ��Ī ��ҵǾ����� �˷���
		_ptr->SendPacket(protocol, buf, packetSize);

		return true;	// ���� ����
	}

 	return false;
}

bool LobbyManager::CanILeaveLobby(C_ClientInfo* _ptr)
{
	//char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	//PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	//// �κ񿡼� Logout�� ��û�ߴٸ�, LoginList�� �����ϴ� LoginManager�� CanILogout()�� ȣ���ؼ� �˻�޾ƾ��Ѵ�.
	//if (protocol == LOGOUT_PROTOCOL)
	//	return LoginManager::GetInstance()->CanILogout(_ptr);

	return false;
}

// ������ ������ �� �ִ���
bool LobbyManager::CanIGotoInGame(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// ���� 4�� ��Ī�� �����Ͽ� �����ߴ� Ŭ�� ������ ���� ���������� �����ٸ� �ΰ����� ���⼱�� â���� �����Ѵ�.
	if (protocol == GOTO_INGAME_PROTOCOL)
	{
		printf("4�� ��Ī����\n");

		// ���� ���� �����ǰ� �ƹ��� ���൵ ���� �ʾҴٸ�
		if (_ptr->GetRoom()->roomStatus == ROOMSTATUS::ROOM_NONE)
		{
			// InGameManager���� Ÿ�̸� �����带 ������ 30�ʸ� ������ ��Ź�Ѵ�.
			_ptr->GetRoom()->timerHandle = (HANDLE)_beginthreadex(nullptr, 0, (_beginthreadex_proc_type)InGameManager::TimerThread, (void*)_ptr, 0, NULL);
			if (_ptr->GetRoom()->timerHandle == nullptr)
				LogManager::GetInstance()->ErrorPrintf("_beginthreadex() in CanIStart()");

			_ptr->GetRoom()->roomStatus = ROOMSTATUS::ROOM_ITEMSEL;	// ���� ������ ���� ���·�
		}

		return true;
	}

	return false;
}

void LobbyManager::SendPacket_Room(C_ClientInfo* _ptr, char* buf, PROTOCOL_LOBBY protocol)
{
	int packetSize = 0;

	int player1 = 1;
	int player2 = 2;
	int player3 = 3;
	int player4 = 4;

	PackPacket(buf, player1, packetSize);
	_ptr->GetRoom()->team1->player1->SendPacket(protocol, buf, packetSize);

	PackPacket(buf, player2, packetSize);
	_ptr->GetRoom()->team1->player2->SendPacket(protocol, buf, packetSize);

	PackPacket(buf, player3, packetSize);
	_ptr->GetRoom()->team2->player1->SendPacket(protocol, buf, packetSize);

	PackPacket(buf, player4, packetSize);
	_ptr->SendPacket(protocol, buf, packetSize);
}
