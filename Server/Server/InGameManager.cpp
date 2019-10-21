#include "stdafx.h"
#include "LogManager.h"
#include "InGameManager.h"
#include "RoomManager.h"
#include "RandomManager.h"
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
	// ���� ������ ����
	WeaponInfo* weaponPtr;
	while ((weaponPtr = DatabaseManager::GetInstance()->LoadWeaponInfo()) != nullptr)
		weaponInfo.emplace_back(weaponPtr);

	// ���� ������ ����
	GameInfo* gamePtr;
	while ((gamePtr = DatabaseManager::GetInstance()->LoadGameInfo()) != nullptr)
		gameInfo.emplace_back(gamePtr);
}

void InGameManager::End()
{
}

void InGameManager::PackPacket(char* _setptr, const int _num, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// int�� ���� ����
	memcpy(ptr, &_num, sizeof(_num));
	ptr = ptr + sizeof(_num);
	_size = _size + sizeof(_num);
}

void InGameManager::PackPacket(char* _setptr, int _num, TCHAR* _string, int& _size)
{
	char* ptr = _setptr;
	int strsize = (int)_tcslen(_string) * sizeof(TCHAR);
	_size = 0;

	// �÷��̾� �ѹ�
	memcpy(ptr, &_num, sizeof(_num));
	ptr = ptr + sizeof(_num);
	_size = _size + sizeof(_num);

	// ���ڿ� ����
	memcpy(ptr, &strsize, sizeof(strsize));
	ptr = ptr + sizeof(strsize);
	_size = _size + sizeof(strsize);

	// ���ڿ�
	memcpy(ptr, _string, strsize);
	ptr = ptr + strsize;
	_size = _size + strsize;
}

void InGameManager::PackPacket(char* _setptr, int _num, Weapon* _struct, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// �÷��̾� �ѹ�
	memcpy(ptr, &_num, sizeof(_num));
	ptr = ptr + sizeof(_num);
	_size = _size + sizeof(_num);

	// �� ���� ��Ŷ
	memcpy(ptr, _struct, sizeof(Weapon));
	ptr = ptr + sizeof(Weapon);
	_size = _size + sizeof(Weapon);
}

void InGameManager::PackPacket(char* _setptr, IngamePacket& _struct, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// �ΰ��� ��Ŷ
	memcpy(ptr, &_struct, sizeof(IngamePacket));
	ptr = ptr + sizeof(IngamePacket);
	_size = _size + sizeof(IngamePacket);
}

void InGameManager::PackPacket(char* _setptr, int _carSeed, GameInfo* &_gameInfo, vector<WeaponInfo*>& _weaponInfo, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 1. ���� �õ�
	memcpy(ptr, &_carSeed, sizeof(_carSeed));
	ptr = ptr + sizeof(_carSeed);
	_size = _size + sizeof(_carSeed);

	// 2. ��������
	memcpy(ptr, _gameInfo, sizeof(GameInfo));
	ptr = ptr + sizeof(GameInfo);
	_size = _size + sizeof(GameInfo);

	// 3. �������� �� �� ����
	int numOfWeapon = (int)_weaponInfo.size();
	memcpy(ptr, &numOfWeapon, sizeof(numOfWeapon));
	ptr = ptr + sizeof(numOfWeapon);
	_size = _size + sizeof(numOfWeapon);

	// 4. ��������(���߿� �ʿ��� �͸� �����ߵǸ� vector���� �ٸ��� ����)
	for (auto iter = _weaponInfo.begin(); iter != _weaponInfo.end(); ++iter)
	{
		memcpy(ptr, *iter, sizeof(WeaponInfo));
		ptr = ptr + sizeof(WeaponInfo);
		_size = _size + sizeof(WeaponInfo);
	}
}

void InGameManager::UnPackPacket(char* _getBuf, int& _num)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// num
	memcpy(&_num, ptr, sizeof(int));
	ptr = ptr + sizeof(int);
}

