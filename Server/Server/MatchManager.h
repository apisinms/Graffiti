#pragma once
#include "C_ClientInfo.h"

class MatchManager : public C_SyncCS<MatchManager>
{
	list<C_ClientInfo*> waitList;	// 매칭 대기중인 유저 리스트
	
private:
	static MatchManager* instance;

	MatchManager() {}
	~MatchManager() {}
public:
	static MatchManager* GetInstance();
	static void Destroy();
	void Init();
	void End();

public:
	bool MatchProcess(C_ClientInfo* _ptr);
	void WaitListRemove(C_ClientInfo* _ptr);
};