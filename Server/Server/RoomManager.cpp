#include "stdafx.h"
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

bool RoomManager::CreateRoom(C_ClientInfo* _players[], int _numOfPlayer)
{
	// �÷��̾� ����� ���� ���� ����.
	RoomInfo* room = new RoomInfo(_players, _numOfPlayer);

	// �� �÷��̾�鿡�� ���� ���� ���� ����.
	for (int i = 0; i < _numOfPlayer; i++)
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

		// ���� ���� �� �ִ��� �˻��ؼ� �����ٸ�
		if (room->LeaveRoom(_ptr) == true)
		{
			/// �׸��� ���� �� 2���� ��� ������ �׳� ���� ��������

			// ��� ��������� �������̾��ٸ� ���� ����
			if (room->IsPlayerListEmpty() == true)
			{
				roomList->Delete(room);
				printf("[�� �Ҹ�]������:%d\n", roomList->GetCount());
			}
		}
	}

	else 
		return false;
}