void InGameManager::UnPackPacket(char* _getBuf, IngamePacket& _struct)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// ����ü ����
	memcpy(&_struct, ptr, sizeof(IngamePacket));
	ptr = ptr + sizeof(IngamePacket);
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

	RESULT_INGAME itemSelectResult = RESULT_INGAME::INGAME_FAIL;

	Weapon* tmpWeapon = new Weapon();
	UnPackPacket(_buf, tmpWeapon);
	
	// Ŭ�� ���� �������� ����!
	if (tmpWeapon != nullptr)
	{
		_ptr->GetPlayerInfo()->SetWeapon(tmpWeapon);
		itemSelectResult = RESULT_INGAME::INGAME_SUCCESS;

		wprintf(L"%s ���� ���� : %d, %d\n",
			_ptr->GetUserInfo()->id,
			_ptr->GetPlayerInfo()->GetWeapon()->mainW,
			_ptr->GetPlayerInfo()->GetWeapon()->subW);
	}


	// 1. ���� �������� ����(�ΰ��� ���·�)
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::START_PROTOCOL, itemSelectResult);

	// 2. �� �õ� + �������� + ���������� ��ŷ
	if (_ptr->GetRoom()->GetCarSeed() == 0)	// �õ� ����
	{
		_ptr->GetRoom()->SetCarSeed(RandomManager::GetInstance()->GetIntNumRandom());
	}
	printf("�� �õ� : %d\n", _ptr->GetRoom()->GetCarSeed());
	PackPacket(buf, _ptr->GetRoom()->GetCarSeed(), gameInfo.at(0), weaponInfo, packetSize);

	// 3. ���� ��Ŷ�� Ŭ�󿡰� ����
	_ptr->SendPacket(protocol, buf, packetSize);

	if (itemSelectResult == RESULT_INGAME::INGAME_SUCCESS)
	{
		_ptr->GetRoom()->SetRoomStatus(ROOMSTATUS::ROOM_GAME);	// ���� �������� �����Ͽ���.
		return true;
	}

	return false;
}

bool InGameManager::LoadingProcess(C_ClientInfo* _ptr, char* _buf)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 1. �÷��̾��� �ε� ���¸� �Ϸ�� �ٲ۴�.
	_ptr->GetPlayerInfo()->SetLoadStatus(true);

	// 2. ���� �濡 �ִ� �÷��̾� ����Ʈ�� ���´�.
	vector<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayers();
	
	// 3. ��� �÷��̾ ����������� �˻��Ѵ�.
	RESULT_INGAME result = RESULT_INGAME::INGAME_SUCCESS;	// �ϴ� �����ߴٰ� ����
	vector<C_ClientInfo*>::iterator iter = playerList.begin();
	for (; iter != playerList.end(); ++iter)
	{
		// ���� ��ΰ� �ε��Ȱ� �ƴ϶�� false�̴�.
		if (((C_ClientInfo*)*iter)->GetPlayerInfo()->GetLoadStatus() == false)
		{
			result = RESULT_INGAME::INGAME_FAIL;	// ��ΰ� �ε��ƴ�
			break;
		}
	}

	// 4. �������� ����
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::LOADING_PROTOCOL, result);

	// 5. ���� ��� �ε��ƴٸ� �÷��̾���� ������ �ʱ�ȭ�Ѵ�.
	if (result == RESULT_INGAME::INGAME_SUCCESS)
	{
		// ��ΰ� �ε� �ƴٸ�(�׷� �ѵ� �� ����� �״�)
		InitalizePlayersInfo(_ptr->GetRoom());
	}

	// 6. ��� �ε��ƴ����� ���� ����� ������.
	ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

	return true;
}

bool InGameManager::InitProcess(C_ClientInfo* _ptr, char* _buf)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	IngamePacket tmpPacket;
	UnPackPacket(_buf, tmpPacket);

	// �÷��̾��� ��Ŷ�� �����صд�.
	IngamePacket* gamePacket = new IngamePacket(tmpPacket);
	_ptr->GetPlayerInfo()->SetIngamePacket(gamePacket);

	// ������ ���
	printf("[�ʱ��ε���]%d ,%f, %f, %f, %d\n", gamePacket->playerNum, gamePacket->posX, gamePacket->posZ, gamePacket->rotY, gamePacket->action);

	C_Sector* sector = _ptr->GetRoom()->GetSector();	// �� ���� ���� �Ŵ����� ��´�.

	// �÷��̾� ��ġ�� ���� ���� ����� �ε����� ���ؼ� �ش� ���Ϳ� �߰��Ѵ�.
	INDEX getIdx;
	if (sector->GetIndex(_ptr->GetPlayerInfo()->GetIndex(), getIdx, tmpPacket.posX, tmpPacket.posZ) == true)
	{
		_ptr->GetPlayerInfo()->SetIndex(getIdx);
		sector->Add(_ptr, getIdx);
	}

	else
		printf("��ȿ���� ���� �ε���!! %d, %d\n", getIdx.i, getIdx.j);


	// 1. ��� �÷��̾�� �ڽ��� ������ ���������� ��ŷ(���� ����) �� ����
	vector<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayers();
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::WEAPON_PROTOCOL, RESULT_INGAME::NOTIFY_WEAPON);
	PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), _ptr->GetPlayerInfo()->GetWeapon(), packetSize);

	// ����(���� ����)
	ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);


	// 2. ��� �÷��̾�� �ڽ��� �г��� ������ �����ش�(���� ����)
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::NICKNAME_PROTOCOL, RESULT_INGAME::NODATA);
	PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), _ptr->GetUserInfo()->nickname, packetSize);

	// ����(���� ����)
	ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

	return true;
}

