#include "stdafx.h"
#include "LogManager.h"
#include "InGameManager.h"
#include "RoomManager.h"
#include "RandomManager.h"
#include "C_ClientInfo.h"
#include <thread>
#include <chrono>

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

	// 리스폰 정보를 얻어옴
	RespawnInfo* respawnPtr;
	while ((respawnPtr = DatabaseManager::GetInstance()->LoadRespawnInfo()) != nullptr)
		respawnInfo.emplace_back(respawnPtr);
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
		_ptr->GetPlayerInfo()->SetWeapon(tmpWeapon);
		itemSelectResult = RESULT_INGAME::INGAME_SUCCESS;

		wprintf(L"%s 선택 무기 : %d, %d\n",
			_ptr->GetUserInfo()->id,
			_ptr->GetPlayerInfo()->GetWeapon()->mainW,
			_ptr->GetPlayerInfo()->GetWeapon()->subW);
	}


	// 1. 시작 프로토콜 세팅(인게임 상태로)
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::START_PROTOCOL, itemSelectResult);

	// 2. 게임정보 + 무기정보를 패킹
	PackPacket(buf, gameInfo.at(_ptr->GetRoom()->GetGameType()), weaponInfo, packetSize);

	// 3. 만든 패킷을 클라에게 전송
	_ptr->SendPacket(protocol, buf, packetSize);

	return false;
}

bool InGameManager::LoadingProcess(C_ClientInfo* _ptr)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 1. 플레이어의 로딩 상태를 완료로 바꾼다.
	_ptr->GetPlayerInfo()->SetLoadStatus(true);

	// 2. 같은 방에 있는 플레이어 리스트를 얻어온다.
	vector<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayers();
	
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
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::LOADING_PROTOCOL, result);
		ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);
	
		// 6. 그리고 방을 게임 상태로 바꾼다.
		_ptr->GetRoom()->SetRoomStatus(ROOMSTATUS::ROOM_GAME);	// 방이 게임으로 입장하였다.

		// 7. 차 스포너 쓰레드를 생성한다.
		_ptr->GetRoom()->SetCarSpawnerHandle
		(
			(HANDLE)_beginthreadex(
				nullptr,
				0,
				(_beginthreadex_proc_type)InGameManager::CarSpawnerThread,
				(LPVOID)_ptr->GetRoom(),
				0,
				NULL)
		);

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

	// 디버깅용 출력
	printf("[초기인덱스]%d ,%f, %f, %f, %d\n", gamePacket->playerNum, gamePacket->posX, gamePacket->posZ, gamePacket->rotY, gamePacket->action);

	C_Sector* sector = _ptr->GetRoom()->GetSector();	// 이 방의 섹터 매니저를 얻는다.

	// 플레이어 위치를 토대로 몇행 몇열인지 인덱스를 구해서 해당 섹터에 추가한다.
	INDEX getIdx;
	if (sector->GetIndex(_ptr->GetPlayerInfo()->GetIndex(), getIdx, tmpPacket.posX, tmpPacket.posZ) == true)
	{
		_ptr->GetPlayerInfo()->SetIndex(getIdx);
		sector->Add(_ptr, getIdx);
	}

	else
		printf("유효하지 않은 인덱스!! %d, %d\n", getIdx.i, getIdx.j);


	// 1. 모든 플레이어에게 자신이 선택한 무기정보를 패킹(본인 포함) 및 전송
	vector<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayers();
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::WEAPON_PROTOCOL, RESULT_INGAME::NOTIFY_WEAPON);
	PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), _ptr->GetPlayerInfo()->GetWeapon(), packetSize);

	// 전송(본인 포함)
	ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

	// 2. 모든 플레이어에게 자신의 닉네임 정보를 보내준다(본인 제외)
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::NICKNAME_PROTOCOL, RESULT_INGAME::NODATA);
	PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), _ptr->GetUserInfo()->nickname, packetSize);

	// 전송(본인 제외)
	ListSendPacket(playerList, _ptr, protocol, buf, packetSize, false);

	return true;
}

