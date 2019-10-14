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

#define RESULT_OFFSET 0x3FF
#define RESULT_MASK		40

// Keep-alive ���� ����
#define KEEPALIVE_TIME 3000								// TIME ms���� keep-alive ��ȣ�� �ְ�޴´�
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

struct IngamePacket
{
	int playerNum;
	float posX;
	float posZ;
	float rotY;
	float speed;
	int action;

	IngamePacket()
	{
		playerNum = 0;
		posX = posZ = rotY = speed = 0.0f;
		action = 0;
	}

	IngamePacket(IngamePacket& _pos)
	{
		this->playerNum = _pos.playerNum;
		this->posX      = _pos.posX;
		this->posZ      = _pos.posZ;
		this->rotY      = _pos.rotY;
		this->speed		= _pos.speed;
		this->action    = _pos.action;
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
	int gameType;				// ������ ���� Ÿ��
	IngamePacket* gamePacket;	// �ΰ��ӿ��� ����ϴ� 0.1�ʸ��� �ְ�޴� ��Ŷ (�÷��̾� ��ȣ + ��ġ + �����̼� + �ִϸ��̼� + ü�� ��)
	INDEX index;				// ���� �÷��̾��� ���� �ε���
	Weapon* weapon;				// ����
	float health;				// ü��
	float speed;				// �ӵ�
	int bullet;					// �Ѿ�

public:
	//static const float MAX_SPEED;
	enum GameType
	{
		NORMAL,
	};

public:
	PlayerInfo()
	{
		loadStatus = false;
		gameType = GameType::NORMAL;		// �ϴ��� ��� �������� ����

		gamePacket = nullptr;
		memset(&index, 0, sizeof(INDEX));
		weapon = nullptr;

		health = speed = 0.0f;
		bullet = 0;

		isFocus = true;
	}

	bool GetLoadStatus() { return loadStatus; }
	void SetLoadStatus(bool _loadStatus) { loadStatus = _loadStatus; }
	int GetGameType() { return gameType; }
	void SetGameType(int _gameType) { gameType = _gameType;}

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

	float GetHealth() { return health; }
	void SetHealth(float _health) { health = _health; }

	int GetBullet() { return bullet; }
	void SetBullet(int _bullet) { bullet = _bullet; }

	void FocusOn() { isFocus = true; }
	void FocusOff() { isFocus = false; }
	bool GetFocus() { return isFocus; }
};

struct WeaponInfo
{
	int num;			// ���� ��ȣ
	int numOfPattern;	// ���� ����
	int maxAmmo;		// �ִ� �Ѿ�
	float fireRate;		// �߻� �ӵ�
	float damage;		// ������
	float accuracy;		// ��Ȯ��
	float range;		// �����Ÿ�
	float speed;		// ź��
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
//const float PlayerInfo::MAX_SPEED = 4.0f;

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