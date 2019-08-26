#pragma once
#include "C_Global.h"
#include "C_Sector.h"

class C_ClientInfo;

//// �� ����
//struct Team
//{
//	C_ClientInfo* player1;
//	C_ClientInfo* player2;
//
//	Team() {}
//	Team(C_ClientInfo* _player1, C_ClientInfo* _player2)
//	{
//		player1 = _player1;
//		player2 = _player2;
//	}
//};

// ���� ����
struct RoomInfo
{
	// ���⵵ private���� �ٲ���ҵ�
	void* weaponTimerHandle;		// ���� ���� Ÿ�̸� �ڵ�
	int numOfPlayer;				// ���� �濡 �ִ� �÷��̾� ��
	ROOMSTATUS roomStatus;			// ���� ����
	list<C_ClientInfo*>playerList;	// �������� ����Ʈ�� ����
	C_Sector* sector;


	RoomInfo(C_ClientInfo** _playerList, int _numOfPlayer)
	{
		weaponTimerHandle = NULL;

		roomStatus = ROOMSTATUS::ROOM_NONE;	// �� ������ �ʱ� ���´� �ƹ� ���µ��ƴ�

		numOfPlayer = _numOfPlayer;

		// �� �÷��̾� ����Ʈ�� �߰�
		for (int i = 0; i < numOfPlayer; i++)
			playerList.emplace_front(_playerList[i]);

		sector = new C_Sector();	// ���Ͱ����� ����
	}

	bool LeaveRoom(C_ClientInfo* _player)
	{
		// ����Ʈ�� ��ȸ�ϸ�
		for (list<C_ClientInfo*>::iterator iter = playerList.begin();
			iter != playerList.end(); ++iter)
		{
			// �ڽ��� ã�� ���
			if (*iter == _player)
			{

				/// ���� �� �Ҹ� �κп��� �ٽ� �ؾ���

				// �ٸ� �÷��̾�鿡�� �ڽ��� �����ٰ� �˸���
				if (InGameManager::GetInstance()->LeaveProcess(_player, (i + 1)) == true)
				{
					// �� �ο����� �����ϰ�, �ڽ��� ������ �����.
					room->curNumOfPlayer--;			// �� �ο��� ����
					room->playerList[i] = nullptr;
					_ptr->SetRoom(nullptr);

					/// �׸��� ���� �� 2���� ��� ������ �׳� ���� ��������
					// ��� ��������� �������̾��ٸ� ���� ����
					if (room->curNumOfPlayer == 0)
					{
						roomList->Delete(room);
						printf("[�� �Ҹ�]������:%d\n", roomList->GetCount());
					}

					return true;
				}
			}
		}


	}

	bool IsPlayerListEmpty()
	{
		return playerList.empty();
	}

};

class RoomManager
{
	C_List<RoomInfo*>* roomList;

private:
	static RoomManager* instance;

	RoomManager() {}
	~RoomManager() {}
public:
	static RoomManager* GetInstance();
	static void Destroy();
	void Init();
	void End();

public:
	bool CreateRoom(C_ClientInfo* _players[], int _numOfPlayer);
	bool CheckLeaveRoom(C_ClientInfo* _ptr);
};