bool InGameManager::UpdateProcess(C_ClientInfo* _ptr, char* _buf)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// ���޵� ��Ŷ�� ����
	IngamePacket recvPacket;
	UnPackPacket(_buf, recvPacket);
	printf("%d ,%f, %f, %f, %d\n", recvPacket.playerNum, recvPacket.posX, recvPacket.posZ, recvPacket.rotY, recvPacket.action);
	printf("��Ŷ %dȸ ����\n", ++numOfPacketSent);
	printf("%s��:%f\n", _ptr->GetUserInfo()->id, _ptr->GetPlayerInfo()->GetIngamePacket()->health);

	list<C_ClientInfo*>sendList;	// ���� ���� + ���� ���Ϳ� �����ϴ� �÷��̾� ����Ʈ
	C_ClientInfo* exceptClient = nullptr;	// ��Ŷ �Ⱥ��� ���

	// 1. ������ �˻�
	if (CheckMovement(_ptr, recvPacket) == true)
	{
		// ���� ��� protocol�� ��ŷ
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::INGAME_SUCCESS);
		
		// ���� ����Ʈ ����
		sendList = _ptr->GetRoom()->GetSector()->GetSectorPlayerList(_ptr->GetPlayerInfo()->GetIndex());
		exceptClient = _ptr;	// �������� �ڽ����� �Ⱥ�����.

		// ���� �� �÷��̾�鿡�� ��Ŷ ����
		PackPacket(buf, recvPacket, packetSize);
		ListSendPacket(sendList, exceptClient, protocol, buf, packetSize, true);

		// ����������, �÷��̾��� ��Ŷ ������ �����صд�.
		IngamePacket* setPacket = new IngamePacket(recvPacket);
		_ptr->GetPlayerInfo()->SetIngamePacket(setPacket);
	}

	// �ҹ� �������� �̹� CheckMovement���� �� ó�������� ����������.
	else
		return false;

	// 2. �Ѿ� �˻�
	CheckBullet(_ptr, recvPacket);


	return true;   // �� �� true��
}

bool InGameManager::GetPosProcess(C_ClientInfo* _ptr, char* _buf)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize = 0;

	// �÷��̾� num�� �����´�.
	IngamePacket pos;
	int playerNum;
	UnPackPacket(_buf, playerNum);

	// �������� ����
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::GET_OTHERPLAYER_POS);

	// �ݺ��ڷ� �����鼭 playerNum�� ��ġ�ϴ� �÷��̾ ã���� �� �÷��̾��� ��ġ(��Ŷ°��)�� �������ش�.
	vector<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayers();	// ����Ʈ�� ����
	C_ClientInfo* player = nullptr;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;
		if (player->GetPlayerInfo()->GetPlayerNum() == playerNum)
		{
			memcpy(&pos, player->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
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
	int packetSize = 0;

	// �������� ����
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::FOCUS_PROTOCOL, RESULT_INGAME::NODATA);

	// ���ο��� �ٸ� ��� �÷��̾��� �ΰ��� ������ �����ش�
	vector<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayers();	// ����Ʈ ����
	IngamePacket gamePacket;
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
			memcpy(&gamePacket, player->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
			PackPacket(buf, gamePacket, packetSize);
			_ptr->SendPacket(protocol, buf, packetSize);
		}
	}

	return true;
}

