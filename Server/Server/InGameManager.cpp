#include "stdafx.h"
#include "InGameManager.h"
#include "RoomManager.h"
#include "C_ClientInfo.h"

InGameManager* InGameManager::instance;

InGameManager* InGameManager::GetInstance()
{
	if (instance == nullptr)
		instance = new InGameManager();

	return instance;
}
void InGameManager::Destroy()
{
	delete instance;
}

void InGameManager::Init()
{
}

void InGameManager::End()
{
}

void InGameManager::PackPacket(char* _setptr, const int &_sec, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// �� ����
	memcpy(ptr, &_sec, sizeof(_sec));
	ptr = ptr + sizeof(_sec);
	_size = _size + sizeof(_sec);
}

void InGameManager::PackPacket(char* _setptr, TCHAR* _str1, int& _size)
{
	char* ptr = _setptr;
	int strsize1 = (int)_tcslen(_str1) * sizeof(TCHAR);
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

void InGameManager::UnPackPacket(char* _getBuf, PositionPacket& _struct)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// ����ü ����
	memcpy(&_struct, ptr, sizeof(PositionPacket));
	ptr = ptr + sizeof(PositionPacket);
}

void InGameManager::PackPacket(char* _setptr, PositionPacket& _struct, unsigned char _playerBit, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// ������ ��Ŷ
	memcpy(ptr, &_struct, sizeof(PositionPacket));
	ptr = ptr + sizeof(PositionPacket);
	_size = _size + sizeof(PositionPacket);

	// �÷��̾� ��Ʈ
	memcpy(ptr, &_playerBit, sizeof(byte));
	ptr = ptr + sizeof(byte);
	_size = _size + sizeof(byte);
}

void InGameManager::UnPackPacket(char* _getBuf, Weapon* &_weapon)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// ����ü ����
	memcpy(_weapon, ptr, sizeof(Weapon));
	ptr = ptr + sizeof(Weapon);
}

void InGameManager::GetProtocol(PROTOCOL_INGAME& _protocol)
{
	// major state�� ������(Ŭ��� state�� �Ⱥ����ϱ�(Ȥ�ó� ���Ŀ� �����ԵǸ� �̺κ��� ����)) protocol�� �������� ���ؼ� ���� 10��Ʈ ��ġ�� ����ũ�� ����
	__int64 mask = ((__int64)PROTOCOL_OFFSET << (64 - PROTOCOL_MASK));

	// ����ũ�� �ɷ��� 1���� ���������� ����ȴ�. 
	PROTOCOL_INGAME protocol = (PROTOCOL_INGAME)(_protocol & (PROTOCOL_INGAME)mask);

	// �ƿ�ǲ�� �����̹Ƿ� �������ش�.
	// ���߿� �ѹ��� �������ִ� ������ ���߿� �߰��� ���� �� �ִ� result �� ���ؼ� protocol �� ������� ���� 
	_protocol = protocol;
}
InGameManager::PROTOCOL_INGAME InGameManager::SetProtocol(STATE_PROTOCOL _state, PROTOCOL_INGAME _protocol, RESULT_INGAME _result)
{
	// �ϼ��� ���������� ���� 
	PROTOCOL_INGAME protocol = (PROTOCOL_INGAME)0;
	protocol = (PROTOCOL_INGAME)(_state | _protocol | _result);
	return protocol;
}

InGameManager::PROTOCOL_INGAME InGameManager::GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf)
{
	__int64 bitProtocol = 0;
	_ptr->GetPacket(bitProtocol, _buf);	// �켱 �ɷ��������� ���������� �����´�.

	// ��¥ ���������� ������ �ش�.(�ȿ��� �������� AND �˻�)
	PROTOCOL_INGAME realProtocol = (PROTOCOL_INGAME)bitProtocol;
	GetProtocol(realProtocol);

	return realProtocol;
}


bool InGameManager::WeaponSelectProcess(C_ClientInfo* _ptr, char* _buf)
{
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize;

	RESULT_INGAME itemSelect = RESULT_INGAME::INGAME_FAIL;

	Weapon* tmpWeapon = new Weapon();
	UnPackPacket(_buf, tmpWeapon);
	

	// Ŭ�� ���� �������� ����!
	if (tmpWeapon != nullptr)
	{
		_ptr->SetWeapon(tmpWeapon);
		itemSelect = RESULT_INGAME::INGAME_SUCCESS;

		wprintf(L"%s ���� ���� : %d, %d\n", _ptr->GetUserInfo()->id, _ptr->GetWeapon()->mainW, _ptr->GetWeapon()->subW);
	}

	// �������� ����(�ΰ��� ���·�)
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::START_PROTOCOL, itemSelect);

	ZeroMemory(buf, sizeof(BUFSIZE));

	// ��ŷ �� ����
	PackPacket(buf, msg, packetSize);	/// msg���µ� ���� �����ʿ����
	_ptr->SendPacket(protocol, buf, packetSize);


	if (itemSelect == RESULT_INGAME::INGAME_SUCCESS)
	{
		_ptr->GetRoom()->SetRoomStatus(ROOMSTATUS::ROOM_GAME);	// ���� �������� �����Ͽ���.
		return true;
	}

	return false;
}

