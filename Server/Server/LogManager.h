#pragma once
#include "C_Global.h"

#define ERR_FILE_NAME			"errLog.log"
#define CONNECT_FILE_NAME		"connectLog.log"
#define HACKED_USER_FILE_NAME	"hackedLog.log"

struct WSAOVERLAPPED_EX;

class LogManager
{
private:
	LogManager() {}
	~LogManager() {}
	static LogManager* instance;
	static FILE* errPtr;		// ���� �α׿�
	static FILE* connectPtr;	// ���� �α׿�
	static FILE* hackPtr;		// ��ŷ ���� �α׿�

public:
	static LogManager* GetInstance();
	static void Destroy();

public:
	void Init();
	void End();

	void ErrQuitMsgBox(const char* msg, ...);		// ���� �޽����ڽ� ���� ����(�α׿� �����)
	void ErrorPrintf(const char* fmt, ...);			// ���� ������ �ֿܼ� ���
	void ErrorFileWrite(const char* fmt, ...);		// �����α׸� ���Ͽ� ���

	void ConnectFileWrite(const char* _fmt, ...);	// ���ӷα׿� ������ ���

	void HackerFileWrite(const char* _fmt, ...);	// ���� ����ϴ� ������ ���� �α׸� ���
	
	void WSAOverlappedResultPrintf(const char* _msg, WSAOVERLAPPED_EX* _overlapped);
};