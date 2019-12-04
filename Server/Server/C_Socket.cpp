#include "stdafx.h"
#include "C_Socket.h"
#include "MainManager.h"

void C_Socket::SetSocket(SOCKET _sock)
{
	sock = _sock;
}
SOCKET C_Socket::GetSocket()
{
	return sock;
}
void C_Socket::SetAddress(SOCKADDR_IN _addr)
{
	addr = _addr;
}
SOCKADDR_IN C_Socket::GetAddress()
{
	return addr;
}

bool C_Socket::Socket(int _af, int _type, int _protocol)
{
	sock = socket(_af, _type, _protocol);
	if (sock == INVALID_SOCKET)
		LogManager::GetInstance()->ErrQuitMsgBox("Socket()");

	return true;
}
bool C_Socket::Bind(int _family, unsigned long _addr, unsigned short _port)
{
	SOCKADDR_IN serveraddr;
	ZeroMemory(&serveraddr, sizeof(serveraddr));
	serveraddr.sin_family = _family;
	serveraddr.sin_addr.s_addr = htonl(_addr);
	serveraddr.sin_port = htons(_port);
	int retval = bind(sock, (SOCKADDR *)&serveraddr, sizeof(serveraddr));
	if (retval == SOCKET_ERROR)
		LogManager::GetInstance()->ErrQuitMsgBox("Bind()");

	return true;
}
bool C_Socket::Listen(int _backlog)
{
	int retval = listen(sock, _backlog);
	if (retval == SOCKET_ERROR)
		LogManager::GetInstance()->ErrQuitMsgBox("Listen()");

	return true;
}
bool C_Socket::Connect()
{
	SOCKADDR_IN clientAddr;
	memset(&clientAddr, 0, sizeof(clientAddr));	// �ʱ�ȭ �ʼ�
	int addrLen = 0;							// �ʱ�ȭ �ʼ�

	// connect()
	addrLen = sizeof(clientAddr);
	int retval = connect(sock, (SOCKADDR *)&clientAddr, addrLen);
	if (retval == SOCKET_ERROR)
		LogManager::GetInstance()->ErrorPrintf("Connect()");

	return true;
}
SOCKET C_Socket::Accept()
{
	SOCKET clientSock;
	SOCKADDR_IN clientAddr;
	int addrLen;

	// accept()
	addrLen = sizeof(clientAddr);
	clientSock = accept(sock, (SOCKADDR*)& clientAddr, &addrLen);
	if (clientSock == INVALID_SOCKET)
	{
		LogManager::GetInstance()->ErrorPrintf("Accept()");
		return clientSock;
	}


	/*
	keep-alive �Ӽ��� �߰��Ѵ�. mstcpip.h�� �߰��ؾ��ϸ�, Windows2000 �̻��� OS������ �����Ѵ�.
	SIO_KEEPALIVE_VALS�� ����ϴµ�, �� �Ӽ��� �ý��� ������Ʈ���� �������� �ʴ´�.
	*/
	DWORD dwRet;
	tcp_keepalive tcpkl;
	tcpkl.onoff             = 1;					// keep-alive�� �Ҵ�.
	tcpkl.keepalivetime     = KEEPALIVE_TIME;		// 1�ʸ��� keep-alive ��ȣ�� �ְ�޴´�
	tcpkl.keepaliveinterval = KEEPALIVE_INTERVAL;	// �� ��ȣ�� ������ ������ ������ 1�ʸ��� �������ϰڴ�(mstcp�� 10ȸ�� ��õ� ��)
	
	// ������ ������ �Ӽ��� �����Ѵ�.
	WSAIoctl(
		clientSock,
		SIO_KEEPALIVE_VALS,
		&tcpkl,
		sizeof(tcp_keepalive),
		0, 0,
		&dwRet, NULL, NULL);

	// accpet�� ���� ������� Ŭ�� ������ �߰��ϰ� �޾ƿ�
	if (SessionManager::GetInstance()->AddSession(clientSock, clientAddr) == true)
	{
		// �ֿܼ� ���
		printf("\n[TCP ����] Ŭ���̾�Ʈ ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n",
			inet_ntoa(clientAddr.sin_addr), ntohs(clientAddr.sin_port));

		// ���� �α׿� �������
		LogManager::GetInstance()->ConnectFileWrite("[TCP ����] Ŭ���̾�Ʈ ����: IP �ּ�=%s, ��Ʈ ��ȣ=%d\n",
			inet_ntoa(clientAddr.sin_addr), ntohs(clientAddr.sin_port));

		printf("���� Ŭ���̾�Ʈ �� : %d\n", SessionManager::GetInstance()->GetSize());
	}

	return clientSock;
}
void C_Socket::CloseSocket()
{
	closesocket(sock);
}