bool InGameManager::UpdateProcess(C_ClientInfo* _ptr, char* _buf)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 전달된 패킷을 얻음
	IngamePacket recvPacket;
	UnPackPacket(_buf, recvPacket);
	//printf("%d ,%f, %f, %f, %d\n", recvPacket.playerNum, recvPacket.posX, recvPacket.posZ, recvPacket.rotY, recvPacket.action);
	//printf("패킷 %d회 보냄\n", ++numOfPacketSent);

	list<C_ClientInfo*>sendList;	// 현재 섹터 + 인접 섹터에 존재하는 플레이어 리스트
	C_ClientInfo* exceptClient = nullptr;	// 패킷 안보낼 대상

	// 1. 움직임 검사
	if (CheckMovement(_ptr, recvPacket) == true)
	{
		// 정상 결과 protocol에 패킹
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::INGAME_SUCCESS);
		
		// 보낼 리스트 구함
		sendList = _ptr->GetRoom()->GetSector()->GetSectorPlayerList(_ptr->GetPlayerInfo()->GetIndex());
		exceptClient = _ptr;	// 움직임은 자신한텐 안보낸다.

		// 섹터 내 플레이어들에게 패킷 전송
		PackPacket(buf, recvPacket, packetSize);
		ListSendPacket(sendList, exceptClient, protocol, buf, packetSize, true);

		// 마지막으로, 플레이어의 패킷 정보를 저장해둔다.
		IngamePacket* setPacket = new IngamePacket(recvPacket);
		_ptr->GetPlayerInfo()->SetIngamePacket(setPacket);
	}

	// 불법 움직임은 이미 CheckMovement에서 다 처리했으니 빠져나간다.
	else
		return false;

	// 2. 총알 검사
	CheckBullet(_ptr, recvPacket);


	return true;   // 걍 다 true여
}

bool InGameManager::GetPosProcess(C_ClientInfo* _ptr, char* _buf)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 플레이어 num을 가져온다.
	IngamePacket pos;
	int playerNum;
	UnPackPacket(_buf, playerNum);

	// 프로토콜 세팅
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::GET_OTHERPLAYER_STATUS);

	// 반복자로 돌리면서 playerNum이 일치하는 플레이어를 찾으면 그 플레이어의 위치(패킷째로)를 전송해준다.
	vector<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayers();	// 리스트를 얻어옴
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
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 프로토콜 세팅
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::FOCUS_PROTOCOL, RESULT_INGAME::NODATA);

	// 본인에게 모든 플레이어의 인게임 정보를 보내준다
	vector<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayers();	// 리스트 얻어옴
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
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 어느 방향으로 얼마만한 힘으로 치였는지를 얻어온다.
	IngamePacket pos;
	float posX, posZ;
	UnPackPacket(_buf, posX, posZ);

	// 이 플레이어 죽은 횟수 1증가
	_ptr->GetPlayerInfo()->GetScore().numOfDeath++;
	
	// 섹터에 있는 플레이어들에게 내가 차에 치여 죽었다는 정보(힘과함께)를 보내준다
	list<C_ClientInfo*> sendList = _ptr->GetRoom()->GetSector()->GetSectorPlayerList(_ptr->GetPlayerInfo()->GetIndex());
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::CAR_HIT);
	PackPacket(buf, _ptr->GetPlayerInfo()->GetPlayerNum(), posX, posZ, packetSize);
	ListSendPacket(sendList, _ptr, protocol, buf, packetSize, true);	// 나 빼고 전송

	// 차에 치이면 이따가 리스폰 시켜줘야됨
	if (_ptr->GetPlayerInfo()->IsRespawning() == false)
	{
		Respawn(_ptr);
	}

	return true;
}

bool InGameManager::LeaveProcess(C_ClientInfo* _ptr, int _playerNum)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	// 끊김 프로토콜 세팅
	protocol = SetProtocol(
		STATE_PROTOCOL::INGAME_STATE, 
		PROTOCOL_INGAME::DISCONNECT_PROTOCOL, 
		RESULT_INGAME::INGAME_SUCCESS);

	// 패킹
	PackPacket(buf, _playerNum, packetSize);

	// 자신을 제외한 다른 클라들에게 자신이 나갔음을 알린다.
	vector<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayers();	// 리스트 얻어옴
	C_ClientInfo* player = nullptr;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;

		// 본인 제외
		if (player == _ptr)
			continue;

		// 다른 플레이어에게 자신이 나갔음을 전송
		else
			player->SendPacket(protocol, buf, packetSize);
	}

	// 나간 플레이어의 섹터, 인접섹터 리스트에서 지워주고
	_ptr->GetRoom()->GetSector()->LeaveProcess(_ptr, _ptr->GetPlayerInfo()->GetIndex());

	// 일단 모든 경우 true라고 가정하고 리턴한다.
	return true;
}

