#include "stdafx.h"
#include "LogManager.h"
#include "InGameManager.h"
#include "RoomManager.h"
#include "RandomManager.h"
#include "C_ClientInfo.h"
#include <thread>
#include <chrono>
#include "Item.h"

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

	// ��ġ ������ ����
	LocationInfo* locationPtr;
	while ((locationPtr = DatabaseManager::GetInstance()->LoadLocationInfo()) != nullptr)
		locationInfo.emplace_back(locationPtr);
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

void InGameManager::PackPacket(char* _setptr, double _time, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// double�� ���� ����
	memcpy(ptr, &_time, sizeof(_time));
	ptr = ptr + sizeof(_time);
	_size = _size + sizeof(_time);
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

void InGameManager::PackPacket(char* _setptr, int _num1, int _num2, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// �÷��̾� �ѹ�1
	memcpy(ptr, &_num1, sizeof(_num1));
	ptr = ptr + sizeof(_num1);
	_size = _size + sizeof(_num1);

	// �÷��̾� �ѹ�2
	memcpy(ptr, &_num2, sizeof(_num2));
	ptr = ptr + sizeof(_num2);
	_size = _size + sizeof(_num2);
}

void InGameManager::PackPacket(char* _setptr, int _num, float _posX, float _posZ, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// playerNum
	memcpy(ptr, &_num, sizeof(_num));
	ptr = ptr + sizeof(_num);
	_size = _size + sizeof(_num);

	// posX
	memcpy(ptr, &_posX, sizeof(_posX));
	ptr = ptr + sizeof(_posX);
	_size = _size + sizeof(_posX);

	// posZ
	memcpy(ptr, &_posZ, sizeof(_posZ));
	ptr = ptr + sizeof(_posZ);
	_size = _size + sizeof(_posZ);
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

void InGameManager::PackPacket(char* _setptr, IngamePacket& _struct, int _code, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// �ΰ��� ��Ŷ
	memcpy(ptr, &_struct, sizeof(IngamePacket));
	ptr = ptr + sizeof(IngamePacket);
	_size = _size + sizeof(IngamePacket);

	// (������)�ڵ�
	memcpy(ptr, &_code, sizeof(_code));
	ptr = ptr + sizeof(_code);
	_size = _size + sizeof(_code);
}

void InGameManager::PackPacket(char* _setptr, GameInfo* &_gameInfo, vector<WeaponInfo*>& _weaponInfo, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 1. ��������
	memcpy(ptr, _gameInfo, sizeof(GameInfo));
	ptr = ptr + sizeof(GameInfo);
	_size = _size + sizeof(GameInfo);

	// 2. �������� �� �� ����
	int numOfWeapon = (int)_weaponInfo.size();
	memcpy(ptr, &numOfWeapon, sizeof(numOfWeapon));
	ptr = ptr + sizeof(numOfWeapon);
	_size = _size + sizeof(numOfWeapon);

	// 3. ��������(���߿� �ʿ��� �͸� �����ߵǸ� vector���� �ٸ��� ����)
	for (auto iter = _weaponInfo.begin(); iter != _weaponInfo.end(); ++iter)
	{
		memcpy(ptr, *iter, sizeof(WeaponInfo));
		ptr = ptr + sizeof(WeaponInfo);
		_size = _size + sizeof(WeaponInfo);
	}
}

void InGameManager::PackPacket(char* _setptr, RoomInfo* _room, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 1. �÷��̾� �� �� ������
	int numOfPlayer = _room->GetNumOfPlayer();
	memcpy(ptr, &numOfPlayer, sizeof(numOfPlayer));
	ptr = ptr + sizeof(numOfPlayer);
	_size = _size + sizeof(numOfPlayer);

	// 2. �÷��̾� �� �°� ������� ���ھ� ��ŷ
	C_ClientInfo* player = nullptr;
	int playerNum = 0;
	for (int i = 0; i < numOfPlayer; i++)
	{
		player = _room->GetPlayerByIndex(i);
		
		// 2-1. �÷��̾� ��ȣ ��ŷ
		playerNum = player->GetPlayerInfo()->GetPlayerNum();
		memcpy(ptr, &playerNum, sizeof(playerNum));
		ptr = ptr + sizeof(playerNum);
		_size = _size + sizeof(playerNum);

		// 2-2. ���ھ� ��ŷ!
		memcpy(ptr, &player->GetPlayerInfo()->GetScore(), sizeof(Score));
		ptr = ptr + sizeof(Score);
		_size = _size + sizeof(Score);
	}
}

void InGameManager::UnPackPacket(char* _getBuf, int& _num)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// num
	memcpy(&_num, ptr, sizeof(int));
	ptr = ptr + sizeof(int);
}

void InGameManager::UnPackPacket(char* _getBuf, float& _posX, float& _posZ)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// X
	memcpy(&_posX, ptr, sizeof(float));
	ptr = ptr + sizeof(float);

	// X
	memcpy(&_posZ, ptr, sizeof(float));
	ptr = ptr + sizeof(float);
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
		// ���� ������ ���Ƿ� �������� ����
		if (tmpWeapon->mainW == WEAPONS::NODATA)
		{
			tmpWeapon->mainW = (int)RandomManager::GetInstance()->GetIntNumRandom(0, WEAPONS::MAIN_MAX_LENGTH - 1);
		}

		// ���� ������ ���Ƿ� �������� ����
		if (tmpWeapon->subW == WEAPONS::NODATA)
		{
			tmpWeapon->subW = (int)RandomManager::GetInstance()->GetIntNumRandom(WEAPONS::MAIN_MAX_LENGTH + 1, WEAPONS::SUB_MAX_LENGTH - 1);
		}

		_ptr->GetPlayerInfo()->SetWeapon(tmpWeapon);

		itemSelectResult = RESULT_INGAME::INIT_INFO;
		wprintf(L"%s ���� ���� : %d, %d\n",
			_ptr->GetUserInfo()->id,
			_ptr->GetPlayerInfo()->GetWeapon()->mainW,
			_ptr->GetPlayerInfo()->GetWeapon()->subW);
	}


	// 1. ���� �������� ����(�ΰ��� ���·�)
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::START_PROTOCOL, 
		itemSelectResult);

	// 2. �������� + ���������� ��ŷ
	PackPacket(buf, gameInfo.at(_ptr->GetRoom()->GetGameType()), weaponInfo, packetSize);

	// 3. ���� ��Ŷ�� Ŭ�󿡰� ����
	_ptr->SendPacket(protocol, buf, packetSize);

	return true;
}

