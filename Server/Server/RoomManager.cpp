#include "stdafx.h"
#include "RoomManager.h"
#include "InGameManager.h"

#include "C_ClientInfo.h"

////////////////////////// RoomInfo ����ü //////////////////////////////
RoomInfo::RoomInfo(int _gameType, const list<C_ClientInfo*>& _playerList, int _numOfPlayer)
{
	weaponTimerHandle = NULL;

	roomStatus = ROOMSTATUS::ROOM_NONE;	// �� ������ �ʱ� ���´� �ƹ� ���µ��ƴ�

	gameType = _gameType;
	maxPlayer = numOfPlayer = _numOfPlayer;
	
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
		}
		break;
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
	if (InGameManager::GetInstance()->LeaveProcess(_player, _player->GetPlayerInfo()->GetIngamePacket()->playerNum) == true)
	{
		// �� �ο����� �����ϰ�, �ڽ��� ������ �����.
		numOfPlayer--;					// �� �ο��� ����
		players.erase(remove(players.begin(), players.end(), _player), players.end());		// ���� �÷��̾� ����Ʈ���� ����
		
		_player->SetRoom(nullptr);		// �÷��̾��� ���� null�� ����

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

bool RoomManager::CreateRoom(list<C_ClientInfo*>_players, int _numOfPlayer)
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
	bool ret = (beforeSize < nowSize);

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
	int beforeSize = (int)roomList.size();
	roomList.remove(_room);

	if (beforeSize > (int)roomList.size())
	{
		printf("[�� �Ҹ� ����]������:%d\n", (int)roomList.size());
		return true;
	}

	return false;
}

bool RoomManager::CheckLeaveRoom(C_ClientInfo* _ptr)
{
	// ���� ���� �ִٸ�
	if (_ptr->GetRoom() != nullptr)
	{
		RoomInfo* room = _ptr->GetRoom();

		// ���� ���� �� �ִ��� �˻��ؼ� �����ٸ�
		if (room->LeaveRoom(_ptr) == true)
		{
			/// �׸��� ���� �� 2���� ��� ������ �׳� ���� ��������

			// ��� ��������� �������̾��ٸ� ���� ����
			if (room->IsPlayerListEmpty() == true)
			{
				roomList.remove(room);
				printf("[�� �Ҹ�]������:%d\n", (int)roomList.size());
			}

			return true;
		}
	}

	return false;
}