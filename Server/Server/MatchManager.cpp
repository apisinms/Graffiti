#include "stdafx.h"
#include "MatchManager.h"
#include "RoomManager.h"
#include "C_ClientInfo.h"
#include "InGameManager.h"

MatchManager* MatchManager::instance;

MatchManager* MatchManager::GetInstance()
{
	// 인스턴스가 없다면 인스턴스를 생성하고 리턴한다.
	if (instance == nullptr)
		instance = new MatchManager();

	return instance;
}

void MatchManager::Init()
{
	waitList = new list<C_ClientInfo*>[RoomInfo::GameType::_MAX_GAMETYPE];
}

void MatchManager::End()
{
	delete[]waitList;
}

void MatchManager::Destroy()
{
	delete instance;
}

void MatchManager::WaitListRemove(C_ClientInfo* _ptr)
{
	IC_CS cs;

	int gameType = _ptr->GetGameType();
	if (gameType != -1)
	{
		// 지워졌을때만 출력함
		int beforeSize = (int)waitList[gameType].size();
		waitList[gameType].remove(_ptr);

		if (beforeSize > (int)waitList[gameType].size())
		{
			_ptr->SetGameType(-1);	// -1로 설정

			switch ((RoomInfo::GameType)gameType)
			{
			case RoomInfo::GameType::_2vs2:
			{
				printf("2:2 대기리스트 삭제 성공 : %d\n", (int)waitList[gameType].size());
			}
			break;

			case RoomInfo::GameType::_1vs1:
			{
				printf("1:1 대기리스트 삭제 성공 : %d\n", (int)waitList[gameType].size());
			}
			break;
			}
		}
	}
}

bool MatchManager::MatchProcess(C_ClientInfo* _ptr)
{
	IC_CS cs;

	int gameType = _ptr->GetGameType();
	waitList[gameType].emplace_back(_ptr);	// 리스트에 뒤로 넣고

	// DB에서 불러온 정보대로 저장
	int MaxPlayerOfThisGameType = InGameManager::GetInstance()->GetGameInfo(gameType)->maxPlayer;

	switch ((RoomInfo::GameType)gameType)
	{
		case RoomInfo::GameType::_2vs2:
		{
			printf("2:2 대기 리스트에 삽입 성공 사이즈 : %d\n", (int)waitList[gameType].size());
		}
		break;

		case RoomInfo::GameType::_1vs1:
		{
			printf("1:1 대기 리스트에 삽입 성공 사이즈 : %d\n", (int)waitList[gameType].size());
		}
		break;
	}

	
	// 인원수가 찼다면
	if (waitList[gameType].size() >= MaxPlayerOfThisGameType)
	{
		// 게임 타입에 맞는 인원수로 플레이어를 새롭게 만들고
		list<C_ClientInfo*> newPlayers;
		for (int i = 0; i < InGameManager::GetInstance()->GetGameInfo(gameType)->maxPlayer; i++)
		{
			newPlayers.emplace_back(waitList[gameType].front());	// 새로운 플레이어 목록의 뒤에 넣고
			waitList[gameType].pop_front();							// 넣은 애는 대기 리스트에서 빼준다.
		}

		// 지금 대기 리스트를 전달해서 방을 생성
		if (RoomManager::GetInstance()->CreateRoom(newPlayers, MaxPlayerOfThisGameType) == true)
		{
			return true;
		}
	}

	// 아직 매칭이 안잡히는 상황이라면
	return false;
}

