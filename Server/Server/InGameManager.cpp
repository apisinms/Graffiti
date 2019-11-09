#include "stdafx.h"
#include "LogManager.h"
#include "InGameManager.h"
#include "RoomManager.h"
#include "RandomManager.h"
#include "C_ClientInfo.h"
#include <thread>
#include <chrono>
#include "Item.h"

InGameManager* InGameManager::instance;

InGameManager* InGameManager::GetInstance()
{
	if (instance == nullptr)
		instance = new InGameManager();

	return instance;
}
void InGameManager::Destroy()
{
	delete instance;
}

void InGameManager::Init()
{
	// 무기 정보를 얻어옴
	WeaponInfo* weaponPtr;
	while ((weaponPtr = DatabaseManager::GetInstance()->LoadWeaponInfo()) != nullptr)
		weaponInfo.emplace_back(weaponPtr);

	// 게임 정보를 얻어옴
	GameInfo* gamePtr;
	while ((gamePtr = DatabaseManager::GetInstance()->LoadGameInfo()) != nullptr)
		gameInfo.emplace_back(gamePtr);

	// 위치 정보를 얻어옴
	LocationInfo* locationPtr;
	while ((locationPtr = DatabaseManager::GetInstance()->LoadLocationInfo()) != nullptr)
		locationInfo.emplace_back(locationPtr);
}

void InGameManager::End()
{
}

void InGameManager::PackPacket(char* _setptr, const int _num, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// int형 변수 셋팅
	memcpy(ptr, &_num, sizeof(_num));
	ptr = ptr + sizeof(_num);
	_size = _size + sizeof(_num);
}

void InGameManager::PackPacket(char* _setptr, double _time, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// double형 변수 셋팅
	memcpy(ptr, &_time, sizeof(_time));
	ptr = ptr + sizeof(_time);
	_size = _size + sizeof(_time);
}

void InGameManager::PackPacket(char* _setptr, int _num, TCHAR* _string, int& _size)
{
	char* ptr = _setptr;
	int strsize = (int)_tcslen(_string) * sizeof(TCHAR);
	_size = 0;

	// 플레이어 넘버
	memcpy(ptr, &_num, sizeof(_num));
	ptr = ptr + sizeof(_num);
	_size = _size + sizeof(_num);

	// 문자열 길이
	memcpy(ptr, &strsize, sizeof(strsize));
	ptr = ptr + sizeof(strsize);
	_size = _size + sizeof(strsize);

	// 문자열
	memcpy(ptr, _string, strsize);
	ptr = ptr + strsize;
	_size = _size + strsize;
}

void InGameManager::PackPacket(char* _setptr, int _num1, int _num2, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 플레이어 넘버1
	memcpy(ptr, &_num1, sizeof(_num1));
	ptr = ptr + sizeof(_num1);
	_size = _size + sizeof(_num1);

	// 플레이어 넘버2
	memcpy(ptr, &_num2, sizeof(_num2));
	ptr = ptr + sizeof(_num2);
	_size = _size + sizeof(_num2);
}

void InGameManager::PackPacket(char* _setptr, int _num, float _posX, float _posZ, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// playerNum
	memcpy(ptr, &_num, sizeof(_num));
	ptr = ptr + sizeof(_num);
	_size = _size + sizeof(_num);

	// posX
	memcpy(ptr, &_posX, sizeof(_posX));
	ptr = ptr + sizeof(_posX);
	_size = _size + sizeof(_posX);

	// posZ
	memcpy(ptr, &_posZ, sizeof(_posZ));
	ptr = ptr + sizeof(_posZ);
	_size = _size + sizeof(_posZ);
}

void InGameManager::PackPacket(char* _setptr, int _num, Weapon* _struct, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 플레이어 넘버
	memcpy(ptr, &_num, sizeof(_num));
	ptr = ptr + sizeof(_num);
	_size = _size + sizeof(_num);

	// 총 종류 패킷
	memcpy(ptr, _struct, sizeof(Weapon));
	ptr = ptr + sizeof(Weapon);
	_size = _size + sizeof(Weapon);
}

void InGameManager::PackPacket(char* _setptr, IngamePacket& _struct, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 인게임 패킷
	memcpy(ptr, &_struct, sizeof(IngamePacket));
	ptr = ptr + sizeof(IngamePacket);
	_size = _size + sizeof(IngamePacket);
}

void InGameManager::PackPacket(char* _setptr, IngamePacket& _struct, int _code, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 인게임 패킷
	memcpy(ptr, &_struct, sizeof(IngamePacket));
	ptr = ptr + sizeof(IngamePacket);
	_size = _size + sizeof(IngamePacket);

	// (아이템)코드
	memcpy(ptr, &_code, sizeof(_code));
	ptr = ptr + sizeof(_code);
	_size = _size + sizeof(_code);
}

void InGameManager::PackPacket(char* _setptr, GameInfo* &_gameInfo, vector<WeaponInfo*>& _weaponInfo, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 1. 게임정보
	memcpy(ptr, _gameInfo, sizeof(GameInfo));
	ptr = ptr + sizeof(GameInfo);
	_size = _size + sizeof(GameInfo);

	// 2. 무기종류 몇 개 인지
	int numOfWeapon = (int)_weaponInfo.size();
	memcpy(ptr, &numOfWeapon, sizeof(numOfWeapon));
	ptr = ptr + sizeof(numOfWeapon);
	_size = _size + sizeof(numOfWeapon);

	// 3. 무기정보(나중에 필요한 것만 보내야되면 vector말고 다른거 쓰자)
	for (auto iter = _weaponInfo.begin(); iter != _weaponInfo.end(); ++iter)
	{
		memcpy(ptr, *iter, sizeof(WeaponInfo));
		ptr = ptr + sizeof(WeaponInfo);
		_size = _size + sizeof(WeaponInfo);
	}
}

void InGameManager::PackPacket(char* _setptr, RoomInfo* _room, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 1. 플레이어 수 몇 명인지
	int numOfPlayer = _room->GetNumOfPlayer();
	memcpy(ptr, &numOfPlayer, sizeof(numOfPlayer));
	ptr = ptr + sizeof(numOfPlayer);
	_size = _size + sizeof(numOfPlayer);

	// 2. 플레이어 수 맞게 순서대로 스코어 패킹
	C_ClientInfo* player = nullptr;
	int playerNum = 0;
	for (int i = 0; i < numOfPlayer; i++)
	{
		player = _room->GetPlayerByIndex(i);
		
		// 2-1. 플레이어 번호 패킹
		playerNum = player->GetPlayerInfo()->GetPlayerNum();
		memcpy(ptr, &playerNum, sizeof(playerNum));
		ptr = ptr + sizeof(playerNum);
		_size = _size + sizeof(playerNum);

		// 2-2. 스코어 패킹!
		memcpy(ptr, &player->GetPlayerInfo()->GetScore(), sizeof(Score));
		ptr = ptr + sizeof(Score);
		_size = _size + sizeof(Score);
	}
}

void InGameManager::UnPackPacket(char* _getBuf, int& _num)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// num
	memcpy(&_num, ptr, sizeof(int));
	ptr = ptr + sizeof(int);
}

void InGameManager::UnPackPacket(char* _getBuf, float& _posX, float& _posZ)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// X
	memcpy(&_posX, ptr, sizeof(float));
	ptr = ptr + sizeof(float);

	// X
	memcpy(&_posZ, ptr, sizeof(float));
	ptr = ptr + sizeof(float);
}

void InGameManager::UnPackPacket(char* _getBuf, IngamePacket& _struct)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// 구조체 받음
	memcpy(&_struct, ptr, sizeof(IngamePacket));
	ptr = ptr + sizeof(IngamePacket);
}

void InGameManager::UnPackPacket(char* _getBuf, Weapon* &_weapon)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// 구조체 받음
	memcpy(_weapon, ptr, sizeof(Weapon));
	ptr = ptr + sizeof(Weapon);
}

void InGameManager::GetProtocol(PROTOCOL_INGAME& _protocol)
{
	// major state를 제외한(클라는 state를 안보내니까(혹시나 추후에 보내게되면 이부분을 수정)) protocol을 가져오기 위해서 상위 10비트 위치에 마스크를 만듦
	__int64 mask = ((__int64)PROTOCOL_OFFSET << (64 - PROTOCOL_MASK));

	// 마스크에 걸러진 1개의 프로토콜이 저장된다. 
	PROTOCOL_INGAME protocol = (PROTOCOL_INGAME)(_protocol & (PROTOCOL_INGAME)mask);

	// 아웃풋용 인자이므로 저장해준다.
	// 나중에 한번더 저장해주는 이유는 나중에 추가로 받을 수 있는 result 에 대해서 protocol 을 살려놓기 위해 
	_protocol = protocol;
}

void InGameManager::GetResult(char* _buf, RESULT_INGAME& _result)
{
	// 걸러지지않은 순수 result 가져옴
	__int64 originalResult;
	memcpy(&originalResult, _buf, sizeof(originalResult));

	// result mask 생성(33~24)
	__int64 mask = ((__int64)RESULT_OFFSET << (64 - RESULT_MASK));

	// 마스크에 걸러진 1개의 result가 저장된다. 
	RESULT_INGAME result = (RESULT_INGAME)(originalResult & (PROTOCOL_INGAME)mask);

	// 아웃풋용 인자이므로 저장해준다.
	// 나중에 한번더 저장해주는 이유는 나중에 추가로 받을 수 있는 extra data에 대해서 살려놓기 위해 
	_result = result;
}

