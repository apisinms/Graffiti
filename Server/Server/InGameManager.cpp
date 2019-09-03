#include "stdafx.h"
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

void InGameManager::PackPacket(char* _setptr, TCHAR* _str1, int& _size)
{
	char* ptr = _setptr;
	int strsize1 = (int)_tcslen(_str1) * sizeof(TCHAR);
	_size = 0;

	// 문자열 길이
	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	// 문자열(유니코드)
	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	_size = _size + strsize1;
}

void InGameManager::UnPackPacket(char* _getBuf, PositionPacket& _struct)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// 구조체 받음
	memcpy(&_struct, ptr, sizeof(PositionPacket));
	ptr = ptr + sizeof(PositionPacket);
}

void InGameManager::PackPacket(char* _setptr, PositionPacket& _struct, unsigned char _playerBit, int& _size)
{
	char* ptr = _setptr;
	_size = 0;

	// 포지션 패킷
	memcpy(ptr, &_struct, sizeof(PositionPacket));
	ptr = ptr + sizeof(PositionPacket);
	_size = _size + sizeof(PositionPacket);

	// 플레이어 비트
	memcpy(ptr, &_playerBit, sizeof(byte));
	ptr = ptr + sizeof(byte);
	_size = _size + sizeof(byte);
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
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize;

	RESULT_INGAME itemSelect = RESULT_INGAME::INGAME_FAIL;

	Weapon* tmpWeapon = new Weapon();
	UnPackPacket(_buf, tmpWeapon);
	

	// 클라가 보낸 무기정보 저장!
	if (tmpWeapon != nullptr)
	{
		_ptr->SetWeapon(tmpWeapon);
		itemSelect = RESULT_INGAME::INGAME_SUCCESS;

		wprintf(L"%s 선택 무기 : %d, %d\n", _ptr->GetUserInfo()->id, _ptr->GetWeapon()->mainW, _ptr->GetWeapon()->subW);
	}

	// 프로토콜 세팅(인게임 상태로)
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::START_PROTOCOL, itemSelect);

	ZeroMemory(buf, sizeof(BUFSIZE));

	// 패킹 및 전송
	PackPacket(buf, msg, packetSize);	/// msg없는데 굳이 보낼필요없음
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
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize;

	RESULT_INGAME result = RESULT_INGAME::INGAME_SUCCESS;

	PositionPacket tmpPos;
	UnPackPacket(_buf, tmpPos);

	// 플레이어의 위치 정보를 저장해둔다.
	PositionPacket* position = new PositionPacket(tmpPos);
	_ptr->SetPosition(position);

	// 디버깅용 출력
	printf("[초기인덱스]%d ,%f, %f, %f, %d\n", position->playerNum, position->posX, position->posZ, position->rotY, position->action);

	C_Sector* sector = _ptr->GetRoom()->GetSector();	// 이 방의 섹터 매니저를 얻는다.

	// 플레이어 위치를 토대로 몇행 몇열인지 인덱스를 구해서 해당 섹터에 추가한다.
	INDEX index = sector->GetIndex(tmpPos.posX, tmpPos.posZ);
	_ptr->SetIndex(index);
	sector->Add(_ptr, index);

	if (result == RESULT_INGAME::INGAME_SUCCESS)
		return true;

	return false;
}

