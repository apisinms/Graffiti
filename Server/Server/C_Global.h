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

struct PositionPacket
{
	int playerNum;
	float posX;
	float posZ;
	float rotY;
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
	Weapon* weapon;
	float health;
	float speed;
	int bullet;

public:
	PlayerInfo()
	{
		position = new PositionPacket();
		weapon   = new Weapon();

		health = speed = 0.0f;
		bullet = 0;
	}

	PositionPacket* GetPosition() { return position; }
	void SetPosition(PositionPacket* _position) { position = _position; }

	Weapon* GetWeapon() { return weapon; }
	void SetWeapon(Weapon* _weapon) { weapon = _weapon; }

	float GetHealth() { return health; }
	void SetHealth(float _health) { health = _health; }

	float GetSpeed() { return speed; }
	void SetSpeed(float _speed) { speed = _speed; }

	int GetBullet() { return bullet; }
	void SetBullet(int _bullet) { bullet = _bullet; }
};

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