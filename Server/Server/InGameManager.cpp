#include "stdafx.h"
#include "LogManager.h"
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

void InGameManager::PackPacket(char* _setptr, PositionPacket& _struct, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// ������ ��Ŷ
	memcpy(ptr, &_struct, sizeof(PositionPacket));
	ptr = ptr + sizeof(PositionPacket);
	_size = _size + sizeof(PositionPacket);
}

void InGameManager::UnPackPacket(char* _getBuf, int& _num)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// num
	memcpy(&_num, ptr, sizeof(int));
	ptr = ptr + sizeof(int);
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

void InGameManager::GetResult(char* _buf, RESULT_INGAME& _result)
{
	// �ɷ��������� ���� result ������
	__int64 originalResult;
	memcpy(&originalResult, _buf, sizeof(originalResult));

	// result mask ����(33~24)
	__int64 mask = ((__int64)RESULT_OFFSET << (64 - RESULT_MASK));

	// ����ũ�� �ɷ��� 1���� result�� ����ȴ�. 
	RESULT_INGAME result = (RESULT_INGAME)(originalResult & (PROTOCOL_INGAME)mask);

	// �ƿ�ǲ�� �����̹Ƿ� �������ش�.
	// ���߿� �ѹ��� �������ִ� ������ ���߿� �߰��� ���� �� �ִ� extra data�� ���ؼ� ������� ���� 
	_result = result;
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
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	RESULT_INGAME itemSelect = RESULT_INGAME::INGAME_FAIL;

	Weapon* tmpWeapon = new Weapon();
	UnPackPacket(_buf, tmpWeapon);
	
	// Ŭ�� ���� �������� ����!
	if (tmpWeapon != nullptr)
	{
		_ptr->GetPlayerInfo()->SetWeapon(tmpWeapon);
		itemSelect = RESULT_INGAME::INGAME_SUCCESS;

		wprintf(L"%s ���� ���� : %d, %d\n",
			_ptr->GetUserInfo()->id,
			_ptr->GetPlayerInfo()->GetWeapon()->mainW,
			_ptr->GetPlayerInfo()->GetWeapon()->subW);
	}

	// �������� ����(�ΰ��� ���·�)
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::START_PROTOCOL, itemSelect);

	// ��ŷ �� ����
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
	PositionPacket tmpPos;
	UnPackPacket(_buf, tmpPos);

	// �÷��̾��� ��ġ ������ �����صд�.
	PositionPacket* position = new PositionPacket(tmpPos);
	_ptr->GetPlayerInfo()->SetPosition(position);

	// ������ ���
	printf("[�ʱ��ε���]%d ,%f, %f, %f, %d\n", position->playerNum, position->posX, position->posZ, position->rotY, position->action);

	C_Sector* sector = _ptr->GetRoom()->GetSector();	// �� ���� ���� �Ŵ����� ��´�.

	// �÷��̾� ��ġ�� ���� ���� ����� �ε����� ���ؼ� �ش� ���Ϳ� �߰��Ѵ�.
	INDEX getIdx;
	if (sector->GetIndex(_ptr->GetPlayerInfo()->GetIndex(), getIdx, tmpPos.posX, tmpPos.posZ) == true)
	{
		_ptr->GetPlayerInfo()->SetIndex(getIdx);
		sector->Add(_ptr, getIdx);
	}

	else
		printf("��ȿ���� ���� �ε���!! %d, %d\n", getIdx.i, getIdx.j);

	return true;
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

	// ���� �̵��� �� ���� ������ �ӵ��� ���ڱ� ������ �����δٸ� �׳� �� �����ϰ� �ش� Ŭ�����׸� ���� ������ ���� ��Ŷ�� ������.
