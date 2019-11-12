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

#define THREAD_END		-777

#define PROTOCOL_OFFSET	0xFFFFF
#define PROTOCOL_MASK	30

#define RESULT_OFFSET	0xFFFFFF
#define RESULT_MASK		54

// Keep-alive ���� ����
#define KEEPALIVE_TIME 3000								// TIME ms���� keep-alive ��ȣ�� �ְ�޴´�
#define KEEPALIVE_INTERVAL (KEEPALIVE_TIME / 20)		// Heart-beat�� ������ INTERVAL ms���� �������Ѵ�(10��)

#define TEMP_MAX_PLAYER	4

class C_ClientInfo;

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
		i = j = 0;
	}

	inline bool operator!= (INDEX _param)
	{
		if ((i != _param.i) || (j != _param.j))
			return true;

		return false;
	}

	void ResetIndex() { i = j = 0; }
};

struct COORD_DOUBLE
{
	double x, z;
	COORD_DOUBLE() { x = z = 0.0; }
};

struct HitPlayersHealth
{
	float health[TEMP_MAX_PLAYER];

	HitPlayersHealth()
	{
		memset(health, 0.0f, sizeof(health));
	}
};

// �Ѿ� �浹 �˻� ����ü
struct BulletCollisionChecker
{
	byte playerBit;
	int playerHitCountBit;
	HitPlayersHealth healths;

	BulletCollisionChecker()
	{
		playerBit = 0;
		playerHitCountBit = 0;
	}

	void ResetBulletCollisionChecker()
	{
		playerBit = 0;
		playerHitCountBit = 0;
		memset(&healths, 0, sizeof(healths));
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
		posX = posZ = rotY = speed = health = 0.0f;
		action = 0;
		isReloading = false;
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

	void ResetIngamePacket()
	{
		playerNum = 0;
		posX = posZ = rotY = speed = health = 0.0f;
		action = 0;
		isReloading = false;
		collisionCheck.ResetBulletCollisionChecker();
	}
};

enum WEAPONS : char
{
	NODATA = -1,

	// main weapons
	AR,
	SG,
	SMG,
	MAIN_MAX_LENGTH,

	// sub weapons
	TRAP,
	GRENADE,
	SUB_MAX_LENGTH,
};

struct Weapon
{
	char mainW;
	char subW;

public:
	Weapon() 
	{
		mainW = subW = 0;
	}

	Weapon(char _mainW, char _subW)
	{
		mainW = _mainW;
		subW  = _subW;
	}
};

struct Score
{
	int numOfKill;		// ���� �� �� �׿�����
	int numOfDeath;		// ���� �� �� �׾�����
	int killScore;		// ų ����
	int captureCount;	// �����غ� �ǹ� ���� 
	int captureNum;		// �������� �ǹ� ����
	int captureScore;	// ������ �ǹ� ���� ���� ����

	Score()
	{
		numOfKill = 0;
		numOfDeath = 0;
		killScore = 0;
		captureCount = 0;
		captureNum = 0;
		captureScore = 0;
	}

	void ResetScore()
	{
		numOfKill = 0;
		numOfDeath = 0;
		killScore = 0;
		captureCount = 0;
		captureNum = 0;
		captureScore = 0;
	}
};

struct PositionInfo
{
	int gameType;
	int playerNum;
	float posX;
	float posZ;

	PositionInfo()
	{
		gameType = playerNum = 0;
		posX = posZ = 0.0f;
	}
};

struct LocationInfo
{
	PositionInfo respawnInfo;
	PositionInfo firstPosInfo;

	LocationInfo() {}
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

	WeaponInfo()
	{
		num = numOfPattern = bulletPerShot = maxAmmo = 0;
		fireRate = damage = accuracy = range = speed = reloadTime = 0.0f;
		memset(weaponName, 0, WEAPON_NAME_SIZE);
	}
};

struct GameInfo
{
	int gameType;				// ���� Ÿ��(���߿� ��尡 ���� �� ���� ���� ������)
	int maxPlayer;				// �ִ� �÷��̾� ��
	float maxSpeed;				// �ִ� �̵��ӵ�
	float maxHealth;			// �ִ� ü��
	float respawnTime;			// ������ �ð�
	float healPackTime;			// ���� ��� �ð�
	float subSprayingTime;		// �������� �� �ð�
	float mainSprayingTime;		// �������� �ð�
	double gameTime;			// ���� �ð�
	int killPoint;				// ų ����
	int capturePoint;			// ���� ����

	GameInfo()
	{
		gameType = maxPlayer = 0;
		maxSpeed = maxHealth = 0.0f;
		respawnTime = healPackTime = subSprayingTime = mainSprayingTime = 0.0f;
		gameTime = 0.0;
		killPoint = capturePoint = 0;
	}
};

struct BuildingInfo
{
	int buildingIndex;
	C_ClientInfo* owner;

