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
	memset(&clientAddr, 0, sizeof(clientAddr));	// 초기화 필수
	int addrLen = 0;							// 초기화 필수

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
	keep-alive 속성을 추가한다. mstcpip.h를 추가해야하며, Windows2000 이상의 OS에서만 동작한다.
	SIO_KEEPALIVE_VALS를 사용하는데, 이 속성은 시스템 레지스트리를 수정하지 않는다.
	*/
	DWORD dwRet;
	tcp_keepalive tcpkl;
	tcpkl.onoff             = 1;					// keep-alive를 켠다.
	tcpkl.keepalivetime     = KEEPALIVE_TIME;		// 1초마다 keep-alive 신호를 주고받는다
	tcpkl.keepaliveinterval = KEEPALIVE_INTERVAL;	// 위 신호를 보내고 응답이 없으면 1초마다 재전송하겠다(mstcp는 10회를 재시도 함)
	
	// 위에서 설정한 속성을 설정한다.
	WSAIoctl(
		clientSock,
		SIO_KEEPALIVE_VALS,
		&tcpkl,
		sizeof(tcp_keepalive),
		0, 0,
		&dwRet, NULL, NULL);

	// accpet때 받은 정보대로 클라 정보를 추가하고 받아옴
	if (SessionManager::GetInstance()->AddSession(clientSock, clientAddr) == true)
	{
		// 콘솔에 출력
		printf("\n[TCP 서버] 클라이언트 접속: IP 주소=%s, 포트 번호=%d\n",
			inet_ntoa(clientAddr.sin_addr), ntohs(clientAddr.sin_port));

		// 접속 로그에 기록해줌
		LogManager::GetInstance()->ConnectFileWrite("[TCP 서버] 클라이언트 접속: IP 주소=%s, 포트 번호=%d\n",
			inet_ntoa(clientAddr.sin_addr), ntohs(clientAddr.sin_port));

		printf("현재 클라이언트 수 : %d\n", SessionManager::GetInstance()->GetSize());
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

	if (retval == SOCKET_ERROR) //강제연결종료요청인 경우
		return ERROR_DISCONNECTED;

	else if (retval == 0)
		return DISCONNECTED;

	else
	{
		recvData.compRecvBytes += retval;
		if (recvData.compRecvBytes == recvData.recvBytes)
		{
			recvData.sizeBytes = recvData.recvBytes;   // 받은 사이즈 저장
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
	// 우선적으로 사이즈를 받는다.
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
	IC_CS cs;	// 동기화!

	if (sendQueue == nullptr)
	{
		return false;
	}

	int retval;
	DWORD sendbytes;
	DWORD flags = 0;

	ZeroMemory(&sOverlapped.overlapped, sizeof(sOverlapped.overlapped));

	// 큐 제일 앞에 저장된 데이터 불러옴
	S_SendBuf* ptr = sendQueue->front();

	// 이제 그 내용을 버퍼, 길이에 채워줌
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
			printf("WSASend()에서 IOCP_Disconnected() 호출함\n");
			return false;
		}
	}

	return true; 
}
int C_Socket::CompleteSend(int _completebyte)
{
	IC_CS cs;	// 동기화!

	// que의 front 해서 (갖고온 놈의)compSendBytes == sendBytes라면 pop해준다.
	// 여기에서 pop을 한다.

	if (sendQueue == nullptr)
	{
		return SOC_ERROR;
	}

	S_SendBuf* sendData = nullptr;
	sendData = sendQueue->front();

	sendData->compSendBytes += _completebyte;

	// 모두 다 보냈을 경우.
	if (sendData->compSendBytes == sendData->sendBytes)
	{
		sendQueue->pop();	// queue를 pop해준다.
		delete sendData;	// 지워주고

		// pop 했는데 큐에 남아있는 데이터가 있다면 그 데이터를 전송한다.
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