InGameManager::PROTOCOL_INGAME InGameManager::SetProtocol(STATE_PROTOCOL _state, PROTOCOL_INGAME _protocol, RESULT_INGAME _result)
{
	// 완성된 프로토콜을 리턴 
	PROTOCOL_INGAME protocol = (PROTOCOL_INGAME)0;
	protocol = (PROTOCOL_INGAME)(_state | _protocol | _result);
	return protocol;
}

InGameManager::PROTOCOL_INGAME InGameManager::GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf)
{
	__int64 bitProtocol = 0;
	_ptr->GetPacket(bitProtocol, _buf);	// 우선 걸러지지않은 프로토콜을 가져온다.

	// 진짜 프로토콜을 가져와 준다.(안에서 프로토콜 AND 검사)
	PROTOCOL_INGAME realProtocol = (PROTOCOL_INGAME)bitProtocol;
	GetProtocol(realProtocol);

	return realProtocol;
}

bool InGameManager::WeaponSelectProcess(C_ClientInfo* _ptr, char* _buf)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	RESULT_INGAME itemSelectResult = RESULT_INGAME::INGAME_FAIL;

	Weapon* tmpWeapon = new Weapon();
	UnPackPacket(_buf, tmpWeapon);

	// 클라가 보낸 무기정보 저장!
	if (tmpWeapon != nullptr)
	{
		// 고른거 없으면 임의로 서버에서 저장
		if (tmpWeapon->mainW == WEAPONS::NODATA)
		{
			tmpWeapon->mainW = (int)RandomManager::GetInstance()->GetIntNumRandom(0, WEAPONS::MAIN_MAX_LENGTH - 1);
		}

		// 고른거 없으면 임의로 서버에서 저장
		if (tmpWeapon->subW == WEAPONS::NODATA)
		{
			tmpWeapon->subW = (int)RandomManager::GetInstance()->GetIntNumRandom(WEAPONS::MAIN_MAX_LENGTH + 1, WEAPONS::SUB_MAX_LENGTH - 1);
		}

		_ptr->GetPlayerInfo()->SetWeapon(tmpWeapon);

		itemSelectResult = RESULT_INGAME::INIT_INFO;
		wprintf(L"%s 선택 무기 : %d, %d\n",
			_ptr->GetUserInfo()->id,
			_ptr->GetPlayerInfo()->GetWeapon()->mainW,
			_ptr->GetPlayerInfo()->GetWeapon()->subW);
	}


	// 1. 시작 프로토콜 세팅(인게임 상태로)
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::START_PROTOCOL, 
		itemSelectResult);

	// 2. 게임정보 + 무기정보를 패킹
	PackPacket(buf, gameInfo.at(_ptr->GetRoom()->GetGameType()), weaponInfo, packetSize);

	// 3. 만든 패킷을 클라에게 전송
	_ptr->SendPacket(protocol, buf, packetSize);

	return true;
}

bool InGameManager::LoadingProcess(C_ClientInfo* _ptr)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 1. 플레이어의 로딩 상태를 완료로 바꾼다.
	_ptr->GetPlayerInfo()->SetLoadStatus(true);

	// 2-1. 같은 방에 있는 플레이어 리스트를 얻어온다.
	vector<C_ClientInfo*>& playerList = _ptr->GetRoom()->GetPlayers();

	// 3. 모든 플레이어가 레디상태인지 검사한다.
	RESULT_INGAME result = RESULT_INGAME::INGAME_SUCCESS;	// 일단 성공했다고 가정
	C_ClientInfo* player = nullptr;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;

		// 만약 모두가 로딩된게 아니라면 false이다.
		if (player->GetPlayerInfo()->GetLoadStatus() == false)
		{
			result = RESULT_INGAME::INGAME_FAIL;	// 모두가 로딩아님
			break;
		}
	}

	// 4. 만약 모두 로딩됐다면 플레이어들의 정보를 초기화하고 로딩 성공 프로토콜을 보낸다.
	if (result == RESULT_INGAME::INGAME_SUCCESS)
	{
		// 모두가 로딩 됐다면(그럼 총도 다 골랐을 테니)
		InitalizePlayersInfo(_ptr->GetRoom());

		// 5. 모두 로딩됐는지에 대한 결과를 보낸다.
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::LOADING_PROTOCOL, 
			result);
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

		// 6. 그리고 방을 게임 상태로 바꾸는건 InitProcess()에서 클라가 초기 패킷을 1회 보내고 난 후에
	}

	return true;
}

bool InGameManager::InitProcess(C_ClientInfo* _ptr, char* _buf)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	IngamePacket tmpPacket;
	UnPackPacket(_buf, tmpPacket);

	// 플레이어의 패킷을 저장해둔다.
	IngamePacket* gamePacket = new IngamePacket(tmpPacket);
	_ptr->GetPlayerInfo()->SetIngamePacket(gamePacket);

	// 1. 모든 플레이어에게 자신이 선택한 무기정보를 패킹(본인 포함) 및 전송
	vector<C_ClientInfo*>& playerList = _ptr->GetRoom()->GetPlayers();
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::WEAPON_PROTOCOL, 
		RESULT_INGAME::NOTIFY_WEAPON);
	PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), _ptr->GetPlayerInfo()->GetWeapon(), packetSize);

	// 전송(본인 포함)
	ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

	// 2. 모든 플레이어에게 자신의 닉네임 정보를 보내준다(본인 제외)
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::INFO_PROTOCOL, 
		RESULT_INGAME::NICKNAME);
	PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), _ptr->GetUserInfo()->nickname, packetSize);

	// 전송(본인 제외)
	ListSendPacket(playerList, _ptr, protocol, buf, packetSize, false);

	// 방이 아직 게임 준비 상태가 아니라면 게임 준비 상태로 바꾼다.
	if (_ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_READY)
	{
		// 돌아가던 타이머 있으면 타이머 끔!
		if (_ptr->GetRoom()->GetInGameTimer()->IsRunning() == true)
		{
			_ptr->GetRoom()->GetInGameTimer()->Stop();
		}

		// 3. 레디 시작 프로토콜 모두에게 보냄
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::START_PROTOCOL,
			RESULT_INGAME::READY_START);
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

		_ptr->GetRoom()->SetRoomStatus(ROOMSTATUS::ROOM_READY);	// 방이 레디 상태로 들어간다.
	}

	return true;
}

bool InGameManager::UpdateProcess(C_ClientInfo* _ptr, char* _buf)
{
	// 혹시 게임 끝났는데 방에 남아있는 경우에 서버 터지면 안되므로 예외처리
	if (_ptr->GetRoom() == nullptr
	|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return false;
	}

	// 전달된 패킷을 얻음
	IngamePacket recvPacket;
	UnPackPacket(_buf, recvPacket);

	// 1. 움직임 검사(false이면 불법 움직임은 false리턴)
	CheckMovement(_ptr, recvPacket);

	// 2. 총알 검사
	CheckBullet(_ptr, recvPacket);

	return true;   // 걍 다 true여
}

bool InGameManager::GetPosProcess(C_ClientInfo* _ptr, char* _buf)
{
	// 방이 없는 경우 나가게 예외처리
	if (_ptr->GetRoom() == nullptr
	|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("다른애 위치 받는데 방이 없음\n");
		return false;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 플레이어 num을 가져온다.
	IngamePacket pos;
	int playerNum;
	UnPackPacket(_buf, playerNum);

	// 프로토콜 세팅
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::UPDATE_PROTOCOL, 
		RESULT_INGAME::GET_OTHERPLAYER_STATUS);

	// 반복자로 돌리면서 playerNum이 일치하는 플레이어를 찾으면 그 플레이어의 위치(패킷째로)를 전송해준다.
	vector<C_ClientInfo*>& playerList = _ptr->GetRoom()->GetPlayers();	// 리스트를 얻어옴
	C_ClientInfo* player = nullptr;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;
		if (player->GetPlayerInfo()->GetPlayerNum() == playerNum)
		{
			memcpy(&pos, player->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
			PackPacket(buf, pos, packetSize);
			_ptr->SendPacket(protocol, buf, packetSize);

			return true;
		}
	}

	return false;
}

bool InGameManager::OnFocusProcess(C_ClientInfo* _ptr)
{
	// 방이 없는 경우 나가게 예외처리
	if (_ptr->GetRoom() == nullptr
	|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("포커스 바꾸는데 방이 없음\n");
		return false;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 프로토콜 세팅
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::FOCUS_PROTOCOL, 
		RESULT_INGAME::NODATA);

	// 본인에게 모든 플레이어의 인게임 정보를 보내준다
	vector<C_ClientInfo*>& playerList = _ptr->GetRoom()->GetPlayers();	// 리스트 얻어옴
	IngamePacket gamePacket;
	C_ClientInfo* player = nullptr;

	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;

		// 플레이어 정보를 자신에게 전송
		memcpy(&gamePacket, player->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
		PackPacket(buf, gamePacket, packetSize);
		_ptr->SendPacket(protocol, buf, packetSize);
	}

	return true;
}

