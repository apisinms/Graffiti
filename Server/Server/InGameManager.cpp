#include "stdafx.h"
#include "LogManager.h"
#include "InGameManager.h"
#include "RoomManager.h"
#include "C_ClientInfo.h"

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
}

void InGameManager::End()
{
}

void InGameManager::PackPacket(char* _setptr, const int &_sec, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 초 셋팅
	memcpy(ptr, &_sec, sizeof(_sec));
	ptr = ptr + sizeof(_sec);
	_size = _size + sizeof(_sec);
}

void InGameManager::PackPacket(char* _setptr, PositionPacket& _struct, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 포지션 패킷
	memcpy(ptr, &_struct, sizeof(PositionPacket));
	ptr = ptr + sizeof(PositionPacket);
	_size = _size + sizeof(PositionPacket);
}

void InGameManager::UnPackPacket(char* _getBuf, int& _num)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// num
	memcpy(&_num, ptr, sizeof(int));
	ptr = ptr + sizeof(int);
}

void InGameManager::UnPackPacket(char* _getBuf, PositionPacket& _struct)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// 구조체 받음
	memcpy(&_struct, ptr, sizeof(PositionPacket));
	ptr = ptr + sizeof(PositionPacket);
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

	RESULT_INGAME itemSelect = RESULT_INGAME::INGAME_FAIL;

	Weapon* tmpWeapon = new Weapon();
	UnPackPacket(_buf, tmpWeapon);
	
	// 클라가 보낸 무기정보 저장!
	if (tmpWeapon != nullptr)
	{
		_ptr->GetPlayerInfo()->SetWeapon(tmpWeapon);
		itemSelect = RESULT_INGAME::INGAME_SUCCESS;

		wprintf(L"%s 선택 무기 : %d, %d\n",
			_ptr->GetUserInfo()->id,
			_ptr->GetPlayerInfo()->GetWeapon()->mainW,
			_ptr->GetPlayerInfo()->GetWeapon()->subW);
	}

	// 프로토콜 세팅(인게임 상태로)
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::START_PROTOCOL, itemSelect);

	// 패킹 및 전송
	_ptr->SendPacket(protocol, buf, packetSize);

	if (itemSelect == RESULT_INGAME::INGAME_SUCCESS)
	{
		_ptr->GetRoom()->SetRoomStatus(ROOMSTATUS::ROOM_GAME);	// 방이 게임으로 입장하였다.
		return true;
	}

	return false;
}

bool InGameManager::InitProcess(C_ClientInfo* _ptr, char* _buf)
{
	PositionPacket tmpPos;
	UnPackPacket(_buf, tmpPos);

	// 플레이어의 위치 정보를 저장해둔다.
	PositionPacket* position = new PositionPacket(tmpPos);
	_ptr->GetPlayerInfo()->SetPosition(position);

	// 디버깅용 출력
	printf("[초기인덱스]%d ,%f, %f, %f, %d\n", position->playerNum, position->posX, position->posZ, position->rotY, position->action);

	C_Sector* sector = _ptr->GetRoom()->GetSector();	// 이 방의 섹터 매니저를 얻는다.

	// 플레이어 위치를 토대로 몇행 몇열인지 인덱스를 구해서 해당 섹터에 추가한다.
	INDEX getIdx;
	if (sector->GetIndex(_ptr->GetPlayerInfo()->GetIndex(), getIdx, tmpPos.posX, tmpPos.posZ) == true)
	{
		_ptr->GetPlayerInfo()->SetIndex(getIdx);
		sector->Add(_ptr, getIdx);
	}

	else
		printf("유효하지 않은 인덱스!! %d, %d\n", getIdx.i, getIdx.j);

	return true;
}