bool InGameManager::LoadingProcess(C_ClientInfo* _ptr)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 1. �÷��̾��� �ε� ���¸� �Ϸ�� �ٲ۴�.
	_ptr->GetPlayerInfo()->SetLoadStatus(true);

	// 2-1. ���� �濡 �ִ� �÷��̾� ����Ʈ�� ���´�.
	vector<C_ClientInfo*>& playerList = _ptr->GetRoom()->GetPlayers();

	// 3. ��� �÷��̾ ����������� �˻��Ѵ�.
	RESULT_INGAME result = RESULT_INGAME::INGAME_SUCCESS;	// �ϴ� �����ߴٰ� ����
	C_ClientInfo* player = nullptr;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;

		// ���� ��ΰ� �ε��Ȱ� �ƴ϶�� false�̴�.
		if (player->GetPlayerInfo()->GetLoadStatus() == false)
		{
			result = RESULT_INGAME::INGAME_FAIL;	// ��ΰ� �ε��ƴ�
			break;
		}
	}

	// 4. ���� ��� �ε��ƴٸ� �÷��̾���� ������ �ʱ�ȭ�ϰ� �ε� ���� ���������� ������.
	if (result == RESULT_INGAME::INGAME_SUCCESS)
	{
		// ��ΰ� �ε� �ƴٸ�(�׷� �ѵ� �� ����� �״�)
		InitalizePlayersInfo(_ptr->GetRoom());

		// 5. ��� �ε��ƴ����� ���� ����� ������.
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::LOADING_PROTOCOL, 
			result);
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

		// 6. �׸��� ���� ���� ���·� �ٲٴ°� InitProcess()���� Ŭ�� �ʱ� ��Ŷ�� 1ȸ ������ �� �Ŀ�
	}

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

	// 1. ��� �÷��̾�� �ڽ��� ������ ���������� ��ŷ(���� ����) �� ����
	vector<C_ClientInfo*>& playerList = _ptr->GetRoom()->GetPlayers();
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::WEAPON_PROTOCOL, 
		RESULT_INGAME::NOTIFY_WEAPON);
	PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), _ptr->GetPlayerInfo()->GetWeapon(), packetSize);

	// ����(���� ����)
	ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

	// 2. ��� �÷��̾�� �ڽ��� �г��� ������ �����ش�(���� ����)
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::INFO_PROTOCOL, 
		RESULT_INGAME::NICKNAME);
	PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), _ptr->GetUserInfo()->nickname, packetSize);

	// ����(���� ����)
	ListSendPacket(playerList, _ptr, protocol, buf, packetSize, false);

	// ���� ���� ���� �غ� ���°� �ƴ϶�� ���� �غ� ���·� �ٲ۴�.
	if (_ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_READY)
	{
		// ���ư��� Ÿ�̸� ������ Ÿ�̸� ��!
		if (_ptr->GetRoom()->GetInGameTimer()->IsRunning() == true)
		{
			_ptr->GetRoom()->GetInGameTimer()->Stop();
		}

		// 3. ���� ���� �������� ��ο��� ����
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::START_PROTOCOL,
			RESULT_INGAME::READY_START);
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

		_ptr->GetRoom()->SetRoomStatus(ROOMSTATUS::ROOM_READY);	// ���� ���� ���·� ����.
	}

	return true;
}

bool InGameManager::UpdateProcess(C_ClientInfo* _ptr, char* _buf)
{
	// Ȥ�� ���� �����µ� �濡 �����ִ� ��쿡 ���� ������ �ȵǹǷ� ����ó��
	if (_ptr->GetRoom() == nullptr
	|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return false;
	}

	// ���޵� ��Ŷ�� ����
	IngamePacket recvPacket;
	UnPackPacket(_buf, recvPacket);

	// 1. ������ �˻�(false�̸� �ҹ� �������� false����)
	CheckMovement(_ptr, recvPacket);

	// 2. �Ѿ� �˻�
	CheckBullet(_ptr, recvPacket);

	return true;   // �� �� true��
}

bool InGameManager::GetPosProcess(C_ClientInfo* _ptr, char* _buf)
{
	// ���� ���� ��� ������ ����ó��
	if (_ptr->GetRoom() == nullptr
	|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("�ٸ��� ��ġ �޴µ� ���� ����\n");
		return false;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// �÷��̾� num�� �����´�.
	IngamePacket pos;
	int playerNum;
	UnPackPacket(_buf, playerNum);

	// �������� ����
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::UPDATE_PROTOCOL, 
		RESULT_INGAME::GET_OTHERPLAYER_STATUS);

	// �ݺ��ڷ� �����鼭 playerNum�� ��ġ�ϴ� �÷��̾ ã���� �� �÷��̾��� ��ġ(��Ŷ°��)�� �������ش�.
	vector<C_ClientInfo*>& playerList = _ptr->GetRoom()->GetPlayers();	// ����Ʈ�� ����
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
	// ���� ���� ��� ������ ����ó��
	if (_ptr->GetRoom() == nullptr
	|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("��Ŀ�� �ٲٴµ� ���� ����\n");
		return false;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// �������� ����
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::FOCUS_PROTOCOL, 
		RESULT_INGAME::NODATA);

	// ���ο��� ��� �÷��̾��� �ΰ��� ������ �����ش�
	vector<C_ClientInfo*>& playerList = _ptr->GetRoom()->GetPlayers();	// ����Ʈ ����
	IngamePacket gamePacket;
	C_ClientInfo* player = nullptr;

	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;

		// �÷��̾� ������ �ڽſ��� ����
		memcpy(&gamePacket, player->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
		PackPacket(buf, gamePacket, packetSize);
		_ptr->SendPacket(protocol, buf, packetSize);
	}

	return true;
}

bool InGameManager::HitAndRunProcess(C_ClientInfo* _ptr, char* _buf)
{
	// ���� ���� ��� ������ ����ó��
	if (_ptr->GetRoom() == nullptr
		|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("���� ġ���µ� ���� ����\n");
		return false;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// ��� �������� �󸶸��� ������ ġ�������� ���´�.
	IngamePacket pos;
	float posX, posZ;
	UnPackPacket(_buf, posX, posZ);

	// �� �÷��̾� ���� Ƚ�� 1����
	_ptr->GetPlayerInfo()->GetScore().numOfDeath++;

	// ��� �÷��̾�鿡�� ���� ���� ġ�� �׾��ٴ� ����(�����Բ�)�� �����ش�
	protocol = SetProtocol(
		INGAME_STATE,
		PROTOCOL_INGAME::UPDATE_PROTOCOL,
		RESULT_INGAME::CAR_HIT);
	PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), posX, posZ, packetSize);
	ListSendPacket(_ptr->GetRoom()->GetPlayers(), _ptr, protocol, buf, packetSize, true);   // �� ���� ����

	// ���� ġ�̸� �̵��� ������ ������ߵ�
	if (_ptr->GetPlayerInfo()->IsRespawning() == false)
	{
		_ptr->GetPlayerInfo()->RespawnOn();   // ������ ����
	}

	return true;
}

bool InGameManager::CaptureProcess(C_ClientInfo* _ptr, char* _buf)
{
	// ���� ���� ��� ������ ����ó��
	if (_ptr->GetRoom() == nullptr
	|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("�ǹ� �����ߴµ� ���� ����\n");
		return false;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// ������ �ǹ� �ε��� ����
	int buildingIdx;
	UnPackPacket(_buf, buildingIdx);

	if (buildingIdx < 0)
	{
		printf("�ǹ� �ε����� ������\n");
		return false;
	}

	// ��, �� ���� ����
	RoomInfo* room = _ptr->GetRoom();
	TeamInfo& team = room->GetTeamInfo(_ptr->GetPlayerInfo()->GetTeamNum());

	// �ǹ� ������ ����
	BuildingInfo* building = room->GetBuildings().at(buildingIdx);
	
	// �� �ǹ� �����ڰ� �ִٸ� ���� �� �ǹ� ���� ����(������ ��� ���� ����)
	if (building->owner != nullptr)
	{
		int teamNum = building->owner->GetPlayerInfo()->GetTeamNum();
		building->owner->GetPlayerInfo()->GetScore().captureNum--;	// ����
	}

	// 2. �׸��� �����ڸ� �� Ŭ����ϰ� �������� �ǹ� ���� ����
	building->owner = _ptr;
	building->owner->GetPlayerInfo()->GetScore().captureNum++;

	// 3. ���� ���� ����
	int capturePoint = gameInfo[_ptr->GetGameType()]->capturePoint;
	_ptr->GetPlayerInfo()->GetScore().captureScore += capturePoint;

	// 4. ���� ī��Ʈ����
	_ptr->GetPlayerInfo()->GetScore().captureCount++;

	wprintf(L"�ǹ� ��ȣ=%d, %s����, �ܵ�����Ƚ��:%d\n",
		buildingIdx,
		_ptr->GetUserInfo()->nickname,
		_ptr->GetPlayerInfo()->GetScore().captureCount);

	// 5. �ǹ� �����ѳ��� �÷��̾� �ѹ� + �ǹ� ��ȣ�� ��� Ŭ��鿡�� ����.
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::CAPTURE_PROTOCOL, 
		RESULT_INGAME::INGAME_SUCCESS);
	PackPacket(buf, building->owner->GetPlayerInfo()->GetPlayerNum(), buildingIdx, packetSize);
	ListSendPacket(room->GetPlayers(), nullptr , protocol, buf, packetSize, true);	// ��ο��� ����!

	return true;
}

