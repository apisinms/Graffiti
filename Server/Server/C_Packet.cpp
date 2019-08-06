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

	// �����͸� ��¥�� �� �޾ƿ´�(�������� ����)
	C_Encrypt::GetInstance()->Decrypt(buf, _getBuf, recvData.sizeBytes);

	// �������� ����
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

	// ��ȣȭ ����/
	encPtr += sizeof(size); // size ���� ���� �̸� Ȯ��
	C_Encrypt::GetInstance()->Encrypt(originalBuf, encPtr, size);

	// ����� �־���
	encPtr = encryptBuf;	// �ٽ� �� ó�� ��ġ�� ���ƿͼ�
	memcpy(encPtr, &size, sizeof(size));	// size�� ũ�⸦ �� �տ� �ִ´�.
	size += sizeof(size);	// �� �������ϴ� ����Ʈ ���� ����

	// (�񵿱�)����������, ��ȣȭ�� encryptBuf�� ť�� �ִ´�.
	S_SendBuf* ptr = new S_SendBuf();
	ptr->sendBytes = size;
	memcpy(ptr->sendBuf, encryptBuf, size);

	sendQueue.push(ptr);	// ť�� �־��ֵ��� �Ѵ�.

	// (����)���������� ��ȣȭ�� encryptBuf�� sendBuf�� �����Ѵ�.
	// memcpy(sendBuf, encryptBuf, size);
	// this->sendBytes = size;	// ������ �� ����Ʈ �� ����
}

// �ϼ��� ��Ŷ�� ���� �Լ� SetPacket�� WSA_Send �߰��� Que ����� Ȯ���ϴ� �۾�** �� ���� ���ش�.
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

	// �����͸� ��¥�� �� �޾ƿ´�(�������� ����)
	C_Encrypt::GetInstance()->Decrypt(buf, _getBuf, recvData.sizeBytes);

	// �������� ����
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

	// ��ȣȭ ����/
	encPtr += sizeof(size); // size ���� ���� �̸� Ȯ��
	C_Encrypt::GetInstance()->Encrypt(originalBuf, encPtr, size);

	// ����� �־���
	encPtr = encryptBuf;	// �ٽ� �� ó�� ��ġ�� ���ƿͼ�
	memcpy(encPtr, &size, sizeof(size));	// size�� ũ�⸦ �� �տ� �ִ´�.
	size += sizeof(size);	// �� �������ϴ� ����Ʈ ���� ����

	// (�񵿱�)����������, ��ȣȭ�� encryptBuf�� ť�� �ִ´�.
	S_SendBuf* ptr = new S_SendBuf();
	ptr->sendBytes = size;
	memcpy(ptr->sendBuf, encryptBuf, size);

	sendQueue.push(ptr);	// ť�� �־��ֵ��� �Ѵ�.

	// (����)���������� ��ȣȭ�� encryptBuf�� sendBuf�� �����Ѵ�.
	// memcpy(sendBuf, encryptBuf, size);
	// this->sendBytes = size;	// ������ �� ����Ʈ �� ����
}

// �ϼ��� ��Ŷ�� ���� �Լ� SetPacket�� WSA_Send �߰��� Que ����� Ȯ���ϴ� �۾�** �� ���� ���ش�.
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