bool InGameManager::MoveProcess(C_ClientInfo* _ptr, char* _buf)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize;

	// 전달된 위치 정보를 얻음
	PositionPacket movedPos;
	UnPackPacket(_buf, movedPos);
	printf("%d ,%f, %f, %f, %d\n", movedPos.playerNum, movedPos.posX, movedPos.posZ, movedPos.rotY, movedPos.action);

	// 만약 이동할 수 없는 정도의 속도로 갑자기 빠르게 움직인다면 그냥 다 무시하고 해당 클라한테만 강제 포지션 셋팅 패킷을 보낸다.
#ifdef DEBUG	// 나중에 릴리즈할때 ifdef를 ifndef로 바꾸면 됨
	if (movedPos.speed > PlayerInfo::MAX_SPEED ||
		abs(_ptr->GetPlayerInfo()->GetPosition()->posX - movedPos.posX) > 0.1f ||
		abs(_ptr->GetPlayerInfo()->GetPosition()->posZ - movedPos.posZ) > 0.1f)
	{
		printf("[불법이동]이전:%f, %f\t이후:%f, %f",
			_ptr->GetPlayerInfo()->GetPosition()->posX,
			_ptr->GetPlayerInfo()->GetPosition()->posZ,
			movedPos.posX,
			movedPos.posZ);

		LogManager::GetInstance()->HackerFileWrite("[SpeedHack]ID:%s, NICK:%s, Speed:%f\n", _ptr->GetUserInfo()->id, _ptr->GetUserInfo()->nickname, movedPos.speed);
		/// 이후에 여기서 Kick하던지 하는게 필요

		// FORCE_MOVE(강제 이동) 결과 protocol에 패킹
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::FORCE_MOVE);
		
		// 기존 플레이어가 가지고 있던 포지션 정보를 movedPos에 저장하고, 이를 자신에게만 보냄
		memcpy(&movedPos, _ptr->GetPlayerInfo()->GetPosition(), sizeof(PositionPacket));
		PackPacket(buf, movedPos, packetSize);
		_ptr->SendPacket(protocol, buf, packetSize);

		return true;	// 빠져나감
	}