bool InGameManager::LeaveProcess(C_ClientInfo* _ptr, int _playerNum)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// ���� �������� ����
	protocol = SetProtocol(
		STATE_PROTOCOL::INGAME_STATE, 
		PROTOCOL_INGAME::DISCONNECT_PROTOCOL, 
		RESULT_INGAME::INGAME_SUCCESS);

	// ��ŷ
	PackPacket(buf, _playerNum, packetSize);

	// �ڽ��� ������ �ٸ� Ŭ��鿡�� �ڽ��� �������� �˸���.
	vector<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayers();	// ����Ʈ ����
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

///////// etc

void InGameManager::InitalizePlayersInfo(RoomInfo* _room)
{
	C_ClientInfo* player = nullptr;
	vector<C_ClientInfo*>players = _room->GetPlayers();
	for (auto iter = players.begin(); iter != players.end(); ++iter)
	{
		player = *iter;
		
		// �ʱ� ü��, �Ѿ� ����(�Ѿ��� ���߿� IngamePacket���� ���� ��Ű��)
		player->GetPlayerInfo()->GetIngamePacket()->health = gameInfo[_room->GetGameType()]->maxHealth;
		player->GetPlayerInfo()->SetBullet(weaponInfo[player->GetPlayerInfo()->GetWeapon()->mainW]->maxAmmo);
	}
}

/// about movement

bool InGameManager::CheckMovement(C_ClientInfo* _ptr, IngamePacket& _recvPacket)
{
#ifndef DEBUG	// ���߿� �������Ҷ� ifdef�� ifndef�� �ٲٸ� ��
	CheckIllegalMovement();
#endif

	// �� ���� ���� �Ŵ����� ��´�.
	C_Sector* sector = _ptr->GetRoom()->GetSector();

	// 1. �÷��̾� ��ġ�� ���� ���� ����� �ε����� ���Ѵ�.
	INDEX getIdx;
	bool isValidIdx = sector->GetIndex(_ptr->GetPlayerInfo()->GetIndex(), getIdx, _recvPacket.posX, _recvPacket.posZ);

	// ��ȿ�� �ε������
	if (isValidIdx == true)
	{
		// 1. ���� ���� �ִ� �ε��� ������ ���� �̵��� �ε��� ������ �ٸ��ٸ� ���� �ִ� ����Ʈ���� ����, ���� ���� ����Ʈ�� �߰����ش�.
		if (getIdx != _ptr->GetPlayerInfo()->GetIndex())
		{
			UpdateSectorAndSend(_ptr, _recvPacket, getIdx);
		}
	
		return true;
	}

	// �ҹ����� �ε������ ���� �ε����� ������ �߰� ��ġ�� ���� ������ ���ý�Ų��.
	else
	{
		IllegalSectorProcess(_ptr, _recvPacket, _ptr->GetPlayerInfo()->GetIndex());

		return false;
	}

}

// (�ҹ��̵�üũ) ���� �̵��� �� ���� ������ �ӵ��� ���ڱ� ������ �����δٸ� �׳� �� �����ϰ� �ش� Ŭ�����׸� ���� ������ ���� ��Ŷ�� ������.
bool InGameManager::CheckIllegalMovement(C_ClientInfo* _ptr, IngamePacket& _recvPacket)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	if (_recvPacket.speed > gameInfo.at(_ptr->GetRoom()->GetGameType())->maxSpeed ||
		abs(_ptr->GetPlayerInfo()->GetIngamePacket()->posX - _recvPacket.posX) > 0.1f ||
		abs(_ptr->GetPlayerInfo()->GetIngamePacket()->posZ - _recvPacket.posZ) > 0.1f)
	{
		printf("[�ҹ��̵�]����:%f, %f\t����:%f, %f",
			_ptr->GetPlayerInfo()->GetIngamePacket()->posX,
			_ptr->GetPlayerInfo()->GetIngamePacket()->posZ,
			_recvPacket.posX,
			_recvPacket.posZ);

		LogManager::GetInstance()->HackerFileWrite("[SpeedHack]ID:%s, NICK:%s, Speed:%f\n", _ptr->GetUserInfo()->id, _ptr->GetUserInfo()->nickname, _recvPacket.speed);
		/// ���Ŀ� ���⼭ Kick�ϴ��� �ϴ°� �ʿ�

		// FORCE_MOVE(���� �̵�) ��� protocol�� ��ŷ
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::FORCE_MOVE);

		// ���� �÷��̾ ������ �ִ� ��Ŷ ������ tmpPacket�� �����ϰ�, �̸� �ڽſ��Ը� ����
		memcpy(&_recvPacket, _ptr->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
		PackPacket(buf, _recvPacket, packetSize);
		_ptr->SendPacket(protocol, buf, packetSize);

		return true;	// true�����ϸ� �ҹ� �̵��� �־��ٴ� ���
	}

	return false;	// false�����ϸ� �ҹ��̵��� �����ٴ� ���
}

