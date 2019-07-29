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
	waitQueue.emplace(_ptr);	// ť�� ����


	// 4���̻��� �ƴٸ� 
	if (waitQueue.size() >= 4)
	{
		// 4���� �÷��̾� ������ ��´�.
		C_ClientInfo* player[4];
		for (int i = 0; i < 4; i++)
		{
			player[i] = waitQueue.front();
			waitQueue.pop();
		}

		//���� �� �տ��� �� ��, �׸��� ���� 2���� �� ������ ���� ���� �����
		RoomManager::GetInstance()->CreateRoom(
		player[0],
		player[1],
		player[2],
		player[3]
		);

		return true;
	}

	// ���� ��Ī�� �������� ��Ȳ�̶��
	else
		return false;
}