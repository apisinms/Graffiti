#pragma once
#include <queue>
#include "C_SyncCS.h"
#include "C_Global.h"

struct WSAOVERLAPPED_EX
{
	WSAOVERLAPPED overlapped;
	void*  ptr;
	IO_TYPE  type;
};

class C_Socket : public C_SyncCS<C_Socket>
{
protected:
	queue<S_SendBuf*> sendQueue;	// Send�� �޽��� ť�̴�.
	SOCKET sock;
	SOCKADDR_IN addr;

	S_SendBuf sendData;	// send�� �����͸� ��� ����ü
	S_RecvBuf recvData;	// recv�� �����͸� ��� ����ü

	WSAOVERLAPPED_EX rOverlapped; // recv
	WSAOVERLAPPED_EX sOverlapped; // send

	WSABUF rWsabuf;
	WSABUF sWsabuf;

public:
	C_Socket() {}
	~C_Socket() {}
	C_Socket(SOCKET _sock, SOCKADDR_IN _addr) : sock(_sock), addr(_addr) {}

	void SetSocket(SOCKET _sock);
	SOCKET GetSocket();

	void SetAddress(SOCKADDR_IN _addr);
	SOCKADDR_IN GetAddress();

	bool Socket(int _af, int _type, int _protocol);
	bool Bind(int _family, unsigned long _addr, unsigned short _port);
	bool Listen(int _backlog);
	bool Connect();
	SOCKET Accept();
	void CloseSocket();

	int PacketRecv();
	int Recv(char* _buf, int _len, int _flags);
	int MessageRecv();
	int Send(char *_buf, int _len, int _flags);
	int MessageSend();

	bool WSA_Recv(LPWSAOVERLAPPED_COMPLETION_ROUTINE _routine);
	int CompleteRecv(int _completebyte);
	bool WSA_Send(LPWSAOVERLAPPED_COMPLETION_ROUTINE _routine);
	int CompleteSend(int _completebyte);




	C_Socket* GetMySocket() { return this; }
	//void SetMySocket(C_Socket* _sock) { sock = _sock->GetSocket(); addr = _sock->GetAddress(); }
	//int GetSendBytes() { return sendBytes; }
};
