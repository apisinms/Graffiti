#pragma once
#include <queue>
#include "C_ClientInfo.h"

class MatchManager
{
	queue<C_ClientInfo*> waitQueue;
	
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
};