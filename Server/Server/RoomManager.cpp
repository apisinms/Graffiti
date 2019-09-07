#include "stdafx.h"
#include "RoomManager.h"
#include "InGameManager.h"

#include "C_ClientInfo.h"

////////////////////////// RoomInfo 구조체 //////////////////////////////
RoomInfo::RoomInfo(C_ClientInfo** _playerList, int _numOfPlayer)
{
	weaponTimerHandle = NULL;

	roomStatus = ROOMSTATUS::ROOM_NONE;	// 방 생성시 초기 상태는 아무 상태도아님

	numOfPlayer = _numOfPlayer;

	// 방 플레이어 리스트에 추가
	for (int i = 0; i < numOfPlayer; i++)
		playerList.emplace_front(_playerList[i]);

	curIter = playerList.begin();	// 현재 반복자 위치를 처음으로 설정

	sector = new C_Sector();	// 이 방의 섹터관리자 생성
}

bool RoomInfo::LeaveRoom(C_ClientInfo* _player)
{															// 이런 식으로 접근하는거 나중에 바꾸자
	if (InGameManager::GetInstance()->LeaveProcess(_player, _player->GetPosition()->playerNum) == true)
	{
		// 방 인원수를 감소하고, 자신의 흔적을 지운다.
		numOfPlayer--;				// 방 인원수 감소
		playerList.remove(_player);		// 방의 플레이어 리스트에서 제거
		_player->SetRoom(nullptr);	// 플레이어의 방을 null로 설정
		curIter = playerList.begin();	// 반복자가 꼬이지 않게 다시 처음으로(나갔으니까)

		/// 그리고 같은 팀 2명이 모두 나가면 그냥 게임 끝나야함

		// 방금 나간사람이 마지막이었다면 방을 없앰
		if (numOfPlayer == 0)
			RoomManager::GetInstance()->DeleteRoom(this);

		return true;
	}

	//// 리스트를 순회하며
	//int i = 0;
	//for (list<C_ClientInfo*>::iterator iter = playerList.begin();
	//	iter != playerList.end(); ++iter, i++)
	//{
	//	// 자신을 찾은 경우
	//	if (*iter == _player)
	//	{
	//		// 다른 플레이어들에게 자신이 나간다고 알리고
	//		if (InGameManager::GetInstance()->LeaveProcess(_player, (i + 1)) == true)
	//		{
	//			// 방 인원수를 감소하고, 자신의 흔적을 지운다.
	//			numOfPlayer--;				// 방 인원수 감소
	//			//playerList.erase(iter++);		// 방의 플레이어 리스트에서 제거
	//			playerList.erase(iter++);		// 방의 플레이어 리스트에서 제거
	//			_player->SetRoom(nullptr);	// 플레이어의 방을 null로 설정
	//			curIter = playerList.begin();	// 반복자가 꼬이지 않게 다시 처음으로(나갔으니까)

	//			/// 그리고 같은 팀 2명이 모두 나가면 그냥 게임 끝나야함

	//			// 방금 나간사람이 마지막이었다면 방을 없앰
	//			if (numOfPlayer == 0)
	//				RoomManager::GetInstance()->DeleteRoom(this);

	//			return true;
	//		}
	//	}
	//}

	return false;	// 못찾은 경우
}

bool RoomInfo::GetPlayer(C_ClientInfo* &_ptr, bool _isReset)
{
	/*
	중간에 누가 나가면 문제가 될 수 있는 코드임...
	*/
	static bool flag = true;	// 플래그가 켜져 있다면 아직 보낼 플레이어 리스트 정보가 남음
	
	// 리셋
	if (_isReset == true)
	{
		flag = true;
		curIter = playerList.begin();

		return false;
	}

	// 끝에 도달하면 flag 비활성
	if (curIter == playerList.end())
	{
		flag = false;
		curIter = playerList.begin();	// 다음번에 이 end() 조건문에 들어오면 안되니까
		return flag;
	}

	// flag가 false이면 다시 셋팅해줘야한다.
	if (flag == false)
	{
		flag = true;
		curIter = playerList.begin();
	}

	_ptr = *curIter;	// 리턴할 플레이어 정보 저장
	++curIter;			// 다음 위치로

	return flag;	// flag를 리턴하여 false가 아닐때까지 반복호출하면 된다.
}


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
	roomList = new C_List<RoomInfo*>();
}
void RoomManager::End()
{
	delete roomList;
}

void RoomManager::Destroy()
{
	delete instance;
}

bool RoomManager::CreateRoom(C_ClientInfo* _players[], int _numOfPlayer)
{
	// 플레이어 목록을 토대로 방을 만듦.
	RoomInfo* room = new RoomInfo(_players, _numOfPlayer);

	// 각 플레이어들에게 현재 속한 방을 설정.
	for (int i = 0; i < _numOfPlayer; i++)
		_players[i]->SetRoom(room);

	// 방 리스트에 추가
	bool ret = roomList->Insert(room);
	printf("[방생성] 갯수 : %d\n", roomList->GetCount());

	return ret;
}

bool RoomManager::DeleteRoom(RoomInfo* _room)
{
	if (roomList->Delete(_room) == true)
	{
		printf("[방 소멸]사이즈:%d\n", roomList->GetCount());
		return true;
	}

	return false;
}

bool RoomManager::CheckLeaveRoom(C_ClientInfo* _ptr)
{
	// 속한 방이 있다면
	if (_ptr->GetRoom() != nullptr)
	{
		RoomInfo* room = _ptr->GetRoom();

		// 방을 나갈 수 있는지 검사해서 나갔다면
		if (room->LeaveRoom(_ptr) == true)
		{
			/// 그리고 같은 팀 2명이 모두 나가면 그냥 게임 끝나야함

			// 방금 나간사람이 마지막이었다면 방을 없앰
			if (room->IsPlayerListEmpty() == true)
			{
				roomList->Delete(room);
				printf("[방 소멸]사이즈:%d\n", roomList->GetCount());
			}

			return true;
		}
	}

	return false;
}