///////// etc

void InGameManager::InitalizePlayersInfo(RoomInfo* _room)
{
	// 리스폰 좌표 설정용
	int gameType = 0;
	int playerNum = 0;
	float posX = 0.0f;
	float posZ = 0.0f;

	C_ClientInfo* player = nullptr;
	vector<C_ClientInfo*>players = _room->GetPlayers();
	for (auto iter = players.begin(); iter != players.end(); ++iter)
	{
		player = *iter;
		
		// 초기 체력, 총알 설정(총알은 나중에 IngamePacket으로 편입 시키자)
		player->GetPlayerInfo()->GetIngamePacket()->health = gameInfo[_room->GetGameType()]->maxHealth;
		player->GetPlayerInfo()->SetBullet(weaponInfo[player->GetPlayerInfo()->GetWeapon()->mainW]->maxAmmo);

		// 리스폰 위치 찾아서
		for (int i = 0; i < respawnInfo.size(); i++)
		{
			gameType = player->GetRoom()->GetGameType();
			playerNum = player->GetPlayerInfo()->GetPlayerNum();

			if (gameType == respawnInfo[i]->gameType
				&& playerNum == respawnInfo[i]->playerNum)
			{
				posX = respawnInfo[i]->posX;
				posZ = respawnInfo[i]->posZ;

				break;
			}
		}
		
		// 리스폰 위치 세팅
		player->GetPlayerInfo()->SetRespawnPos(posX, posZ);
	}
}

/// about movement

bool InGameManager::CheckMovement(C_ClientInfo* _ptr, IngamePacket& _recvPacket)
{
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
			UpdateSectorAndSend(_ptr, _recvPacket, getIdx);
		}
	
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
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::FORCE_MOVE);

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
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::FORCE_MOVE);

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

	sector->Delete(_ptr, _ptr->GetPlayerInfo()->GetIndex());    // 기존에 있던 인덱스에서는 빼고
	sector->Add(_ptr, _newIdx);                  // 새로운 인덱스위치 리스트에 추가한다.

	list<C_ClientInfo*>enterList;
	list<C_ClientInfo*>exitList;

	// 입, 퇴장한 섹터 리스트
	byte playerBit = 0;	// 새롭게 입장한 인접 섹터의 플레이어 비트
	playerBit = sector->GetMovedSectorPlayerList(_ptr->GetPlayerInfo()->GetIndex(), _newIdx, enterList, exitList);

	// 1. 섹터 퇴장 알림 패킷 조립 및 전송
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::EXIT_SECTOR);
	PackPacket(buf, _recvPacket, packetSize);

	ListSendPacket(exitList, _ptr, protocol, buf, packetSize, true);

	// 2. 섹터 입장 알림 패킷 조립 및 전송
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::ENTER_SECTOR);
	PackPacket(buf, _recvPacket, packetSize);

	ListSendPacket(enterList, _ptr, protocol, buf, packetSize, true);

	// 3. 본인에게는 새롭게 입장한 인접 섹터의 플레이어 리스트를 활성화된 비트로 보내준다.
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::UPDATE_PLAYER);
	PackPacket(buf, playerBit, packetSize);

	_ptr->SendPacket(protocol, buf, packetSize);   // 전송

	_ptr->GetPlayerInfo()->SetIndex(_newIdx);            // 변경된 새로운 인덱스 설정
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
	if ( (_numOfBullet <= shotPlayerWeapon->bulletPerShot) && (_numOfBullet > 0))
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
				// 총알을 맞은 이 플레이어가 유효한 숫자의 총알을 맞았는지 다시 검사한다.
				int numOfBullet = GetNumOfBullet(_recvPacket.collisionCheck.playerHitCountBit, (i + 1));
				if (CheckMaxFire(_ptr, numOfBullet) == false)	// 유효하지 않으면 그냥 건너 뛴다.
				{
					continue;
				}

				// 유효한 발 수라면 사정거리 검사 -> 총알 개수 감소 -> 실제 데미지 적용 순으로 간다.
				C_ClientInfo* hitPlayer = _ptr->GetRoom()->GetPlayerByIndex(i);

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
		// 맞은놈의 인접섹터에 있는 플레이어들의 리스트를 얻음
		playersInSector = _hitPlayers[i]->GetRoom()->GetSector()->GetSectorPlayerList(_hitPlayers[i]->GetPlayerInfo()->GetIndex());

		// 섹터 내 플레이어들에게 패킷 전송
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::BULLET_HIT);
		memcpy(&packet, _hitPlayers[i]->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
		PackPacket(buf, packet, packetSize);
		ListSendPacket(playersInSector, nullptr, protocol, buf, packetSize, true);

		// 피 0이면 시간 지나면 리스폰 시켜줘야됨
		if (_hitPlayers[i]->GetPlayerInfo()->GetIngamePacket()->health <= 0
			&& _hitPlayers[i]->GetPlayerInfo()->IsRespawning() == false)
		{
			Kill(_shotPlayer, _hitPlayers[i]);	// 죽였으니 전적, 스코어 처리하고
			
			// 방에 있는 플레이어 다 얻어와서
			allPlayersInRoom = _shotPlayer->GetRoom()->GetPlayers();
			
			// 프로토콜 세팅하고
			protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::KILL);
			
			// 쏜놈 번호, 맞은놈 번호 순으로 조립해서 보냄
			PackPacket(buf, 
				_shotPlayer->GetPlayerInfo()->GetPlayerNum(), 
				_hitPlayers[i]->GetPlayerInfo()->GetPlayerNum(),
				packetSize);
			ListSendPacket(allPlayersInRoom, nullptr, protocol, buf, packetSize, true);

			// 그리고 이따가 부활시키게 리스폰
			Respawn(_hitPlayers[i]);
		}
	}
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

