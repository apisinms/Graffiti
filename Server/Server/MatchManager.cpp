#include "stdafx.h"
#include "MatchManager.h"
#include "RoomManager.h"
#include "C_ClientInfo.h"

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
}

void MatchManager::End()
{
}

void MatchManager::Destroy()
{
	delete instance;
}

void MatchManager::WaitListRemove(C_ClientInfo* _ptr)
{
	IC_CS cs;

	waitList.remove(_ptr);
	wprintf(L"대기리스트 삭제 성공 : %d\n", (int)waitList.size());
}

bool MatchManager::MatchProcess(C_ClientInfo* _ptr)
{
	IC_CS cs;

	// 리스트에 뒤로 넣고
	waitList.emplace_back(_ptr);
	
	printf("대기 리스트에 삽입 성공 사이즈 : %d\n", (int)waitList.size());


	// 4인이상이 됐다면 
	if (waitList.size() >= MAX_PLAYER)
	{
		// 마지막으로 매칭을 누른 플레이어들 순서로 정보를 얻어서 배열에 저장한다.
		C_ClientInfo* players[MAX_PLAYER];
		for (int i = 0; i < MAX_PLAYER; i++)
		{
			players[i] = waitList.front();
			waitList.pop_front();	// 앞으로 뺀다

			//PlayerInfo* info = players[i]->GetPlayerInfo();
		}
		
		//나랑 내 앞에를 한 팀, 그리고 남은 2명을 한 팀으로 만들어서 방을 만들고 결과 리턴
		return RoomManager::GetInstance()->CreateRoom(players, MAX_PLAYER);
	}

	// 아직 매칭이 안잡히는 상황이라면
	else
		return false;
}