bool InGameManager::ItemGetProcess(C_ClientInfo* _ptr, char* _buf)
{
	// ���� ���� ��� ������ ����ó��
	if (_ptr->GetRoom() == nullptr
	|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("�� �Ծ��µ� ���� ����\n");
		return false;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// ���� ������ �ڵ带 ����
	int itemInt;
	ItemCode code;
	UnPackPacket(_buf, itemInt);

	code = (ItemCode)itemInt;

	// ������ �ڵ带 switch�Ͽ� �׿� �´� �Ӽ��� ��´�.
	switch (code)
	{
		// HP�� ��� �Ծ��� ��
		case ItemCode::HP_NORMAL:
		{
			// HP ��ָ�ŭ ü�� +�ϰ�
			ChangeHealthAmount(_ptr, +ItemAttribute::HP_NORMAL);
		}
		break;


		default:
			return false;
	}

	// ������ �ڵ带 �ٽ� �����ؼ� �ٸ� �÷��̾�鵵 �����ϵ��� �Ѵ�.
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::ITEM_PROTOCOL, 
		RESULT_INGAME::INGAME_SUCCESS);
	PackPacket(buf, *(_ptr->GetPlayerInfo()->GetIngamePacket()), code, packetSize);
	ListSendPacket(_ptr->GetPlayerInfo()->GetSectorPlayerList(), nullptr, protocol, buf, packetSize, true);	// ��ο��� ����

	return true;
}



bool InGameManager::GameEndProcess(RoomInfo* _room, RESULT_INGAME _result)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	vector<C_ClientInfo*>& players = _room->GetPlayers();
	protocol = SetProtocol(
		INGAME_STATE,
		PROTOCOL_INGAME::GAME_END_PROTOCOL,
		_result);

	switch (_result)
	{
		// �����ٰ� ������ �������
		case RESULT_INGAME::GAME_END_TEXT_SHOW:
		{
			ListSendPacket(players, nullptr, protocol, buf, packetSize, false);	// ��ο��� ����
			
			// ���� ���� ���·�
			_room->SetRoomStatus(ROOMSTATUS::ROOM_GAME_END);
		}
		break;

		// ��¥ �������� ���ھ �������
		case RESULT_INGAME::SCORE_SHOW:
		{
			PackPacket(buf, _room, packetSize);
			ListSendPacket(players, nullptr, protocol, buf, packetSize, false);	// ��ο��� ����

			// �濡 �ִ� �÷��̾���� �������� �ʱ�ȭ
			for (size_t i = 0; i < players.size(); i++)
			{
				ResetPlayerInfo(players[i]);
			}

			// �� ���� ���·�
			_room->SetRoomStatus(ROOMSTATUS::ROOM_END);
		}
		break;
	}

	return true;
}

bool InGameManager::LeaveProcess(C_ClientInfo* _ptr)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	switch (_ptr->GetRoom()->GetRoomStatus())
	{
		// ���� �������� ����(���� ��)
		case ROOMSTATUS::ROOM_ITEMSEL:
		{
			protocol = SetProtocol(
				STATE_PROTOCOL::INGAME_STATE,
				PROTOCOL_INGAME::DISCONNECT_PROTOCOL,
				RESULT_INGAME::WEAPON_SEL);

			// 1. �濡�ִ� �ڽ��� ������ �ٸ� Ŭ��鿡�� �ڽ��� �������� �˸���.
			PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), packetSize);
			ListSendPacket(_ptr->GetRoom()->GetPlayers(), _ptr, protocol, buf, packetSize, true);

			_ptr->GetRoom()->SetRoomStatus(ROOMSTATUS::ROOM_END);	// �� ����
		}
		break;

		// �ε� ����
		case ROOMSTATUS::ROOM_READY:
		case ROOMSTATUS::ROOM_LOAD:
		{
			protocol = SetProtocol(
				STATE_PROTOCOL::INGAME_STATE,
				PROTOCOL_INGAME::DISCONNECT_PROTOCOL,
				RESULT_INGAME::BEFORE_LOAD);

			// 1. �濡�ִ� �ڽ��� ������ �ٸ� Ŭ��鿡�� �ڽ��� �������� �˸���.
			PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), packetSize);
			ListSendPacket(_ptr->GetRoom()->GetPlayers(), _ptr, protocol, buf, packetSize, true);

			_ptr->GetRoom()->SetRoomStatus(ROOMSTATUS::ROOM_END);	// �� ����
		}
		break;

		// ���� �������� ����(���� ��)
		case ROOMSTATUS::ROOM_GAME:
		{
			protocol = SetProtocol(
				STATE_PROTOCOL::INGAME_STATE,
				PROTOCOL_INGAME::DISCONNECT_PROTOCOL,
				RESULT_INGAME::ABORT);

			// �濡�ִ� �ڽ��� ������ �ٸ� �÷��̾�鿡�� �ڽ��� �������� �˸���.
			PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), packetSize);
			ListSendPacket(_ptr->GetRoom()->GetPlayers(), _ptr, protocol, buf, packetSize, true);
		}
		break;
	}

	// �ϴ� ��� ��� true��� �����ϰ� �����Ѵ�.
	return true;
}

///////// etc

void InGameManager::InitalizePlayersInfo(RoomInfo* _room)
{
	// ������ ��ǥ ������
	int gameType = 0;
	int playerNum = 0;
	float respawnPosX = 0.0f;
	float respawnPosZ = 0.0f;

	C_ClientInfo* player = nullptr;
	vector<C_ClientInfo*>&players = _room->GetPlayers();
	for (auto iter = players.begin(); iter != players.end(); ++iter)
	{
		player = *iter;

		// �ʱ� ü��, �Ѿ� ����
		player->GetPlayerInfo()->GetIngamePacket()->health = gameInfo[_room->GetGameType()]->maxHealth;
		player->GetPlayerInfo()->SetBullet(weaponInfo[player->GetPlayerInfo()->GetWeapon()->mainW]->maxAmmo);
		
		// ������ ����
		vector<PositionInfo*>& respawnInfo = player->GetPlayerInfo()->GetPlayerRespawnInfoList();

		// ���� �ʱ���ġ�� ������ ���� �� ����
		for (int i = 0; i < locationInfo.size(); i++)
		{
			gameType = player->GetRoom()->GetGameType();
			playerNum = player->GetPlayerInfo()->GetPlayerNum();

			// �÷��̾� �ѹ� + ����Ÿ���� ��ġ�ϴ� row�� ã����
			if (gameType == locationInfo[i]->respawnInfo.gameType
				&& playerNum == locationInfo[i]->respawnInfo.playerNum)
			{
				// 1. ������ ���� �߰�
				respawnInfo.emplace_back(new PositionInfo(locationInfo[i]->respawnInfo));

				// 2. ���� �ʱ� ��ġ�� ���Եƴٸ� �̸� �ʱ���ġ�� ���� �ε��� ���� �� ������ �÷��̾� ����Ʈ�� �߰�
				if (locationInfo[i]->firstPosInfo.posX != 0.0f
					&&locationInfo[i]->firstPosInfo.posZ != 0.0f)
				{
					C_Sector* sector = player->GetRoom()->GetSector();	// �� ���� ���� �Ŵ����� ��´�.
					INDEX getIdx;
					if (sector->GetIndex(player->GetPlayerInfo()->GetIndex(), getIdx, locationInfo[i]->firstPosInfo.posX, locationInfo[i]->firstPosInfo.posZ) == true)
					{
						player->GetPlayerInfo()->SetIndex(getIdx);
						sector->Add(player, getIdx);
					}

					else
					{
						printf("��ȿ���� ���� �ε���!! %d, %d\n", getIdx.i, getIdx.j);
					}
				}
			}
		}
	}

	// ���� ��� �÷��̾���� �������� playerList�� �����´�.
	//for (auto iter = players.begin(); iter != players.end(); ++iter)
	for (size_t i = 0; i < players.size(); i++)
	{
		list<C_ClientInfo*> playerList = players[i]->GetRoom()->GetSector()->GetSectorPlayerList(players[i]->GetPlayerInfo()->GetIndex());
		players[i]->GetPlayerInfo()->SetSectorPlayerList(playerList);
	}
}

