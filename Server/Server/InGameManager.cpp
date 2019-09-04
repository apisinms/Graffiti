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

void InGameManager::PackPacket(char* _setptr, PositionPacket& _struct, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// ������ ��Ŷ
	memcpy(ptr, &_struct, sizeof(PositionPacket));
	ptr = ptr + sizeof(PositionPacket);
	_size = _size + sizeof(PositionPacket);
}

void InGameManager::PackPacket(char* _setptr, byte _playerBit, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// ��Ʈ ����
	memcpy(ptr, &_playerBit, sizeof(byte));
	ptr = ptr + sizeof(byte);
	_size = _size + sizeof(byte);
}

void InGameManager::UnPackPacket(char* _getBuf, PositionPacket& _struct)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// ����ü ����
	memcpy(&_struct, ptr, sizeof(PositionPacket));
	ptr = ptr + sizeof(PositionPacket);
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
   PROTOCOL_INGAME protocol;
   char buf[BUFSIZE] = { 0, };
   int packetSize;

   // ���޵� ��ġ ������ ����
   PositionPacket movedPos;
   UnPackPacket(_buf, movedPos);
   printf("%d ,%f, %f, %f, %d\n", movedPos.playerNum, movedPos.posX, movedPos.posZ, movedPos.rotY, movedPos.action);

   // �÷��̾��� ��ġ ������ �����صд�.
   PositionPacket* position = new PositionPacket(movedPos);
   _ptr->SetPosition(position);

   // �� ���� ���� �Ŵ����� ��´�.
   C_Sector* sector = _ptr->GetRoom()->GetSector();   

   // 1. �÷��̾� ��ġ�� ���� ���� ����� �ε����� ���Ѵ�.
   INDEX index = sector->GetIndex(movedPos.posX, movedPos.posZ);
   
   // ���� ���� �ִ� �ε��� ������ ���� �̵��� �ε��� ������ �ٸ��ٸ� ���� �ִ� ����Ʈ���� ����, ���� ���� ����Ʈ�� �߰����ش�.
   if (index != _ptr->GetIndex())
   {
      sector->Delete(_ptr, _ptr->GetIndex());         // ������ �ִ� �ε��������� ����
      sector->Add(_ptr, index);                  // ���ο� �ε�����ġ ����Ʈ�� �߰��Ѵ�.

      list<C_ClientInfo*>exitList = sector->GetMergedPlayerList(_ptr->GetIndex());   // ������ ���� ����Ʈ
      list<C_ClientInfo*>enterList    = sector->GetMergedPlayerList(index);         // ������ ���� ����Ʈ

      // 1. ���� ���� �˸� ��Ŷ ���� �� ����
      protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::EXIT_SECTOR);
      PackPacket(buf, movedPos, packetSize);

      ListSendPacket(exitList, _ptr, protocol, buf, packetSize);

      // 2. ���� ���� �˸� ��Ŷ ���� �� ����
      protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::ENTER_SECTOR);
      PackPacket(buf, movedPos, packetSize);

      ListSendPacket(enterList, _ptr, protocol, buf, packetSize);

      // 3. ���ο��Դ� ���Ӱ� ������ ���� ������ �÷��̾� ����Ʈ�� �����ش�.
      byte playerBit = 0;
      FlagPlayerBit(enterList, playerBit);
      protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::UPDATE_PLAYER);
      PackPacket(buf, playerBit, packetSize);

      _ptr->SendPacket(protocol, buf, packetSize);   // ����

      _ptr->SetIndex(index);            // ����� ���ο� �ε��� ����
   }

   // 2. ������ �ε����� �ش� ���Ϳ� ���� ������ �÷��̾� ����Ʈ�� �����Ͽ� ��´�.
   list<C_ClientInfo*>sendList = sector->GetMergedPlayerList(index);

   // 3. ���� �� �����̵� �������� ���� �� ��ŷ
   protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::INGAME_SUCCESS);
   PackPacket(buf, movedPos, packetSize);

   ListSendPacket(sendList, _ptr, protocol, buf, packetSize);

   return true;   // �� �� true��
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

void InGameManager::ListSendPacket(list<C_ClientInfo*> _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize)
{
	// ������ List�� ������� ���� ��쿡��
	if (!_list.empty())
	{
		// ����Ʈ �ȿ��ִ� �÷��̾�鿡�� ��Ŷ�� �����Ѵ�.
		C_ClientInfo* player = nullptr;
		for (list<C_ClientInfo*>::iterator iter = _list.begin(); iter != _list.end(); ++iter)
		{
			player = *iter;

			// ���� ������ Ŭ�� �ǳʶ�
			if (player == _exceptClient)
				continue;

			player->SendPacket(_protocol, _buf, _packetSize);
		}
	}
}

void InGameManager::FlagPlayerBit(list<C_ClientInfo*> _list, byte& _playerBit)
{
	_playerBit = 0;
	// ���� �ٹ� ���Ϳ� �ִ� �÷��̾���� bit�� Ȱ��ȭ ���Ѽ� ������ ������.
	for (auto iter = _list.begin(); iter != _list.end(); ++iter)
	{
		// �÷��̾� �ѹ��� �о bit�� Ȱ��ȭ ��Ŵ(ex : 1011 << 1,3,4 �÷��̾ ���� ���Ϳ� ����)
		switch (((C_ClientInfo*)(*iter))->GetPosition()->playerNum)
		{
		case 1:
			_playerBit |= PLAYER_1;
			break;

		case 2:
			_playerBit |= PLAYER_2;
			break;

		case 3:
			_playerBit |= PLAYER_3;
			break;

		case 4:
			_playerBit |= PLAYER_4;
			break;
		}
	}
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