bool InGameManager::InitProcess(C_ClientInfo* _ptr, char* _buf)
{
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize;

	RESULT_INGAME result = RESULT_INGAME::INGAME_SUCCESS;

	PositionPacket tmpPos;
	UnPackPacket(_buf, tmpPos);

	// �÷��̾��� ��ġ ������ �����صд�.
	PositionPacket* position = new PositionPacket(tmpPos);
	_ptr->SetPosition(position);

	// ������ ���
	printf("[�ʱ��ε���]%d ,%f, %f, %f, %d\n", position->playerNum, position->posX, position->posZ, position->rotY, position->action);

	C_Sector* sector = _ptr->GetRoom()->GetSector();	// �� ���� ���� �Ŵ����� ��´�.

	// �÷��̾� ��ġ�� ���� ���� ����� �ε����� ���ؼ� �ش� ���Ϳ� �߰��Ѵ�.
	INDEX index = sector->GetIndex(tmpPos.posX, tmpPos.posZ);
	_ptr->SetIndex(index);
	sector->Add(_ptr, index);

	if (result == RESULT_INGAME::INGAME_SUCCESS)
		return true;

	return false;
}

bool InGameManager::MoveProcess(C_ClientInfo* _ptr, char* _buf)
{
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize;

	RESULT_INGAME move = RESULT_INGAME::INGAME_SUCCESS;

	// ���޵� ��ġ ������ ����
	PositionPacket tmpPos;
	UnPackPacket(_buf, tmpPos);

	// �÷��̾��� ��ġ ������ �����صд�.
	PositionPacket* position = new PositionPacket(tmpPos);
	_ptr->SetPosition(position);


	// ������ ���
	printf("%d ,%f, %f, %f, %d\n", position->playerNum, position->posX, position->posZ, position->rotY, position->action);
	

	// �� ���� ���� �Ŵ����� ��´�.
	C_Sector* sector = _ptr->GetRoom()->GetSector();	

	// 1. �÷��̾� ��ġ�� ���� ���� ����� �ε����� ���Ѵ�.
	INDEX index = sector->GetIndex(tmpPos.posX, tmpPos.posZ);
	
	// ���� ���� �ִ� �ε��� ������ ���� �̵��� �ε��� ������ �ٸ��ٸ� ���� �ִ� ����Ʈ���� ����, ���� ���� ����Ʈ�� �߰����ش�.
	if (index != _ptr->GetIndex())
	{
		sector->Delete(_ptr, _ptr->GetIndex());			// ������ �ִ� �ε��������� ����
		sector->Add(_ptr, index);						// ���ο� �ε�����ġ ����Ʈ�� �߰��Ѵ�.
		_ptr->SetIndex(index);							// �ε��� ����

		/// ��ġ�� �̵������� ������ ���� ���͵鿡�� ���� �̵��ߴٰ� �˷�����ϰ�,
		/// ��ġ�� �̵������� ���Ӱ� ���� ���� ���͵鿡�� ���� �Դٰ� �˷������.. ���� �ù� �����ϳ�
	}

	// 2. ������ �ε����� �ش� ���Ϳ� ���� ������ �÷��̾� ����Ʈ�� �����Ͽ� ���, ���� ���Ϳ� �ִ� �÷��̾��� bit�� ��´�.
	byte playerBit = 0;	// �÷��̾� ��Ʈ
	list<C_ClientInfo*>sendList = sector->GetMergedPlayerList(index, playerBit);

	// ������ sendList�� ������� ���� ��쿡��
	if (!sendList.empty())
	{
		// 3. �������� ���� �� ��ŷ
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, move);
		ZeroMemory(buf, sizeof(BUFSIZE));
		PackPacket(buf, tmpPos, playerBit, packetSize);

		// 4. ������ ������ �ٸ� �÷��̾�鿡�� ��ġ�� �����Ѵ�.
		C_ClientInfo* player = nullptr;
		for (list<C_ClientInfo*>::iterator iter = sendList.begin(); iter != sendList.end(); ++iter)
		{
			player = *iter;

			// �ڱ�� ����
			if (_ptr == player)
				continue;

			player->SendPacket(protocol, buf, packetSize);
		}
	}

	if (move == RESULT_INGAME::INGAME_SUCCESS)
		return true;

	return false;
}