/// about movement

bool InGameManager::CheckMovement(C_ClientInfo* _ptr, IngamePacket& _recvPacket)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

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
			// �������� ������Ʈ
			UpdateSectorAndSend(_ptr, _recvPacket, getIdx);

			// 1. ���� �÷��̾� ����Ʈ ����
			list<C_ClientInfo*> sectorPlayerList = _ptr->GetRoom()->GetSector()->GetSectorPlayerList(getIdx);
			_ptr->GetPlayerInfo()->SetSectorPlayerList(sectorPlayerList);

			// 2. �濡�ִ� �ٸ� ����� �÷��̾� ����Ʈ ����
			UpdatePlayerList(_ptr->GetRoom()->GetPlayers(), _ptr);
		}

		// ���� ��� protocol�� ��ŷ
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::UPDATE_PROTOCOL, 
			RESULT_INGAME::INGAME_SUCCESS);

		// ���� �� �÷��̾�鿡�� ��Ŷ ����
		PackPacket(buf, _recvPacket, packetSize);
		ListSendPacket(_ptr->GetPlayerInfo()->GetSectorPlayerList(), _ptr, protocol, buf, packetSize, true);

		// ����������, �÷��̾��� ��Ŷ ������ �����صд�.
		IngamePacket* setPacket = new IngamePacket(_recvPacket);
		_ptr->GetPlayerInfo()->SetIngamePacket(setPacket);

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
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::UPDATE_PROTOCOL, 
			RESULT_INGAME::FORCE_MOVE);

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
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::UPDATE_PROTOCOL, 
		RESULT_INGAME::FORCE_MOVE);

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

	// 1. ���� �����ϴ� ���Ϳ� �ִ� �÷��̾���� ����Ʈ�� ������Ʈ���ְ�
	// 2. �����ϴ� ���Ϳ� �ִ� �÷��̾���� ����Ʈ�� ������Ʈ ���ش�.
	// �ٵ� �� ���߿� ��ġ�� �÷��̾ �ִٸ�

	sector->Remove(_ptr, _ptr->GetPlayerInfo()->GetIndex());    // ������ �ִ� �ε��������� ����
	sector->Add(_ptr, _newIdx);                  // ���ο� �ε�����ġ ����Ʈ�� �߰��Ѵ�.

	list<C_ClientInfo*>enterList;
	list<C_ClientInfo*>exitList;

	// ��, ������ ���� ����Ʈ
	byte playerBit = 0;	// ���Ӱ� ������ ���� ������ �÷��̾� ��Ʈ
	playerBit = sector->GetMovedSectorPlayerList(_ptr->GetPlayerInfo()->GetIndex(), _newIdx, enterList, exitList);

	// 1. ���� ���� �˸� ��Ŷ ���� �� ����
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::UPDATE_PROTOCOL, 
		RESULT_INGAME::EXIT_SECTOR);
	PackPacket(buf, _recvPacket, packetSize);

	ListSendPacket(exitList, _ptr, protocol, buf, packetSize, true);

	// 2. ���� ���� �˸� ��Ŷ ���� �� ����
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::UPDATE_PROTOCOL, 
		RESULT_INGAME::ENTER_SECTOR);
	PackPacket(buf, _recvPacket, packetSize);

	ListSendPacket(enterList, _ptr, protocol, buf, packetSize, true);

	// 3. ���ο��Դ� ���Ӱ� ������ ���� ������ �÷��̾� ����Ʈ�� Ȱ��ȭ�� ��Ʈ�� �����ش�.
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::UPDATE_PROTOCOL, 
		RESULT_INGAME::UPDATE_PLAYER);
	PackPacket(buf, playerBit, packetSize);

	_ptr->SendPacket(protocol, buf, packetSize);   // ����

	_ptr->GetPlayerInfo()->SetIndex(_newIdx);            // ����� ���ο� �ε��� ����
}

// ���� ������Ʈ
void InGameManager::UpdatePlayerList(C_ClientInfo* _player)
{
	INDEX curIdx = _player->GetPlayerInfo()->GetIndex();	// ���� �ִ� �ε���
	C_Sector* sector = _player->GetRoom()->GetSector();		// �� ���� ���� �Ŵ����� ��´�.

	// �� �÷��̾��� ���� �÷��̾� ����Ʈ�� �ٽ� �����Ѵ�.
	_player->GetPlayerInfo()->SetSectorPlayerList(sector->GetSectorPlayerList(curIdx));
}

// �÷��̾���� ���� ������Ʈ
void InGameManager::UpdatePlayerList(vector<C_ClientInfo*>& _players, C_ClientInfo* _exceptPlayer)
{
	for (auto iter = _players.begin(); iter != _players.end(); ++iter)
	{
		// ���� �÷��̾�
		if (*iter == _exceptPlayer)
			continue;

		UpdatePlayerList(*iter);
	}
}


