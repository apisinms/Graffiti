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

bool MatchManager::MatchProcess(C_ClientInfo* _ptr)
{
	waitQueue.emplace(_ptr);


	// 4인이상이 됐다면 
	if (waitQueue.size() >= 4)
	{
		// 매칭을 누른 플레이어들의 정보를 얻어서 배열에 저장한다.
		C_ClientInfo* players[4];
		for (int i = 0; i < 4; i++)
		{
			players[i] = waitQueue.front();
			waitQueue.pop();
		}
		
		//나랑 내 앞에를 한 팀, 그리고 남은 2명을 한 팀으로 만들어서 방을 만들고
		RoomManager::GetInstance()->CreateRoom(players);

		return true;
	}

	// 아직 매칭이 안잡히는 상황이라면
	else
		return false;
}