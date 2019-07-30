#include "C_Packet.h"
#include "C_Encrypt.h"
#include "C_Global.h"
#include "LogManager.h"

#ifdef __64BIT__
void C_Packet::GetPacket(__int64& _protocol, char* _getBuf)
{
	char* recvPtr = recvData.recvBuf;
	char buf[BUFSIZE];
	char* bufPtr = buf;

	ZeroMemory(buf, BUFSIZE);

	memcpy(bufPtr, recvPtr, sizeof(_protocol));
	recvPtr += sizeof(_protocol);
	bufPtr += sizeof(_protocol);

	memcpy(bufPtr, recvPtr, recvData.sizeBytes - sizeof(_protocol));
	recvPtr += (recvData.sizeBytes - sizeof(_protocol));
	bufPtr += (recvData.sizeBytes - sizeof(_protocol));

	// 데이터를 통짜로 다 받아온다(프로토콜 포함)
	C_Encrypt::GetInstance()->Decrypt(buf, _getBuf, recvData.sizeBytes);

	// 프로토콜 저장
	memcpy(&_protocol, _getBuf, sizeof(_protocol));
}
void C_Packet::SetPacket(__int64& _protocol, char* _setBuf, int _packetSize)
{
	int size = 0;
	char originalBuf[BUFSIZE];
	char* originalPtr = originalBuf;
	char encryptBuf[BUFSIZE];
	char* encPtr = encryptBuf;

	ZeroMemory(originalBuf, BUFSIZE);
	ZeroMemory(encryptBuf, BUFSIZE);

	memcpy(originalPtr, &_protocol, sizeof(_protocol));
	originalPtr += sizeof(_protocol);
	size += sizeof(_protocol);

	memcpy(originalPtr, _setBuf, _packetSize);
	originalPtr += _packetSize;
	size += _packetSize;

	// 암호화 진행/
	encPtr += sizeof(size); // size 넣을 공간 미리 확보
	C_Encrypt::GetInstance()->Encrypt(originalBuf, encPtr, size);

	// 사이즈를 넣어줌
	encPtr = encryptBuf;	// 다시 맨 처음 위치로 돌아와서
	memcpy(encPtr, &size, sizeof(size));	// size의 크기를 맨 앞에 넣는다.
	size += sizeof(size);	// 총 보내야하는 바이트 수를 갱신

	// (비동기)마지막으로, 암호화된 encryptBuf를 큐에 넣는다.
	S_SendBuf* ptr = new S_SendBuf();
	ptr->sendBytes = size;
	memcpy(ptr->sendBuf, encryptBuf, size);

	sendQueue.push(ptr);	// 큐에 넣어주도록 한다.

	// (동기)마지막으로 암호화된 encryptBuf를 sendBuf에 저장한다.
	// memcpy(sendBuf, encryptBuf, size);
	// this->sendBytes = size;	// 보내야 될 바이트 수 지정
}

// 완성된 패킷을 보낼 함수 SetPacket과 WSA_Send 중간에 Que 사이즈를 확인하는 작업** 도 같이 해준다.
bool C_Packet::SendPacket(__int64 _protocol, char* _buf, int _size)
{
	SetPacket(_protocol, _buf, _size);

	if (sendQueue.size() == 1)
	{
		// first Send
		if (!WSA_Send(nullptr))
		{
			return false;
		}
	}

	return true;
}
#endif

#ifdef __32BIT__
void C_Packet::GetPacket(int& _protocol, char* _getBuf)
{
	char* recvPtr = recvData.recvBuf;
	char buf[BUFSIZE];
	char* bufPtr = buf;

	ZeroMemory(buf, BUFSIZE);

	memcpy(bufPtr, recvPtr, sizeof(_protocol));
	recvPtr += sizeof(_protocol);
	bufPtr += sizeof(_protocol);

	memcpy(bufPtr, recvPtr, recvData.sizeBytes - sizeof(_protocol));
	recvPtr += (recvData.sizeBytes - sizeof(_protocol));
	bufPtr += (recvData.sizeBytes - sizeof(_protocol));

	// 데이터를 통짜로 다 받아온다(프로토콜 포함)
	C_Encrypt::GetInstance()->Decrypt(buf, _getBuf, recvData.sizeBytes);

	// 프로토콜 저장
	memcpy(&_protocol, _getBuf, sizeof(_protocol));
}
void C_Packet::SetPacket(int& _protocol, char* _setBuf, int _packetSize)
{
	int size = 0;
	char originalBuf[BUFSIZE];
	char* originalPtr = originalBuf;
	char encryptBuf[BUFSIZE];
	char* encPtr = encryptBuf;

	ZeroMemory(originalBuf, BUFSIZE);
	ZeroMemory(encryptBuf, BUFSIZE);

	memcpy(originalPtr, &_protocol, sizeof(_protocol));
	originalPtr += sizeof(_protocol);
	size += sizeof(_protocol);

	memcpy(originalPtr, _setBuf, _packetSize);
	originalPtr += _packetSize;
	size += _packetSize;

	// 암호화 진행/
	encPtr += sizeof(size); // size 넣을 공간 미리 확보
	C_Encrypt::GetInstance()->Encrypt(originalBuf, encPtr, size);

	// 사이즈를 넣어줌
	encPtr = encryptBuf;	// 다시 맨 처음 위치로 돌아와서
	memcpy(encPtr, &size, sizeof(size));	// size의 크기를 맨 앞에 넣는다.
	size += sizeof(size);	// 총 보내야하는 바이트 수를 갱신

	// (비동기)마지막으로, 암호화된 encryptBuf를 큐에 넣는다.
	S_SendBuf* ptr = new S_SendBuf();
	ptr->sendBytes = size;
	memcpy(ptr->sendBuf, encryptBuf, size);

	sendQueue.push(ptr);	// 큐에 넣어주도록 한다.

	// (동기)마지막으로 암호화된 encryptBuf를 sendBuf에 저장한다.
	// memcpy(sendBuf, encryptBuf, size);
	// this->sendBytes = size;	// 보내야 될 바이트 수 지정
}

// 완성된 패킷을 보낼 함수 SetPacket과 WSA_Send 중간에 Que 사이즈를 확인하는 작업** 도 같이 해준다.
bool C_Packet::SendPacket(int _protocol, char* _buf, int _size)
{
	SetPacket(_protocol, _buf, _size);

	if (sendQueue.size() == 1)
	{
		// first Send
		if (!WSA_Send(nullptr))
		{
			return false;
		}
	}

	return true;
}
#endif