	BuildingInfo()
	{
		buildingIndex = 0;
		owner = nullptr;
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
	vector<PositionInfo*>respawnInfo;	// ������ ����
	bool isRespawning;			// ������ ������
	double respawnElapsedTime;	// ������ on �϶� ����� �ð�(��)
	Score score;				// �� ���ھ�
	int teamNum;				// �� ��ȣ
	list<C_ClientInfo*> sectorPlayerList;	// ���� ���Ϳ� �ִ� �÷��̾� ����Ʈ

public:
	PlayerInfo() 
	{ 
		loadStatus = false;
		isFocus = true;	// ��Ŀ���� �ݵ�� true������!!
		bullet = 0;
		isRespawning = false;
		respawnElapsedTime = 0.0;
		teamNum = 0;

		weapon = nullptr;
		gamePacket = nullptr;
	}
	~PlayerInfo()
	{
		ResetPlayerInfo();
	}

	void ResetPlayerInfo()
	{
		loadStatus = false;
		isFocus = true;
		if (gamePacket != nullptr)
		{
			delete gamePacket;
			gamePacket = nullptr;
		}
		index.ResetIndex();
		if (weapon != nullptr)
		{
			delete weapon;
			weapon = nullptr;
		}
		for (size_t i = 0; i < respawnInfo.size(); i++)
		{
			if (respawnInfo[i] != nullptr)
			{
				delete respawnInfo[i];
				respawnInfo[i] = nullptr;
			}
		}
		vector<PositionInfo*>().swap(respawnInfo);	// �� ���Ϳ� swap�Ͽ� ���͸� ������ Clear�����ش�.
		score.ResetScore();
		isRespawning = false;
		respawnElapsedTime = 0.0;
		teamNum = 0;
		sectorPlayerList.clear();
	}

	bool GetLoadStatus() { return loadStatus; }
	void SetLoadStatus(bool _loadStatus) { loadStatus = _loadStatus; }

	void FocusOn() { isFocus = true; }
	void FocusOff() { isFocus = false; }
	bool GetFocus() { return isFocus; }

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

	//PlayerRespawnInfo& GetPlayerRespawnInfo() { return playerRespawnInfo; }
	//void SetPlayerRespawnInfo(PlayerRespawnInfo& _info) { playerRespawnInfo = _info; }

	vector<PositionInfo*>& GetPlayerRespawnInfoList() { return respawnInfo; }
	PositionInfo* GetPlayerRespawnInfo(int _idx)
	{
		if (_idx < 0 || _idx > respawnInfo.size())
			return nullptr;

		return respawnInfo[_idx];
	}

	bool IsRespawning() { return isRespawning; }
	void RespawnOn() { isRespawning = true; }
	void RespawnOff() { respawnElapsedTime = 0.0; isRespawning = false; }
	
	double GetRespawnElapsedTime() { return respawnElapsedTime; }
	void SetRespawnElapsedTime(double _time) { respawnElapsedTime = _time; }

	Score& GetScore() { return score; }

	int GetTeamNum() {return teamNum;}
	void SetTeamNum(int _teamNum) {teamNum = _teamNum;}

	list<C_ClientInfo*>& GetSectorPlayerList() { return sectorPlayerList; }
	void SetSectorPlayerList(list<C_ClientInfo*> _playerList) { sectorPlayerList = _playerList; }	// Set�Ҷ��� ������ ������ �������� �Ҹ�Ǹ鼭 �ҹ����� ��
};

enum STATE : int
{
	STATE_LOGIN = 1, STATE_LOBBY, STATE_INGAME,
};

enum ROOMSTATUS
{
	ROOM_NONE = -1, 
	ROOM_ITEMSEL = 1,	// ���� ����
	ROOM_LOAD,			// �ε�
	ROOM_READY,			// ���� �غ�
	ROOM_GAME,			// ���� ��
	ROOM_GAME_END,		// ���� ����
	ROOM_END,			// �� ����
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

	S_SendBuf()
	{
		memset(sendBuf, 0, BUFSIZE);
		sendBytes = compSendBytes = 0;
	}
};

struct S_RecvBuf
{
	char recvBuf[BUFSIZE];
	int recvBytes;
	int compRecvBytes;
	
	bool rSizeFlag;
	int sizeBytes;

	S_RecvBuf()
	{
		memset(recvBuf, 0, BUFSIZE);

		recvBytes = compRecvBytes = 0;
		rSizeFlag = false;
		sizeBytes = 0;
	}
};

struct UserInfo
{
	TCHAR id[IDSIZE];
	TCHAR pw[PWSIZE];
	TCHAR nickname[NICKNAMESIZE];

	UserInfo()
	{ 
		memset(id, 0, IDSIZE);
		memset(pw, 0, PWSIZE);
		memset(nickname, 0, NICKNAMESIZE);
	}

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