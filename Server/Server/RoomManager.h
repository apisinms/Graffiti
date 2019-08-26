#pragma once
#include "C_Global.h"
#include "C_Sector.h"

class C_ClientInfo;

//// 팀 정보
//struct Team
//{
//	C_ClientInfo* player1;
//	C_ClientInfo* player2;
//
//	Team() {}
//	Team(C_ClientInfo* _player1, C_ClientInfo* _player2)
//	{
//		player1 = _player1;
//		player2 = _player2;
//	}
//};

// 방의 정보
struct RoomInfo
{
	// 여기도 private으로 바꿔야할듯
	void* weaponTimerHandle;		// 무기 선택 타이머 핸들
	int numOfPlayer;				// 현재 방에 있는 플레이어 수
	ROOMSTATUS roomStatus;			// 방의 상태
	list<C_ClientInfo*>playerList;	// 유저들을 리스트에 저장
	C_Sector* sector;


	RoomInfo(C_ClientInfo** _playerList, int _numOfPlayer)
	{
		weaponTimerHandle = NULL;

		roomStatus = ROOMSTATUS::ROOM_NONE;	// 방 생성시 초기 상태는 아무 상태도아님

		numOfPlayer = _numOfPlayer;

		// 방 플레이어 리스트에 추가
		for (int i = 0; i < numOfPlayer; i++)
			playerList.emplace_front(_playerList[i]);

		sector = new C_Sector();	// 섹터관리자 생성
	}

	bool LeaveRoom(C_ClientInfo* _player)
	{
		// 리스트를 순회하며
		for (list<C_ClientInfo*>::iterator iter = playerList.begin();
			iter != playerList.end(); ++iter)
		{
			// 자신을 찾은 경우
			if (*iter == _player)
			{

				/// 여기 방 소멸 부분에서 다시 해야함

				// 다른 플레이어들에게 자신이 나간다고 알리고
				if (InGameManager::GetInstance()->LeaveProcess(_player, (i + 1)) == true)
				{
					// 방 인원수를 감소하고, 자신의 흔적을 지운다.
					room->curNumOfPlayer--;			// 방 인원수 감소
					room->playerList[i] = nullptr;
					_ptr->SetRoom(nullptr);

					/// 그리고 같은 팀 2명이 모두 나가면 그냥 게임 끝나야함
					// 방금 나간사람이 마지막이었다면 방을 없앰
					if (room->curNumOfPlayer == 0)
					{
						roomList->Delete(room);
						printf("[방 소멸]사이즈:%d\n", roomList->GetCount());
					}

					return true;
				}
			}
		}


	}

	bool IsPlayerListEmpty()
	{
		return playerList.empty();
	}

};

class RoomManager
{
	C_List<RoomInfo*>* roomList;

private:
	static RoomManager* instance;

	RoomManager() {}
	~RoomManager() {}
public:
	static RoomManager* GetInstance();
	static void Destroy();
	void Init();
	void End();

public:
	bool CreateRoom(C_ClientInfo* _players[], int _numOfPlayer);
	bool CheckLeaveRoom(C_ClientInfo* _ptr);
};