#include "LobbyManager.h"
#include "LoginManager.h"
#include "LogManager.h"
#include "MatchManager.h"
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


//bool LobbyManager::CreateRoomProcess(C_ClientInfo* _ptr, char* _buf)
//{
//	RESULT_LOBBY createResult = RESULT_LOBBY::NODATA;
//	TCHAR msg[MSGSIZE] = { 0, };
//	PROTOCOL_LOBBY protocol;
//
//	char buf[BUFSIZE];		// ����
//	int packetSize = 0;     // �� ������
//
//	// �Ѱܿ��� ���� : �� ������ ��´�.
//	TCHAR tmpRoomTitle[ROOM_TITLE_LEN] = { 0, };
//	UnPackPacket(_buf, tmpRoomTitle);
//
//	// ���� ���� ������ �ִ밹���� �����ʾҴٸ� ������ �� �ִ�.
//	if (roomList->GetCount() < MAX_ROOM_SIZE)
//		createResult = RESULT_LOBBY::CREATE_ROOM_SUCCESS;
//	else
//		createResult = RESULT_LOBBY::CREATE_ROOM_FAIL;
//
//	switch (createResult)
//	{
//	case RESULT_LOBBY::CREATE_ROOM_FAIL:
//		_tcscpy_s(msg, MSGSIZE, CREATE_ROOM_FAIL_MSG);
//		break;
//
//
//	case RESULT_LOBBY::CREATE_ROOM_SUCCESS:
//	{
//		// ���� �޽��� ���� ��
//		_tcscpy_s(msg, MSGSIZE, CREATE_ROOM_SUCCESS_MSG);
//
//		// �� ����Ʈ�� �߰��Ѵ�.
//		RoomInfo* ptr = new RoomInfo(_ptr, tmpRoomTitle);
//		roomList->Insert(ptr);
//		_ptr->SetRoomNum(ptr->roomNum);	// �� ��ȣ ����
//	}
//	break;
//	}
//
//	// �������� ���� 
//	protocol = SetProtocol(LOBBY_STATE, PROTOCOL_LOBBY::CREATE_ROOM_PROTOCOL, createResult);
//	ZeroMemory(buf, sizeof(BUFSIZE));
//
//	// ��ŷ �� ����
//	PackPacket(buf, msg, packetSize);
//	_ptr->SendPacket(protocol, buf, packetSize);
//
//	if (createResult == RESULT_LOBBY::CREATE_ROOM_SUCCESS)
//		return true;
//
//	return false;
//}
//
//bool LobbyManager::EnterRoomProcess(C_ClientInfo* _ptr, char* _buf)
//{
//	RESULT_LOBBY enterResult = RESULT_LOBBY::NODATA;
//	TCHAR msg[MSGSIZE] = { 0, };
//	PROTOCOL_LOBBY protocol;
//
//	char buf[BUFSIZE];		// ����
//	int packetSize = 0;     // �� ������
//
//	// �Ѱܿ��� ���� : �� ��ȣ�� ��´�.
//	TCHAR tmpRoomNum[ROOM_NUM_LEN] = { 0, };
//	UnPackPacket(_buf, tmpRoomNum);
//
//	// �� ��ȣ�� �� ����Ʈ�� ������ �� �� �ִ�.(���ϰ��� ���尡���� �� �� ������)
//	RoomInfo* room = CheckEnterRoom(_tstoi(tmpRoomNum), enterResult);
//
//	switch (enterResult)
//	{
//	case RESULT_LOBBY::ENTER_ROOM_FAIL:
//		_tcscpy_s(msg, MSGSIZE, ENTER_ROOM_FAIL_MSG);
//		break;
//
//
//	case RESULT_LOBBY::ENTER_ROOM_SUCCESS:
//	{
//		// ���� �޽��� ���� ��
//		_tcscpy_s(msg, MSGSIZE, ENTER_ROOM_SUCCESS_MSG);
//
//		// �� ����Ʈ�� �߰��Ѵ�.
//		room->userList->Insert(_ptr);
//		_ptr->SetRoomNum(_ttoi(tmpRoomNum));	// �� ��ȣ ����
//	}
//	break;
//	}
//
//	// �������� ���� 
//	protocol = SetProtocol(LOBBY_STATE, PROTOCOL_LOBBY::ENTER_ROOM_PROTOCOL, enterResult);
//	ZeroMemory(buf, sizeof(BUFSIZE));
//
//	// ��ŷ �� ����
//	PackPacket(buf, msg, packetSize);
//	_ptr->SendPacket(protocol, buf, packetSize);
//
//	if (enterResult == RESULT_LOBBY::ENTER_ROOM_SUCCESS)
//		return true;
//
//	return false;
//}
//RoomInfo* LobbyManager::CheckEnterRoom(int _roomNum, RESULT_LOBBY& _result)
//{
//	_result = RESULT_LOBBY::NODATA;
//	RoomInfo* info = nullptr;	// SearchData�� ���Ϲ޾Ƽ� ����Ʈ�� ��ȸ�ϴ� �뵵�� ����
//
//	// ���� �� ����Ʈ�� �˻����� �ƴ϶��
//	if (roomList->SearchCheck() == false)
//	{
//		roomList->SearchStart();	// �˻� ����
//		while (1)
//		{
//			info = roomList->SearchData();	// �����͸� �޾Ƽ�
//			if (info == nullptr)	// �����Ͱ� nullptr�̸� ����Ʈ�� ������ ���̻� ����.
//			{
//				// ������ �߰��Ѱ� �ƹ��͵� ���ٸ�, �� ���忡 �����Ѵ�.
//				if (_result == RESULT_LOBBY::NODATA)
//					_result = RESULT_LOBBY::ENTER_ROOM_FAIL;
//
//				break;
//			}
//
//			// �� ��ȣ�� ��ġ�ϴ� ���� ���� �� ����Ʈ�� �ִٸ� ���� �����̴�.
//			if (info->roomNum == _roomNum)
//			{
//				_result = RESULT_LOBBY::ENTER_ROOM_SUCCESS;
//				break;
//			}
//		}
//		roomList->SearchEnd();	// �˻� ����
//	}
//
//	return info;
//}
//
//bool LobbyManager::RoomListProcess(C_ClientInfo* _ptr, char* _buf)
//{
//	TCHAR msg[HALF_BUFSIZE] = { 0, };
//	PROTOCOL_LOBBY protocol;
//
//	char buf[BUFSIZE];		// ����
//	int packetSize = 0;     // �� ������
//
//	// ��� �� ������ �ٿ��� ����
//	if (GetRoomList(msg) == false)
//	{
//		// ������ ���� ������ ���ڿ��� ����
//		_tcscpy_s(msg, HALF_BUFSIZE, ROOMLIST_NOEXIST_MSG);
//	}
//
//	// �������� ���� 
//	protocol = SetProtocol(LOBBY_STATE, PROTOCOL_LOBBY::ROOMLIST_PROTOCOL, (RESULT_LOBBY)0);
//	ZeroMemory(buf, sizeof(BUFSIZE));
//
//	// ��ŷ �� ����
//	PackPacket(buf, msg, packetSize);
//	_ptr->SendPacket(protocol, buf, packetSize);
//
//	return true;	// ���� �����Ȱ� �ֵ� ���� ��� ����
//}
//
//bool LobbyManager::GetRoomList(TCHAR* _msg)
//{
//	// ���� �� ����Ʈ�� �˻����� �ƴ϶��
//	if (roomList->SearchCheck() == false)
//	{
//		RoomInfo* info = nullptr;	// ����Ʈ�� ��ȸ�ϸ鼭 �޾ƿ� �� ���� ������
//
//		roomList->SearchStart();	// �˻� ����
//		while (1)
//		{
//			info = roomList->SearchData();	// �����͸� �޾Ƽ�
//			if (info == nullptr)	// �����Ͱ� nullptr�̸� ����Ʈ�� ������ ���̻� ����.
//				break;
//
//			else
//			{
//				if (lstrlen(_msg) == 0)
//					_tcscat_s(_msg, HALF_BUFSIZE, TEXT("<<<�� ���>>>\n"));
//
//				TCHAR frame[MSGSIZE] = { 0, };
//				wsprintf(frame, TEXT("[%d����]\t%s\n"), info->roomNum, info->roomTitle);
//				_tcscat_s(_msg, HALF_BUFSIZE, frame);
//			}
//		}
//		roomList->SearchEnd();	// �˻� ����
//	}
//
//	// ������ �� ������ �ִٸ� true����
//	if (lstrlen(_msg) > 0)
//		return true;
//
//	else
//		return false;
//}
//
//
//bool LobbyManager::CheckLeaveRoom(C_ClientInfo* _ptr)
//{
//	bool retFlag = false;
//
//	// ���� �� ����Ʈ�� �˻����� �ƴ϶��
//	if (roomList->SearchCheck() == false)
//	{
//		RoomInfo* roomInfo = nullptr;	// SearchData�� ���Ϲ޾Ƽ� ����Ʈ�� ��ȸ�ϴ� �뵵�� ����
//
//		roomList->SearchStart();	// �˻� ����
//		while (1)
//		{
//			roomInfo = roomList->SearchData();	// �����͸� �޾Ƽ�
//			if (roomInfo == nullptr)	// �����Ͱ� nullptr�̸� ����Ʈ�� ������ ���̻� ����.
//				break;
//
//
//			// �� Ŭ�� ���� ��(�� ��ȣ��ã��)�� ��������Ʈ���� ���ش�
//			if (roomInfo->roomNum == _ptr->GetRoomNum())
//			{
//				// �켱 �ش� �濡 ���� ������ ������ �����Ѵ�.
//				roomInfo->userList->Delete(_ptr);
//
//				// �ٵ� ���� ��� ���� ������ ���� ������ ����̾��ٸ� �� �浵 �����Ѵ�.
//				if (roomInfo->userList->GetCount() == 0)
//					roomList->Delete(roomInfo);
//
//				retFlag = true;
//			}
//
//			if (retFlag == true)
//				break;
//		}
//		roomList->SearchEnd();	// �˻� ����
//	}
//
//	return retFlag;
//}
//
////POSITION LobbyManager::RecvProcess(C_ClientInfo* _ptr)
////{
////	LogManager::GetInstance()->ErrorPrintf("�κ�Ŵ��� ���ú� ���μ���\n");
////
////	__int64 bitProtocol;
////	char buf[BUFSIZE]; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
////	_ptr->GetPacket(bitProtocol, buf);	// �켱 �ɷ��������� ���������� �����´�.
////
////	// ��¥ ���������� ������ �ش�.(�ȿ��� �������� AND �˻�)
////	PROTOCOL_LOBBY realProtocol = (PROTOCOL_LOBBY)bitProtocol;
////	GetProtocol(realProtocol);
////	switch (realProtocol)
////	{
////	case ENTER_ROOM_PROTOCOL:
////		if(EnterRoomProcess(_ptr, buf) == true)
////			return ROOM;
////
////	case CREATE_ROOM_PROTOCOL:
////		if(CreateRoomProcess(_ptr, buf) == true)
////			return ROOM;
////
////	case ROOMLIST_PROTOCOL:
////		if (RoomListProcess(_ptr, buf) == true)
////			return LOBBY;
////
////	case LOGOUT_PROTOCOL:
////		if(LoginManager::GetInstance()->LogoutProcess(_ptr) == true)
////		return LOGIN;
////	}
////
////	return LOBBY;
////}
////bool LobbyManager::SendProcess(C_ClientInfo* _ptr)
////{
////	LogManager::GetInstance()->ErrorPrintf("�κ�Ŵ��� ���� ���μ���\n");
////
////	return true;
////}
//
//
//
//bool LobbyManager::CanIEnterRoom(C_ClientInfo* _ptr)
//{
//	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
//	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);
//
//	if (protocol == ENTER_ROOM_PROTOCOL)
//		return EnterRoomProcess(_ptr, buf);
//
//	return false;
//}
//
//bool LobbyManager::CanICreateRoom(C_ClientInfo* _ptr)
//{
//	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
//	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);
//
//	if (protocol == CREATE_ROOM_PROTOCOL)
//		return CreateRoomProcess(_ptr, buf);
//
//	return false;
//}
//
//bool LobbyManager::CanIGetRoomList(C_ClientInfo* _ptr)
//{
//	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
//	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);
//
//	if (protocol == ROOMLIST_PROTOCOL)
//		return RoomListProcess(_ptr, buf);
//
//	return false;
//}
//
//bool LobbyManager::CanILeaveLobby(C_ClientInfo* _ptr)
//{
//	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
//	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);
//
//	// �κ񿡼� Logout�� ��û�ߴٸ�, LoginList�� �����ϴ� LoginManager�� CanILogout()�� ȣ���ؼ� �˻�޾ƾ��Ѵ�.
//	if (protocol == LOGOUT_PROTOCOL)
//		return LoginManager::GetInstance()->CanILogout(_ptr);
//
//	return false;
//}
//
//
//C_ClientInfo* LobbyManager::GetRoomClient(int _roomNum)
//{
//	static C_ClientInfo* ptr = nullptr;
//
//	// ���� �� ����Ʈ�� �˻����� �ƴ϶��
//	if (roomList->SearchCheck() == false)
//	{
//		RoomInfo* roomInfo = nullptr;	// SearchData�� ���Ϲ޾Ƽ� ����Ʈ�� ��ȸ�ϴ� �뵵�� ����
//
//		roomList->SearchStart();	// �˻� ����
//		while (1)
//		{
//			roomInfo = roomList->SearchData();	// �����͸� �޾Ƽ�
//			if (roomInfo == nullptr)	// �����Ͱ� nullptr�̸� ����Ʈ�� ������ ���̻� ����.
//				break;
//
//
//			// ���� ���� ã�Ҵٸ� �ش� ���� ���������� �����Ѵ�.
//			if (roomInfo->roomNum == _roomNum)
//			{
//				// ó�� �޴°Ÿ� �ϴ� �˻� ��带 Ǫ���س��´�.
//				if(ptr == nullptr)
//					roomInfo->userList->SearchStart();
//
//				ptr = roomInfo->userList->SearchData();
//
//				// ��� �� �������� �ٽ� �˻���带 ���Ѵ�.
//				if(ptr == nullptr)
//					roomInfo->userList->SearchEnd();
//
//				break;
//			}
//
//		}
//		roomList->SearchEnd();	// �˻� ����
//	}
//
//	return ptr;
//}


bool LobbyManager::CanIMatch(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_LOBBY protocol = GetBufferAndProtocol(_ptr, buf);

	// �κ񿡼� ��Ī��ư�� �����ٸ�, ��Ī�Ŵ������� ó���ؾ��Ѵ�.
	if (protocol == MATCH_PROTOCOL)
		return MatchManager::GetInstance()->MatchProcess(_ptr);

	return false;
}
