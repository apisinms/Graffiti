#pragma once
#include <tchar.h>
using namespace std;
	
#define __64BIT__

#define BUFSIZE			4096
#define HALF_BUFSIZE	2048

#define SERVERPORT		9000

#define IDSIZE			255
#define PWSIZE			255
#define NICKNAMESIZE	255
#define MSGSIZE			512

#define THREAD_END		-777

enum POSITION : int
{
	LOGIN, LOBBY, MATCH, INGAME,
};

#ifdef __64BIT__
// 상위 5비트 스테이트를 표현해주는 프로토콜
enum STATE_PROTOCOL : __int64
{
	LOGIN_STATE  = ((__int64)0x1 << 63),
	LOBBY_STATE  = ((__int64)0x1 << 62),
	CHAT_STATE   = ((__int64)0x1 << 61),
	INGAME_STATE = ((__int64)0x1 << 60),
	//60
	//59
};
#endif

#ifdef __32BIT__
// 상위 5비트 스테이트를 표현해주는 프로토콜
enum STATE_PROTOCOL : int
{
	LOGIN_STATE  = ((int)0x1 << 31),
	LOBBY_STATE  = ((int)0x1 << 30),
	CHAT_STATE   = ((int)0x1 << 29),
	INGAME_STATE = ((int)0x1 << 28),
	//60
	//59
};
#endif


enum IO_TYPE
{
	IO_RECV = 1,
	IO_SEND,
	IO_ACCEPT
};

enum
{
	NONE = 1,
	SOC_ERROR,
	SOC_TRUE,
	SOC_FALSE,
	ERROR_DISCONNECTED,
	DISCONNECTED,
};

struct S_SendBuf
{
	char sendBuf[BUFSIZE];
	int sendBytes;
	int compSendBytes;
};

struct S_RecvBuf
{
	char recvBuf[BUFSIZE];
	int recvBytes;
	int compRecvBytes;
	
	bool rSizeFlag;
	int sizeBytes;
};

struct UserInfo
{
	TCHAR id[IDSIZE];
	TCHAR pw[PWSIZE];
	TCHAR nickname[NICKNAMESIZE];

	UserInfo() {}
	UserInfo(UserInfo &_info)
	{
		_tcscpy_s(id, IDSIZE, _info.id);
		_tcscpy_s(pw, PWSIZE, _info.pw);
		_tcscpy_s(nickname, NICKNAMESIZE, _info.nickname);
	}
};









//enum
//{
//	READ_SET = 1,
//	WRITE_SET,
//	EXCEPTION_SET
//};