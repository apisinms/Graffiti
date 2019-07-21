#pragma once
#include <stdio.h>
#include <time.h>
#include "C_Global.h"

#define ERR_FILE_NAME		"errLog.log"
#define CONNECT_FILE_NAME	"connectLog.log"

struct WSAOVERLAPPED_EX;

class LogManager
{
private:
	LogManager() {}
	~LogManager() {}
	static LogManager* instance;
	static FILE* errPtr;		// 에러 로그용
	static FILE* connectPtr;	// 접속 로그용

public:
	static LogManager* GetInstance();
	static void Destroy();

public:
	void Init();
	void End();

	void ErrQuitMsgBox(const char* msg, ...);		// 오류 메시지박스 띄우고 종료(로그에 기록함)
	void ErrorPrintf(const char* fmt, ...);			// 오류 내용을 콘솔에 출력
	void ErrorFileWrite(const char* fmt, ...);		// 오류로그를 파일에 기록

	void ConnectFileWrite(const char* _fmt, ...);	// 접속로그에 내용을 기록
	void WSAOverlappedResultPrintf(const char* _msg, WSAOVERLAPPED_EX* _overlapped);
};