#endif

	// 이 방의 섹터 매니저를 얻는다.
	C_Sector* sector = _ptr->GetRoom()->GetSector();

	// 1. 플레이어 위치를 토대로 몇행 몇열인지 인덱스를 구한다.
	INDEX getIdx;
	bool isValidIdx = sector->GetIndex(_ptr->GetPlayerInfo()->GetIndex(), getIdx, movedPos.posX, movedPos.posZ);

	//// 2. 구해진 인덱스로 해당 섹터와 인접 섹터의 플레이어 리스트를 병합하여 얻는다.
	list<C_ClientInfo*>sendList = sector->GetSectorPlayerList(getIdx);

	C_ClientInfo* exceptClient = nullptr;	// 전송 제외 대상
	// 유효한 인덱스라면
	if (isValidIdx == true)
	{
		exceptClient = _ptr;	// 유효 인덱스면 자신이 전송 제외 대상이다.

		// 1. 원래 갖고 있던 인덱스 정보와 현재 이동한 인덱스 정보가 다르다면 원래 있던 리스트에서 빼고, 지금 들어온 리스트에 추가해준다.
		if (getIdx != _ptr->GetPlayerInfo()->GetIndex())
		{
			sector->Delete(_ptr, _ptr->GetPlayerInfo()->GetIndex());    // 기존에 있던 인덱스에서는 빼고
			sector->Add(_ptr, getIdx);                  // 새로운 인덱스위치 리스트에 추가한다.

			list<C_ClientInfo*>enterList;
			list<C_ClientInfo*>exitList;

			// 입, 퇴장한 섹터 리스트
			byte playerBit = 0;	// 새롭게 입장한 인접 섹터의 플레이어 비트
			playerBit = sector->GetMovedSectorPlayerList(_ptr->GetPlayerInfo()->GetIndex(), getIdx, enterList, exitList);

			// 1. 섹터 퇴장 알림 패킷 조립 및 전송
			protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::EXIT_SECTOR);
			PackPacket(buf, movedPos, packetSize);

			ListSendPacket(exitList, exceptClient, protocol, buf, packetSize);

			// 2. 섹터 입장 알림 패킷 조립 및 전송
			protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::ENTER_SECTOR);
			PackPacket(buf, movedPos, packetSize);

			ListSendPacket(enterList, exceptClient, protocol, buf, packetSize);

			// 3. 본인에게는 새롭게 입장한 인접 섹터의 플레이어 리스트를 활성화된 비트로 보내준다.
			protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::UPDATE_PLAYER);
			PackPacket(buf, playerBit, packetSize);

			_ptr->SendPacket(protocol, buf, packetSize);   // 전송

			_ptr->GetPlayerInfo()->SetIndex(getIdx);            // 변경된 새로운 인덱스 설정
		}
	
		// 정상 이동 결과 protocol에 패킹
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::INGAME_SUCCESS);
	}

	// 불법적인 인덱스라면 이전 인덱스의 섹터의 중간 위치로 강제 포지션 셋팅시킨다.
	else
	{
		exceptClient = nullptr;	// 유효하지 않은 인덱스면 모든 클라에게(자신포함) 위치 패킷을 보내야 한다.

		COORD_DOUBLE LT = sector->GetLeftTop(getIdx);
		COORD_DOUBLE RB = sector->GetRightBottom(getIdx);

		movedPos.posX = (float)(LT.x + RB.x) / 2;
		movedPos.posZ = (float)(LT.z + RB.z) / 2;

		// FORCE_MOVE(강제 이동) 결과 protocol에 패킹
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::FORCE_MOVE);
		
		LogManager::GetInstance()->HackerFileWrite("[TeleportHack]ID:%s, NICK:%s\n", _ptr->GetUserInfo()->id, _ptr->GetUserInfo()->nickname);
		/// 이후에 여기서 Kick하던지 하는게 필요
	}

	// 3. 섹터 내 이동 패킷 전송
	PackPacket(buf, movedPos, packetSize);
	ListSendPacket(sendList, exceptClient, protocol, buf, packetSize);

	// 마지막으로, 플레이어의 위치 정보를 저장해둔다.
	PositionPacket* position = new PositionPacket(movedPos);
	_ptr->GetPlayerInfo()->SetPosition(position);

	return true;   // 걍 다 true여
}

bool InGameManager::GetPosProcess(C_ClientInfo* _ptr, char* _buf)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize;

	// 플레이어 num을 가져온다.
	PositionPacket pos;
	int playerNum;
	UnPackPacket(_buf, playerNum);

	// 프로토콜 세팅
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, RESULT_INGAME::GET_OTHERPLAYER_POS);

	// 반복자로 돌리면서 playerNum이 일치하는 플레이어를 찾으면 그 플레이어의 위치를 전송해준다.
	list<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayerList();	// 리스트를 얻어옴
	C_ClientInfo* player = nullptr;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;
		if (player->GetPlayerInfo()->GetPlayerNum() == playerNum)
		{
			memcpy(&pos, player->GetPlayerInfo()->GetPosition(), sizeof(PositionPacket));
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
	char buf[BUFSIZE];
	int packetSize;

	// 프로토콜 세팅
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::FOCUS_PROTOCOL, RESULT_INGAME::NODATA);

	// 본인에게 다른 모든 플레이어의 인게임 정보를 보내준다(일단은 positionPacket만)
	list<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayerList();	// 리스트 얻어옴
	PositionPacket pos;
	C_ClientInfo* player = nullptr;
	for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
	{
		player = *iter;

		// 본인 제외
		if (player == _ptr)
			continue;

		// 다른 플레이어 정보를 자신에게 전송
		else
		{
			memcpy(&pos, player->GetPlayerInfo()->GetPosition(), sizeof(PositionPacket));
			PackPacket(buf, pos, packetSize);
			_ptr->SendPacket(protocol, buf, packetSize);
		}
	}

	return true;
}