#ifdef DEBUG	// ���߿� �������Ҷ� ifdef�� ifndef�� �ٲٸ� ��
	if (movedPos.speed > PlayerInfo::MAX_SPEED ||
		abs(_ptr->GetPlayerInfo()->GetPosition()->posX - movedPos.posX) > 0.1f ||
		abs(_ptr->GetPlayerInfo()->GetPosition()->posZ - movedPos.posZ) > 0.1f)
	{
		printf("[�ҹ��̵�]����:%f, %f\t����:%f, %f",
			_ptr->GetPlayerInfo()->GetPosition()->posX,
			_ptr->GetPlayerInfo()->GetPosition()->posZ,
			movedPos.posX,
			movedPos.posZ);

		LogManager::GetInstance()->HackerFileWrite("[SpeedHack]ID:%s, NICK:%s, Speed:%f\n", _ptr->GetUserInfo()->id, _ptr->GetUserInfo()->nickname, movedPos.speed);
		/// ���Ŀ� ���⼭ Kick�ϴ��� �ϴ°� �ʿ�

		// FORCE_MOVE(���� �̵�) ��� protocol�� ��ŷ
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::FORCE_MOVE);
		
		// ���� �÷��̾ ������ �ִ� ������ ������ movedPos�� �����ϰ�, �̸� �ڽſ��Ը� ����
		memcpy(&movedPos, _ptr->GetPlayerInfo()->GetPosition(), sizeof(PositionPacket));
		PackPacket(buf, movedPos, packetSize);
		_ptr->SendPacket(protocol, buf, packetSize);

		return true;	// ��������
	}
#endif

	// �� ���� ���� �Ŵ����� ��´�.
	C_Sector* sector = _ptr->GetRoom()->GetSector();

	// 1. �÷��̾� ��ġ�� ���� ���� ����� �ε����� ���Ѵ�.
	INDEX getIdx;
	bool isValidIdx = sector->GetIndex(_ptr->GetPlayerInfo()->GetIndex(), getIdx, movedPos.posX, movedPos.posZ);

	//// 2. ������ �ε����� �ش� ���Ϳ� ���� ������ �÷��̾� ����Ʈ�� �����Ͽ� ��´�.
	list<C_ClientInfo*>sendList = sector->GetSectorPlayerList(getIdx);

	C_ClientInfo* exceptClient = nullptr;	// ���� ���� ���
	// ��ȿ�� �ε������
	if (isValidIdx == true)
	{
		exceptClient = _ptr;	// ��ȿ �ε����� �ڽ��� ���� ���� ����̴�.

		// 1. ���� ���� �ִ� �ε��� ������ ���� �̵��� �ε��� ������ �ٸ��ٸ� ���� �ִ� ����Ʈ���� ����, ���� ���� ����Ʈ�� �߰����ش�.
		if (getIdx != _ptr->GetPlayerInfo()->GetIndex())
		{
			sector->Delete(_ptr, _ptr->GetPlayerInfo()->GetIndex());    // ������ �ִ� �ε��������� ����
			sector->Add(_ptr, getIdx);                  // ���ο� �ε�����ġ ����Ʈ�� �߰��Ѵ�.

			list<C_ClientInfo*>enterList;
			list<C_ClientInfo*>exitList;

			// ��, ������ ���� ����Ʈ
			byte playerBit = 0;	// ���Ӱ� ������ ���� ������ �÷��̾� ��Ʈ
			playerBit = sector->GetMovedSectorPlayerList(_ptr->GetPlayerInfo()->GetIndex(), getIdx, enterList, exitList);

			// 1. ���� ���� �˸� ��Ŷ ���� �� ����
			protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::EXIT_SECTOR);
			PackPacket(buf, movedPos, packetSize);

			ListSendPacket(exitList, exceptClient, protocol, buf, packetSize);

			// 2. ���� ���� �˸� ��Ŷ ���� �� ����
			protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::ENTER_SECTOR);
			PackPacket(buf, movedPos, packetSize);

			ListSendPacket(enterList, exceptClient, protocol, buf, packetSize);

			// 3. ���ο��Դ� ���Ӱ� ������ ���� ������ �÷��̾� ����Ʈ�� Ȱ��ȭ�� ��Ʈ�� �����ش�.
			protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::UPDATE_PLAYER);
			PackPacket(buf, playerBit, packetSize);

			_ptr->SendPacket(protocol, buf, packetSize);   // ����

			_ptr->GetPlayerInfo()->SetIndex(getIdx);            // ����� ���ο� �ε��� ����
		}
	
		// ���� �̵� ��� protocol�� ��ŷ
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::INGAME_SUCCESS);
	}

	// �ҹ����� �ε������ ���� �ε����� ������ �߰� ��ġ�� ���� ������ ���ý�Ų��.
	else
	{
		exceptClient = nullptr;	// ��ȿ���� ���� �ε����� ��� Ŭ�󿡰�(�ڽ�����) ��ġ ��Ŷ�� ������ �Ѵ�.

		COORD_DOUBLE LT = sector->GetLeftTop(getIdx);
		COORD_DOUBLE RB = sector->GetRightBottom(getIdx);

		movedPos.posX = (float)(LT.x + RB.x) / 2;
		movedPos.posZ = (float)(LT.z + RB.z) / 2;

		// FORCE_MOVE(���� �̵�) ��� protocol�� ��ŷ
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::FORCE_MOVE);
		
		LogManager::GetInstance()->HackerFileWrite("[TeleportHack]ID:%s, NICK:%s\n", _ptr->GetUserInfo()->id, _ptr->GetUserInfo()->nickname);
		/// ���Ŀ� ���⼭ Kick�ϴ��� �ϴ°� �ʿ�
	}

	// 3. ���� �� �̵� ��Ŷ ����
	PackPacket(buf, movedPos, packetSize);
	ListSendPacket(sendList, exceptClient, protocol, buf, packetSize);

	// ����������, �÷��̾��� ��ġ ������ �����صд�.
	PositionPacket* position = new PositionPacket(movedPos);
	_ptr->GetPlayerInfo()->SetPosition(position);

	return true;   // �� �� true��
}