/// about bullet
bool InGameManager::CheckBullet(C_ClientInfo* _ptr, IngamePacket& _recvPacket)
{
	IC_CS cs;

	vector<C_ClientInfo*> hitPlayers;	// _ptr�� �ѿ� ������� ����Ʈ

	// ���������� �¾Ҵ��� �¾����� true��
	bool validHitFlag = CheckBulletHitAndGetHitPlayers(_ptr, _recvPacket, hitPlayers);

	// ��������� ���¸� �ٸ� Ŭ��鿡�� ������
	if (validHitFlag == true)
	{
		BulletHitSend(_ptr, hitPlayers);
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
	if ((_numOfBullet <= shotPlayerWeapon->bulletPerShot) && (_numOfBullet > 0))
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
	byte bitMask = 0xFF;			// 8��Ʈ ����� ����ũ(1111 1111)

	// 0�� �Ű������� �Ѿ���� �߻�� ��ü �Ѿ� ������ �޶�� �ǹ̴�.
	if (_hitPlayerNum == 0)
	{
		int bulletCountBit = 0;

		bulletCountBit = _shootCountBit;	//ó�� ī��Ʈ ��Ʈ ������
		for (int i = 1; i <= TEMP_MAX_PLAYER; i++)
		{
			shifter = 8 * (TEMP_MAX_PLAYER - i);	// �̵� ���꿡 �ʿ��� ��

			if ((bulletCountBit >> shifter) > 0)
			{
				bulletCount += (bulletCountBit & (bitMask << shifter)) >> shifter;
			}
		}
	}

	else
	{
		shifter = 8 * (TEMP_MAX_PLAYER - _hitPlayerNum);	// �̵� ���꿡 �ʿ��� ��
		bulletCount = (_shootCountBit & (bitMask << shifter)) >> shifter;
	}

	return bulletCount;
}
bool InGameManager::BulletHitProcess(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer, int _numOfBullet)
{
	WeaponInfo* shotPlayerWeapon = weaponInfo[_shotPlayer->GetPlayerInfo()->GetWeapon()->mainW];
	float originalHealth = _hitPlayer->GetPlayerInfo()->GetIngamePacket()->health;
	float totalDamage = (shotPlayerWeapon->damage * _numOfBullet);

	// �̹� �� 0�̸� �� ����
	if (originalHealth == 0)
	{
		return false;
	}

	// ���� ü���� ������
	if (originalHealth - totalDamage <= 0)
	{
		_hitPlayer->GetPlayerInfo()->GetIngamePacket()->health = 0.0;
	}

	else
	{
		_hitPlayer->GetPlayerInfo()->GetIngamePacket()->health = (originalHealth - totalDamage);
	}

	return true;
}
void InGameManager::BulletDecrease(C_ClientInfo* _shotPlayer, int _numOfBullet)
{
	WeaponInfo* shotPlayerWeapon = weaponInfo[_shotPlayer->GetPlayerInfo()->GetWeapon()->mainW];

	// �� �� �� ��ŭ �Ѿ� ��
	int originalBullet = _shotPlayer->GetPlayerInfo()->GetBullet();
	int minusBullet = (_numOfBullet / shotPlayerWeapon->bulletPerShot);

	// �̹� �Ѿ� 0���̸� ����
	if (originalBullet == 0)
	{
		return;
	}

	if (originalBullet - minusBullet <= 0)
	{
		_shotPlayer->GetPlayerInfo()->SetBullet(0);
	}

	else
	{
		_shotPlayer->GetPlayerInfo()->SetBullet(originalBullet - minusBullet);
	}
}
bool InGameManager::CheckBulletHitAndGetHitPlayers(C_ClientInfo* _ptr, IngamePacket& _recvPacket, vector<C_ClientInfo*>& _hitPlayers)
{
	bool validHitFlag = false;	// ������ �¾Ҵ���

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
		for (int i = 0; i < _ptr->GetRoom()->GetMaxPlayer(); i++, bitMask >>= 1)
		{
			// ������ �˻� ����
			if (i == (_ptr->GetPlayerInfo()->GetPlayerNum() - 1))
				continue;

			// �÷��̾� ��Ʈ�� Ȱ��ȭ �Ǿ� ������
			if ((bitMask & _recvPacket.collisionCheck.playerBit) > 0)
			{
				// ���� ���̸� �¾Ҿ �� ����
				if (CheckSameTeam(_ptr, (i + 1)) == true)
				{
					continue;
				}

				// �Ѿ��� ���� �� �÷��̾ ��ȿ�� ������ �Ѿ��� �¾Ҵ��� �ٽ� �˻��Ѵ�.
				int numOfBullet = GetNumOfBullet(_recvPacket.collisionCheck.playerHitCountBit, (i + 1));
				if (CheckMaxFire(_ptr, numOfBullet) == false)	// ��ȿ���� ������ �׳� �ǳ� �ڴ�.
				{
					continue;
				}

				// ��ȿ�� �� ����� �����Ÿ� �˻� -> �Ѿ� ���� ���� -> ���� ������ ���� ������ ����.
				C_ClientInfo* hitPlayer = _ptr->GetRoom()->GetPlayerByPlayerNum(i + 1);	// �÷��̾� ��ȣ�� ã��

				// �����Ÿ� �˻��ؼ� �Ÿ��̳���� �������� ������.
				if (CheckBulletRange(_ptr, hitPlayer) == true)
				{
					// ���� �ǰ� �̹� 0�̶�� �˻���� �׳� �Ѿ��.
					if (BulletHitProcess(_ptr, hitPlayer, numOfBullet) == false)
					{
						continue;
					}

					_hitPlayers.emplace_back(hitPlayer);	// ������ ����Ʈ�� �߰�

					validHitFlag = true;
				}
			}
		}
	}

	return validHitFlag;	// ��� ����
}
void InGameManager::BulletHitSend(C_ClientInfo* _shotPlayer, const vector<C_ClientInfo*>& _hitPlayers)
{
	IC_CS cs;

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	IngamePacket packet;
	list<C_ClientInfo*>playersInSector;
	vector<C_ClientInfo*>allPlayersInRoom;
	for (int i = 0; i < _hitPlayers.size(); i++)
	{
		// ���� �� �÷��̾�鿡�� ��Ŷ ����
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::UPDATE_PROTOCOL, 
			RESULT_INGAME::BULLET_HIT);
		memcpy(&packet, _hitPlayers[i]->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
		PackPacket(buf, packet, packetSize);
		ListSendPacket(_hitPlayers[i]->GetPlayerInfo()->GetSectorPlayerList(), nullptr, protocol, buf, packetSize, true);

		// �� 0�̸� �ð� ������ ������ ������ߵ�
		if (_hitPlayers[i]->GetPlayerInfo()->GetIngamePacket()->health <= 0
			&& _hitPlayers[i]->GetPlayerInfo()->IsRespawning() == false)
		{
			Kill(_shotPlayer, _hitPlayers[i]);	// �׿����� ����, ���ھ� ó���ϰ�

			// �濡 �ִ� �÷��̾� �� ���ͼ�
			allPlayersInRoom = _shotPlayer->GetRoom()->GetPlayers();

			// �������� �����ϰ�
			protocol = SetProtocol(
				INGAME_STATE, 
				PROTOCOL_INGAME::UPDATE_PROTOCOL, 
				RESULT_INGAME::KILL);

			// ��� ��ȣ, ������ ��ȣ ������ �����ؼ� ����
			PackPacket(buf,
				_shotPlayer->GetPlayerInfo()->GetPlayerNum(),
				_hitPlayers[i]->GetPlayerInfo()->GetPlayerNum(),
				packetSize);
			ListSendPacket(allPlayersInRoom, nullptr, protocol, buf, packetSize, true);

			// �׸��� �̵��� ��Ȱ��Ű�� ������
			_hitPlayers[i]->GetPlayerInfo()->RespawnOn();
		}
	}
}
bool InGameManager::CheckSameTeam(C_ClientInfo* _player, int _otherPlayerNum)
{
	int myTeamNum = _player->GetPlayerInfo()->GetTeamNum();	// �� �� ��ȣ
	int isSameTeam = false;									// ���� ���� ������ ���

	// �� �������Ʈ �� �����ٰ�
	vector<C_ClientInfo*>& memberList = _player->GetRoom()->GetTeamInfo(myTeamNum).teamMemberList;
	for (auto iter = memberList.begin(); iter != memberList.end(); ++iter)
	{
		// ������ �� �ǳʶٰ�
		if (*iter == _player)
			continue;

		// �÷��̾� ��ȣ�� ��ġ�ϴ� ����� �� ��� ����Ʈ���� ã����
		if ((*iter)->GetPlayerInfo()->GetPlayerNum() == _otherPlayerNum)
		{
			isSameTeam = true;	// ���� ���� ���̴�.
			break;
		}
	}

	return isSameTeam;	// ��� ����
}

/// about property
void InGameManager::RefillBulletAndHealth(C_ClientInfo* _respawnPlayer)
{
	RefillBullet(_respawnPlayer);
	RefillHealth(_respawnPlayer);
}
void InGameManager::RefillBullet(C_ClientInfo* _player)
{
	int maxbullet = weaponInfo[_player->GetPlayerInfo()->GetWeapon()->mainW]->maxAmmo;
	_player->GetPlayerInfo()->SetBullet(maxbullet);
}
void InGameManager::RefillHealth(C_ClientInfo* _player)
{
	float maxHealth = gameInfo[_player->GetRoom()->GetGameType()]->maxHealth;
	_player->GetPlayerInfo()->GetIngamePacket()->health = maxHealth;
}
void InGameManager::ChangeHealthAmount(C_ClientInfo* _player, float _amount)
{
	float maxHealth = gameInfo[_player->GetGameType()]->maxHealth;			// �ִ� ü��
	float& playerHP = _player->GetPlayerInfo()->GetIngamePacket()->health;	// �÷��̾� ü��

	// 0�̸��̸� 0���� ���� ����
	if (playerHP + _amount < 0)
	{
		playerHP = 0.0f;
	}

	// �ִ�ü�� �̻��̸� �ִ�ü������ ���� ����
	else if (playerHP + _amount > maxHealth)
	{
		playerHP = maxHealth;
	}

	// �װ� �ƴϸ� �׳� amount��ŭ ����(������ -�ǰ���)
	else
	{
		playerHP += _amount;
	}
}

