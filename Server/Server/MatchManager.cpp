#include "stdafx.h"
#include "MatchManager.h"
#include "RoomManager.h"
#include "C_ClientInfo.h"

MatchManager* MatchManager::instance;

MatchManager* MatchManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
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
	wprintf(L"��⸮��Ʈ ���� ���� : %d\n", (int)waitList.size());
}

bool MatchManager::MatchProcess(C_ClientInfo* _ptr)
{
	IC_CS cs;

	// ����Ʈ�� �ڷ� �ְ�
	waitList.emplace_back(_ptr);
	
	printf("��� ����Ʈ�� ���� ���� ������ : %d\n", (int)waitList.size());


	// 4���̻��� �ƴٸ� 
	if (waitList.size() >= MAX_PLAYER)
	{
		// ���������� ��Ī�� ���� �÷��̾�� ������ ������ �� �迭�� �����Ѵ�.
		C_ClientInfo* players[MAX_PLAYER];
		for (int i = 0; i < MAX_PLAYER; i++)
		{
			players[i] = waitList.front();
			waitList.pop_front();	// ������ ����

			//PlayerInfo* info = players[i]->GetPlayerInfo();
		}
		
		//���� �� �տ��� �� ��, �׸��� ���� 2���� �� ������ ���� ���� ����� ��� ����
		return RoomManager::GetInstance()->CreateRoom(players, MAX_PLAYER);
	}

	// ���� ��Ī�� �������� ��Ȳ�̶��
	else
		return false;
}

