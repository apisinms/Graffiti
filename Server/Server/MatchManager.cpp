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

void MatchManager::WaitListDelete(C_ClientInfo* _ptr)
{
	waitList.remove(_ptr);
	wprintf(L"��⸮��Ʈ ���� ���� : %d\n", waitList.size());
}

bool MatchManager::MatchProcess(C_ClientInfo* _ptr)
{
	// ����Ʈ�� �տ����� �ְ�
	waitList.emplace_front(_ptr);
	
	printf("��� ����Ʈ�� ���� ���� ������ : %d\n", waitList.size());


	// 4���̻��� �ƴٸ� 
	if (waitList.size() >= MAX_PLAYER)
	{
		// ���������� ��Ī�� ���� �÷��̾�� ������ ������ �� �迭�� �����Ѵ�.
		C_ClientInfo* players[MAX_PLAYER];
		for (int i = 0; i < MAX_PLAYER; i++)
		{
			players[i] = waitList.back();
			waitList.pop_back();
		}
		
		//���� �� �տ��� �� ��, �׸��� ���� 2���� �� ������ ���� ���� �����
		RoomManager::GetInstance()->CreateRoom(players, MAX_PLAYER);

		return true;
	}

	// ���� ��Ī�� �������� ��Ȳ�̶��
	else
		return false;
}

