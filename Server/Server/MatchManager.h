#pragma once
#include "C_ClientInfo.h"

class MatchManager : public C_SyncCS<MatchManager>
{
	list<C_ClientInfo*> waitList;	// ��Ī ������� ���� ����Ʈ
	
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