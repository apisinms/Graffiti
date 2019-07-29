#include "LobbyManager.h"
#include "LoginManager.h"
#include "LogManager.h"
#include "MatchManager.h"
#include "RoomManager.h"
#include "C_ClientInfo.h"

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
	__int64 mask = ((__int64)0x1f << (64 - 10));

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
	__int64 bitProtocol;
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

			// ��� �÷��̾�鿡�� ������ �����϶�� ���������� ������ �ΰ��ӻ��·� �Ѿ
			protocol = SetProtocol(LOBBY_STATE, PROTOCOL_LOBBY::START_PROTOCOL, RESULT_LOBBY::MATCH_SUCCESS);
			ZeroMemory(buf, sizeof(BUFSIZE));

			// ��ŷ �� ����
			int packetSize = 0;
			//PackPacket(buf, nullptr, packetSize);	// �� �κ��� null�� �Ѱ��� �� ó���� ����??
			_ptr->GetRoom()->team1->player1->SendPacket(protocol, buf, packetSize);
			_ptr->GetRoom()->team1->player2->SendPacket(protocol, buf, packetSize);
			_ptr->GetRoom()->team2->player1->SendPacket(protocol, buf, packetSize);
			_ptr->SendPacket(protocol, buf, packetSize);

			wprintf(L"1�� : %s, %s\n2�� : %s, %s\n"
				, _ptr->GetRoom()->team1->player1->GetUserInfo()->id
				, _ptr->GetRoom()->team1->player2->GetUserInfo()->id
				, _ptr->GetRoom()->team2->player1->GetUserInfo()->id
				, _ptr->GetRoom()->team2->player2->GetUserInfo()->id);

			return true;
		}
		
	}

	return false;
}

bool LobbyManager::CanILeaveLobby(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// �κ񿡼� Logout�� ��û�ߴٸ�, LoginList�� �����ϴ� LoginManager�� CanILogout()�� ȣ���ؼ� �˻�޾ƾ��Ѵ�.
	if (protocol == LOGOUT_PROTOCOL)
		return LoginManager::GetInstance()->CanILogout(_ptr);

	return false;
}

// ������ ������ �� �ִ���
bool LobbyManager::CanIStart(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// ���� 4�� ��Ī�� �����Ͽ� �����ߴ� Ŭ�� ������ ���� ���������� �����ٸ� �ΰ������� �����Ѵ�.
	if (protocol == START_PROTOCOL)
		return true;

	return false;
}