bool InGameManager::GetPosProcess(C_ClientInfo* _ptr, char* _buf)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize;

	// �÷��̾� num�� �����´�.
	PositionPacket pos;
	int playerNum;
	UnPackPacket(_buf, playerNum);

	// �������� ����
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::GET_OTHERPLAYER_POS);

	// �ݺ��ڷ� �����鼭 playerNum�� ��ġ�ϴ� �÷��̾ ã���� �� �÷��̾��� ��ġ�� �������ش�.
	list<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayerList();	// ����Ʈ�� ����
	C_ClientInfo* player = nullptr;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;
		if (player->GetPlayerInfo()->GetPlayerNum() == playerNum)
		{
			memcpy(&pos, player->GetPlayerInfo()->GetPosition(), sizeof(PositionPacket));
			PackPacket(buf, pos, packetSize);
			_ptr->SendPacket(protocol, buf, packetSize);
			
			return true;
		}
	}

	return false;
}

bool InGameManager::OnFocusProcess(C_ClientInfo* _ptr)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize;

	// �������� ����
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::FOCUS_PROTOCOL, RESULT_INGAME::NODATA);

	// ���ο��� �ٸ� ��� �÷��̾��� �ΰ��� ������ �����ش�(�ϴ��� positionPacket��)
	list<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayerList();	// ����Ʈ ����
	PositionPacket pos;
	C_ClientInfo* player = nullptr;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;

		// ���� ����
		if (player == _ptr)
			continue;

		// �ٸ� �÷��̾� ������ �ڽſ��� ����
		else
		{
			memcpy(&pos, player->GetPlayerInfo()->GetPosition(), sizeof(PositionPacket));
			PackPacket(buf, pos, packetSize);
			_ptr->SendPacket(protocol, buf, packetSize);
		}
	}

	return true;
}