void InGameManager::Kill(C_ClientInfo* _shotPlayer, C_ClientInfo* _hitPlayer)
{
	_shotPlayer->GetPlayerInfo()->GetScore().numOfKill++;	// 쏜 놈
	_hitPlayer->GetPlayerInfo()->GetScore().numOfDeath++;	// 죽은 놈

	/// 팀 처리는 나중에 한꺼번에 teamScore = 1p+2p 이런식으로

	// 점수 더함
	_shotPlayer->GetPlayerInfo()->GetScore().killScore +=
		gameInfo[_shotPlayer->GetGameType()]->killPoint;

	// 팀 점수도 더함
	_shotPlayer->GetRoom()->GetTeamInfo(
		_shotPlayer->GetPlayerInfo()->GetTeamNum()).teamKillScore +=
		gameInfo[_shotPlayer->GetGameType()]->killPoint;

}
void InGameManager::Respawn(C_ClientInfo* _player)
{
	std::thread respawnThread(RespawnWaitAndRevive, _player);		// 1회용 리스폰 쓰레드 생성
	respawnThread.detach();											// 이 쓰레드에서 손 뗌 넌 자유

	_player->GetPlayerInfo()->RespawnOn();	// 리스폰 시작
}

//////// public
bool InGameManager::CanISelectWeapon(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);

	if (protocol == WEAPON_PROTOCOL)
		return WeaponSelectProcess(_ptr, buf);

	return false;
}

bool InGameManager::LoadingSuccess(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);

	// 로딩 검사
	if (protocol == LOADING_PROTOCOL)
		return LoadingProcess(_ptr);

	return false;
}

bool InGameManager::CanIStart(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);

	if (protocol == START_PROTOCOL)
		return InitProcess(_ptr, buf);

	return false;
}

bool InGameManager::CanIUpdate(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);
	RESULT_INGAME result;
	GetResult(buf, result);

	// 업데이트 프로토콜 들어왔을 시 업데이트 프로세스 수행
	if (protocol == UPDATE_PROTOCOL)
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

	return false;
}

bool InGameManager::CanIChangeFocus(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);
	RESULT_INGAME result;
	GetResult(buf, result);

	// 포커스 변경 프로토콜 들어왔을 시
	if (protocol == FOCUS_PROTOCOL)
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

	return true;
}

void InGameManager::ListSendPacket(list<C_ClientInfo*> _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept)
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

void InGameManager::ListSendPacket(vector<C_ClientInfo*> _list, C_ClientInfo* _exceptClient, PROTOCOL_INGAME _protocol, char* _buf, int _packetSize, bool _notFocusExcept)
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

