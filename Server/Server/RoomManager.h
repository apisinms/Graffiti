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
private:
	void* weaponTimerHandle;		// ���� ���� Ÿ�̸� �ڵ�
	int numOfPlayer;				// ���� �濡 �ִ� �÷��̾� ��
	ROOMSTATUS roomStatus;			// ���� ����
	list<C_ClientInfo*>playerList;	// �������� ����Ʈ�� ����
	C_Sector* sector;				// �� ���� ���� �Ŵ���
	int carSeed;					// ���� ����

public:
	RoomInfo(C_ClientInfo** _playerList, int _numOfPlayer);

	bool LeaveRoom(C_ClientInfo* _player);

	// �÷��̾� ����Ʈ�� �������ֵ�, ������ ���纻�� ���޵ǹǷ� ������ ������ ����.
	list<C_ClientInfo*> GetPlayerList() { return playerList; }

	bool IsPlayerListEmpty() { return playerList.empty(); }

	int GetPlayerListSize() { return (int)playerList.size(); }

	void SetWeaponTimerHandle(void* _handle) { weaponTimerHandle = _handle; }
	void* GetWeaponTimerHandle() { return weaponTimerHandle; }

	void SetNumOfPlayer(int _numOfPlayer) { numOfPlayer = _numOfPlayer; }
	int GetNumOfPlayer() { return numOfPlayer; }

	void SetCarSeed(int _cardSeed) { carSeed = _cardSeed; }
	int GetCarSeed() { return carSeed; }

	void SetRoomStatus(ROOMSTATUS _roomStatus) { roomStatus = _roomStatus; }
	ROOMSTATUS GetRoomStatus() { return roomStatus; }

	C_Sector* GetSector() { return sector; }
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
	bool DeleteRoom(RoomInfo* _room);
	bool CheckLeaveRoom(C_ClientInfo* _ptr);
};