bool InGameManager::LeaveProcess(C_ClientInfo* _ptr, int _playerNum)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize;

	// ���� �������� ����
	protocol = SetProtocol(
		STATE_PROTOCOL::INGAME_STATE, 
		PROTOCOL_INGAME::DISCONNECT_PROTOCOL, 
		RESULT_INGAME::INGAME_SUCCESS);

	// ��ŷ
	PackPacket(buf, _playerNum, packetSize);

	// �ڽ��� ������ �ٸ� Ŭ��鿡�� �ڽ��� �������� �˸���.
	list<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayerList();	// ����Ʈ ����
	C_ClientInfo* player = nullptr;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;

		// ���� ����
		if (player == _ptr)
			continue;

		// �ٸ� �÷��̾�� �ڽ��� �������� ����
		else
			player->SendPacket(protocol, buf, packetSize);
	}

	// ���� �÷��̾��� ����, �������� ����Ʈ���� �����ְ�
	_ptr->GetRoom()->GetSector()->LeaveProcess(_ptr, _ptr->GetPlayerInfo()->GetIndex());

	// �ϴ� ��� ��� true��� �����ϰ� �����Ѵ�.
	return true;
}


//////// public
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
	RESULT_INGAME result;
	GetResult(buf, result);

	// �̵� �������� ������ �� �̵� ���μ��� ����
	if (protocol == MOVE_PROTOCOL)
	{
		switch (result)
		{
			// �ٸ� ��� ��ġ ��û �Ͻ�
		case GET_OTHERPLAYER_POS:
			return GetPosProcess(_ptr, buf);

			// �׳� �̵��� ��
		default:
			return MoveProcess(_ptr, buf);
		}
	}

	return false;
}

bool InGameManager::CanIChangeFocus(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);
	RESULT_INGAME result;
	GetResult(buf, result);

	// ��Ŀ�� ���� �������� ������ ��
	if (protocol == FOCUS_PROTOCOL)
	{
		switch (result)
		{
			// focus on�� �Ѱ�, ������ Process���� �� bool�� ����!
		case INGAME_SUCCESS:
			_ptr->GetPlayerInfo()->FocusOn();
			return OnFocusProcess(_ptr);

			// focus off��
		case INGAME_FAIL:
			_ptr->GetPlayerInfo()->FocusOff();
			break;
		}
	}

	return true;
}

void InGameManager::ListSendPacket(list<C_ClientInfo*> _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept)
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

			// ��Ŀ�� ������ ������ �ʱⰡ �����Ǿ��ٸ�
			if (_notFocusExcept)
			{
				// ��Ŀ���� ���� ����� ��Ŷ ������ �ǳ� ��
				if (player->GetPlayerInfo()->GetFocus() == false)
					continue;
			}

			player->SendPacket(_protocol, _buf, _packetSize);
		}
	}
}

// ���� ���� Ÿ�̸� ������
unsigned long __stdcall InGameManager::WeaponSelectTimerThread(void* _arg)
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
			packetSize = 0;

			// ���� �濡 �ִ� "���" �÷��̾�� ���⸦ ������� ���������� ������.
			list<C_ClientInfo*> playerList = ptr->GetRoom()->GetPlayerList();	// ����Ʈ ����
			C_ClientInfo* player = nullptr;
			for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
			{
				player = *iter;
				player->SendPacket(protocol, buf, packetSize);
			}

			break;
		}

		// 1�ʸ��� ���� ���ϸ�
		if (sec < (int)duringTime)
		{
			sec = (int)duringTime;	// ���Ӱ� �� ������ ���Ž����ش�.(before sec�� ����)

			// 1�ʿ� �� ���� �ð��� �˷��ִ� ���������� ����.
			protocol = InGameManager::GetInstance()->SetProtocol(INGAME_STATE, PROTOCOL_INGAME::TIMER_PROTOCOL, RESULT_INGAME::NODATA);
			packetSize = 0;

			// ���� ����������� ���� �ð� ��Ŷ ����
			InGameManager::GetInstance()->PackPacket(buf, (WEAPON_SELTIME - sec), packetSize);

			// ���� �濡 �ִ� "���" �÷��̾�� ���� ���� ����������� ���� �ð��� ������
			list<C_ClientInfo*> playerList = ptr->GetRoom()->GetPlayerList();	// ����Ʈ ����
			C_ClientInfo* player = nullptr;
			for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
			{
				player = *iter;
				player->SendPacket(protocol, buf, packetSize);
			}
		}

		Sleep(50);	// �� �־������ �ƴϸ� ȥ�� CPU �� ��Ƹ���
	}

	return 0;	// �׸��� ������ ����
}