// 무기 선택 타이머 쓰레드
DWORD WINAPI InGameManager::WeaponSelectTimerThread(void* _arg)
{
	RoomInfo* room = (RoomInfo*)_arg;	// 방 정보를 얻음

	InGameManager* gameManager = InGameManager::GetInstance();

	PROTOCOL_INGAME protocol = (PROTOCOL_INGAME)0;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;
	
	LARGE_INTEGER frequency;
	LARGE_INTEGER beginTime;
	LARGE_INTEGER endTime;
	__int64 elapsed;
	double duringTime = 0.0;	// 실제로 경과된 시간을(초단위) 가지고 있을 double 변수
	int sec = 0;				// 초 단위

	QueryPerformanceFrequency(&frequency);	// 최초 1회 주파수 얻음
	QueryPerformanceCounter(&beginTime);	// 시작 시간 얻음
	while (1)
	{
		QueryPerformanceCounter(&endTime);					// 종료 시간 얻음
		elapsed = endTime.QuadPart - beginTime.QuadPart;	// 경과된 시간 계산
		
		
		duringTime = (double)elapsed / (double)frequency.QuadPart;	// 실제로 흐른 시간을 초 단위로 계산
		
		// 만약 아이템 선택시간(상수)을 넘었다면 쓰레드 핸들 반납 후, 무한루프를 빠져나간다.
		if (duringTime >= WEAPON_SELTIME)
		{
			// 누구 하나라도 게임 시작전에 나갔으면 이 방은 터진거임
			if (room->GetMaxPlayer() > room->GetNumOfPlayer())
			{
				/// 그리고 여기에서 클라들에게 방 터졌다는 프로토콜을 보냄
				return -1;
			}

			// 쓰레드 핸들 반납
			CloseHandle(room->GetWeaponTimerHandle());
			room->SetWeaponTimerHandle(nullptr);

			// 무기 정보를 얻어오기위한 프로토콜 조립
			protocol = InGameManager::GetInstance()->SetProtocol(INGAME_STATE, PROTOCOL_INGAME::WEAPON_PROTOCOL, RESULT_INGAME::NODATA);
			packetSize = 0;

			// 같은 방에 있는 "모든" 플레 이어에게 무기를 보내라고 프로토콜을 전송함.
			vector<C_ClientInfo*> playerList = room->GetPlayers();	// 리스트 얻어옴
			InGameManager::GetInstance()->ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);

			break;
		}

		// 1초마다 값이 변하면
		if (sec < (int)duringTime)
		{
			// 누구 하나라도 게임 시작전에 나갔으면 이 방은 터진거임
			if (room->GetMaxPlayer() > room->GetNumOfPlayer())
			{
				/// 그리고 여기에서 클라들에게 방 터졌다는 프로토콜을 보냄
				return -1;
			}

			sec = (int)duringTime;	// 새롭게 초 단위를 갱신시켜준다.(before sec의 역할)

			// 1초에 한 번씩 시간을 알려주는 프로토콜을 보냄.
			protocol = InGameManager::GetInstance()->SetProtocol(INGAME_STATE, PROTOCOL_INGAME::TIMER_PROTOCOL, RESULT_INGAME::NODATA);
			packetSize = 0;

			// 무기 선택종료까지 남은 시간 패킷 세팅
			InGameManager::GetInstance()->PackPacket(buf, (WEAPON_SELTIME - sec), packetSize);

			// 같은 방에 있는 "모든" 플레이어에게 현재 무기 선택종료까지 남은 시간을 보내줌
			vector<C_ClientInfo*> playerList = room->GetPlayers();	// 리스트 얻어옴
			InGameManager::GetInstance()->ListSendPacket(playerList, nullptr, protocol, buf, packetSize, false);
		}

		Sleep(50);	// 꼭 넣어줘야함 아니면 혼자 CPU 다 잡아먹음
	}

	return 0;	// 그리고 쓰레드 종료
}

