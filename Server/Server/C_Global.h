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

// Keep-alive 설정 관련
#define KEEPALIVE_TIME 5000								// TIME ms마다 keep-alive 신호를 주고받는다
#define KEEPALIVE_INTERVAL (KEEPALIVE_TIME / 20)		// Heart-beat가 없을시 INTERVAL ms마다 재전송한다(10번)

class C_ClientInfo;

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

// 총알 충돌 검사 구조체
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

struct Score
{
	int numOfKill;		// 내가 몇 명 죽였는지
	int numOfDeath;		// 내가 몇 번 죽었는지
	int killScore;		// 킬 점수
	int captureCount;	// 점령해본 건물 개수 

	Score()
	{
		numOfKill = numOfDeath = killScore = captureCount = 0;
	}
};

struct PlayerRespawnInfo
{
	float respawnPosX;			// 리스폰 위치 x
	float respawnPosZ;			// 리스폰 위치 z
	bool isRespawning;			// 리스폰 중인지
	double elapsedSec;				// 리스폰 on 일때 경과한 시간(초)

	PlayerRespawnInfo()
	{
		respawnPosX = respawnPosZ = 0.0f;
		isRespawning = false;
		elapsedSec = 0.0;
	}

	void RespawnDone()
	{
		elapsedSec = 0.0;
		isRespawning = false;
	}
};

struct PlayerInfo
{
private:
	bool loadStatus;			// 로딩 다 됐는지 상태
	bool isFocus;				// 기기가 Focus중인지 
	IngamePacket* gamePacket;	// 인게임에서 사용하는 0.1초마다 주고받는 패킷 (플레이어 번호 + 위치 + 로테이션 + 애니메이션 + 체력 등)
	INDEX index;				// 현재 플레이어의 섹터 인덱스
	Weapon* weapon;				// 무기
	int bullet;					// 총알
	PlayerRespawnInfo playerRespawnInfo;	// 리스폰 정보
	Score score;				// 내 스코어
	int teamNum;				// 팀 번호
	list<C_ClientInfo*> sectorPlayerList;	// 인접 섹터에 있는 플레이어 리스트

public:
	PlayerInfo()
	{
		loadStatus = false;
		isFocus = true;
		gamePacket = nullptr;
		weapon = nullptr;
		bullet = 0;

		// 구조체들은 내부에서 알아서 초기화 함
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

	PlayerRespawnInfo& GetPlayerRespawnInfo() { return playerRespawnInfo; }
	void SetPlayerRespawnInfo(PlayerRespawnInfo& _info) { playerRespawnInfo = _info; }

	Score& GetScore() { return score; }

	int GetTeamNum() {return teamNum;}
	void SetTeamNum(int _teamNum) {teamNum = _teamNum;}

	list<C_ClientInfo*>& GetSectorPlayerList() { return sectorPlayerList; }
	void SetSectorPlayerList(list<C_ClientInfo*> _playerList) { sectorPlayerList = _playerList; }	// Set할때는 참조로 받으면 지역변수 소멸되면서 불법접근 됨
};

struct WeaponInfo
{
	int num;			// 무기 번호
	int numOfPattern;	// 패턴 갯수
	int bulletPerShot;	// 한 번 쏘면 몇 발 나가는지
	int maxAmmo;		// 최대 총알
	float fireRate;		// 발사 속도
	float damage;		// 데미지
	float accuracy;		// 정확도
	float range;		// 사정거리
	float speed;		// 탄속
	float reloadTime;	// 재장전 시간
	TCHAR weaponName[WEAPON_NAME_SIZE];	// 무기 이름
};

struct GameInfo
{
	int gameType;		// 게임 타입(나중에 모드가 여러 개 생길 수도 있으니)
	int maxPlayer;		// 최대 플레이어 수
	float maxSpeed;		// 최대 이동속도
	float maxHealth;	// 최대 체력
	int respawnTime;		// 리스폰 시간
	int gameTime;		// 게임 시간(ex 180초)
	int killPoint;		// 킬 점수
	int capturePoint;	// 점령 점수
};

struct PositionInfo
{
	int gameType;
	int playerNum;
	float posX;
	float posZ;
};

struct LocationInfo
{
	PositionInfo respawnInfo;
	PositionInfo firstPosInfo;
};

struct BuildingInfo
{
	int buildingIndex;
	C_ClientInfo* owner;
};

enum STATE : int
{
	STATE_LOGIN = 1, STATE_LOBBY, STATE_INGAME,
};

enum ROOMSTATUS
{
	ROOM_NONE = -1, 
	ROOM_WAIT =1, 
	ROOM_ITEMSEL, 
	ROOM_GAME,
	ROOM_GAME_END,
	ROOM_END,	// 방 종료
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