void InGameManager::Kill(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer)
{
	// ���� ���� ��� ������ ����ó��
	if (_shotPlayer->GetRoom() == nullptr
	|| _shotPlayer->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("�׿��µ� ���� ����\n");
		return;
	}

	_shotPlayer->GetPlayerInfo()->GetScore().numOfKill++;	// �� ��
	_hitPlayer->GetPlayerInfo()->GetScore().numOfDeath++;	// ���� ��

	// ���� ų ���� ����
	_shotPlayer->GetPlayerInfo()->GetScore().killScore +=
		gameInfo[_shotPlayer->GetGameType()]->killPoint;

}

void InGameManager::ResetPlayerInfo(C_ClientInfo* _player)
{
	_player->GetPlayerInfo()->ResetPlayerInfo();
	_player->SetGameType(-1);
	_player->SetRoom(nullptr);
}

void InGameManager::AddCaptureBonus(RoomInfo* _room, int& _team1CaptureBonus, int& _team2CaptureBonus)
{
	int capturePoint = gameInfo[_room->GetGameType()]->capturePoint;	// �⺻ ���� ����
	int bonusPoint = 0;			// ���ʽ� ���� �����
	_team1CaptureBonus = _team2CaptureBonus = 0;

	
	// �ϴ� 1:1�̵� 2:2�� ���� 2���̴�.
	int teamBonusScore[2] = { 0, };			// �� ���� ���ʽ� ����
	vector<C_ClientInfo*> memberList;
	for (int i = 0; i < 2; i++)
	{
		TeamInfo& team = _room->GetTeamInfo(i);	// ���� �� ���� ����
		memberList = team.teamMemberList;		// �� ���� �Ҽӵ� �÷��̾� ����Ʈ ����

		for (size_t j = 0; j < team.teamMemberList.size(); j++)
		{
			// ������ �ǹ� �ϳ��� �����ϰ� �ִٸ�
			Score& memberScore = memberList[j]->GetPlayerInfo()->GetScore();
			if(memberScore.captureNum > 0)
			{
				// �ǹ� ���� * (�������� / 2)�� ���ʽ� ����Ʈ�� �ش�.
				bonusPoint = (int)(memberScore.captureNum * (capturePoint * 0.5));
				memberScore.captureScore += bonusPoint;	// �� �÷��̾� ���ʽ� ���� �߰�!
				
				teamBonusScore[i] += bonusPoint;		// �� �� ���ʽ� ���� ����
			}
		}
	}

	// �� ���� �� ���� ���ھ� ����
	_team1CaptureBonus = teamBonusScore[0];
	_team2CaptureBonus = teamBonusScore[1];
}

//////// public

bool InGameManager::IngameProtocolChecker(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);
	RESULT_INGAME result;
	GetResult(buf, result);

	switch (protocol)
	{
		case WEAPON_PROTOCOL:
			return WeaponSelectProcess(_ptr, buf);

		case LOADING_PROTOCOL:
			return LoadingProcess(_ptr);

		case START_PROTOCOL:
			return InitProcess(_ptr, buf);

		case UPDATE_PROTOCOL:
		{
			switch (result)
			{
				// �ٸ� ��� ���� ��û �Ͻ�
			case GET_OTHERPLAYER_STATUS:
				return GetPosProcess(_ptr, buf);

				// ���� ġ���� ��
			case CAR_HIT:
				return HitAndRunProcess(_ptr, buf);

				// �׳� ������Ʈ�� ��
			default:
				return UpdateProcess(_ptr, buf);
			}
		}

		case FOCUS_PROTOCOL:
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
		break;

		case CAPTURE_PROTOCOL:
			return CaptureProcess(_ptr, buf);

		case ITEM_PROTOCOL:
			return ItemGetProcess(_ptr, buf);
	}

	return false;
}

bool InGameManager::CanIGotoLobby(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);

	// �κ�� ����ʹٴ� ���������̸�
	if (protocol == GOTO_LOBBY_PROTOCOL)
	{
		ResetPlayerInfo(_ptr);	// �ʱ�ȭ
		return true;
	}

	return false;
}

