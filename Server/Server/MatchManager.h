#pragma once
#include "C_List.h"

class C_ClientInfo;

class MatchManager
{
	C_List<C_ClientInfo*>* waitList;	// 매칭 대기중인 리스트 목록
	
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