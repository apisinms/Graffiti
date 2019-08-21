#include "RoomManager.h"
#include "InGameManager.h"

#include "C_ClientInfo.h"

RoomManager* RoomManager::instance;

RoomManager* RoomManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
	if (instance == nullptr)
		instance = new RoomManager();

	return instance;
}

void RoomManager::Init()
{
	roomList = new C_List<RoomInfo*>();
}
void RoomManager::End()
{
	delete roomList;
}

void RoomManager::Destroy()
{
	delete instance;
}

bool RoomManager::CreateRoom(C_ClientInfo* _players[]/*, int _numOfPlayer ���� 3:3���� ��尡 �߰��� ��� �ο����� ����ؼ� �� ����*/)
{
	//// �÷��̾ 2�� ������ ���� �����, �̸� �� ������ �߰��Ѵ�.
	//Team* team1 = new Team(_players[0], _players[1]);
	//Team* team2 = new Team(_players[2], _players[3]);

	//RoomInfo* room = new RoomInfo(team1, team2);

	RoomInfo* room = new RoomInfo(
		_players[0],
		_players[1],
		_players[2],
		_players[3]);

	// �� �÷��̾�鿡�� ���� ���� ���� ����.(4 ����� numOfPlayer��(���� ��尡�ִٸ�))
	for (int i = 0; i < 4; i++)
		_players[i]->SetRoom(room);

	// �� ����Ʈ�� �߰�
	bool ret = roomList->Insert(room);
	printf("[�����] ���� : %d\n", roomList->GetCount());
	return ret;
}

bool RoomManager::CheckLeaveRoom(C_ClientInfo* _ptr)
{
	// ���� ���� �ִٸ�
	if (_ptr->GetRoom() != nullptr)
	{
		RoomInfo* room = _ptr->GetRoom();
		// �濡 ����Ʈ�� ������
		for (int i = 0; i < 4; i++)
		{
			// �ڽ��� ã����
			if (_ptr == room->playerList[i])
			{
				// �ٸ� �÷��̾�鿡�� �ڽ��� �����ٰ� �˸���
				if (InGameManager::GetInstance()->LeaveProcess(_ptr, (i + 1)) == true)
				{
					// �� �ο����� �����ϰ�, �ڽ��� ������ �����.
					room->curNumOfPlayer--;			// �� �ο��� ����
					room->playerList[i] = nullptr;
					_ptr->SetRoom(nullptr);

					break;
				}
			}
		}
		
		/// �׸��� ���� �� 2���� ��� ������ �׳� ���� ��������
		// ��� ��������� �������̾��ٸ� ���� ����
		if (room->curNumOfPlayer == 0)
		{
			roomList->Delete(room);
			printf("[�� �Ҹ�]������:%d\n", roomList->GetCount());
		}
	}

	else 
		return false;
}