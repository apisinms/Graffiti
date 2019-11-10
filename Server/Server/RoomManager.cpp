#include "stdafx.h"
#include "RoomManager.h"
#include "InGameManager.h"
#include "C_Timer.h"

#include "C_ClientInfo.h"

////////////////////////// RoomInfo ����ü //////////////////////////////
RoomInfo::RoomInfo(int _gameType, const list<C_ClientInfo*>& _playerList, int _numOfPlayer)
{
	carSpawnTimeElapsed = 0.0;
	captureBonusTimeElapsed = 0.0;
	InGameTimeSyncElapsed = 0.0;
	InGameTimer = new C_Timer();	// Ÿ�̸� ����

	roomStatus = ROOMSTATUS::ROOM_NONE;	// �� ������ �ʱ� ���´� �ƹ� ���µ��ƴ�

	gameType = _gameType;
	maxPlayer = numOfPlayer = _numOfPlayer;
	
	int MAX_BUILDING_NUM = 0;

	switch (gameType)
	{
		case GameType::_2vs2:
		{
			teamInfo = new TeamInfo[2];

			// 2�� ��� ����
			int teamIdx = 0;
			int i = 0;
			for (auto iter = _playerList.begin(); iter != _playerList.end(); ++iter)
			{
				teamInfo[teamIdx].teamMemberList.emplace_back(*iter);
				((C_ClientInfo*)(*iter))->GetPlayerInfo()->SetTeamNum(teamIdx);

				if ((++i) % 2 == 0)
					teamIdx++;
			}

			MAX_BUILDING_NUM = MAX_BUILDINGS_2VS2;
		}
		break;

		case GameType::_1vs1:
		{
			teamInfo = new TeamInfo[2];	// ���߿� 3�� �̻� ���� ���� ����ٸ�..(Ȯ�强 ���)

			// �� ���� 1�� ����
			int i = 0;
			for (auto iter = _playerList.begin(); iter != _playerList.end(); ++iter, i++)
			{
				teamInfo[i].teamMemberList.emplace_back(*iter);
				((C_ClientInfo*)(*iter))->GetPlayerInfo()->SetTeamNum(i);
			}

			MAX_BUILDING_NUM = MAX_BUILDINGS_1VS1;
		}
		break;
	}

	// �ǹ� ����
	BuildingInfo info;
	for (int i = 0; i < MAX_BUILDING_NUM; i++)
	{
		info.buildingIndex = i;
		info.owner = nullptr;

		buildings.emplace_back(new BuildingInfo(info));
	}

	// �� �÷��̾� ����Ʈ�� �߰�
	for (auto iter = _playerList.begin(); iter != _playerList.end(); ++iter)
	{
		players.emplace_back(*iter);
	}

	sector = new C_Sector();	// �� ���� ���Ͱ����� ����
}

bool RoomInfo::LeaveRoom(C_ClientInfo* _player)
{	
	// ���� �����ٴ� ���� ���� �濡 �ִ� �÷��̾�鿡�� ������
	if (InGameManager::GetInstance()->LeaveProcess(_player) == true)
	{
		// 1. �� �ο����� �����ϰ�, �ڽ��� ������ �����.
		numOfPlayer--;
		sector->Remove(_player, _player->GetPlayerInfo()->GetIndex());
		players.erase(remove(players.begin(), players.end(), _player), players.end());		// ���� �÷��̾� ����Ʈ���� ����
		
		// 2. �ٸ� �÷��̾�鿡�� ���Ϳ� �ִ� �÷��̾� ����Ʈ�� ������Ʈ �����ش�.
		InGameManager::GetInstance()->UpdatePlayerList(_player->GetRoom()->GetPlayers(), _player);

		return true;
	}

	return false;	// ��ã�� ���
}

//C_ClientInfo* RoomInfo::GetPlayerByNum(int _playerNum)
//{
//}

////////////////////////// RoomManager Ŭ���� //////////////////////////
RoomManager* RoomManager::instance;

RoomManager* RoomManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
	if (instance == nullptr)
		instance = new RoomManager();

	return instance;
}

void RoomManager::Init()
{
}
void RoomManager::End()
{
}

void RoomManager::Destroy()
{
	delete instance;
}

