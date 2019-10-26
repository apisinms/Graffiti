#pragma once
#include <tchar.h>
using namespace std;

#define DEBUG

#define BUFSIZE			4096
#define HALF_BUFSIZE	2048

#define SERVERPORT		10823

#define IDSIZE				255
#define PWSIZE				255
#define NICKNAMESIZE		255
#define MSGSIZE				512
#define WEAPON_NAME_SIZE	32

#define MAX_PLAYER		2

#define THREAD_END		-777

#define PROTOCOL_OFFSET	0xFFFFF
#define PROTOCOL_MASK	30

#define RESULT_OFFSET	0xFFFFFF
#define RESULT_MASK		54

#define CAR_SPAWN_TIME	4000

// Keep-alive ���� ����
#define KEEPALIVE_TIME 5000								// TIME ms���� keep-alive ��ȣ�� �ְ�޴´�
#define KEEPALIVE_INTERVAL (KEEPALIVE_TIME / 20)		// Heart-beat�� ������ INTERVAL ms���� �������Ѵ�(10��)

// �÷��̾� �÷���
enum PLAYER_BIT : byte
{
	PLAYER_1 = (1 << 3),
	PLAYER_2 = (1 << 2),
	PLAYER_3 = (1 << 1),
	PLAYER_4 = (1 << 0),
};


struct INDEX
{
	int i, j;

	INDEX()
	{
		i = j = -1;
	}

	inline bool operator!= (INDEX _param)
	{
		if ((i != _param.i) || (j != _param.j))
			return true;

		return false;
	}
};

struct COORD_DOUBLE
{
	double x, z;
	COORD_DOUBLE() { x = z = 0; }
};

// �Ѿ� �浹 �˻� ����ü
struct BulletCollisionChecker
{
	byte playerBit;
	int playerHitCountBit;
	BulletCollisionChecker()
	{
		playerBit = 0;
		playerHitCountBit = 0;
	}
};

struct IngamePacket
{
	int playerNum;
	float posX;
	float posZ;
	float rotY;
	float speed;
	int action;
	float health;
	bool isReloading;
	BulletCollisionChecker collisionCheck;

	IngamePacket()
	{
		playerNum = 0;
		posX = posZ = rotY = speed = 0.0f;
		action = 0;
		health = 0.0;
		isReloading = false;
		memset(&collisionCheck, 0, sizeof(BulletCollisionChecker));
	}

	IngamePacket(IngamePacket& _pos)
	{
		this->playerNum      = _pos.playerNum;
		this->posX           = _pos.posX;
		this->posZ           = _pos.posZ;
		this->rotY           = _pos.rotY;
		this->speed		     = _pos.speed;
		this->action         = _pos.action;
		this->health         = _pos.health;
		this->collisionCheck = _pos.collisionCheck;
		this->isReloading    = false;
	}
};

struct Weapon
{
	char mainW;
	char subW;

public:
	Weapon() {}

	Weapon(char _mainW, char _subW)
	{
		mainW = _mainW;
		subW  = _subW;
	}
};

struct PlayerInfo
{
private:
	bool loadStatus;			// �ε� �� �ƴ��� ����
	bool isFocus;				// ��Ⱑ Focus������ 
	IngamePacket* gamePacket;	// �ΰ��ӿ��� ����ϴ� 0.1�ʸ��� �ְ�޴� ��Ŷ (�÷��̾� ��ȣ + ��ġ + �����̼� + �ִϸ��̼� + ü�� ��)
	INDEX index;				// ���� �÷��̾��� ���� �ε���
	Weapon* weapon;				// ����
	int bullet;					// �Ѿ�
	float respawnPosX;			// ������ x��ǥ
	float respawnPosZ;			// ������ z��ǥ
	bool isRespawning;			// ������ ������

public:
	PlayerInfo()
	{
		loadStatus = false;

		gamePacket = nullptr;
		memset(&index, 0, sizeof(INDEX));
		weapon = nullptr;

		bullet = 0;

		isFocus = true;

		respawnPosX = respawnPosZ = 0.0f;
		isRespawning = false;
	}

	bool GetLoadStatus() { return loadStatus; }
	void SetLoadStatus(bool _loadStatus) { loadStatus = _loadStatus; }