bool InGameManager::HitAndRunProcess(C_ClientInfo* _ptr, char* _buf)
{
	// 방이 없는 경우 나가게 예외처리
	if (_ptr->GetRoom() == nullptr
		|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("차에 치였는데 방이 없음\n");
		return false;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 어느 방향으로 얼마만한 힘으로 치였는지를 얻어온다.
	IngamePacket pos;
	float posX, posZ;
	UnPackPacket(_buf, posX, posZ);

	// 이 플레이어 죽은 횟수 1증가
	_ptr->GetPlayerInfo()->GetScore().numOfDeath++;

	// 모든 플레이어들에게 내가 차에 치여 죽었다는 정보(힘과함께)를 보내준다
	protocol = SetProtocol(
		INGAME_STATE,
		PROTOCOL_INGAME::UPDATE_PROTOCOL,
		RESULT_INGAME::CAR_HIT);
	PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), posX, posZ, packetSize);
	ListSendPacket(_ptr->GetRoom()->GetPlayers(), _ptr, protocol, buf, packetSize, true);   // 나 빼고 전송

	// 차에 치이면 이따가 리스폰 시켜줘야됨
	if (_ptr->GetPlayerInfo()->IsRespawning() == false)
	{
		_ptr->GetPlayerInfo()->RespawnOn();   // 리스폰 시작
	}

	return true;
}

bool InGameManager::CaptureProcess(C_ClientInfo* _ptr, char* _buf)
{
	// 방이 없는 경우 나가게 예외처리
	if (_ptr->GetRoom() == nullptr
	|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("건물 점령했는데 방이 없음\n");
		return false;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 점령한 건물 인덱스 얻어옴
	int buildingIdx;
	UnPackPacket(_buf, buildingIdx);

	if (buildingIdx < 0)
	{
		printf("건물 인덱스가 음수다\n");
		return false;
	}

	// 방, 팀 정보 얻어옴
	RoomInfo* room = _ptr->GetRoom();
	TeamInfo& team = room->GetTeamInfo(_ptr->GetPlayerInfo()->GetTeamNum());

	// 건물 정보를 얻어옴
	BuildingInfo* building = room->GetBuildings().at(buildingIdx);
	
	// 이 건물 소유자가 있다면 뺏긴 놈 건물 개수 줄임(개수는 계속 뺏고 뺏김)
	if (building->owner != nullptr)
	{
		int teamNum = building->owner->GetPlayerInfo()->GetTeamNum();
		building->owner->GetPlayerInfo()->GetScore().captureNum--;	// 개인
	}

	// 2. 그리고 소유자를 이 클라로하고 점령중인 건물 개수 증가
	building->owner = _ptr;
	building->owner->GetPlayerInfo()->GetScore().captureNum++;

	// 3. 점령 점수 더함
	int capturePoint = gameInfo[_ptr->GetGameType()]->capturePoint;
	_ptr->GetPlayerInfo()->GetScore().captureScore += capturePoint;

	// 4. 점령 카운트증가
	_ptr->GetPlayerInfo()->GetScore().captureCount++;

	wprintf(L"건물 번호=%d, %s점령, 단독점령횟수:%d\n",
		buildingIdx,
		_ptr->GetUserInfo()->nickname,
		_ptr->GetPlayerInfo()->GetScore().captureCount);

	// 5. 건물 점령한놈의 플레이어 넘버 + 건물 번호를 모든 클라들에게 보냄.
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::CAPTURE_PROTOCOL, 
		RESULT_INGAME::INGAME_SUCCESS);
	PackPacket(buf, building->owner->GetPlayerInfo()->GetPlayerNum(), buildingIdx, packetSize);
	ListSendPacket(room->GetPlayers(), nullptr , protocol, buf, packetSize, true);	// 모두에게 전송!

	return true;
}

bool InGameManager::ItemGetProcess(C_ClientInfo* _ptr, char* _buf)
{
	// 방이 없는 경우 나가게 예외처리
	if (_ptr->GetRoom() == nullptr
	|| _ptr->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("템 먹었는데 방이 없음\n");
		return false;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 얻은 아이템 코드를 얻어옴
	int itemInt;
	ItemCode code;
	UnPackPacket(_buf, itemInt);

	code = (ItemCode)itemInt;

	// 아이템 코드를 switch하여 그에 맞는 속성을 얻는다.
	switch (code)
	{
		// HP팩 노멀 먹었을 때
		case ItemCode::HP_NORMAL:
		{
			// HP 노멀만큼 체력 +하고
			ChangeHealthAmount(_ptr, +ItemAttribute::HP_NORMAL);
		}
		break;


		default:
			return false;
	}

	// 아이템 코드를 다시 전송해서 다른 플레이어들도 갱신하도록 한다.
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::ITEM_PROTOCOL, 
		RESULT_INGAME::INGAME_SUCCESS);
	PackPacket(buf, *(_ptr->GetPlayerInfo()->GetIngamePacket()), code, packetSize);
	ListSendPacket(_ptr->GetPlayerInfo()->GetSectorPlayerList(), nullptr, protocol, buf, packetSize, true);	// 모두에게 전송

	return true;
}



bool InGameManager::GameEndProcess(RoomInfo* _room, RESULT_INGAME _result)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	vector<C_ClientInfo*>& players = _room->GetPlayers();
	protocol = SetProtocol(
		INGAME_STATE,
		PROTOCOL_INGAME::GAME_END_PROTOCOL,
		_result);

	switch (_result)
	{
		// 끝났다고 문구나 보여줘라
		case RESULT_INGAME::GAME_END_TEXT_SHOW:
		{
			ListSendPacket(players, nullptr, protocol, buf, packetSize, false);	// 모두에게 전송
			
			// 게임 종료 상태로
			_room->SetRoomStatus(ROOMSTATUS::ROOM_GAME_END);
		}
		break;

		// 진짜 끝났으니 스코어나 보여줘라
		case RESULT_INGAME::SCORE_SHOW:
		{
			PackPacket(buf, _room, packetSize);
			ListSendPacket(players, nullptr, protocol, buf, packetSize, false);	// 모두에게 전송

			// 방에 있는 플레이어들의 게임정보 초기화
			for (size_t i = 0; i < players.size(); i++)
			{
				ResetPlayerInfo(players[i]);
			}

			// 방 종료 상태로
			_room->SetRoomStatus(ROOMSTATUS::ROOM_END);
		}
		break;
	}

	return true;
}

bool InGameManager::LeaveProcess(C_ClientInfo* _ptr)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	switch (_ptr->GetRoom()->GetRoomStatus())
	{
		// 끊김 프로토콜 세팅(게임 전)
		case ROOMSTATUS::ROOM_ITEMSEL:
		{
			protocol = SetProtocol(
				STATE_PROTOCOL::INGAME_STATE,
				PROTOCOL_INGAME::DISCONNECT_PROTOCOL,
				RESULT_INGAME::WEAPON_SEL);

			// 1. 방에있는 자신을 제외한 다른 클라들에게 자신이 나갔음을 알린다.
			PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), packetSize);
			ListSendPacket(_ptr->GetRoom()->GetPlayers(), _ptr, protocol, buf, packetSize, true);

			_ptr->GetRoom()->SetRoomStatus(ROOMSTATUS::ROOM_END);	// 방 종료
		}
		break;

		// 로딩 도중
		case ROOMSTATUS::ROOM_READY:
		case ROOMSTATUS::ROOM_LOAD:
		{
			protocol = SetProtocol(
				STATE_PROTOCOL::INGAME_STATE,
				PROTOCOL_INGAME::DISCONNECT_PROTOCOL,
				RESULT_INGAME::BEFORE_LOAD);

			// 1. 방에있는 자신을 제외한 다른 클라들에게 자신이 나갔음을 알린다.
			PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), packetSize);
			ListSendPacket(_ptr->GetRoom()->GetPlayers(), _ptr, protocol, buf, packetSize, true);

			_ptr->GetRoom()->SetRoomStatus(ROOMSTATUS::ROOM_END);	// 방 종료
		}
		break;

		// 끊김 프로토콜 세팅(게임 중)
		case ROOMSTATUS::ROOM_GAME:
		{
			protocol = SetProtocol(
				STATE_PROTOCOL::INGAME_STATE,
				PROTOCOL_INGAME::DISCONNECT_PROTOCOL,
				RESULT_INGAME::ABORT);

			// 방에있는 자신을 제외한 다른 플레이어들에게 자신이 나갔음을 알린다.
			PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), packetSize);
			ListSendPacket(_ptr->GetRoom()->GetPlayers(), _ptr, protocol, buf, packetSize, true);
		}
		break;
	}

	// 일단 모든 경우 true라고 가정하고 리턴한다.
	return true;
}

///////// etc