int C_Socket::Recv(char* _buf, int _len, int _flags)
{
	return recv(GetSocket(), _buf, _len, _flags);
}
int C_Socket::MessageRecv()
{
	int retval = Recv((recvData.recvBuf + recvData.compRecvBytes), (recvData.recvBytes - recvData.compRecvBytes), 0);

	if (retval == SOCKET_ERROR) //�������������û�� ���
		return ERROR_DISCONNECTED;

	else if (retval == 0)
		return DISCONNECTED;

	else
	{
		recvData.compRecvBytes += retval;
		if (recvData.compRecvBytes == recvData.recvBytes)
		{
			recvData.sizeBytes = recvData.recvBytes;   // ���� ������ ����
			recvData.compRecvBytes = 0;
			recvData.recvBytes = 0;
			return SOC_TRUE;
		}
		return SOC_FALSE;
	}
}
int C_Socket::PacketRecv()
{
	int retval = 0;
	if (recvData.rSizeFlag == false)
	{
		recvData.recvBytes = sizeof(int);
		retval = MessageRecv();
		switch (retval)
		{
		case SOC_TRUE:
			memcpy(&recvData.recvBytes, recvData.recvBuf, sizeof(int));
			recvData.rSizeFlag = true;
			return SOC_FALSE;

		case SOC_FALSE:
			return SOC_FALSE;

		case ERROR_DISCONNECTED:
			LogManager::GetInstance()->ErrorPrintf("Recv error");
			return DISCONNECTED;

		case DISCONNECTED:
			return DISCONNECTED;
		}
	}

	retval = MessageRecv();
	switch (retval)
	{
	case SOC_TRUE:
		recvData.rSizeFlag = false;
		return SOC_TRUE;

	case SOC_FALSE:
		return SOC_FALSE;

	case ERROR_DISCONNECTED:
		LogManager::GetInstance()->ErrorPrintf("Recv error()");
		return DISCONNECTED;

	case DISCONNECTED:
		return DISCONNECTED;
	}

	return -1;
}
int C_Socket::Send(char* _buf, int _len, int _flags)
{
	return send(GetSocket(), _buf, _len, _flags);
}
int C_Socket::MessageSend()
{
	int retval = Send((sendData.sendBuf + sendData.compSendBytes), (sendData.sendBytes - sendData.compSendBytes), 0);

	if (retval == SOCKET_ERROR)
		return ERROR_DISCONNECTED;

	else if (retval == 0)
		DISCONNECTED;

	else
	{
		sendData.compSendBytes += retval;

		if (sendData.sendBytes == sendData.compSendBytes)
		{
			sendData.sendBytes = 0;
			sendData.compSendBytes = 0;

			return SOC_TRUE;
		}

		else
			return SOC_FALSE;
	}

	return -1;
}

