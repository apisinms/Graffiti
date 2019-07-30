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

bool MatchManager::MatchProcess(C_ClientInfo* _ptr)
{
	waitQueue.emplace(_ptr);


	// 4���̻��� �ƴٸ� 
	if (waitQueue.size() >= 4)
	{
		// ��Ī�� ���� �÷��̾���� ������ �� �迭�� �����Ѵ�.
		C_ClientInfo* players[4];
		for (int i = 0; i < 4; i++)
		{
			players[i] = waitQueue.front();
			waitQueue.pop();
		}
		
		//���� �� �տ��� �� ��, �׸��� ���� 2���� �� ������ ���� ���� �����
		RoomManager::GetInstance()->CreateRoom(players);

		return true;
	}

	// ���� ��Ī�� �������� ��Ȳ�̶��
	else
		return false;
}