// 차 스포너
DWORD WINAPI InGameManager::CarSpawnerThread(LPVOID _arg)
{
	RoomInfo* room = (RoomInfo*)_arg;

	PROTOCOL_INGAME protocol = InGameManager::GetInstance()->SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::CAR_SPAWN);
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	int seed = 0;

	vector<C_ClientInfo*>playerList;

	// 게임중에 계속 스폰
	while (room->GetRoomStatus() == ROOMSTATUS::ROOM_GAME)
	{
		Sleep(CAR_SPAWN_TIME);

		IC_CS cs;

		// 방에 있는 '포커스 있는' 플레이어들에게 자동차 스폰하라고 알려줌
		playerList = room->GetPlayers();
		seed = RandomManager::GetInstance()->GetIntNumRandom();				// 랜덤 씨드 얻고
		InGameManager::GetInstance()->PackPacket(buf, seed, packetSize);	// 패킹 후
		InGameManager::GetInstance()->ListSendPacket(playerList, nullptr, protocol, buf, packetSize, true);
	}

	// 쓰레드 핸들 반납
	CloseHandle(room->GetCarSpawnerHandle());
	room->SetCarSpawnerHandle(nullptr);

	return 0;
}

void InGameManager::RespawnWaitAndRevive(C_ClientInfo* _player)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	InGameManager* gameManager = InGameManager::GetInstance();


	// 이 플레이어가 선택한 게임의 리스폰 시간만큼 리스폰 대기한다.
	printf("RespawnWaitAndRevive 슬립 시작\n");
	std::this_thread::sleep_for(
		std::chrono::seconds(
			gameManager->gameInfo[_player->GetRoom()->GetGameType()]->respawnTime
		));
	printf("RespawnWaitAndRevive 슬립 종료\n");
	
	IC_CS cs;	// 동기화 시작! 중요!!

	// 대기 끝났는데 이 클라가 나가버렸으면 그냥 쓰레드 종료!
	if (SessionManager::GetInstance()->IsClientExist(_player) == false)
	{
		return;
	}

	// 이 플레이어의 기존 패킷 정보 얻어옴
	IngamePacket packet;
	memcpy(&packet, _player->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));

	// 얻은 패킷 정보에서 위치만 리스폰 위치로 바꿔준다.
	packet.posX = _player->GetPlayerInfo()->GetRespawnPosX();
	packet.posZ = _player->GetPlayerInfo()->GetRespawnPosZ();
	packet.action = 0;	// 그리고 아이들 상태로!

	// 1. 리스폰 위치로 변경된 인게임 패킷 저장
	_player->GetPlayerInfo()->SetIngamePacket(new IngamePacket(packet));

	// 2. 그리고 리스폰 해야되니까 체력이랑, 총알 리셋 한다.
	gameManager->RefillBulletAndHealth(_player);

	// 4. 리스폰 될 위치의 같은 섹터에 있는 플레이어 리스트를 얻어온다.
	//list<C_ClientInfo*> playerList = _player->GetRoom()->GetSector()->GetSectorPlayerList(_player->GetPlayerInfo()->GetIndex());
	
	// 3. 방에 있는 모든 플레이어 목록을 얻는다.
	vector<C_ClientInfo*> playerList = _player->GetRoom()->GetPlayers();

	// 4. ListSend 함수로 같은 방에 있는 모든 플레이어들에게 리스폰 한다고 전송한다.
	protocol = gameManager->SetProtocol(INGAME_STATE, PROTOCOL_INGAME::UPDATE_PROTOCOL, RESULT_INGAME::RESPAWN);	// 리스폰 프로토콜 세팅

	// 인게임 정보 패킹
	memcpy(&packet, _player->GetPlayerInfo()->GetIngamePacket(), sizeof(IngamePacket));
	gameManager->PackPacket(buf, packet, packetSize);

	// 전송
	gameManager->ListSendPacket(playerList, nullptr, protocol, buf, packetSize, true);

	// 5. 리스폰 할 위치의 인덱스를 얻는다.
	INDEX getIdx;
	bool isValidIdx = _player->GetRoom()->GetSector()->GetIndex(_player->GetPlayerInfo()->GetIndex(), getIdx, packet.posX, packet.posZ);
	if (isValidIdx == true)
	{
		// 6. 만약 죽은 곳 섹터랑 리스폰 섹터랑 다르다면 섹터 업데이트 해주고, 섹터 인덱스 갱신한다.
		if (getIdx != _player->GetPlayerInfo()->GetIndex())
		{
			gameManager->UpdateSectorAndSend(_player, packet, getIdx);
			_player->GetPlayerInfo()->SetIndex(getIdx);
		}
	}
	else
	{
		_tprintf(TEXT("ID:%s 리스폰 실패(위치정보오류)\n"), _player->GetUserInfo()->id);
		return;
	}

	_player->GetPlayerInfo()->RespawnOff();		// 리스폰 끝!
}