void InGameManager::ListSendPacket(list<C_ClientInfo*>& _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept)
{
	IC_CS cs;

	// ������ List�� ������� ���� ��쿡��
	if (!_list.empty())
	{
		// ����Ʈ �ȿ��ִ� �÷��̾�鿡�� ��Ŷ�� �����Ѵ�.
		C_ClientInfo* player = nullptr;
		for (list<C_ClientInfo*>::iterator iter = _list.begin(); iter != _list.end(); ++iter)
		{
			player = *iter;

			// ��ȿ�� Ŭ�� �ƴϸ� �� �ǳ� ��
			if (SessionManager::GetInstance()->IsClientExist(player) == false)
				continue;

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

void InGameManager::ListSendPacket(vector<C_ClientInfo*>& _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept)
{
	IC_CS cs;

	// ������ List�� ������� ���� ��쿡��
	if (!_list.empty())
	{
		// ����Ʈ �ȿ��ִ� �÷��̾�鿡�� ��Ŷ�� �����Ѵ�.
		C_ClientInfo* player = nullptr;
		for (vector<C_ClientInfo*>::iterator iter = _list.begin(); iter != _list.end(); ++iter)
		{
			player = *iter;

			// ��ȿ�� Ŭ�� �ƴϸ� �� �ǳ� ��
			if (SessionManager::GetInstance()->IsClientExist(player) == false)
				continue;

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

///threads

// �ΰ��ӿ��� ���� ���� Ÿ�̸�
DWORD WINAPI InGameManager::InGameTimerThread(LPVOID _arg)
{
	RoomInfo* room = (RoomInfo*)_arg;	// �� ������ ����
	C_Timer* timer = room->GetInGameTimer();	// Ÿ�̸� ����

	InGameManager* gameManager = InGameManager::GetInstance();

	PROTOCOL_INGAME protocol = (PROTOCOL_INGAME)0;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	bool endFlag = false;

	while (endFlag == false)
	{
		// �� ���¸� ���� ó��
		switch (room->GetRoomStatus())
		{
		// ���� ���� 
		case ROOMSTATUS::ROOM_ITEMSEL:
		{
			// Ÿ�̸� �ȵ����������� Ÿ�̸� ������ ������
			if (timer->IsRunning() == false)
			{
				timer->Start();
			}

			// ���� �� �˻�
			std::this_thread::sleep_for(std::chrono::seconds(1));	// 1�ʿ� �ѹ��� ������ ��
			gameManager->WeaponTimerChecker(room);
		}
		break;

		// �ε���
		case ROOMSTATUS::ROOM_LOAD:
		{
			// Ÿ�̸� �ȵ����������� Ÿ�̸� ������ ������
			if (timer->IsRunning() == false)
			{
				timer->Start();
			}

			// 1�� ���� �� �ε� �ִ� ��� �ð� �˻�
			std::this_thread::sleep_for(std::chrono::seconds(1));	// 1�ʿ� �ѹ��� ������ ��
			gameManager->LoadingTimeChecker(room);
		}
		break;

		// ����
		case ROOMSTATUS::ROOM_READY:
		{
			// Ÿ�̸� �ȵ����������� Ÿ�̸� ������ ������
			if (timer->IsRunning() == false)
			{
				timer->Start();
			}

			// ���� ��� �ð� �˻�
			std::this_thread::sleep_for(std::chrono::milliseconds(TIMER_INTERVAL));	// �� �־������ �ƴϸ� ȥ�� CPU �� ��Ƹ���
			gameManager->ReadyTimeChecker(room);
		}
		break;

		// ���� ��
		case ROOMSTATUS::ROOM_GAME:
		{
			// ���� Ÿ�̸Ӱ� �ȵ��ư��� �ִٸ� Ÿ�̸Ӹ� ������ �����Ѵ�.
			if (room->GetInGameTimer()->IsRunning() == false)
			{
				room->GetInGameTimer()->Start();
			}

			gameManager->RespawnChecker(room);		// ������ �˻�
			gameManager->CarSpawnChecker(room);		// �� ���� �˻�
			gameManager->InGameTimeSync(room);		// �ð� ����ȭ �˻�
			gameManager->CaptureBonusTimeChecker(room);	// ���� ���ʽ� �˻�
			gameManager->GameEndTimeChecker(room);	// ���� ���� �˻�
			
			// ���� �� �ð� ����
			std::this_thread::sleep_for(std::chrono::milliseconds(TIMER_INTERVAL));	// �� �־������ �ƴϸ� ȥ�� CPU �� ��Ƹ���
		}
		break;

		case ROOMSTATUS::ROOM_GAME_END:
		{
			// Ÿ�̸� �ȵ����������� Ÿ�̸� ������ ������
			if (timer->IsRunning() == false)
			{
				timer->Start();
			}

			// �����ð� ���� �� ���� ���� ó��
			std::this_thread::sleep_for(std::chrono::seconds(1));	// 1�ʿ� �ѹ��� ������ ��
			
			// ���� ���� ��, ��� �ð����� �����ٸ� ��¥ ���ھ� ��쵵���ϰ�, ���� �ݾ��ش�.
			if (timer->ElapsedSeconds() >= GAME_END_WAIT_TIME)
			{
				// ���� ����ó�� �Լ� ȣ��(��¥ ���ھ� �����ֶ��)
				gameManager->GameEndProcess(room, RESULT_INGAME::SCORE_SHOW);
			}
		}
		break;

		// �� ����!(���⿡�� ������ ����ߵ�!)
		case ROOMSTATUS::ROOM_END:
		{
			// Ÿ�̸� ������ �ڵ� ������
			CloseHandle(room->GetInGameTimerHandle());
			room->SetInGameTimerHandle(nullptr);

			RoomManager::GetInstance()->OnlyDeleteRoom(room);	// ��¥ �� ����
			endFlag = true;
		}
		break;

		// �� �̿ܿ��� �׳� CPU �ð� �纸�Ѵ�.
		default:
		{
			std::this_thread::sleep_for(std::chrono::milliseconds(TIMER_INTERVAL));	// �� �־������ �ƴϸ� ȥ�� CPU �� ��Ƹ���
		}
		break;
		}
	}

	printf("�ΰ���Ÿ�̸� ������ ����!\n");
	return 0;	// �׸��� ������ ����
}

// ������ üĿ
void InGameManager::RespawnChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	vector<C_ClientInfo*>&playerList = _room->GetPlayers();	// �濡 �ִ� �÷��̾� ����Ʈ

	// �濡 �ִ� ����Ʈ ��ȸ �ϸ鼭
	C_ClientInfo* player;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;

		// �׾ ������ ������� ���¶��
		if (player->GetPlayerInfo()->IsRespawning() == true)
		{
			// ������ ��� �� �󸶳� �������� ���´�.
			double respawnElapsedTime = player->GetPlayerInfo()->GetRespawnElapsedTime();
			
			// ���� ������ �ð���ŭ �ȵ����� �׳� ��� �ð��� �ø�
			if (respawnElapsedTime < gameInfo[_room->GetGameType()]->respawnTime)
			{
				respawnElapsedTime += TIMER_INTERVAL_TIMES_MILLISEC;	// �и��� ������ ����
				player->GetPlayerInfo()->SetRespawnElapsedTime(respawnElapsedTime);	// �׸��� ����
			}

			// ������ �Ǿ��ϸ�
			else
			{
				// �� �÷��̾��� ���� ��Ŷ ���� ����
				IngamePacket packet;
				memcpy(&packet, player->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));

				// ���� ��Ŷ �������� ��ġ�� ������ ��ġ�� �ٲ��ش�.
				vector<PositionInfo*>& respawnInfo = player->GetPlayerInfo()->GetPlayerRespawnInfoList();
				int rand = RandomManager::GetInstance()->GetIntNumRandom(0, (int)(respawnInfo.size() - 1));	// ���� �ε��� ����
				packet.posX = respawnInfo[rand]->posX;
				packet.posZ = respawnInfo[rand]->posZ;
				packet.action = 0;	// �׸��� ���̵� ���·�!

				// 1. ������ ��ġ�� ����� �ΰ��� ��Ŷ ����
				player->GetPlayerInfo()->SetIngamePacket(new IngamePacket(packet));

				// 2. �׸��� ������ �ؾߵǴϱ� ü���̶�, �Ѿ� ���� �Ѵ�.
				RefillBulletAndHealth(player);

				// 3. ListSend �Լ��� ���� �濡 �ִ� ��� �÷��̾�鿡�� ������ �Ѵٰ� �����Ѵ�.
				protocol = SetProtocol(
					INGAME_STATE, 
					PROTOCOL_INGAME::UPDATE_PROTOCOL, 
					RESULT_INGAME::RESPAWN);	// ������ �������� ����

				// �ΰ��� ���� ��ŷ
				memcpy(&packet, player->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
				PackPacket(buf, packet, packetSize);

				// ����
				ListSendPacket(playerList, nullptr, protocol, buf, packetSize, true);

				// 4. ������ �� ��ġ�� �ε����� ��´�.
				INDEX getIdx;
				bool isValidIdx = player->GetRoom()->GetSector()->GetIndex(player->GetPlayerInfo()->GetIndex(), getIdx, packet.posX, packet.posZ);
				if (isValidIdx == true)
				{
					// 5. ���� ���� �� ���Ͷ� ������ ���Ͷ� �ٸ��ٸ� ���� ������Ʈ ���ְ�, ���� �ε��� �����Ѵ�.
					if (getIdx != player->GetPlayerInfo()->GetIndex())
					{
						UpdateSectorAndSend(player, packet, getIdx);
						player->GetPlayerInfo()->SetIndex(getIdx);
					}
				}
				else
				{
					_tprintf(TEXT("ID:%s ������ ����(��ġ��������)\n"), player->GetUserInfo()->id);
					return;
				}

				player->GetPlayerInfo()->RespawnOff();	// ������ ��!
			}
		}
	}
}

// �ڵ��� ���� üĿ
void InGameManager::CarSpawnChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	int seed = 0;
	vector<C_ClientInfo*>& playerList = _room->GetPlayers();

	double carSpawnTimeElapsed = _room->GetCarSpawnTimeElapsed();

	// 2:2, 1:1���� �� ������ �ð� �޶���
	int CAR_SPAWN_TIME_SEC = 0;
	switch ((RoomInfo::GameType)gameInfo[_room->GetGameType()]->gameType)
	{
		case RoomInfo::GameType::_2vs2:
		{
			CAR_SPAWN_TIME_SEC = CAR_SPAWN_TIME_2vs2_SEC;
		}
		break;

		case RoomInfo::GameType::_1vs1:
		{
			CAR_SPAWN_TIME_SEC = CAR_SPAWN_TIME_1vs1_SEC;
		}
		break;
	}

	// ���� ���� �ð� �Ǹ�
	if (carSpawnTimeElapsed >= CAR_SPAWN_TIME_SEC)
	{
		// �濡 �ִ� '��Ŀ�� �ִ�' �÷��̾�鿡�� �ڵ��� �����϶�� �˷���
		seed = RandomManager::GetInstance()->GetIntNumRandom();	// ���� ���� ���
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::UPDATE_PROTOCOL, 
			RESULT_INGAME::CAR_SPAWN);
		PackPacket(buf, seed, packetSize);	// ��ŷ ��
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, true);

		_room->SetCarSpawnTimeElasped(0.0);	// �ð� �ٽ� �ʱ�ȭ
	}

	else
	{
		_room->SetCarSpawnTimeElasped(carSpawnTimeElapsed + TIMER_INTERVAL_TIMES_MILLISEC);
	}
}

void InGameManager::InGameTimeSync(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return;
	}


	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	int seed = 0;
	vector<C_ClientInfo*>& playerList = _room->GetPlayers();

	double InGameSyncTimeElapsed = _room->GetSyncTimeElapsed();
	double InGameTotalTimeElapsed = _room->GetInGameTimer()->ElapsedSeconds();

	// �ð� ����ȭ�� �ð� �Ǹ�
	if (InGameSyncTimeElapsed >= TIME_SYNC_INTERVAL)	// ���� �����ϰ� 1�ʷ� ��ƾ��ҵ�
	{
		// �濡 �ִ� ��� �÷��̾�鿡�� ���� �ΰ��� �귯�� �ð� ������
		protocol = SetProtocol(
			INGAME_STATE,
			PROTOCOL_INGAME::TIMER_PROTOCOL,
			RESULT_INGAME::INGAME_SYNC);
		PackPacket(buf, InGameTotalTimeElapsed, packetSize);	// ��ŷ ��
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

		_room->SetSyncTimeElasped(0.0);	// �ð� �ٽ� �ʱ�ȭ
	}

	else
	{
		_room->SetSyncTimeElasped(InGameSyncTimeElapsed + TIMER_INTERVAL_TIMES_MILLISEC);
	}
}