// ��ȿ�������� ���� �ε����϶� ó��
void InGameManager::IllegalSectorProcess(C_ClientInfo* _ptr, IngamePacket& _recvPacket, INDEX _beforeIdx)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	C_Sector* sector = _ptr->GetRoom()->GetSector();

	COORD_DOUBLE LT = sector->GetLeftTop(_beforeIdx);
	COORD_DOUBLE RB = sector->GetRightBottom(_beforeIdx);

	_recvPacket.posX = (float)(LT.x + RB.x) / 2;
	_recvPacket.posZ = (float)(LT.z + RB.z) / 2;

	// FORCE_MOVE(���� �̵�) ��� protocol�� ��ŷ
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::FORCE_MOVE);

	LogManager::GetInstance()->HackerFileWrite("[TeleportHack]ID:%s, NICK:%s\n", _ptr->GetUserInfo()->id, _ptr->GetUserInfo()->nickname);
	/// ���Ŀ� ���⼭ Kick�ϴ��� �ϴ°� �ʿ�


	// �̸� �ڽſ��Ը� ����
	PackPacket(buf, _recvPacket, packetSize);
	_ptr->SendPacket(protocol, buf, packetSize);
}

// ���Ͱ� �ٲ��� ������Ʈ�ϰ�, �ش� ���� �÷��̾�鿡�� ������ ��Ŷ�� ������ �Լ�
void InGameManager::UpdateSectorAndSend(C_ClientInfo* _ptr, IngamePacket& _recvPacket, INDEX& _newIdx)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;


	C_Sector* sector = _ptr->GetRoom()->GetSector();

	sector->Delete(_ptr, _ptr->GetPlayerInfo()->GetIndex());    // ������ �ִ� �ε��������� ����
	sector->Add(_ptr, _newIdx);                  // ���ο� �ε�����ġ ����Ʈ�� �߰��Ѵ�.

	list<C_ClientInfo*>enterList;
	list<C_ClientInfo*>exitList;

	// ��, ������ ���� ����Ʈ
	byte playerBit = 0;	// ���Ӱ� ������ ���� ������ �÷��̾� ��Ʈ
	playerBit = sector->GetMovedSectorPlayerList(_ptr->GetPlayerInfo()->GetIndex(), _newIdx, enterList, exitList);

	// 1. ���� ���� �˸� ��Ŷ ���� �� ����
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::EXIT_SECTOR);
	PackPacket(buf, _recvPacket, packetSize);

	ListSendPacket(exitList, _ptr, protocol, buf, packetSize, true);

	// 2. ���� ���� �˸� ��Ŷ ���� �� ����
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::ENTER_SECTOR);
	PackPacket(buf, _recvPacket, packetSize);

	ListSendPacket(enterList, _ptr, protocol, buf, packetSize, true);

	// 3. ���ο��Դ� ���Ӱ� ������ ���� ������ �÷��̾� ����Ʈ�� Ȱ��ȭ�� ��Ʈ�� �����ش�.
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::UPDATE_PLAYER);
	PackPacket(buf, playerBit, packetSize);

	_ptr->SendPacket(protocol, buf, packetSize);   // ����

	_ptr->GetPlayerInfo()->SetIndex(_newIdx);            // ����� ���ο� �ε��� ����
}