bool InGameManager::LeaveProcess(C_ClientInfo* _ptr, int _playerNum)
{
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize;

	// 끊김 프로토콜 세팅
	protocol = SetProtocol(
		STATE_PROTOCOL::INGAME_STATE, 
		PROTOCOL_INGAME::DISCONNECT_PROTOCOL, 
		RESULT_INGAME::INGAME_SUCCESS);

	// 패킹
	PackPacket(buf, _playerNum, packetSize);

	// 자신을 제외한 다른 클라들에게 자신이 나갔음을 알린다.
	list<C_ClientInfo*> playerList = _ptr->GetRoom()->GetPlayerList();	// 리스트 얻어옴
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


//////// public
bool InGameManager::CanISelectWeapon(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);

	if (protocol == WEAPON_PROTOCOL)
		return WeaponSelectProcess(_ptr, buf);

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

bool InGameManager::CanIMove(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);
	RESULT_INGAME result;
	GetResult(buf, result);

	// 이동 프로토콜 들어왔을 시 이동 프로세스 수행
	if (protocol == MOVE_PROTOCOL)
	{
		switch (result)
		{
			// 다른 사람 위치 요청 일시
		case GET_OTHERPLAYER_POS:
			return GetPosProcess(_ptr, buf);

			// 그냥 이동일 시
		default:
			return MoveProcess(_ptr, buf);
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

// 무기 선택 타이머 쓰레드
unsigned long __stdcall InGameManager::WeaponSelectTimerThread(void* _arg)
{
	C_ClientInfo* ptr = (C_ClientInfo*)_arg;

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
			// 쓰레드 핸들 반납
			CloseHandle(ptr->GetRoom()->GetWeaponTimerHandle());
			ptr->GetRoom()->SetWeaponTimerHandle(nullptr);

			// 무기 정보를 얻어오기위한 프로토콜 조립
			protocol = InGameManager::GetInstance()->SetProtocol(INGAME_STATE, PROTOCOL_INGAME::WEAPON_PROTOCOL, RESULT_INGAME::NODATA);
			packetSize = 0;

			// 같은 방에 있는 "모든" 플레이어에게 무기를 보내라고 프로토콜을 전송함.
			list<C_ClientInfo*> playerList = ptr->GetRoom()->GetPlayerList();	// 리스트 얻어옴
			C_ClientInfo* player = nullptr;
			for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
			{
				player = *iter;
				player->SendPacket(protocol, buf, packetSize);
			}

			break;
		}

		// 1초마다 값이 변하면
		if (sec < (int)duringTime)
		{
			sec = (int)duringTime;	// 새롭게 초 단위를 갱신시켜준다.(before sec의 역할)

			// 1초에 한 번씩 시간을 알려주는 프로토콜을 보냄.
			protocol = InGameManager::GetInstance()->SetProtocol(INGAME_STATE, PROTOCOL_INGAME::TIMER_PROTOCOL, RESULT_INGAME::NODATA);
			packetSize = 0;

			// 무기 선택종료까지 남은 시간 패킷 세팅
			InGameManager::GetInstance()->PackPacket(buf, (WEAPON_SELTIME - sec), packetSize);

			// 같은 방에 있는 "모든" 플레이어에게 현재 무기 선택종료까지 남은 시간을 보내줌
			list<C_ClientInfo*> playerList = ptr->GetRoom()->GetPlayerList();	// 리스트 얻어옴
			C_ClientInfo* player = nullptr;
			for (auto iter = playerList.begin(); iter != playerList.end(); ++iter)
			{
				player = *iter;
				player->SendPacket(protocol, buf, packetSize);
			}
		}

		Sleep(50);	// 꼭 넣어줘야함 아니면 혼자 CPU 다 잡아먹음
	}

	return 0;	// 그리고 쓰레드 종료
}