void InGameManager::InitalizePlayersInfo(RoomInfo* _room)
{
	// 리스폰 좌표 설정용
	int gameType = 0;
	int playerNum = 0;
	float respawnPosX = 0.0f;
	float respawnPosZ = 0.0f;

	C_ClientInfo* player = nullptr;
	vector<C_ClientInfo*>&players = _room->GetPlayers();
	for (auto iter = players.begin(); iter != players.end(); ++iter)
	{
		player = *iter;

		// 초기 체력, 총알 설정
		player->GetPlayerInfo()->GetIngamePacket()->health = gameInfo[_room->GetGameType()]->maxHealth;
		player->GetPlayerInfo()->SetBullet(weaponInfo[player->GetPlayerInfo()->GetWeapon()->mainW]->maxAmmo);
		
		// 리스폰 정보
		vector<PositionInfo*>& respawnInfo = player->GetPlayerInfo()->GetPlayerRespawnInfoList();

		// 본인 초기위치랑 리스폰 정보 다 얻어옴
		for (int i = 0; i < locationInfo.size(); i++)
		{
			gameType = player->GetRoom()->GetGameType();
			playerNum = player->GetPlayerInfo()->GetPlayerNum();

			// 플레이어 넘버 + 게임타입이 일치하는 row를 찾으면
			if (gameType == locationInfo[i]->respawnInfo.gameType
				&& playerNum == locationInfo[i]->respawnInfo.playerNum)
			{
				// 1. 리스폰 정보 추가
				respawnInfo.emplace_back(new PositionInfo(locationInfo[i]->respawnInfo));

				// 2. 만약 초기 위치가 포함됐다면 이를 초기위치로 보고 인덱스 저장 및 섹터의 플레이어 리스트에 추가
				if (locationInfo[i]->firstPosInfo.posX != 0.0f
					&&locationInfo[i]->firstPosInfo.posZ != 0.0f)
				{
					C_Sector* sector = player->GetRoom()->GetSector();	// 이 방의 섹터 매니저를 얻는다.
					INDEX getIdx;
					if (sector->GetIndex(player->GetPlayerInfo()->GetIndex(), getIdx, locationInfo[i]->firstPosInfo.posX, locationInfo[i]->firstPosInfo.posZ) == true)
					{
						player->GetPlayerInfo()->SetIndex(getIdx);
						sector->Add(player, getIdx);
					}

					else
					{
						printf("유효하지 않은 인덱스!! %d, %d\n", getIdx.i, getIdx.j);
					}
				}
			}
		}
	}

	// 이제 모든 플레이어들의 인접섹터 playerList를 얻어놓는다.
	//for (auto iter = players.begin(); iter != players.end(); ++iter)
	for (size_t i = 0; i < players.size(); i++)
	{
		list<C_ClientInfo*> playerList = players[i]->GetRoom()->GetSector()->GetSectorPlayerList(players[i]->GetPlayerInfo()->GetIndex());
		players[i]->GetPlayerInfo()->SetSectorPlayerList(playerList);
	}
}

/// about movement

bool InGameManager::CheckMovement(C_ClientInfo* _ptr, IngamePacket& _recvPacket)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

#ifndef DEBUG	// 나중에 릴리즈할때 ifdef를 ifndef로 바꾸면 됨
	CheckIllegalMovement();
#endif

	// 이 방의 섹터 매니저를 얻는다.
	C_Sector* sector = _ptr->GetRoom()->GetSector();

	// 1. 플레이어 위치를 토대로 몇행 몇열인지 인덱스를 구한다.
	INDEX getIdx;
	bool isValidIdx = sector->GetIndex(_ptr->GetPlayerInfo()->GetIndex(), getIdx, _recvPacket.posX, _recvPacket.posZ);

	// 유효한 인덱스라면
	if (isValidIdx == true)
	{
		// 1. 원래 갖고 있던 인덱스 정보와 현재 이동한 인덱스 정보가 다르다면 원래 있던 리스트에서 빼고, 지금 들어온 리스트에 추가해준다.
		if (getIdx != _ptr->GetPlayerInfo()->GetIndex())
		{
			// 인접섹터 업데이트
			UpdateSectorAndSend(_ptr, _recvPacket, getIdx);

			// 1. 본인 플레이어 리스트 셋팅
			list<C_ClientInfo*> sectorPlayerList = _ptr->GetRoom()->GetSector()->GetSectorPlayerList(getIdx);
			_ptr->GetPlayerInfo()->SetSectorPlayerList(sectorPlayerList);

			// 2. 방에있는 다른 사람들 플레이어 리스트 셋팅
			UpdatePlayerList(_ptr->GetRoom()->GetPlayers(), _ptr);
		}

		// 정상 결과 protocol에 패킹
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::UPDATE_PROTOCOL, 
			RESULT_INGAME::INGAME_SUCCESS);

		// 섹터 내 플레이어들에게 패킷 전송
		PackPacket(buf, _recvPacket, packetSize);
		ListSendPacket(_ptr->GetPlayerInfo()->GetSectorPlayerList(), _ptr, protocol, buf, packetSize, true);

		// 마지막으로, 플레이어의 패킷 정보를 저장해둔다.
		IngamePacket* setPacket = new IngamePacket(_recvPacket);
		_ptr->GetPlayerInfo()->SetIngamePacket(setPacket);

		return true;
	}

	// 불법적인 인덱스라면 이전 인덱스의 섹터의 중간 위치로 강제 포지션 셋팅시킨다.
	else
	{
		IllegalSectorProcess(_ptr, _recvPacket, _ptr->GetPlayerInfo()->GetIndex());

		return false;
	}

}

// (불법이동체크) 만약 이동할 수 없는 정도의 속도로 갑자기 빠르게 움직인다면 그냥 다 무시하고 해당 클라한테만 강제 포지션 셋팅 패킷을 보낸다.
bool InGameManager::CheckIllegalMovement(C_ClientInfo* _ptr, IngamePacket& _recvPacket)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	if (_recvPacket.speed > gameInfo.at(_ptr->GetRoom()->GetGameType())->maxSpeed ||
		abs(_ptr->GetPlayerInfo()->GetIngamePacket()->posX - _recvPacket.posX) > 0.1f ||
		abs(_ptr->GetPlayerInfo()->GetIngamePacket()->posZ - _recvPacket.posZ) > 0.1f)
	{
		printf("[불법이동]이전:%f, %f\t이후:%f, %f",
			_ptr->GetPlayerInfo()->GetIngamePacket()->posX,
			_ptr->GetPlayerInfo()->GetIngamePacket()->posZ,
			_recvPacket.posX,
			_recvPacket.posZ);

		LogManager::GetInstance()->HackerFileWrite("[SpeedHack]ID:%s, NICK:%s, Speed:%f\n", _ptr->GetUserInfo()->id, _ptr->GetUserInfo()->nickname, _recvPacket.speed);
		/// 이후에 여기서 Kick하던지 하는게 필요

		// FORCE_MOVE(강제 이동) 결과 protocol에 패킹
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::UPDATE_PROTOCOL, 
			RESULT_INGAME::FORCE_MOVE);

		// 기존 플레이어가 가지고 있던 패킷 정보를 tmpPacket에 저장하고, 이를 자신에게만 보냄
		memcpy(&_recvPacket, _ptr->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
		PackPacket(buf, _recvPacket, packetSize);
		_ptr->SendPacket(protocol, buf, packetSize);

		return true;	// true리턴하면 불법 이동이 있었다는 얘기
	}

	return false;	// false리턴하면 불법이동은 없었다는 얘기
}

// 유효하지않은 섹터 인덱스일때 처리
void InGameManager::IllegalSectorProcess(C_ClientInfo* _ptr, IngamePacket& _recvPacket, INDEX _beforeIdx)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	C_Sector* sector = _ptr->GetRoom()->GetSector();

	COORD_DOUBLE LT = sector->GetLeftTop(_beforeIdx);
	COORD_DOUBLE RB = sector->GetRightBottom(_beforeIdx);

	_recvPacket.posX = (float)(LT.x + RB.x) / 2;
	_recvPacket.posZ = (float)(LT.z + RB.z) / 2;

	// FORCE_MOVE(강제 이동) 결과 protocol에 패킹
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::UPDATE_PROTOCOL, 
		RESULT_INGAME::FORCE_MOVE);

	LogManager::GetInstance()->HackerFileWrite("[TeleportHack]ID:%s, NICK:%s\n", _ptr->GetUserInfo()->id, _ptr->GetUserInfo()->nickname);
	/// 이후에 여기서 Kick하던지 하는게 필요


	// 이를 자신에게만 보냄
	PackPacket(buf, _recvPacket, packetSize);
	_ptr->SendPacket(protocol, buf, packetSize);
}

// 섹터가 바뀐경우 업데이트하고, 해당 섹터 플레이어들에게 입퇴장 패킷을 보내는 함수
void InGameManager::UpdateSectorAndSend(C_ClientInfo* _ptr, IngamePacket& _recvPacket, INDEX& _newIdx)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;


	C_Sector* sector = _ptr->GetRoom()->GetSector();

	// 1. 먼저 퇴장하는 섹터에 있는 플레이어들의 리스트를 업데이트해주고
	// 2. 입장하는 섹터에 있는 플레이어들의 리스트를 업데이트 해준다.
	// 근데 그 와중에 겹치는 플레이어가 있다면

	sector->Remove(_ptr, _ptr->GetPlayerInfo()->GetIndex());    // 기존에 있던 인덱스에서는 빼고
	sector->Add(_ptr, _newIdx);                  // 새로운 인덱스위치 리스트에 추가한다.

	list<C_ClientInfo*>enterList;
	list<C_ClientInfo*>exitList;

	// 입, 퇴장한 섹터 리스트
	byte playerBit = 0;	// 새롭게 입장한 인접 섹터의 플레이어 비트
	playerBit = sector->GetMovedSectorPlayerList(_ptr->GetPlayerInfo()->GetIndex(), _newIdx, enterList, exitList);

	// 1. 섹터 퇴장 알림 패킷 조립 및 전송
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::UPDATE_PROTOCOL, 
		RESULT_INGAME::EXIT_SECTOR);
	PackPacket(buf, _recvPacket, packetSize);

	ListSendPacket(exitList, _ptr, protocol, buf, packetSize, true);

	// 2. 섹터 입장 알림 패킷 조립 및 전송
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::UPDATE_PROTOCOL, 
		RESULT_INGAME::ENTER_SECTOR);
	PackPacket(buf, _recvPacket, packetSize);

	ListSendPacket(enterList, _ptr, protocol, buf, packetSize, true);

	// 3. 본인에게는 새롭게 입장한 인접 섹터의 플레이어 리스트를 활성화된 비트로 보내준다.
	protocol = SetProtocol(
		INGAME_STATE, 
		PROTOCOL_INGAME::UPDATE_PROTOCOL, 
		RESULT_INGAME::UPDATE_PLAYER);
	PackPacket(buf, playerBit, packetSize);

	_ptr->SendPacket(protocol, buf, packetSize);   // 전송

	_ptr->GetPlayerInfo()->SetIndex(_newIdx);            // 변경된 새로운 인덱스 설정
}

