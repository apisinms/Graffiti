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
	carSeed = 0;			// �ڵ��� ����

	// �� �÷��̾� ����Ʈ�� �߰�
	for (int i = 0; i < numOfPlayer; i++)
		playerList.emplace_front(_playerList[i]);

	sector = new C_Sector();	// �� ���� ���Ͱ����� ����
}

bool RoomInfo::LeaveRoom(C_ClientInfo* _player)
{	
	// ���� �����ٴ� ���� ���� �濡 �ִ� �÷��̾�鿡�� ������
	if (InGameManager::GetInstance()->LeaveProcess(_player, _player->GetPlayerInfo()->GetIngamePacket()->playerNum) == true)
	{
		// �� �ο����� �����ϰ�, �ڽ��� ������ �����.
		numOfPlayer--;					// �� �ο��� ����
		playerList.remove(_player);		// ���� �÷��̾� ����Ʈ���� ����

		/// �׸��� ���� �� 2���� ��� ������ �׳� ���� ��������

		// ��� ��������� �������̾��ٸ� ���� ����
		if (numOfPlayer == 0)
		{
			RoomManager::GetInstance()->DeleteRoom(this);					// ���� �Ҹ��Ų��.
		}

		_player->SetRoom(nullptr);		// �÷��̾��� ���� null�� ����

		return true;
	}

	return false;	// ��ã�� ���
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