bool InGameManager::MoveProcess(C_ClientInfo* _ptr, char* _buf)
{
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize;

	RESULT_INGAME move = RESULT_INGAME::INGAME_SUCCESS;

	// 전달된 위치 정보를 얻음
	PositionPacket tmpPos;
	UnPackPacket(_buf, tmpPos);

	// 플레이어의 위치 정보를 저장해둔다.
	PositionPacket* position = new PositionPacket(tmpPos);
	_ptr->SetPosition(position);


	// 디버깅용 출력
	printf("%d ,%f, %f, %f, %d\n", position->playerNum, position->posX, position->posZ, position->rotY, position->action);
	

	// 이 방의 섹터 매니저를 얻는다.
	C_Sector* sector = _ptr->GetRoom()->GetSector();	

	// 1. 플레이어 위치를 토대로 몇행 몇열인지 인덱스를 구한다.
	INDEX index = sector->GetIndex(tmpPos.posX, tmpPos.posZ);
	
	// 원래 갖고 있던 인덱스 정보와 현재 이동한 인덱스 정보가 다르다면 원래 있던 리스트에서 빼고, 지금 들어온 리스트에 추가해준다.
	if (index != _ptr->GetIndex())
	{
		sector->Delete(_ptr, _ptr->GetIndex());			// 기존에 있던 인덱스에서는 빼고
		sector->Add(_ptr, index);						// 새로운 인덱스위치 리스트에 추가한다.
		_ptr->SetIndex(index);							// 인덱스 설정

		/// 위치를 이동했으니 기존의 인접 섹터들에게 내가 이동했다고 알려줘야하고,
		/// 위치를 이동했으니 새롭게 만난 인접 섹터들에게 내가 왔다고 알려줘야함.. 어휴 시발 언제하냐
	}

	// 2. 구해진 인덱스로 해당 섹터와 인접 섹터의 플레이어 리스트를 병합하여 얻고, 인접 섹터에 있는 플레이어의 bit도 얻는다.
	byte playerBit = 0;	// 플레이어 비트
	list<C_ClientInfo*>sendList = sector->GetMergedPlayerList(index, playerBit);

	// 가져온 sendList가 비어있지 않은 경우에만
	if (!sendList.empty())
	{
		// 3. 프로토콜 세팅 및 패킹
		protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::MOVE_PROTOCOL, move);
		ZeroMemory(buf, sizeof(BUFSIZE));
		PackPacket(buf, tmpPos, playerBit, packetSize);

		// 4. 본인을 제외한 다른 플레이어들에게 위치를 전송한다.
		C_ClientInfo* player = nullptr;
		for (list<C_ClientInfo*>::iterator iter = sendList.begin(); iter != sendList.end(); ++iter)
		{
			player = *iter;

			// 자기는 제외
			if (_ptr == player)
				continue;

			player->SendPacket(protocol, buf, packetSize);
		}
	}

	if (move == RESULT_INGAME::INGAME_SUCCESS)
		return true;

	return false;
}

bool InGameManager::LeaveProcess(C_ClientInfo* _ptr, int _playerNum)
{
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_INGAME protocol;
	char buf[BUFSIZE];
	int packetSize;

	// 끊김 프로토콜 세팅
	protocol = SetProtocol(
		STATE_PROTOCOL::INGAME_STATE, 
		PROTOCOL_INGAME::DISCONNECT_PROTOCOL, 
		RESULT_INGAME::INGAME_SUCCESS);

	ZeroMemory(buf, sizeof(BUFSIZE));

	// 패킹
	PackPacket(buf, _playerNum, packetSize);

	// 자신을 제외한 다른 클라들에게 자신이 나갔음을 알린다.
	C_ClientInfo* player = nullptr;
	while (_ptr->GetRoom()->GetPlayer(player) == true)
	{
		// 자기는 제외
		if (_ptr == player)
			continue;

		player->SendPacket(protocol, buf, packetSize);
	}

	return true;
}

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

	// 이동 프로토콜 들어왔을 시 이동 프로세스 수행
	if (protocol == MOVE_PROTOCOL)
		return MoveProcess(_ptr, buf);

	return false;
}


// 무기 선택 타이머 쓰레드
unsigned long __stdcall InGameManager::TimerThread(void* _arg)
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
			ZeroMemory(buf, BUFSIZE);
			packetSize = 0;

			// 같은 방에 있는 "모든" 플레이어에게 무기를 보내라고 프로토콜을 전송함.
			C_ClientInfo* player = nullptr;
			while (ptr->GetRoom()->GetPlayer(player) == true)
				player->SendPacket(protocol, buf, packetSize);

			break;
		}

		// 1초마다 값이 변하면
		if (sec < (int)duringTime)
		{
			sec = (int)duringTime;	// 새롭게 초 단위를 갱신시켜준다.(before sec의 역할)

			// 1초에 한 번씩 시간을 알려주는 프로토콜을 보냄.
			protocol = InGameManager::GetInstance()->SetProtocol(INGAME_STATE, PROTOCOL_INGAME::TIMER_PROTOCOL, RESULT_INGAME::NODATA);
			ZeroMemory(buf, BUFSIZE);
			packetSize = 0;

			// 무기 선택종료까지 남은 시간 패킷 세팅
			InGameManager::GetInstance()->PackPacket(buf, (WEAPON_SELTIME - sec), packetSize);

			// 같은 방에 있는 "모든" 플레이어에게 현재 무기 선택종료까지 남은 시간을 보내줌
			C_ClientInfo* player = nullptr;
			while (ptr->GetRoom()->GetPlayer(player) == true)
				player->SendPacket(protocol, buf, packetSize);
		}
	}

	return 0;	// 그리고 쓰레드 종료
}