// 섹터 업데이트
void InGameManager::UpdatePlayerList(C_ClientInfo* _player)
{
	INDEX curIdx = _player->GetPlayerInfo()->GetIndex();	// 현재 있는 인덱스
	C_Sector* sector = _player->GetRoom()->GetSector();		// 이 방의 섹터 매니저를 얻는다.

	// 이 플레이어의 섹터 플레이어 리스트를 다시 설정한다.
	_player->GetPlayerInfo()->SetSectorPlayerList(sector->GetSectorPlayerList(curIdx));
}

// 플레이어들의 섹터 업데이트
void InGameManager::UpdatePlayerList(vector<C_ClientInfo*>& _players, C_ClientInfo* _exceptPlayer)
{
	for (auto iter = _players.begin(); iter != _players.end(); ++iter)
	{
		// 제외 플레이어
		if (*iter == _exceptPlayer)
			continue;

		UpdatePlayerList(*iter);
	}
}


/// about bullet
bool InGameManager::CheckBullet(C_ClientInfo* _ptr, IngamePacket& _recvPacket)
{
	IC_CS cs;

	vector<C_ClientInfo*> hitPlayers;	// _ptr의 총에 맞은놈들 리스트

	// 정상적으로 맞았는지 맞았으면 true임
	bool validHitFlag = CheckBulletHitAndGetHitPlayers(_ptr, _recvPacket, hitPlayers);

	// 맞은놈들의 상태를 다른 클라들에게 전송함
	if (validHitFlag == true)
	{
		BulletHitSend(_ptr, hitPlayers);
	}

	return validHitFlag;
}

// shot:쏜놈, hit:맞은놈
bool InGameManager::CheckBulletRange(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer)
{
	float shotPlayerPosX = _shotPlayer->GetPlayerInfo()->GetIngamePacket()->posX;
	float shotPlayerPosZ = _shotPlayer->GetPlayerInfo()->GetIngamePacket()->posZ;
	float hitPlayerPosX = _hitPlayer->GetPlayerInfo()->GetIngamePacket()->posX;
	float hitPlayerPosZ = _hitPlayer->GetPlayerInfo()->GetIngamePacket()->posZ;

	WeaponInfo* shotPlayerWeapon = weaponInfo[_shotPlayer->GetPlayerInfo()->GetWeapon()->mainW];

	// 사정거리 이내라면 true리턴
	if (UtilityManager::GetInstance()->GetDotDistanceNoSqrt(
		shotPlayerPosX, shotPlayerPosZ,
		hitPlayerPosX, hitPlayerPosZ) <= shotPlayerWeapon->range)
	{
		return true;
	}

	return false;
}
bool InGameManager::CheckMaxFire(C_ClientInfo* _shotPlayer, int _numOfBullet)
{
	WeaponInfo* shotPlayerWeapon = weaponInfo[_shotPlayer->GetPlayerInfo()->GetWeapon()->mainW];
	bool result = false;

	// 원래 0.1초당 나갈 수 있는 발 수를 계산해서 검사해야하지만 최소 1발(정수)은 보장해야하기 때문에 간단히 검사함
	if ((_numOfBullet <= shotPlayerWeapon->bulletPerShot) && (_numOfBullet > 0))
	{
		result = true;
	}

	return result;
}
int InGameManager::GetNumOfBullet(int _shootCountBit, byte _hitPlayerNum)
{
	int shifter = 0;
	int bulletCount = 0;
	int TEMP_MAX_PLAYER = 4;
	byte bitMask = 0xFF;			// 8비트 지우기 마스크(1111 1111)

	// 0이 매개변수로 넘어오면 발사된 전체 총알 갯수를 달라는 의미다.
	if (_hitPlayerNum == 0)
	{
		int bulletCountBit = 0;

		bulletCountBit = _shootCountBit;	//처음 카운트 비트 값으로
		for (int i = 1; i <= TEMP_MAX_PLAYER; i++)
		{
			shifter = 8 * (TEMP_MAX_PLAYER - i);	// 이동 연산에 필요한 값

			if ((bulletCountBit >> shifter) > 0)
			{
				bulletCount += (bulletCountBit & (bitMask << shifter)) >> shifter;
			}
		}
	}

	else
	{
		shifter = 8 * (TEMP_MAX_PLAYER - _hitPlayerNum);	// 이동 연산에 필요한 값
		bulletCount = (_shootCountBit & (bitMask << shifter)) >> shifter;
	}

	return bulletCount;
}
bool InGameManager::BulletHitProcess(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer, int _numOfBullet)
{
	WeaponInfo* shotPlayerWeapon = weaponInfo[_shotPlayer->GetPlayerInfo()->GetWeapon()->mainW];
	float originalHealth = _hitPlayer->GetPlayerInfo()->GetIngamePacket()->health;
	float totalDamage = (shotPlayerWeapon->damage * _numOfBullet);

	// 이미 피 0이면 걍 나감
	if (originalHealth == 0)
	{
		return false;
	}

	// 깎은 체력을 적용함
	if (originalHealth - totalDamage <= 0)
	{
		_hitPlayer->GetPlayerInfo()->GetIngamePacket()->health = 0.0;
	}

	else
	{
		_hitPlayer->GetPlayerInfo()->GetIngamePacket()->health = (originalHealth - totalDamage);
	}

	return true;
}
void InGameManager::BulletDecrease(C_ClientInfo* _shotPlayer, int _numOfBullet)
{
	WeaponInfo* shotPlayerWeapon = weaponInfo[_shotPlayer->GetPlayerInfo()->GetWeapon()->mainW];

	// 쏜 발 수 만큼 총알 뺌
	int originalBullet = _shotPlayer->GetPlayerInfo()->GetBullet();
	int minusBullet = (_numOfBullet / shotPlayerWeapon->bulletPerShot);

	// 이미 총알 0발이면 나감
	if (originalBullet == 0)
	{
		return;
	}

	if (originalBullet - minusBullet <= 0)
	{
		_shotPlayer->GetPlayerInfo()->SetBullet(0);
	}

	else
	{
		_shotPlayer->GetPlayerInfo()->SetBullet(originalBullet - minusBullet);
	}
}
bool InGameManager::CheckBulletHitAndGetHitPlayers(C_ClientInfo* _ptr, IngamePacket& _recvPacket, vector<C_ClientInfo*>& _hitPlayers)
{
	bool validHitFlag = false;	// 누구라도 맞았는지

	// 누구라도 쐈다면 플레이어 비트가 셋팅되어있으므로
	if (_recvPacket.collisionCheck.playerBit != 0)
	{
		// 1. 우선 최대 발 수 이내로 쐈는지 검사한다.
		int totalNumOfBullet = GetNumOfBullet(_recvPacket.collisionCheck.playerHitCountBit, 0);
		if (CheckMaxFire(_ptr, totalNumOfBullet) == false)
		{
			return false;
		}
		else
		{
			BulletDecrease(_ptr, totalNumOfBullet);	// 쏜 만큼 총알 뺌
		}

		// 2. 맞은 플레이어들이 사정거리에 있는지 검사한다.
		byte bitMask = (byte)PLAYER_BIT::PLAYER_1;
		for (int i = 0; i < _ptr->GetRoom()->GetMaxPlayer(); i++, bitMask >>= 1)
		{
			// 본인은 검사 안함
			if (i == (_ptr->GetPlayerInfo()->GetPlayerNum() - 1))
				continue;

			// 플레이어 비트가 활성화 되어 있으면
			if ((bitMask & _recvPacket.collisionCheck.playerBit) > 0)
			{
				// 같은 팀이면 맞았어도 걍 무시
				if (CheckSameTeam(_ptr, (i + 1)) == true)
				{
					continue;
				}

				// 총알을 맞은 이 플레이어가 유효한 숫자의 총알을 맞았는지 다시 검사한다.
				int numOfBullet = GetNumOfBullet(_recvPacket.collisionCheck.playerHitCountBit, (i + 1));
				if (CheckMaxFire(_ptr, numOfBullet) == false)	// 유효하지 않으면 그냥 건너 뛴다.
				{
					continue;
				}

				// 유효한 발 수라면 사정거리 검사 -> 총알 개수 감소 -> 실제 데미지 적용 순으로 간다.
				C_ClientInfo* hitPlayer = _ptr->GetRoom()->GetPlayerByPlayerNum(i + 1);	// 플레이어 번호로 찾자

				// 사정거리 검사해서 거리이내라면 데미지를 입힌다.
				if (CheckBulletRange(_ptr, hitPlayer) == true)
				{
					// 이전 피가 이미 0이라면 검사없이 그냥 넘어간다.
					if (BulletHitProcess(_ptr, hitPlayer, numOfBullet) == false)
					{
						continue;
					}

					_hitPlayers.emplace_back(hitPlayer);	// 맞은놈 리스트에 추가

					validHitFlag = true;
				}
			}
		}
	}

	return validHitFlag;	// 결과 리턴
}
void InGameManager::BulletHitSend(C_ClientInfo* _shotPlayer, const vector<C_ClientInfo*>& _hitPlayers)
{
	IC_CS cs;

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	IngamePacket packet;
	list<C_ClientInfo*>playersInSector;
	vector<C_ClientInfo*>allPlayersInRoom;
	for (int i = 0; i < _hitPlayers.size(); i++)
	{
		// 섹터 내 플레이어들에게 패킷 전송
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::UPDATE_PROTOCOL, 
			RESULT_INGAME::BULLET_HIT);
		memcpy(&packet, _hitPlayers[i]->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
		PackPacket(buf, packet, packetSize);
		ListSendPacket(_hitPlayers[i]->GetPlayerInfo()->GetSectorPlayerList(), nullptr, protocol, buf, packetSize, true);

		// 피 0이면 시간 지나면 리스폰 시켜줘야됨
		if (_hitPlayers[i]->GetPlayerInfo()->GetIngamePacket()->health <= 0
			&& _hitPlayers[i]->GetPlayerInfo()->IsRespawning() == false)
		{
			Kill(_shotPlayer, _hitPlayers[i]);	// 죽였으니 전적, 스코어 처리하고

			// 방에 있는 플레이어 다 얻어와서
			allPlayersInRoom = _shotPlayer->GetRoom()->GetPlayers();

			// 프로토콜 세팅하고
			protocol = SetProtocol(
				INGAME_STATE, 
				PROTOCOL_INGAME::UPDATE_PROTOCOL, 
				RESULT_INGAME::KILL);

			// 쏜놈 번호, 맞은놈 번호 순으로 조립해서 보냄
			PackPacket(buf,
				_shotPlayer->GetPlayerInfo()->GetPlayerNum(),
				_hitPlayers[i]->GetPlayerInfo()->GetPlayerNum(),
				packetSize);
			ListSendPacket(allPlayersInRoom, nullptr, protocol, buf, packetSize, true);

			// 그리고 이따가 부활시키게 리스폰
			_hitPlayers[i]->GetPlayerInfo()->RespawnOn();
		}
	}
}
bool InGameManager::CheckSameTeam(C_ClientInfo* _player, int _otherPlayerNum)
{
	int myTeamNum = _player->GetPlayerInfo()->GetTeamNum();	// 내 팀 번호
	int isSameTeam = false;									// 나랑 같은 팀인지 결과

	// 팀 멤버리스트 얻어서 돌리다가
	vector<C_ClientInfo*>& memberList = _player->GetRoom()->GetTeamInfo(myTeamNum).teamMemberList;
	for (auto iter = memberList.begin(); iter != memberList.end(); ++iter)
	{
		// 본인은 걍 건너뛰고
		if (*iter == _player)
			continue;

		// 플레이어 번호가 일치하는 사람을 팀 멤버 리스트에서 찾으면
		if ((*iter)->GetPlayerInfo()->GetPlayerNum() == _otherPlayerNum)
		{
			isSameTeam = true;	// 나랑 같은 팀이다.
			break;
		}
	}

	return isSameTeam;	// 결과 리턴
}