bool C_Socket::WSA_Recv(LPWSAOVERLAPPED_COMPLETION_ROUTINE _routine)
{
	int retval;
	DWORD recvbytes;
	DWORD flags = 0;

	ZeroMemory(&rOverlapped.overlapped, sizeof(rOverlapped.overlapped));

	rWsabuf.buf = recvData.recvBuf + recvData.compRecvBytes;

	if (recvData.rSizeFlag)
	{
		rWsabuf.len = recvData.recvBytes - recvData.compRecvBytes;
	}
	else
	{
		rWsabuf.len = sizeof(int) - recvData.compRecvBytes;
	}

	retval = WSARecv(sock, &rWsabuf, 1, &recvbytes,
		&flags, &rOverlapped.overlapped, _routine);

	if (retval == SOCKET_ERROR)
	{
		if (WSAGetLastError() != WSA_IO_PENDING)
		{
			LogManager::GetInstance()->ErrorPrintf("WSARecv()");
			return false;
		}
	}

	return true;

}

int C_Socket::CompleteRecv(int _completebyte)
{
	// �켱������ ����� �޴´�.
	if (!recvData.rSizeFlag)
	{
		recvData.compRecvBytes += _completebyte;
		if (recvData.compRecvBytes == sizeof(int))
		{
			memcpy(&recvData.recvBytes, recvData.recvBuf, sizeof(int));
			recvData.compRecvBytes = 0;
			recvData.sizeBytes = recvData.recvBytes;
			recvData.rSizeFlag = true;
		}

		if (!WSA_Recv(nullptr))
		{
			return SOC_ERROR;
		}

		return SOC_FALSE;
	}

	recvData.compRecvBytes += _completebyte;

	if (recvData.compRecvBytes == recvData.recvBytes)
	{
		recvData.compRecvBytes = 0;
		recvData.recvBytes = 0;
		recvData.rSizeFlag = false;

		return SOC_TRUE;
	}

	return SOC_FALSE;

}
bool C_Socket::WSA_Send(LPWSAOVERLAPPED_COMPLETION_ROUTINE _routine)
{
	IC_CS cs;	// ����ȭ!

	if (sendQueue == nullptr)
	{
		return false;
	}

	int retval;
	DWORD sendbytes;
	DWORD flags = 0;

	ZeroMemory(&sOverlapped.overlapped, sizeof(sOverlapped.overlapped));

	// ť ���� �տ� ����� ������ �ҷ���
	S_SendBuf* ptr = sendQueue->front();

	// ���� �� ������ ����, ���̿� ä����
	sWsabuf.buf = ptr->sendBuf + ptr->compSendBytes;
	sWsabuf.len = ptr->sendBytes - ptr->compSendBytes;

	retval = WSASend(sock, &sWsabuf, 1, &sendbytes,
		flags, &sOverlapped.overlapped, _routine);

	if (retval == SOCKET_ERROR)
	{
		if (WSAGetLastError() != WSA_IO_PENDING)
		{
			LogManager::GetInstance()->ErrorPrintf("WSASend()");

			MainManager::GetInstance()->IOCP_Disconnected(sOverlapped.ptr);
			printf("WSASend()���� IOCP_Disconnected() ȣ����\n");
			return false;
		}
	}

	return true; 
}
int C_Socket::CompleteSend(int _completebyte)
{
	IC_CS cs;	// ����ȭ!

	// que�� front �ؼ� (����� ����)compSendBytes == sendBytes��� pop���ش�.
	// ���⿡�� pop�� �Ѵ�.

	if (sendQueue == nullptr)
	{
		return SOC_ERROR;
	}

	S_SendBuf* sendData = nullptr;
	sendData = sendQueue->front();

	sendData->compSendBytes += _completebyte;

	// ��� �� ������ ���.
	if (sendData->compSendBytes == sendData->sendBytes)
	{
		sendQueue->pop();	// queue�� pop���ش�.
		delete sendData;	// �����ְ�

		// pop �ߴµ� ť�� �����ִ� �����Ͱ� �ִٸ� �� �����͸� �����Ѵ�.
		if (sendQueue->empty() == false)
		{
			if (!WSA_Send(nullptr))
			{
				return SOC_ERROR;
			}
		}

		return SOC_TRUE;
	}

	if (!WSA_Send(nullptr))
	{
		return SOC_ERROR;
	}
	return SOC_FALSE;
}