bool InGameManager::LeaveProcess(C_ClientInfo* _ptr, int _playerNum)
{
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize;

	// ���� �������� ����
	protocol = SetProtocol(
		STATE_PROTOCOL::INGAME_STATE, 
		PROTOCOL_INGAME::DISCONNECT_PROTOCOL, 
		RESULT_INGAME::INGAME_SUCCESS);

	ZeroMemory(buf, sizeof(BUFSIZE));

	// ��ŷ
	PackPacket(buf, _playerNum, packetSize);

	// �ڽ��� ������ �ٸ� Ŭ��鿡�� �ڽ��� �������� �˸���.
	C_ClientInfo* player = nullptr;
	while (_ptr->GetRoom()->GetPlayer(player) == true)
	{
		// �ڱ�� ����
		if (_ptr == player)
			continue;

		player->SendPacket(protocol, buf, packetSize);
	}

	return true;
}

bool InGameManager::CanISelectWeapon(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);

	if (protocol == WEAPON_PROTOCOL)
		return WeaponSelectProcess(_ptr, buf);

	return false;
}

bool InGameManager::CanIStart(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);

	if (protocol == START_PROTOCOL)
		return InitProcess(_ptr, buf);

	return false;
}

bool InGameManager::CanIMove(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);

	// �̵� �������� ������ �� �̵� ���μ��� ����
	if (protocol == MOVE_PROTOCOL)
		return MoveProcess(_ptr, buf);

	return false;
}


// ���� ���� Ÿ�̸� ������
unsigned long __stdcall InGameManager::TimerThread(void* _arg)
{
	C_ClientInfo* ptr = (C_ClientInfo*)_arg;

	PROTOCOL_INGAME protocol = (PROTOCOL_INGAME)0;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	
	LARGE_INTEGER frequency;
	LARGE_INTEGER beginTime;
	LARGE_INTEGER endTime;
	__int64 elapsed;
	double duringTime = 0.0;	// ������ ����� �ð���(�ʴ���) ������ ���� double ����
	int sec = 0;				// �� ����

	QueryPerformanceFrequency(&frequency);	// ���� 1ȸ ���ļ� ����
	QueryPerformanceCounter(&beginTime);	// ���� �ð� ����
	while (1)
	{
		QueryPerformanceCounter(&endTime);					// ���� �ð� ����
		elapsed = endTime.QuadPart - beginTime.QuadPart;	// ����� �ð� ���
		
		
		duringTime = (double)elapsed / (double)frequency.QuadPart;	// ������ �帥 �ð��� �� ������ ���
		
		// ���� ������ ���ýð�(���)�� �Ѿ��ٸ� ������ �ڵ� �ݳ� ��, ���ѷ����� ����������.
		if (duringTime >= WEAPON_SELTIME)
		{
			// ������ �ڵ� �ݳ�
			CloseHandle(ptr->GetRoom()->GetWeaponTimerHandle());
			ptr->GetRoom()->SetWeaponTimerHandle(nullptr);

			// ���� ������ ���������� �������� ����
			protocol = InGameManager::GetInstance()->SetProtocol(INGAME_STATE, PROTOCOL_INGAME::WEAPON_PROTOCOL, RESULT_INGAME::NODATA);
			ZeroMemory(buf, BUFSIZE);
			packetSize = 0;

			// ���� �濡 �ִ� "���" �÷��̾�� ���⸦ ������� ���������� ������.
			C_ClientInfo* player = nullptr;
			while (ptr->GetRoom()->GetPlayer(player) == true)
				player->SendPacket(protocol, buf, packetSize);

			break;
		}

		// 1�ʸ��� ���� ���ϸ�
		if (sec < (int)duringTime)
		{
			sec = (int)duringTime;	// ���Ӱ� �� ������ ���Ž����ش�.(before sec�� ����)

			// 1�ʿ� �� ���� �ð��� �˷��ִ� ���������� ����.
			protocol = InGameManager::GetInstance()->SetProtocol(INGAME_STATE, PROTOCOL_INGAME::TIMER_PROTOCOL, RESULT_INGAME::NODATA);
			ZeroMemory(buf, BUFSIZE);
			packetSize = 0;

			// ���� ����������� ���� �ð� ��Ŷ ����
			InGameManager::GetInstance()->PackPacket(buf, (WEAPON_SELTIME - sec), packetSize);

			// ���� �濡 �ִ� "���" �÷��̾�� ���� ���� ����������� ���� �ð��� ������
			C_ClientInfo* player = nullptr;
			while (ptr->GetRoom()->GetPlayer(player) == true)
				player->SendPacket(protocol, buf, packetSize);
		}
	}

	return 0;	// �׸��� ������ ����
}