/// about property
void InGameManager::RefillBulletAndHealth(C_ClientInfo* _respawnPlayer)
{
	RefillBullet(_respawnPlayer);
	RefillHealth(_respawnPlayer);
}
void InGameManager::RefillBullet(C_ClientInfo* _player)
{
	int maxbullet = weaponInfo[_player->GetPlayerInfo()->GetWeapon()->mainW]->maxAmmo;
	_player->GetPlayerInfo()->SetBullet(maxbullet);
}
void InGameManager::RefillHealth(C_ClientInfo* _player)
{
	float maxHealth = gameInfo[_player->GetRoom()->GetGameType()]->maxHealth;
	_player->GetPlayerInfo()->GetIngamePacket()->health = maxHealth;
}
void InGameManager::ChangeHealthAmount(C_ClientInfo* _player, float _amount)
{
	float maxHealth = gameInfo[_player->GetGameType()]->maxHealth;			// 최대 체력
	float& playerHP = _player->GetPlayerInfo()->GetIngamePacket()->health;	// 플레이어 체력

	// 0미만이면 0으로 강제 고정
	if (playerHP + _amount < 0)
	{
		playerHP = 0.0f;
	}

	// 최대체력 이상이면 최대체력으로 강제 고정
	else if (playerHP + _amount > maxHealth)
	{
		playerHP = maxHealth;
	}

	// 그게 아니면 그냥 amount만큼 더함(음수면 -되겠지)
	else
	{
		playerHP += _amount;
	}
}

void InGameManager::Kill(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer)
{
	// 방이 없는 경우 나가게 예외처리
	if (_shotPlayer->GetRoom() == nullptr
	|| _shotPlayer->GetRoom()->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		printf("죽였는데 방이 없음\n");
		return;
	}

	_shotPlayer->GetPlayerInfo()->GetScore().numOfKill++;	// 쏜 놈
	_hitPlayer->GetPlayerInfo()->GetScore().numOfDeath++;	// 죽은 놈

	// 개인 킬 점수 더함
	_shotPlayer->GetPlayerInfo()->GetScore().killScore +=
		gameInfo[_shotPlayer->GetGameType()]->killPoint;

}

void InGameManager::ResetPlayerInfo(C_ClientInfo* _player)
{
	_player->GetPlayerInfo()->ResetPlayerInfo();
	_player->SetGameType(-1);
	_player->SetRoom(nullptr);
}

void InGameManager::AddCaptureBonus(RoomInfo* _room, int& _team1CaptureBonus, int& _team2CaptureBonus)
{
	int capturePoint = gameInfo[_room->GetGameType()]->capturePoint;	// 기본 점령 점수
	int bonusPoint = 0;			// 보너스 점수 저장용
	_team1CaptureBonus = _team2CaptureBonus = 0;

	
	// 일단 1:1이든 2:2든 팀은 2팀이다.
	int teamBonusScore[2] = { 0, };			// 각 팀별 보너스 점수
	vector<C_ClientInfo*> memberList;
	for (int i = 0; i < 2; i++)
	{
		TeamInfo& team = _room->GetTeamInfo(i);	// 현재 팀 정보 얻음
		memberList = team.teamMemberList;		// 이 팀에 소속된 플레이어 리스트 얻음

		for (size_t j = 0; j < team.teamMemberList.size(); j++)
		{
			// 팀원이 건물 하나라도 점령하고 있다면
			Score& memberScore = memberList[j]->GetPlayerInfo()->GetScore();
			if(memberScore.captureNum > 0)
			{
				// 건물 개수 * (점령점수 / 2)로 보너스 포인트를 준다.
				bonusPoint = (int)(memberScore.captureNum * (capturePoint * 0.5));
				memberScore.captureScore += bonusPoint;	// 이 플레이어 보너스 점수 추가!
				
				teamBonusScore[i] += bonusPoint;		// 이 팀 보너스 점수 누적
			}
		}
	}

	// 각 팀의 총 점령 스코어 저장
	_team1CaptureBonus = teamBonusScore[0];
	_team2CaptureBonus = teamBonusScore[1];
}

//////// public

bool InGameManager::IngameProtocolChecker(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);
	RESULT_INGAME result;
	GetResult(buf, result);

	switch (protocol)
	{
		case WEAPON_PROTOCOL:
			return WeaponSelectProcess(_ptr, buf);

		case LOADING_PROTOCOL:
			return LoadingProcess(_ptr);

		case START_PROTOCOL:
			return InitProcess(_ptr, buf);

		case UPDATE_PROTOCOL:
		{
			switch (result)
			{
				// 다른 사람 상태 요청 일시
			case GET_OTHERPLAYER_STATUS:
				return GetPosProcess(_ptr, buf);

				// 차에 치였을 시
			case CAR_HIT:
				return HitAndRunProcess(_ptr, buf);

				// 그냥 업데이트일 시
			default:
				return UpdateProcess(_ptr, buf);
			}
		}

		case FOCUS_PROTOCOL:
		{
			switch (result)
			{
				// focus on시 켜고, 수행할 Process실행 후 bool값 리턴!
			case INGAME_SUCCESS:
				_ptr->GetPlayerInfo()->FocusOn();
				return OnFocusProcess(_ptr);

				// focus off시
			case INGAME_FAIL:
				_ptr->GetPlayerInfo()->FocusOff();
				break;
			}
		}
		break;

		case CAPTURE_PROTOCOL:
			return CaptureProcess(_ptr, buf);

		case ITEM_PROTOCOL:
			return ItemGetProcess(_ptr, buf);
	}

	return false;
}

bool InGameManager::CanIGotoLobby(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);

	// 로비로 가고싶다는 프로토콜이면
	if (protocol == GOTO_LOBBY_PROTOCOL)
	{
		ResetPlayerInfo(_ptr);	// 초기화
		return true;
	}

	return false;
}

void InGameManager::ListSendPacket(list<C_ClientInfo*>& _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept)
{
	IC_CS cs;

	// 가져온 List가 비어있지 않은 경우에만
	if (!_list.empty())
	{
		// 리스트 안에있는 플레이어들에게 패킷을 전송한다.
		C_ClientInfo* player = nullptr;
		for (list<C_ClientInfo*>::iterator iter = _list.begin(); iter != _list.end(); ++iter)
		{
			player = *iter;

			// 유효한 클라가 아니면 걍 건너 뜀
			if (SessionManager::GetInstance()->IsClientExist(player) == false)
				continue;

			// 전송 제외할 클라 건너뜀
			if (player == _exceptClient)
				continue;

			// 포커스 없을때 보내지 않기가 설정되었다면
			if (_notFocusExcept)
			{
				// 포커스가 없는 대상은 패킷 보내기 건너 뜀
				if (player->GetPlayerInfo()->GetFocus() == false)
					continue;
			}

			player->SendPacket(_protocol, _buf, _packetSize);
		}
	}
}

