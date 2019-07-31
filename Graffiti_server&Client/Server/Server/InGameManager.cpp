#include "InGameManager.h"
#include "LogManager.h"
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

void InGameManager::PackPacket(char* _setptr, TCHAR* _str1, int& _size)
{
	char* ptr = _setptr;
	int strsize1 = _tcslen(_str1) * sizeof(TCHAR);
	_size = 0;

	// ���ڿ� ����
	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	// ���ڿ�(�����ڵ�)
	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	_size = _size + strsize1;
}

void InGameManager::UnPackPacket(char* _getBuf, Weapon& _struct)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// ����ü ����
	memcpy(&_struct, ptr, sizeof(Weapon));
	ptr = ptr + sizeof(Weapon);

}

void InGameManager::UnPackPacket(char* _getBuf, int& _num1, int& _num2)
{
	char* ptr = _getBuf + sizeof(PROTOCOL_INGAME);

	// ���� 1����
	memcpy(&_num1, ptr, sizeof(int));
	ptr = ptr + sizeof(int);

	memcpy(&_num2, ptr, sizeof(int));
	ptr = ptr + sizeof(int);
}

void InGameManager::GetProtocol(PROTOCOL_INGAME& _protocol)
{
	// major state�� ������(Ŭ��� state�� �Ⱥ����ϱ�(Ȥ�ó� ���Ŀ� �����ԵǸ� �̺κ��� ����)) protocol�� �������� ���ؼ� ���� 10��Ʈ ��ġ�� ����ũ�� ����
#ifdef __64BIT__
	__int64 mask = ((__int64)0x1f << (64 - 10));
#endif

#ifdef __32BIT__
	int mask = ((int)0x1f << (32 - 10));
#endif

	// ����ũ�� �ɷ��� 1���� ���������� ����ȴ�. 
	PROTOCOL_INGAME protocol = (PROTOCOL_INGAME)(_protocol & (PROTOCOL_INGAME)mask);

	// �ƿ�ǲ�� �����̹Ƿ� �������ش�.
	// ���߿� �ѹ��� �������ִ� ������ ���߿� �߰��� ���� �� �ִ� result �� ���ؼ� protocol �� ������� ���� 
	_protocol = protocol;
}
InGameManager::PROTOCOL_INGAME InGameManager::SetProtocol(STATE_PROTOCOL _state, PROTOCOL_INGAME _protocol, RESULT_INGAME _result)
{
	// �ϼ��� ���������� ���� 
	PROTOCOL_INGAME protocol = (PROTOCOL_INGAME)0;
	protocol = (PROTOCOL_INGAME)(_state | _protocol | _result);
	return protocol;
}

InGameManager::PROTOCOL_INGAME InGameManager::GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf)
{
#ifdef __64BIT__
	__int64 bitProtocol = 0;
#endif

#ifdef __32BIT__
	int bitProtocol = 0;
#endif
	_ptr->GetPacket(bitProtocol, _buf);	// �켱 �ɷ��������� ���������� �����´�.

	// ��¥ ���������� ������ �ش�.(�ȿ��� �������� AND �˻�)
	PROTOCOL_INGAME realProtocol = (PROTOCOL_INGAME)bitProtocol;
	GetProtocol(realProtocol);

	return realProtocol;
}


bool InGameManager::ItemSelctProcess(C_ClientInfo* _ptr, char* _buf)
{
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_INGAME protocol;
	int mainW, subW;
	char buf[BUFSIZE];
	int packetSize;

	RESULT_INGAME itemSelect = RESULT_INGAME::INGAME_SUCCESS;

	UnPackPacket(_buf, weapon);
	//UnPackPacket(_buf, mainW, subW);

	// �������� ����
	protocol = SetProtocol(INGAME_STATE, PROTOCOL_INGAME::WEAPON_PROTOCOL, itemSelect);

	ZeroMemory(buf, sizeof(BUFSIZE));

	// ��ŷ �� ����
	PackPacket(buf, msg, packetSize);
	_ptr->SendPacket(protocol, buf, packetSize);


	if (itemSelect == RESULT_INGAME::INGAME_SUCCESS)
		return true;

	return false;
}

bool InGameManager::CanIItemSelect(C_ClientInfo* _ptr)
{

	//////////// 4���� ���⼱�� �� �ؾ� ������ send�� ��
	//////////// STATE ������ 2ĭ �и��� PROTOCOL �ڷ� 1ĭ �и�


	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_INGAME protocol = GetBufferAndProtocol(_ptr, buf);

	// �κ񿡼� Logout�� ��û�ߴٸ�, LoginList�� �����ϴ� LoginManager�� CanILogout()�� ȣ���ؼ� �˻�޾ƾ��Ѵ�.
	if (protocol == WEAPON_PROTOCOL)
		return ItemSelctProcess(_ptr, buf);

	return false;
}

unsigned long __stdcall InGameManager::TimerThread(void* _arg)
{
	C_ClientInfo* ptr = (C_ClientInfo*)_arg;
	
	LARGE_INTEGER frequency;
	LARGE_INTEGER beginTime;
	LARGE_INTEGER endTime;
	__int64 elapsed;
	double duringTime = 0.0;	// ������ ����� �ð���(�ʴ���) ������ ���� double ����

	QueryPerformanceFrequency(&frequency);	// ���� 1ȸ ���ļ� ����
	QueryPerformanceCounter(&beginTime);	// ���� �ð� ����
	while (1)
	{
		QueryPerformanceCounter(&endTime);// ���� �ð� ����
		elapsed = endTime.QuadPart - beginTime.QuadPart;	// ����� �ð� ���
		
		
		duringTime = (double)elapsed / (double)frequency.QuadPart;	// ������ �帥 �ð��� �� ������ ���

		// ���� ������ ���ýð�(���)�� �Ѿ��ٸ� ������ �ڵ� �ݳ� ��, ���ѷ����� ����������.
		if (duringTime >= itemSelTime)
		{
			CloseHandle(ptr->GetRoom()->timerHandle);
			ptr->GetRoom()->timerHandle = nullptr;


			/// ���⿡�� ����޶�� ���������� ������
			/// Ŭ�� �ڽ��� ������ ���⸦ ������ ������
			/// ������ �� ���� ������ �޾Ƽ� �ش� Ŭ�� ������ ������ѵд�.
			PROTOCOL_INGAME protocol = InGameManager::GetInstance()->SetProtocol(INGAME_STATE, PROTOCOL_INGAME::WEAPON_PROTOCOL, RESULT_INGAME::NODATA);
			char buf[BUFSIZE] = { 0, };
			int packetSize = 0;

			// ���� �濡 �ִ� ��� �÷��̾�� ���⸦ ������� ���������� ������.
			ptr->GetRoom()->team1->player1->SendPacket(protocol, buf, packetSize);
			ptr->GetRoom()->team1->player2->SendPacket(protocol, buf, packetSize);
			ptr->GetRoom()->team2->player1->SendPacket(protocol, buf, packetSize);
			ptr->GetRoom()->team2->player2->SendPacket(protocol, buf, packetSize);

			break;
		}
	}

	return 0;	// �׸��� ������ ����
}