	IngamePacket* GetIngamePacket() { return gamePacket; }
	void SetIngamePacket(IngamePacket* _gamePacket)
	{
		if (gamePacket != nullptr)
			delete gamePacket;

		gamePacket = _gamePacket;
	}

	void SetPlayerNum(int _num)
	{
		if (gamePacket == nullptr)
			gamePacket = new IngamePacket();

		gamePacket->playerNum = _num;
	}
	int GetPlayerNum() { return gamePacket->playerNum; }
	int GetAnimation() { return gamePacket->action; }

	INDEX GetIndex() { return index; }
	void SetIndex(INDEX _index) { index = _index; }

	Weapon* GetWeapon() { return weapon; }
	void SetWeapon(Weapon* _weapon) 
	{
		if (weapon != nullptr)
			delete weapon;

		weapon = _weapon;
	}

	int GetBullet() { return bullet; }
	void SetBullet(int _bullet) { bullet = _bullet; }

	void FocusOn() { isFocus = true; }
	void FocusOff() { isFocus = false; }
	bool GetFocus() { return isFocus; }

	void SetRespawnPos(float _posX, float _posZ) { respawnPosX = _posX; respawnPosZ = _posZ; }
	float GetRespawnPosX() { return respawnPosX; }
	float GetRespawnPosZ() { return respawnPosZ; }

	bool IsRespawning() { return isRespawning; }
	void RespawnOn() { isRespawning = true; }
	void RespawnOff() { isRespawning = false; }
};

struct WeaponInfo
{
	int num;			// ���� ��ȣ
	int numOfPattern;	// ���� ����
	int bulletPerShot;	// �� �� ��� �� �� ��������
	int maxAmmo;		// �ִ� �Ѿ�
	float fireRate;		// �߻� �ӵ�
	float damage;		// ������
	float accuracy;		// ��Ȯ��
	float range;		// �����Ÿ�
	float speed;		// ź��
	float reloadTime;	// ������ �ð�
	TCHAR weaponName[WEAPON_NAME_SIZE];	// ���� �̸�
};

struct GameInfo
{
	int gameType;		// ���� Ÿ��(���߿� ��尡 ���� �� ���� ���� ������)
	float maxSpeed;		// �ִ� �̵��ӵ�
	float maxHealth;	// �ִ� ü��
	int responTime;		// ������ �ð�
	int gameTime;		// ���� �ð�(ex 180��)
};

struct RespawnInfo
{
	int gameType;
	int playerNum;
	float posX;
	float posZ;
};

enum STATE : int
{
	STATE_LOGIN = 1, STATE_LOBBY, STATE_INGAME,
};

enum ROOMSTATUS
{
	ROOM_NONE = -1, ROOM_ITEMSEL = 1, ROOM_GAME
};

// ���� 10��Ʈ ������Ʈ�� ǥ�����ִ� ��������	63 ~ 54
enum STATE_PROTOCOL : __int64
{
	LOGIN_STATE  = ((__int64)0x1 << 63),
	LOBBY_STATE  = ((__int64)0x1 << 62),
	CHAT_STATE   = ((__int64)0x1 << 61),
	INGAME_STATE = ((__int64)0x1 << 60),
	// 59 ~ 54
};


enum IO_TYPE
{
	IO_RECV = 1,
	IO_SEND,
	IO_ACCEPT
};

enum
{
	NONE = 1,
	SOC_ERROR,
	SOC_TRUE,
	SOC_FALSE,
	ERROR_DISCONNECTED,
	DISCONNECTED,
};

struct S_SendBuf
{
	char sendBuf[BUFSIZE];
	int sendBytes;
	int compSendBytes;
};

struct S_RecvBuf
{
	char recvBuf[BUFSIZE];
	int recvBytes;
	int compRecvBytes;
	
	bool rSizeFlag;
	int sizeBytes;
};

struct UserInfo
{
	TCHAR id[IDSIZE];
	TCHAR pw[PWSIZE];
	TCHAR nickname[NICKNAMESIZE];

	UserInfo() {}
	UserInfo(UserInfo &_info)
	{
		_tcscpy_s(id, IDSIZE, _info.id);
		_tcscpy_s(pw, PWSIZE, _info.pw);
		_tcscpy_s(nickname, NICKNAMESIZE, _info.nickname);
	}
};





//enum
//{
//	READ_SET = 1,
//	WRITE_SET,
//	EXCEPTION_SET
//};