void InGameManager::ListSendPacket(vector<C_ClientInfo*>& _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept)
{
	IC_CS cs;

	// 가져온 List가 비어있지 않은 경우에만
	if (!_list.empty())
	{
		// 리스트 안에있는 플레이어들에게 패킷을 전송한다.
		C_ClientInfo* player = nullptr;
		for (vector<C_ClientInfo*>::iterator iter = _list.begin(); iter != _list.end(); ++iter)
		{
			player = *iter;

			// 유효한 클라가 아니면 걍 건너 뜀
			if (SessionManager::GetInstance()->IsClientExist(player) == false)
				continue;

			// 전송 제외할 클라 건너뜀
			if (player == _exceptClient)
				continue;

			// 포커스 없을때 보내지 않기가 설정되었다면
			if (_notFocusExcept)
			{
				// 포커스가 없는 대상은 패킷 보내기 건너 뜀
				if (player->GetPlayerInfo()->GetFocus() == false)
					continue;
			}

			player->SendPacket(_protocol, _buf, _packetSize);
		}
	}
}

///threads

// 인게임에서 쓰는 종합 타이머
DWORD WINAPI InGameManager::InGameTimerThread(LPVOID _arg)
{
	RoomInfo* room = (RoomInfo*)_arg;	// 방 정보를 얻음
	C_Timer* timer = room->GetInGameTimer();	// 타이머 얻음

	InGameManager* gameManager = InGameManager::GetInstance();

	PROTOCOL_INGAME protocol = (PROTOCOL_INGAME)0;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	bool endFlag = false;

	while (endFlag == false)
	{
		// 방 상태를 보고 처리
		switch (room->GetRoomStatus())
		{
		// 무기 선택 
		case ROOMSTATUS::ROOM_ITEMSEL:
		{
			// 타이머 안돌리고있으면 타이머 돌리기 시작함
			if (timer->IsRunning() == false)
			{
				timer->Start();
			}

			// 슬립 후 검사
			std::this_thread::sleep_for(std::chrono::seconds(1));	// 1초에 한번씩 들어오면 됨
			gameManager->WeaponTimerChecker(room);
		}
		break;

		// 로딩중
		case ROOMSTATUS::ROOM_LOAD:
		{
			// 타이머 안돌리고있으면 타이머 돌리기 시작함
			if (timer->IsRunning() == false)
			{
				timer->Start();
			}

			// 1초 슬립 후 로딩 최대 대기 시간 검사
			std::this_thread::sleep_for(std::chrono::seconds(1));	// 1초에 한번씩 들어오면 됨
			gameManager->LoadingTimeChecker(room);
		}
		break;

		// 레디
		case ROOMSTATUS::ROOM_READY:
		{
			// 타이머 안돌리고있으면 타이머 돌리기 시작함
			if (timer->IsRunning() == false)
			{
				timer->Start();
			}

			// 게임 대기 시간 검사
			std::this_thread::sleep_for(std::chrono::milliseconds(TIMER_INTERVAL));	// 꼭 넣어줘야함 아니면 혼자 CPU 다 잡아먹음
			gameManager->ReadyTimeChecker(room);
		}
		break;

		// 게임 중
		case ROOMSTATUS::ROOM_GAME:
		{
			// 만약 타이머가 안돌아가고 있다면 타이머를 돌리기 시작한다.
			if (room->GetInGameTimer()->IsRunning() == false)
			{
				room->GetInGameTimer()->Start();
			}

			gameManager->RespawnChecker(room);		// 리스폰 검사
			gameManager->CarSpawnChecker(room);		// 차 스폰 검사
			gameManager->InGameTimeSync(room);		// 시간 동기화 검사
			gameManager->CaptureBonusTimeChecker(room);	// 점령 보너스 검사
			gameManager->GameEndTimeChecker(room);	// 게임 종료 검사
			
			// 슬립 후 시간 증가
			std::this_thread::sleep_for(std::chrono::milliseconds(TIMER_INTERVAL));	// 꼭 넣어줘야함 아니면 혼자 CPU 다 잡아먹음
		}
		break;

		case ROOMSTATUS::ROOM_GAME_END:
		{
			// 타이머 안돌리고있으면 타이머 돌리기 시작함
			if (timer->IsRunning() == false)
			{
				timer->Start();
			}

			// 지정시간 슬립 후 게임 종료 처리
			std::this_thread::sleep_for(std::chrono::seconds(1));	// 1초에 한번씩 들어오면 됨
			
			// 게임 종료 후, 대기 시간마저 끝났다면 진짜 스코어 띄우도록하고, 방을 닫아준다.
			if (timer->ElapsedSeconds() >= GAME_END_WAIT_TIME)
			{
				// 게임 종료처리 함수 호출(진짜 스코어 보여주라고)
				gameManager->GameEndProcess(room, RESULT_INGAME::SCORE_SHOW);
			}
		}
		break;

		// 방 종료!(여기에서 뒷정리 해줘야됨!)
		case ROOMSTATUS::ROOM_END:
		{
			// 타이머 쓰레드 핸들 뒷정리
			CloseHandle(room->GetInGameTimerHandle());
			room->SetInGameTimerHandle(nullptr);

			RoomManager::GetInstance()->OnlyDeleteRoom(room);	// 진짜 방 지움
			endFlag = true;
		}
		break;

		// 그 이외에는 그냥 CPU 시간 양보한다.
		default:
		{
			std::this_thread::sleep_for(std::chrono::milliseconds(TIMER_INTERVAL));	// 꼭 넣어줘야함 아니면 혼자 CPU 다 잡아먹음
		}
		break;
		}
	}

	printf("인게임타이머 쓰레드 종료!\n");
	return 0;	// 그리고 쓰레드 종료
}

// 리스폰 체커
void InGameManager::RespawnChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	vector<C_ClientInfo*>&playerList = _room->GetPlayers();	// 방에 있는 플레이어 리스트

	// 방에 있는 리스트 순회 하면서
	C_ClientInfo* player;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;

		// 죽어서 리스폰 대기중인 상태라면
		if (player->GetPlayerInfo()->IsRespawning() == true)
		{
			// 리스폰 대기 후 얼마나 지났는지 얻어온다.
			double respawnElapsedTime = player->GetPlayerInfo()->GetRespawnElapsedTime();
			
			// 아직 리스폰 시간만큼 안됐으면 그냥 경과 시간만 늘림
			if (respawnElapsedTime < gameInfo[_room->GetGameType()]->respawnTime)
			{
				respawnElapsedTime += TIMER_INTERVAL_TIMES_MILLISEC;	// 밀리초 단위로 더함
				player->GetPlayerInfo()->SetRespawnElapsedTime(respawnElapsedTime);	// 그리고 세팅
			}

			// 리스폰 되야하면
			else
			{
				// 이 플레이어의 기존 패킷 정보 얻어옴
				IngamePacket packet;
				memcpy(&packet, player->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));

				// 얻은 패킷 정보에서 위치만 리스폰 위치로 바꿔준다.
				vector<PositionInfo*>& respawnInfo = player->GetPlayerInfo()->GetPlayerRespawnInfoList();
				int rand = RandomManager::GetInstance()->GetIntNumRandom(0, (int)(respawnInfo.size() - 1));	// 랜덤 인덱스 얻음
				packet.posX = respawnInfo[rand]->posX;
				packet.posZ = respawnInfo[rand]->posZ;
				packet.action = 0;	// 그리고 아이들 상태로!

				// 1. 리스폰 위치로 변경된 인게임 패킷 저장
				player->GetPlayerInfo()->SetIngamePacket(new IngamePacket(packet));

				// 2. 그리고 리스폰 해야되니까 체력이랑, 총알 리셋 한다.
				RefillBulletAndHealth(player);

				// 3. ListSend 함수로 같은 방에 있는 모든 플레이어들에게 리스폰 한다고 전송한다.
				protocol = SetProtocol(
					INGAME_STATE, 
					PROTOCOL_INGAME::UPDATE_PROTOCOL, 
					RESULT_INGAME::RESPAWN);	// 리스폰 프로토콜 세팅

				// 인게임 정보 패킹
				memcpy(&packet, player->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
				PackPacket(buf, packet, packetSize);

				// 전송
				ListSendPacket(playerList, nullptr, protocol, buf, packetSize, true);

				// 4. 리스폰 할 위치의 인덱스를 얻는다.
				INDEX getIdx;
				bool isValidIdx = player->GetRoom()->GetSector()->GetIndex(player->GetPlayerInfo()->GetIndex(), getIdx, packet.posX, packet.posZ);
				if (isValidIdx == true)
				{
					// 5. 만약 죽은 곳 섹터랑 리스폰 섹터랑 다르다면 섹터 업데이트 해주고, 섹터 인덱스 갱신한다.
					if (getIdx != player->GetPlayerInfo()->GetIndex())
					{
						UpdateSectorAndSend(player, packet, getIdx);
						player->GetPlayerInfo()->SetIndex(getIdx);
					}
				}
				else
				{
					_tprintf(TEXT("ID:%s 리스폰 실패(위치정보오류)\n"), player->GetUserInfo()->id);
					return;
				}

				player->GetPlayerInfo()->RespawnOff();	// 리스폰 끝!
			}
		}
	}
}

