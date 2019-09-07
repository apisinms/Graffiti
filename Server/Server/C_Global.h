#pragma once
#include <tchar.h>
using namespace std;

#define DEBUG

#define BUFSIZE			4096
#define HALF_BUFSIZE	2048

#define SERVERPORT		10823

#define IDSIZE			255
#define PWSIZE			255
#define NICKNAMESIZE	255
#define MSGSIZE			512

#define MAX_PLAYER		2

#define THREAD_END		-777

#define PROTOCOL_OFFSET	0xFFFFF
#define PROTOCOL_MASK	30

#define RESULT_OFFSET 0x3FF
#define RESULT_MASK		40

//#define MAX_SPEED 4.0f;	//////// 나중에 바꿀거임

	// 플레이어 플래그
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
};

struct PositionPacket
{
	int playerNum;
	float posX;
	float posZ;
	float rotY;
	float speed;
	int action;

	PositionPacket() 
	{
		playerNum = 0;
		posX = posZ = rotY = speed = 0.0f;
		action = 0;
	}

	PositionPacket(PositionPacket& _pos)
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
	PositionPacket* position;
	INDEX index;
	Weapon* weapon;
	float health;
	float speed;
	int bullet;

public:
	static const float MAX_SPEED;

public:
	PlayerInfo()
	{
		position = nullptr;
		memset(&index, 0, sizeof(INDEX));
		weapon = nullptr;

		health = speed = 0.0f;
		bullet = 0;
	}

	PositionPacket* GetPosition() { return position; }
	void SetPosition(PositionPacket* _position)
	{
		if (position != nullptr)
			delete position;

		position = _position;
	}

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

	float GetSpeed() { return speed; }
	void SetSpeed(float _speed) { speed = _speed; }

	int GetBullet() { return bullet; }
	void SetBullet(int _bullet) { bullet = _bullet; }
};
const float PlayerInfo::MAX_SPEED = 4.0f;

enum STATE : int
{
	STATE_LOGIN = 1, STATE_LOBBY, STATE_INGAME,
};

enum ROOMSTATUS
{
	ROOM_NONE = -1, ROOM_ITEMSEL = 1, ROOM_GAME
};

// 상위 10비트 스테이트를 표현해주는 프로토콜	63 ~ 54
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