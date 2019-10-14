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

// Keep-alive 설정 관련
#define KEEPALIVE_TIME 3000								// TIME ms마다 keep-alive 신호를 주고받는다
#define KEEPALIVE_INTERVAL (KEEPALIVE_TIME / 20)		// Heart-beat가 없을시 INTERVAL ms마다 재전송한다(10번)

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
	bool loadStatus;			// 로딩 다 됐는지 상태
	bool isFocus;				// 기기가 Focus중인지 
	int gameType;				// 선택한 게임 타입
	IngamePacket* gamePacket;	// 인게임에서 사용하는 0.1초마다 주고받는 패킷 (플레이어 번호 + 위치 + 로테이션 + 애니메이션 + 체력 등)
	INDEX index;				// 현재 플레이어의 섹터 인덱스
	Weapon* weapon;				// 무기
	float health;				// 체력
	float speed;				// 속도
	int bullet;					// 총알

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
		gameType = GameType::NORMAL;		// 일단은 노멀 게임으로 셋팅

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
	int num;			// 무기 번호
	int numOfPattern;	// 패턴 갯수
	int maxAmmo;		// 최대 총알
	float fireRate;		// 발사 속도
	float damage;		// 데미지
	float accuracy;		// 정확도
	float range;		// 사정거리
	float speed;		// 탄속
	TCHAR weaponName[WEAPON_NAME_SIZE];	// 무기 이름
};

struct GameInfo
{
	int gameType;		// 게임 타입(나중에 모드가 여러 개 생길 수도 있으니)
	float maxSpeed;		// 최대 이동속도
	float maxHealth;	// 최대 체력
	int responTime;		// 리스폰 시간
	int gameTime;		// 게임 시간(ex 180초)
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