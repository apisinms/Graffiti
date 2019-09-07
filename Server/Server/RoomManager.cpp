#include "stdafx.h"
#include "RoomManager.h"
#include "InGameManager.h"

#include "C_ClientInfo.h"

////////////////////////// RoomInfo ����ü //////////////////////////////
RoomInfo::RoomInfo(C_ClientInfo** _playerList, int _numOfPlayer)
{
	weaponTimerHandle = NULL;

	roomStatus = ROOMSTATUS::ROOM_NONE;	// �� ������ �ʱ� ���´� �ƹ� ���µ��ƴ�

	numOfPlayer = _numOfPlayer;

	// �� �÷��̾� ����Ʈ�� �߰�
	for (int i = 0; i < numOfPlayer; i++)
		playerList.emplace_front(_playerList[i]);

	curIter = playerList.begin();	// ���� �ݺ��� ��ġ�� ó������ ����

	sector = new C_Sector();	// �� ���� ���Ͱ����� ����
}

bool RoomInfo::LeaveRoom(C_ClientInfo* _player)
{															// �̷� ������ �����ϴ°� ���߿� �ٲ���
	if (InGameManager::GetInstance()->LeaveProcess(_player, _player->GetPosition()->playerNum) == true)
	{
		// �� �ο����� �����ϰ�, �ڽ��� ������ �����.
		numOfPlayer--;				// �� �ο��� ����
		playerList.remove(_player);		// ���� �÷��̾� ����Ʈ���� ����
		_player->SetRoom(nullptr);	// �÷��̾��� ���� null�� ����
		curIter = playerList.begin();	// �ݺ��ڰ� ������ �ʰ� �ٽ� ó������(�������ϱ�)

		/// �׸��� ���� �� 2���� ��� ������ �׳� ���� ��������

		// ��� ��������� �������̾��ٸ� ���� ����
		if (numOfPlayer == 0)
			RoomManager::GetInstance()->DeleteRoom(this);

		return true;
	}

	//// ����Ʈ�� ��ȸ�ϸ�
	//int i = 0;
	//for (list<C_ClientInfo*>::iterator iter = playerList.begin();
	//	iter != playerList.end(); ++iter, i++)
	//{
	//	// �ڽ��� ã�� ���
	//	if (*iter == _player)
	//	{
	//		// �ٸ� �÷��̾�鿡�� �ڽ��� �����ٰ� �˸���
	//		if (InGameManager::GetInstance()->LeaveProcess(_player, (i + 1)) == true)
	//		{
	//			// �� �ο����� �����ϰ�, �ڽ��� ������ �����.
	//			numOfPlayer--;				// �� �ο��� ����
	//			//playerList.erase(iter++);		// ���� �÷��̾� ����Ʈ���� ����
	//			playerList.erase(iter++);		// ���� �÷��̾� ����Ʈ���� ����
	//			_player->SetRoom(nullptr);	// �÷��̾��� ���� null�� ����
	//			curIter = playerList.begin();	// �ݺ��ڰ� ������ �ʰ� �ٽ� ó������(�������ϱ�)

	//			/// �׸��� ���� �� 2���� ��� ������ �׳� ���� ��������

	//			// ��� ��������� �������̾��ٸ� ���� ����
	//			if (numOfPlayer == 0)
	//				RoomManager::GetInstance()->DeleteRoom(this);

	//			return true;
	//		}
	//	}
	//}

	return false;	// ��ã�� ���
}

bool RoomInfo::GetPlayer(C_ClientInfo* &_ptr, bool _isReset)
{
	/*
	�߰��� ���� ������ ������ �� �� �ִ� �ڵ���...
	*/
	static bool flag = true;	// �÷��װ� ���� �ִٸ� ���� ���� �÷��̾� ����Ʈ ������ ����
	
	// ����
	if (_isReset == true)
	{
		flag = true;
		curIter = playerList.begin();

		return false;
	}

	// ���� �����ϸ� flag ��Ȱ��
	if (curIter == playerList.end())
	{
		flag = false;
		curIter = playerList.begin();	// �������� �� end() ���ǹ��� ������ �ȵǴϱ�
		return flag;
	}

	// flag�� false�̸� �ٽ� ����������Ѵ�.
	if (flag == false)
	{
		flag = true;
		curIter = playerList.begin();
	}

	_ptr = *curIter;	// ������ �÷��̾� ���� ����
	++curIter;			// ���� ��ġ��

	return flag;	// flag�� �����Ͽ� false�� �ƴҶ����� �ݺ�ȣ���ϸ� �ȴ�.
}


////////////////////////// RoomManager Ŭ���� //////////////////////////
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

bool RoomManager::DeleteRoom(RoomInfo* _room)
{
	if (roomList->Delete(_room) == true)
	{
		printf("[�� �Ҹ�]������:%d\n", roomList->GetCount());
		return true;
	}

	return false;
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

			return true;
		}
	}

	return false;
}