bool RoomManager::CreateRoom(list<C_ClientInfo*>&_players, int _numOfPlayer)
{
	int gameType = (_players.front())->GetGameType();	// ���� Ÿ��!
	
	// �÷��̾� ����� ���� ���� ����.
	RoomInfo* room = new RoomInfo(
		gameType,	
		_players, _numOfPlayer);

	// �� �÷��̾�鿡�� ���� ���� ���� ����.
	for (auto iter = _players.begin(); iter != _players.end(); ++iter)
	{
		((C_ClientInfo*)(*iter))->SetRoom(room);
	}

	// �� ����Ʈ�� �߰�
	int beforeSize = (int)roomList.size();
	roomList.emplace_back(room);

	int nowSize = (int)roomList.size();
	bool ret = (beforeSize < nowSize) && (room != nullptr);	// ���� ���������� ���������, ����Ʈ�� �߰������� true

	switch ((RoomInfo::GameType)gameType)
	{
		case RoomInfo::GameType::_2vs2:
		{
			printf("[2:2][�����] �� ���� : %d\n", nowSize);
		}
		break;

		case RoomInfo::GameType::_1vs1:
		{
			printf("[1:1][�����] �� ���� : %d\n", nowSize);
		}
		break;
	}

	return ret;
}

bool RoomManager::DeleteRoom(RoomInfo* _room)
{
	if (_room == nullptr)
	{
		return false;
	}

	// Ÿ�̸� �ڵ� ���ư��� ���̸� ���� ������ ���
	HANDLE IngameTimerHandle = _room->GetInGameTimerHandle();
	if (IngameTimerHandle != nullptr)
	{
		_room->SetRoomStatus(ROOMSTATUS::ROOM_END);	// �� ����

		return false;	// �� ������
	}

	// �ȵ��ư��� ���̸� �׳� �ٷ� delete
	else
	{
		OnlyDeleteRoom(_room);
	}

	return false;
}

bool RoomManager::OnlyDeleteRoom(RoomInfo* _room)
{
	IC_CS cs;

	int beforeSize = (int)roomList.size();

	// 0. �濡 �ִ� ��� �÷��̾���� ���̻� �� ���� �����͸� ����Ű�� �ʵ��� ��
	for (auto iter = _room->GetPlayers().begin(); iter < _room->GetPlayers().end(); ++iter)
	{
		(*iter)->SetRoom(nullptr);
		(*iter)->SetGameType(-1);
	}

	// 1. �濡 �ִ� �ǹ��� �� ����
	for (auto iter = _room->GetBuildings().begin(); iter != _room->GetBuildings().end(); ++iter)
	{
		if (*iter != nullptr)
		{
			delete *iter;
			*iter = nullptr;
		}
	}

	// 2. �� ������ ����
	_room->DeleteTeam();

	// 3. ���� ����(���ο��� ����)
	_room->SetSector(nullptr);

	// 4. Ÿ�̸� ����(���ο��� ����)
	_room->SetInGameTimer(nullptr);

	// 5. �� ��ü�� ����
	roomList.remove(_room);

	if (_room != nullptr)
	{
		delete _room;
		_room = nullptr;
	}

	if (beforeSize > (int)roomList.size())
	{
		printf("[�� �Ҹ� ����]������:%d\n", (int)roomList.size());
		return true;	// ���� �� ����
	}

	return false;
}

bool RoomManager::CheckLeaveRoom(C_ClientInfo* _ptr)
{
	IC_CS cs;

	// ���� ���� �ִٸ�
	if (_ptr->GetRoom() != nullptr)
	{
		RoomInfo* room = _ptr->GetRoom();
		int playerTeamNum = _ptr->GetPlayerInfo()->GetTeamNum();

		// ���� ���� �� �ִ��� �˻��ؼ� �����ٸ�
		if (room->LeaveRoom(_ptr) == true)
		{
			// 1. ������ ���� �� �������Ʈ���� �����ش�.
			vector<C_ClientInfo*>& memberList = _ptr->GetRoom()->GetTeamInfo(playerTeamNum).teamMemberList;
			memberList.erase(remove(memberList.begin(), memberList.end(), _ptr), memberList.end());		// �� ��� ����Ʈ���� ����

			// �����߿� ������ ��� �������� ���� ��!
			if (room->GetRoomStatus() == ROOMSTATUS::ROOM_GAME
				&& memberList.size() == 0)
			{
				InGameManager::GetInstance()->GameEndProcess(_ptr->GetRoom());
			}

			// ��� ��������� �������̾��ٸ� ���� �����.
			if (room->IsPlayerListEmpty() == true)
			{
				InGameManager::GetInstance()->ResetPlayerInfo(_ptr);	// ���� �� ģ�� �÷��̾� ���� �ʱ�ȭ
				DeleteRoom(room);
			}

			_ptr->SetRoom(nullptr);	// ���⼭ ������� DeleteRoom()�� ȣ�Ⱑ��
			return true;
		}
	}

	return false;
}