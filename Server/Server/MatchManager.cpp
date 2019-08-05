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

void MatchManager::WaitListDelete(C_ClientInfo* _ptr)
{
	waitList.remove(_ptr);
	wprintf(L"대기리스트 삭제 성공 : %d\n", waitList.size());
}

bool MatchManager::MatchProcess(C_ClientInfo* _ptr)
{
	// 리스트에 앞에부터 넣고
	waitList.emplace_front(_ptr);
	
	printf("대기 리스트에 삽입 성공 사이즈 : %d\n", waitList.size());


	// 4인이상이 됐다면 
	if (waitList.size() >= 4)
	{
		// 마지막으로 매칭을 누른 플레이어들 순서로 정보를 얻어서 배열에 저장한다.
		C_ClientInfo* players[4];
		for (int i = 0; i < 4; i++)
		{
			players[i] = waitList.back();
			waitList.pop_back();
		}
		
		//나랑 내 앞에를 한 팀, 그리고 남은 2명을 한 팀으로 만들어서 방을 만들고
		RoomManager::GetInstance()->CreateRoom(players);

		return true;
	}

	// 아직 매칭이 안잡히는 상황이라면
	else
		return false;
}