/// about bullet
bool InGameManager::CheckBullet(C_ClientInfo* _ptr, IngamePacket& _recvPacket)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	bool validHitFlag = false;	// ���������� �¾Ҵ��� �¾����� true��

	vector<C_ClientInfo*> hitPlayers;	// _ptr�� �ѿ� ������� ����Ʈ

	// ������ ���ٸ� �÷��̾� ��Ʈ�� ���õǾ������Ƿ�
	if (_recvPacket.collisionCheck.playerBit != 0)
	{
		// 1. �켱 �ִ� �� �� �̳��� ������ �˻��Ѵ�.
		int totalNumOfBullet = GetNumOfBullet(_recvPacket.collisionCheck.playerHitCountBit, 0);
		if (CheckMaxFire(_ptr, totalNumOfBullet) == false)
		{
			return false;
		}
		else
		{
			BulletDecrease(_ptr, totalNumOfBullet);	// �� ��ŭ �Ѿ� ��
		}

		// 2. ���� �÷��̾���� �����Ÿ��� �ִ��� �˻��Ѵ�.
		byte bitMask = (byte)PLAYER_BIT::PLAYER_1;
		for (int i = 0; i < MAX_PLAYER; i++, bitMask >>= 1)
		{
			// ������ �˻� ����
			if (i == (_ptr->GetPlayerInfo()->GetPlayerNum() - 1))
				continue;

			// �÷��̾� ��Ʈ�� Ȱ��ȭ �Ǿ� ������
			if ((bitMask & _recvPacket.collisionCheck.playerBit) > 0)
			{
				C_ClientInfo* hitPlayer = _ptr->GetRoom()->GetPlayerByIndex(i);
				
				// �����Ÿ� �˻��ؼ� �Ÿ��̳���� �������� ������.
				if (CheckBulletRange(_ptr, hitPlayer) == true)
				{
					int numOfBullet = GetNumOfBullet(_recvPacket.collisionCheck.playerHitCountBit, (i + 1));
					BulletHitProcess(_ptr, hitPlayer, numOfBullet);
					
					printf("%s��:%f\n", hitPlayer->GetUserInfo()->id ,hitPlayer->GetPlayerInfo()->GetIngamePacket()->health);

					hitPlayers.emplace_back(hitPlayer);	// ������ ����Ʈ�� �߰�

					validHitFlag = true;	// ���������� ���� �÷��� Ȱ��ȭ
				}
			}
		}
	}
	
	// ��� �ƴϸ� �� ��������
	else
	{
		return false;
	}


	// ��������� ���¸� �ٸ� Ŭ��鿡�� ������
	if (hitPlayers.size() != 0)
	{
		IngamePacket packet;
		list<C_ClientInfo*>sendList;
		for (int i = 0; i < hitPlayers.size(); i++)
		{
			// �������� �������Ϳ� �ִ� �÷��̾���� ����Ʈ�� ����
			sendList = hitPlayers[i]->GetRoom()->GetSector()->GetSectorPlayerList(hitPlayers[i]->GetPlayerInfo()->GetIndex());
		
			// ���� �� �÷��̾�鿡�� ��Ŷ ����
			protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::BULLET_HIT);
			memcpy(&packet, hitPlayers[i]->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
			PackPacket(buf, packet, packetSize);
			ListSendPacket(sendList, nullptr, protocol, buf, packetSize, true);
		}
	}


	return validHitFlag;
}

