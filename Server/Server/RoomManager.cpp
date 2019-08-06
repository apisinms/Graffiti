#include "RoomManager.h"

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
	return roomList->Insert(room);
}

bool RoomManager::CheckLeaveRoom(C_ClientInfo* _ptr)
{
	// ���� ���� �ִٸ�
	if (_ptr->GetRoom() != nullptr)
	{
		//// ���� �ڽ��� ������ ��� �÷��̾�� ���� �������� ����
		//if (_ptr->GetRoom()->team1->player1 != _ptr)
		//{
		//	// SendProtocol
		//}

		// ���� �÷��̾� ����Ʈ���� �����.
		for (int i = 0; i < 4; i++)
		{
			if (_ptr == _ptr->GetRoom()->playerList[i])
			{
				_ptr->GetRoom()->playerList[i] = nullptr;
				_ptr->SetRoom(nullptr);

				break;
			}
		}

		return true;
	}

	else 
		return false;
}