// 자동차 스폰 체커
void InGameManager::CarSpawnChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	int seed = 0;
	vector<C_ClientInfo*>& playerList = _room->GetPlayers();

	double carSpawnTimeElapsed = _room->GetCarSpawnTimeElapsed();

	// 2:2, 1:1마다 차 나오는 시간 달라짐
	int CAR_SPAWN_TIME_SEC = 0;
	switch ((RoomInfo::GameType)gameInfo[_room->GetGameType()]->gameType)
	{
		case RoomInfo::GameType::_2vs2:
		{
			CAR_SPAWN_TIME_SEC = CAR_SPAWN_TIME_2vs2_SEC;
		}
		break;

		case RoomInfo::GameType::_1vs1:
		{
			CAR_SPAWN_TIME_SEC = CAR_SPAWN_TIME_1vs1_SEC;
		}
		break;
	}

	// 차량 스폰 시간 되면
	if (carSpawnTimeElapsed >= CAR_SPAWN_TIME_SEC)
	{
		// 방에 있는 '포커스 있는' 플레이어들에게 자동차 스폰하라고 알려줌
		seed = RandomManager::GetInstance()->GetIntNumRandom();	// 랜덤 씨드 얻고
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::UPDATE_PROTOCOL, 
			RESULT_INGAME::CAR_SPAWN);
		PackPacket(buf, seed, packetSize);	// 패킹 후
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, true);

		_room->SetCarSpawnTimeElasped(0.0);	// 시간 다시 초기화
	}

	else
	{
		_room->SetCarSpawnTimeElasped(carSpawnTimeElapsed + TIMER_INTERVAL_TIMES_MILLISEC);
	}
}

void InGameManager::InGameTimeSync(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return;
	}


	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	int seed = 0;
	vector<C_ClientInfo*>& playerList = _room->GetPlayers();

	double InGameSyncTimeElapsed = _room->GetSyncTimeElapsed();
	double InGameTotalTimeElapsed = _room->GetInGameTimer()->ElapsedSeconds();

	// 시간 동기화할 시간 되면
	if (InGameSyncTimeElapsed >= TIME_SYNC_INTERVAL)	// 많이 정밀하게 1초로 잡아야할듯
	{
		// 방에 있는 모든 플레이어들에게 현재 인게임 흘러간 시간 보내줌
		protocol = SetProtocol(
			INGAME_STATE,
			PROTOCOL_INGAME::TIMER_PROTOCOL,
			RESULT_INGAME::INGAME_SYNC);
		PackPacket(buf, InGameTotalTimeElapsed, packetSize);	// 패킹 후
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

		_room->SetSyncTimeElasped(0.0);	// 시간 다시 초기화
	}

	else
	{
		_room->SetSyncTimeElasped(InGameSyncTimeElapsed + TIMER_INTERVAL_TIMES_MILLISEC);
	}
}

// 건물 보너스 시간 체커
void InGameManager::CaptureBonusTimeChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	double bonusTimeElapsed = _room->GetCaptureBonusTimeElapsed();

	// 점령 보너스 받을 시간이 되면
	if (bonusTimeElapsed >= CAPTURE_BONUS_TIME_SEC)
	{
		int team1CaptureBonus, team2CaptureBonus;
		AddCaptureBonus(_room, team1CaptureBonus, team2CaptureBonus);	// 보너스 점수를 적용시키고

		// 업데이트 할 점수가 있는 경우에만 보낸다.
		if (team1CaptureBonus != 0 || team2CaptureBonus != 0)
		{
			vector<C_ClientInfo*>&playerList = _room->GetPlayers();
			protocol = SetProtocol(
				INGAME_STATE,
				PROTOCOL_INGAME::CAPTURE_PROTOCOL,
				RESULT_INGAME::BONUS);
			PackPacket(buf, team1CaptureBonus, team2CaptureBonus, packetSize);	// 패킹 후
			ListSendPacket(playerList, nullptr, protocol, buf, packetSize, true);
		}

		_room->SetCaptureBonusTimeElasped(0.0);	// 시간 다시 초기화
	}

	else
	{
		_room->SetCaptureBonusTimeElasped(bonusTimeElapsed + TIMER_INTERVAL_TIMES_MILLISEC);
	}
}

// 무기 타이머 체커
void InGameManager::WeaponTimerChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_ITEMSEL)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	vector<C_ClientInfo*>& playerList = _room->GetPlayers();	// 리스트 얻어옴

	if (_room->GetNumOfPlayer() < _room->GetMaxPlayer())
	{
		printf("어떤놈 무기 초 받다가 도중 나감\n");

		// 끊김 프로토콜 세팅
		protocol = SetProtocol(
			STATE_PROTOCOL::INGAME_STATE,
			PROTOCOL_INGAME::DISCONNECT_PROTOCOL,
			RESULT_INGAME::WEAPON_SEL);

		// 방에 모든 플레이어들에게 방 터졌다고 알림
		ListSendPacket(_room->GetPlayers(), nullptr, protocol, buf, packetSize, true);

		_room->SetRoomStatus(ROOMSTATUS::ROOM_END);	// 방 타이머도 삭제
		return;
	}

	// 지금까지 흐른 초를 얻어옴
	double weaponTimeElapsedSec = _room->GetInGameTimer()->ElapsedSeconds();

	// 만약 아이템 선택시간(상수)을 넘었다면 무기를 보내라고 프로토콜을 보내준다.
	if (weaponTimeElapsedSec >= WEAPON_SELTIME)
	{
		// 무기 정보를 얻어오기위한 프로토콜 조립
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::WEAPON_PROTOCOL, 
			RESULT_INGAME::NODATA);

		// 같은 방에 있는 "모든" 플레이어에게 무기를 보내라고 프로토콜을 전송함.
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

		_room->GetInGameTimer()->Stop();	// 무기선택 타이머 끝!
		_room->SetRoomStatus(ROOMSTATUS::ROOM_LOAD);	// 방 잠깐 로딩 상태로
	}

	// 아직 무기 선택 시간이 남았다면 남은 시간을 보내준다.
	else
	{
		// 같은 방에 있는 "모든" 플레이어에게 현재 무기 선택종료까지 남은 시간을 보내줌
		protocol = SetProtocol(
			INGAME_STATE, 
			PROTOCOL_INGAME::TIMER_PROTOCOL, 
			RESULT_INGAME::WEAPON_SEL);
		PackPacket(buf, (int)((WEAPON_SELTIME - (int)weaponTimeElapsedSec)), packetSize);
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);
	}
}

// 로딩 최대 시간 체커
void InGameManager::LoadingTimeChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_LOAD)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	vector<C_ClientInfo*>& playerList = _room->GetPlayers();

	// 지금까지 흐른 초를 얻어옴
	double loadingTimeElapsed = _room->GetInGameTimer()->ElapsedSeconds();

	// 로딩 최대 대기시간 넘어가면
	if (loadingTimeElapsed >= MAX_LOADING_WAIT_TIME)
	{
		// 로그 함 찍고
		printf("최대 로딩시간 넘어감\n");

		// 방에 있는 플레이어들에게 대기시간 초과 프로토콜 날림
		protocol = SetProtocol(
			STATE_PROTOCOL::INGAME_STATE,
			PROTOCOL_INGAME::DISCONNECT_PROTOCOL,
			RESULT_INGAME::MAX_LOADING_TIMEWAIT);
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, true);

		_room->SetRoomStatus(ROOMSTATUS::ROOM_END);	// 방 타이머도 삭제
	}
}

// 레디 시간 체커
void InGameManager::ReadyTimeChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_READY)
	{
		return;
	}

	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	vector<C_ClientInfo*>& playerList = _room->GetPlayers();

	// 지금까지 흐른 초를 얻어옴
	double readyTimeElapsedSec = _room->GetInGameTimer()->ElapsedSeconds();

	// 레디 시간 지나면
	if (readyTimeElapsedSec >= READY_TIME)
	{
		// 로그 함 찍고
		printf("레디시간 끝\n");

		// 방에 있는 플레이어들에게 레디 끝! 프로토콜 날림
		protocol = SetProtocol(
			STATE_PROTOCOL::INGAME_STATE,
			PROTOCOL_INGAME::START_PROTOCOL,
			RESULT_INGAME::READY_END);

		// 방에 모든 플레이어들에게 프로토콜 전송
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

		_room->GetInGameTimer()->Stop();				// 레디 타이머 끝!
		_room->SetRoomStatus(ROOMSTATUS::ROOM_GAME);	// 이제 게임상태로!
	}
}

// 게임 종료 타이머 체커
void InGameManager::GameEndTimeChecker(RoomInfo* _room)
{
	IC_CS cs;

	if (_room->GetRoomStatus() != ROOMSTATUS::ROOM_GAME)
	{
		return;
	}

	// 지금까지 흐른 초를 얻어옴
	double IngameTimeElapsed = _room->GetInGameTimer()->ElapsedSeconds();

	// 타이머 돌다가 이 방 게임 타입 최대 시간 지나면 게임 끝났다고 보내줘야됨
	if (IngameTimeElapsed >= gameInfo[_room->GetGameType()]->gameTime)
	{
		// 게임끝
		printf("게임 끝!\n");

		// 타이머 켜져있으면 타이머 끔
		C_Timer* timer = _room->GetInGameTimer();
		if (timer->IsRunning() == true)
		{
			timer->Stop();
		}

		GameEndProcess(_room, RESULT_INGAME::GAME_END_TEXT_SHOW);	// 일단 끝났다고 텍스트 먼저 보여주자
	}
}
