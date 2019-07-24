#include "LobbyManager.h"
#include "LoginManager.h"
#include "LogManager.h"
#include "C_ClientInfo.h"


int RoomInfo::incRoomNum = 0;

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
	roomList = new C_List<RoomInfo*>();
}

void LobbyManager::End()
{
	delete roomList;
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

bool LobbyManager::CanILeaveLobby(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// �κ񿡼� Logout�� ��û�ߴٸ�, LoginList�� �����ϴ� LoginManager�� CanILogout()�� ȣ���ؼ� �˻�޾ƾ��Ѵ�.
	if (protocol == LOGOUT_PROTOCOL)
		return LoginManager::GetInstance()->CanILogout(_ptr);

	return false;
}

bool LobbyManager::StartProcess(C_ClientInfo* _ptr)
{
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_LOBBY protocol;
	char buf[BUFSIZE];
	int packetSize;

	RESULT_LOBBY startResult = RESULT_LOBBY::START_SUCCESS;

	// �������� ����
	protocol = SetProtocol(LOGIN_STATE, PROTOCOL_LOBBY::START_PROTOCOL, startResult);

	ZeroMemory(buf, sizeof(BUFSIZE));

	// ��ŷ �� ����
	PackPacket(buf, msg, packetSize);
	_ptr->SendPacket(protocol, buf, packetSize);


	if (startResult == RESULT_LOBBY::START_SUCCESS)
		return true;

	return false;
}


bool LobbyManager::CanIStart(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// �κ񿡼� Logout�� ��û�ߴٸ�, LoginList�� �����ϴ� LoginManager�� CanILogout()�� ȣ���ؼ� �˻�޾ƾ��Ѵ�.
	if (protocol == START_PROTOCOL)
		return StartProcess(_ptr);

	return false;
}