// �ǹ� ���ʽ� �ð� üĿ
void InGameManager::CaptureBonusTimeChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	double bonusTimeElapsed = _room->GetCaptureBonusTimeElapsed();

	// ���� ���ʽ� ���� �ð��� �Ǹ�
	if (bonusTimeElapsed >= CAPTURE_BONUS_TIME_SEC)
	{
		int team1CaptureBonus, team2CaptureBonus;
		AddCaptureBonus(_room, team1CaptureBonus, team2CaptureBonus);	// ���ʽ� ������ �����Ű��

		// ������Ʈ �� ������ �ִ� ��쿡�� ������.
		if (team1CaptureBonus != 0 || team2CaptureBonus != 0)
		{
			vector<C_ClientInfo*>&playerList = _room->GetPlayers();
			protocol = SetProtocol(
				INGAME_STATE,
				PROTOCOL_INGAME::CAPTURE_PROTOCOL,
				RESULT_INGAME::BONUS);
			PackPacket(buf, team1CaptureBonus, team2CaptureBonus, packetSize);	// ��ŷ ��
			ListSendPacket(playerList, nullptr, protocol, buf, packetSize, true);
		}

		_room->SetCaptureBonusTimeElasped(0.0);	// �ð� �ٽ� �ʱ�ȭ
	}

	else
	{
		_room->SetCaptureBonusTimeElasped(bonusTimeElapsed + TIMER_INTERVAL_TIMES_MILLISEC);
	}
}

// ���� Ÿ�̸� üĿ
void InGameManager::WeaponTimerChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_ITEMSEL)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	vector<C_ClientInfo*>& playerList = _room->GetPlayers();	// ����Ʈ ����

	if (_room->GetNumOfPlayer() < _room->GetMaxPlayer())
	{
		printf("��� ���� �� �޴ٰ� ���� ����\n");

		// ���� �������� ����
		protocol = SetProtocol(
			STATE_PROTOCOL::INGAME_STATE,
			PROTOCOL_INGAME::DISCONNECT_PROTOCOL,
			RESULT_INGAME::WEAPON_SEL);

		// �濡 ��� �÷��̾�鿡�� �� �����ٰ� �˸�
		ListSendPacket(_room->GetPlayers(), nullptr, protocol, buf, packetSize, true);

		_room->SetRoomStatus(ROOMSTATUS::ROOM_END);	// �� Ÿ�̸ӵ� ����
		return;
	}

	// ���ݱ��� �帥 �ʸ� ����
	double weaponTimeElapsedSec = _room->GetInGameTimer()->ElapsedSeconds();

	// ���� ������ ���ýð�(���)�� �Ѿ��ٸ� ���⸦ ������� ���������� �����ش�.
	if (weaponTimeElapsedSec >= WEAPON_SELTIME)
	{
		// ���� ������ ���������� �������� ����
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::WEAPON_PROTOCOL, 
			RESULT_INGAME::NODATA);

		// ���� �濡 �ִ� "���" �÷��̾�� ���⸦ ������� ���������� ������.
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

		_room->GetInGameTimer()->Stop();	// ���⼱�� Ÿ�̸� ��!
		_room->SetRoomStatus(ROOMSTATUS::ROOM_LOAD);	// �� ��� �ε� ���·�
	}

	// ���� ���� ���� �ð��� ���Ҵٸ� ���� �ð��� �����ش�.
	else
	{
		// ���� �濡 �ִ� "���" �÷��̾�� ���� ���� ����������� ���� �ð��� ������
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::TIMER_PROTOCOL, 
			RESULT_INGAME::WEAPON_SEL);
		PackPacket(buf, (int)((WEAPON_SELTIME - (int)weaponTimeElapsedSec)), packetSize);
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);
	}
}

// �ε� �ִ� �ð� üĿ
void InGameManager::LoadingTimeChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_LOAD)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	vector<C_ClientInfo*>& playerList = _room->GetPlayers();

	// ���ݱ��� �帥 �ʸ� ����
	double loadingTimeElapsed = _room->GetInGameTimer()->ElapsedSeconds();

	// �ε� �ִ� ���ð� �Ѿ��
	if (loadingTimeElapsed >= MAX_LOADING_WAIT_TIME)
	{
		// �α� �� ���
		printf("�ִ� �ε��ð� �Ѿ\n");

		// �濡 �ִ� �÷��̾�鿡�� ���ð� �ʰ� �������� ����
		protocol = SetProtocol(
			STATE_PROTOCOL::INGAME_STATE,
			PROTOCOL_INGAME::DISCONNECT_PROTOCOL,
			RESULT_INGAME::MAX_LOADING_TIMEWAIT);
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, true);

		_room->SetRoomStatus(ROOMSTATUS::ROOM_END);	// �� Ÿ�̸ӵ� ����
	}
}

// ���� �ð� üĿ
void InGameManager::ReadyTimeChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_READY)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	vector<C_ClientInfo*>& playerList = _room->GetPlayers();

	// ���ݱ��� �帥 �ʸ� ����
	double readyTimeElapsedSec = _room->GetInGameTimer()->ElapsedSeconds();

	// ���� �ð� ������
	if (readyTimeElapsedSec >= READY_TIME)
	{
		// �α� �� ���
		printf("����ð� ��\n");

		// �濡 �ִ� �÷��̾�鿡�� ���� ��! �������� ����
		protocol = SetProtocol(
			STATE_PROTOCOL::INGAME_STATE,
			PROTOCOL_INGAME::START_PROTOCOL,
			RESULT_INGAME::READY_END);

		// �濡 ��� �÷��̾�鿡�� �������� ����
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

		_room->GetInGameTimer()->Stop();				// ���� Ÿ�̸� ��!
		_room->SetRoomStatus(ROOMSTATUS::ROOM_GAME);	// ���� ���ӻ��·�!
	}
}

// ���� ���� Ÿ�̸� üĿ
void InGameManager::GameEndTimeChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return;
	}

	// ���ݱ��� �帥 �ʸ� ����
	double IngameTimeElapsed = _room->GetInGameTimer()->ElapsedSeconds();

	// Ÿ�̸� ���ٰ� �� �� ���� Ÿ�� �ִ� �ð� ������ ���� �����ٰ� ������ߵ�
	if (IngameTimeElapsed >= gameInfo[_room->GetGameType()]->gameTime)
	{
		// ���ӳ�
		printf("���� ��!\n");

		// Ÿ�̸� ���������� Ÿ�̸� ��
		C_Timer* timer = _room->GetInGameTimer();
		if (timer->IsRunning() == true)
		{
			timer->Stop();
		}

		GameEndProcess(_room, RESULT_INGAME::GAME_END_TEXT_SHOW);	// �ϴ� �����ٰ� �ؽ�Ʈ ���� ��������
	}
}
