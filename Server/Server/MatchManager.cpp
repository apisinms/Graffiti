#include "MatchManager.h"
#include "C_ClientInfo.h"

MatchManager* MatchManager::GetInstance()
{
	// 인스턴스가 없다면 인스턴스를 생성하고 리턴한다.
	if (instance == nullptr)
		instance = new MatchManager();

	return instance;
}

void MatchManager::Init()
{
	instance->waitList = new C_List<C_ClientInfo*>();
}
void MatchManager::End()
{
	delete waitList;
}

void MatchManager::Destroy()
{
	delete instance;
}

bool MatchManager::MatchProcess(C_ClientInfo* _ptr)
{
	// 4인이상이 됐다면 나랑 내 앞에를 한 팀, 남은 2명을 한 팀으로 만들어서
	if (waitList->GetCount() >= 4)
	{
		// 리스트에 있는 4명을 빼내서 방을 만들고
		// 이제 무기 선택 화면으로

		return true;
	}

	// 아직 매칭이 안잡히는 상황이라면
	else
	{
		// 대기리스트에 넣고 false 리턴
		waitList->Insert(_ptr);	
		return false;
	}
}