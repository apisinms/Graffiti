#include "stdafx.h"
#include "RoomManager.h"
#include "InGameManager.h"
#include "C_Timer.h"

#include "C_ClientInfo.h"

////////////////////////// RoomInfo 구조체 //////////////////////////////
RoomInfo::RoomInfo(int _gameType, const list<C_ClientInfo*>& _playerList, int _numOfPlayer)
{
	carSpawnTimeElapsed = 0.0;
	captureBonusTimeElapsed = 0.0;
	InGameTimeSyncElapsed = 0.0;
	InGameTimer = new C_Timer();	// 타이머 생성

	roomStatus = ROOMSTATUS::ROOM_NONE;	// 방 생성시 초기 상태는 아무 상태도아님

	gameType = _gameType;
	maxPlayer = numOfPlayer = _numOfPlayer;
	
	int MAX_BUILDING_NUM = 0;

	switch (gameType)
	{
		case GameType::_2vs2:
		{
			teamInfo = new TeamInfo[2];

			// 2명씩 끊어서 저장
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
			teamInfo = new TeamInfo[2];	// 나중에 3개 이상 팀이 만약 생긴다면..(확장성 고려)

			// 각 팀에 1명씩 저장
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

	// 건물 생성
	BuildingInfo info;
	for (int i = 0; i < MAX_BUILDING_NUM; i++)
	{
		info.buildingIndex = i;
		info.owner = nullptr;

		buildings.emplace_back(new BuildingInfo(info));
	}

	// 방 플레이어 리스트에 추가
	for (auto iter = _playerList.begin(); iter != _playerList.end(); ++iter)
	{
		players.emplace_back(*iter);
	}

	sector = new C_Sector();	// 이 방의 섹터관리자 생성
}

bool RoomInfo::LeaveRoom(C_ClientInfo* _player)
{	
	// 내가 나간다는 것을 같은 방에 있는 플레이어들에게 보내줌
	if (InGameManager::GetInstance()->LeaveProcess(_player) == true)
	{
		// 1. 방 인원수를 감소하고, 자신의 흔적을 지운다.
		numOfPlayer--;
		sector->Remove(_player, _player->GetPlayerInfo()->GetIndex());
		players.erase(remove(players.begin(), players.end(), _player), players.end());		// 방의 플레이어 리스트에서 제거
		
		// 2. 다른 플레이어들에게 섹터에 있는 플레이어 리스트를 업데이트 시켜준다.
		InGameManager::GetInstance()->UpdatePlayerList(_player->GetRoom()->GetPlayers(), _player);

		return true;
	}

	return false;	// 못찾은 경우
}

//C_ClientInfo* RoomInfo::GetPlayerByNum(int _playerNum)
//{
//}

////////////////////////// RoomManager 클래스 //////////////////////////
RoomManager* RoomManager::instance;

RoomManager* RoomManager::GetInstance()
{
	// 인스턴스가 없다면 인스턴스를 생성하고 리턴한다.
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
	int gameType = (_players.front())->GetGameType();	// 게임 타입!
	
	// 플레이어 목록을 토대로 방을 만듦.
	RoomInfo* room = new RoomInfo(
		gameType,	
		_players, _numOfPlayer);

	// 각 플레이어들에게 현재 속한 방을 설정.
	for (auto iter = _players.begin(); iter != _players.end(); ++iter)
	{
		((C_ClientInfo*)(*iter))->SetRoom(room);
	}

	// 방 리스트에 추가
	int beforeSize = (int)roomList.size();
	roomList.emplace_back(room);

	int nowSize = (int)roomList.size();
	bool ret = (beforeSize < nowSize) && (room != nullptr);	// 방이 정상적으로 만들어지고, 리스트에 추가됐으면 true

	switch ((RoomInfo::GameType)gameType)
	{
		case RoomInfo::GameType::_2vs2:
		{
			printf("[2:2][방생성] 총 갯수 : %d\n", nowSize);
		}
		break;

		case RoomInfo::GameType::_1vs1:
		{
			printf("[1:1][방생성] 총 갯수 : %d\n", nowSize);
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

	// 타이머 핸들 돌아가는 중이면 끝날 때까지 대기
	HANDLE IngameTimerHandle = _room->GetInGameTimerHandle();
	if (IngameTimerHandle != nullptr)
	{
		_room->SetRoomStatus(ROOMSTATUS::ROOM_END);	// 방 종료

		return false;	// 방 못지움
	}

	// 안돌아가는 중이면 그냥 바로 delete
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

	// 0. 방에 있는 모든 플레이어들이 더이상 이 방의 포인터를 가리키지 않도록 함
	for (auto iter = _room->GetPlayers().begin(); iter < _room->GetPlayers().end(); ++iter)
	{
		(*iter)->SetRoom(nullptr);
		(*iter)->SetGameType(-1);
	}

	// 1. 방에 있는 건물들 싹 지움
	for (auto iter = _room->GetBuildings().begin(); iter != _room->GetBuildings().end(); ++iter)
	{
		if (*iter != nullptr)
		{
			delete *iter;
			*iter = nullptr;
		}
	}

	// 2. 팀 포인터 지움
	_room->DeleteTeam();

	// 3. 섹터 지움(내부에서 지움)
	_room->SetSector(nullptr);

	// 4. 타이머 지움(내부에서 지움)
	_room->SetInGameTimer(nullptr);

	// 5. 방 자체를 지움
	roomList.remove(_room);

	if (_room != nullptr)
	{
		delete _room;
		_room = nullptr;
	}

	if (beforeSize > (int)roomList.size())
	{
		printf("[방 소멸 성공]사이즈:%d\n", (int)roomList.size());
		return true;	// 정상 방 삭제
	}

	return false;
}

bool RoomManager::CheckLeaveRoom(C_ClientInfo* _ptr)
{
	IC_CS cs;

	// 속한 방이 있다면
	if (_ptr->GetRoom() != nullptr)
	{
		RoomInfo* room = _ptr->GetRoom();
		int playerTeamNum = _ptr->GetPlayerInfo()->GetTeamNum();

		// 방을 나갈 수 있는지 검사해서 나갔다면
		if (room->LeaveRoom(_ptr) == true)
		{
			// 1. 본인이 속한 팀 멤버리스트에서 지워준다.
			vector<C_ClientInfo*>& memberList = _ptr->GetRoom()->GetTeamInfo(playerTeamNum).teamMemberList;
			memberList.erase(remove(memberList.begin(), memberList.end(), _ptr), memberList.end());		// 팀 멤버 리스트에서 제거

			// 게임중에 팀원이 모두 나갔으면 게임 끝!
			if (room->GetRoomStatus() == ROOMSTATUS::ROOM_GAME
				&& memberList.size() == 0)
			{
				InGameManager::GetInstance()->GameEndProcess(_ptr->GetRoom());
			}

			// 방금 나간사람이 마지막이었다면 방을 지운다.
			if (room->IsPlayerListEmpty() == true)
			{
				InGameManager::GetInstance()->ResetPlayerInfo(_ptr);	// 나간 이 친구 플레이어 정보 초기화
				DeleteRoom(room);
			}

			_ptr->SetRoom(nullptr);	// 여기서 지워줘야 DeleteRoom()도 호출가능
			return true;
		}
	}

	return false;
}