// shot:���, hit:������
bool InGameManager::CheckBulletRange(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer)
{
	float shotPlayerPosX = _shotPlayer->GetPlayerInfo()->GetIngamePacket()->posX;
	float shotPlayerPosZ = _shotPlayer->GetPlayerInfo()->GetIngamePacket()->posZ;
	float hitPlayerPosX = _hitPlayer->GetPlayerInfo()->GetIngamePacket()->posX;
	float hitPlayerPosZ = _hitPlayer->GetPlayerInfo()->GetIngamePacket()->posZ;

	WeaponInfo* shotPlayerWeapon = weaponInfo[_shotPlayer->GetPlayerInfo()->GetWeapon()->mainW];

	// �����Ÿ� �̳���� true����
	if (UtilityManager::GetInstance()->GetDotDistanceNoSqrt(
		shotPlayerPosX, shotPlayerPosZ,
		hitPlayerPosX, hitPlayerPosZ) <= shotPlayerWeapon->range)
	{
		return true;
	}

	return false;
}
bool InGameManager::CheckMaxFire(C_ClientInfo* _shotPlayer, int _numOfBullet)
{
	WeaponInfo* shotPlayerWeapon = weaponInfo[_shotPlayer->GetPlayerInfo()->GetWeapon()->mainW];
	bool result = false;

	// ���� 0.1�ʴ� ���� �� �ִ� �� ���� ����ؼ� �˻��ؾ������� �ּ� 1��(����)�� �����ؾ��ϱ� ������ ������ �˻���
	if (_numOfBullet <= shotPlayerWeapon->bulletPerShot)
	{
		result = true;
	}

	return result;
}
int InGameManager::GetNumOfBullet(int _shootCountBit, byte _hitPlayerNum)
{
	int shifter = 0;
	int bulletCount = 0;
	int TEMP_MAX_PLAYER = 4;

	// 0�� �Ű������� �Ѿ���� �߻�� ��ü �Ѿ� ������ �޶�� �ǹ̴�.
	if (_hitPlayerNum == 0)
	{
		int bulletCountBit = 0;

		byte bitEraseMask = 0x00;			// 8��Ʈ ����� ����ũ(00000000)
		bulletCountBit = _shootCountBit;	//ó�� ī��Ʈ ��Ʈ ������
		for (int i = 1; i <= TEMP_MAX_PLAYER; i++)
		{
			shifter = 8 * (TEMP_MAX_PLAYER - i);	// �̵� ���꿡 �ʿ��� ��
			
			if ((bulletCountBit >> shifter) > 0)
			{
				bulletCount += (bulletCountBit >> shifter);
				bulletCountBit &= bitEraseMask << shifter;	// ���� ���� 8��Ʈ�� ����ũ�� ������
			}
		}
	}

	else
	{
		shifter = 8 * (TEMP_MAX_PLAYER - _hitPlayerNum);	// �̵� ���꿡 �ʿ��� ��
		bulletCount = (_shootCountBit >> shifter);
	}

	return bulletCount;
}
void InGameManager::BulletHitProcess(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer, int _numOfBullet)
{
	WeaponInfo* shotPlayerWeapon = weaponInfo[_shotPlayer->GetPlayerInfo()->GetWeapon()->mainW];
	float originalHealth = _hitPlayer->GetPlayerInfo()->GetIngamePacket()->health;
	float totalDamage = (shotPlayerWeapon->damage * _numOfBullet);

	// ���� ü���� ������
	if (originalHealth - totalDamage < 0)
	{
		_hitPlayer->GetPlayerInfo()->GetIngamePacket()->health = 0.0;
	}

	else
	{
		_hitPlayer->GetPlayerInfo()->GetIngamePacket()->health = (originalHealth - totalDamage);
	}
}
void InGameManager::BulletDecrease(C_ClientInfo* _shotPlayer, int _numOfBullet)
{
	WeaponInfo* shotPlayerWeapon = weaponInfo[_shotPlayer->GetPlayerInfo()->GetWeapon()->mainW];

	// �� �� �� ��ŭ �Ѿ� ��
	int originalBullet = _shotPlayer->GetPlayerInfo()->GetBullet();
	int minusBullet = (_numOfBullet / shotPlayerWeapon->bulletPerShot);

	if (originalBullet - minusBullet < 0)
	{
		_shotPlayer->GetPlayerInfo()->SetBullet(0);
	}

	else
	{
		_shotPlayer->GetPlayerInfo()->SetBullet(originalBullet - minusBullet);
	}
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

bool InGameManager::LoadingSuccess(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);

	// �ε� �˻�
	if (protocol == LOADING_PROTOCOL)
		return LoadingProcess(_ptr, buf);

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

bool InGameManager::CanIUpdate(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);
	RESULT_INGAME result;
	GetResult(buf, result);

	// ������Ʈ �������� ������ �� ������Ʈ ���μ��� ����
	if (protocol == UPDATE_PROTOCOL)
	{
		switch (result)
		{
			// �ٸ� ��� ��ġ ��û �Ͻ�
		case GET_OTHERPLAYER_POS:
			return GetPosProcess(_ptr, buf);

			// �׳� ������Ʈ�� ��
		default:
			return UpdateProcess(_ptr, buf);
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

void InGameManager::ListSendPacket(vector<C_ClientInfo*> _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept)
{
	// ������ List�� ������� ���� ��쿡��
	if (!_list.empty())
	{
		// ����Ʈ �ȿ��ִ� �÷��̾�鿡�� ��Ŷ�� �����Ѵ�.
		C_ClientInfo* player = nullptr;
		for (vector<C_ClientInfo*>::iterator iter = _list.begin(); iter != _list.end(); ++iter)
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
			vector<C_ClientInfo*> playerList = ptr->GetRoom()->GetPlayers();	// ����Ʈ ����
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
			vector<C_ClientInfo*> playerList = ptr->GetRoom()->GetPlayers();	// ����Ʈ ����
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
