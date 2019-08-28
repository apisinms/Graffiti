#include "stdafx.h"
#include "C_ClientInfo.h"

LogManager* LogManager::instance;	// 초기화
FILE* LogManager::errPtr = nullptr;
FILE* LogManager::connectPtr = nullptr;

LogManager* LogManager::GetInstance()
{
	// 인스턴스가 없다면 인스턴스를 생성하고 리턴한다.
	if (instance == nullptr)
	{
		instance = new LogManager();
		errPtr = fopen(ERR_FILE_NAME, "at");
		if (errPtr == nullptr)
		{
			instance->ErrorPrintf("fopen() errPtr");
			return nullptr;
		}

		connectPtr = fopen(CONNECT_FILE_NAME, "at");
		if (connectPtr == nullptr)
		{
			instance->ErrorPrintf("fopen() connectPtr");
			return nullptr;
		}
	}

	return instance;
}

void LogManager::Init() 
{
}

void LogManager::End()
{
	fclose(errPtr);
	fclose(connectPtr);
}

void LogManager::Destroy()
{
	delete instance;
}

void LogManager::ErrQuitMsgBox(const char* _fmt, ...)
{
	char totalBuf[BUFSIZE] = { 0, };

	// 오류메시지 얻어옴
	va_list ap;
	LPVOID lpMsgBuf;
	FormatMessageA(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPSTR)&lpMsgBuf, 0, NULL);

	// 인자 문자열 추가
	va_start(ap, _fmt);
	vsprintf(totalBuf + strlen(totalBuf), _fmt, ap);
	va_end(ap);

	// 오류 문자열 추가
	strcat(totalBuf, (const char*)lpMsgBuf);

	// 오류 메시지박스 띄움
	MessageBoxA(NULL, (LPCSTR)totalBuf, _fmt, MB_ICONERROR);

	// 로그에 기록
	ErrorFileWrite(totalBuf);

	LocalFree(lpMsgBuf);
	exit(1);
}
void LogManager::ErrorPrintf(const char* _fmt, ...)
{
	char totalBuf[BUFSIZE] = { 0, };
	char timeBuf[HALF_BUFSIZE] = { 0, };
	time_t t = time(NULL);
	struct tm tm = *localtime(&t);
	va_list ap;
	LPVOID lpMsgBuf;
	FormatMessageA(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPSTR)&lpMsgBuf, 0, NULL);

	// 만약 이 함수에서 파일에 기록해야한다면 이 위치에 인자문자열+오류문자열을 더해서 ErrorFileWrite()를 호출하면 된다.

	// 시간을 얻어서 totalBuf에 저장하고
	sprintf(timeBuf, "[%d-%d-%d %d:%d:%d] ",
		tm.tm_year + 1900, tm.tm_mon + 1, tm.tm_mday,
		tm.tm_hour, tm.tm_min, tm.tm_sec);
	strcpy(totalBuf, timeBuf);

	// 인자로 전달된 문자열을 추가한다.
	va_start(ap, _fmt);
	vsprintf(totalBuf + strlen(totalBuf), _fmt, ap);
	va_end(ap);

	// 구체적인 오류내용을 추가한다.
	strcat(totalBuf, (const char*)lpMsgBuf);
	puts(totalBuf);
	
	LocalFree(lpMsgBuf);
}
void LogManager::ErrorFileWrite(const char* _fmt, ...)
{
	char totalBuf[BUFSIZE] = { 0, };
	char timeBuf[HALF_BUFSIZE] = { 0, };
	time_t t = time(NULL);
	struct tm tm = *localtime(&t);
	va_list ap;

	// 시간 정보를 추가해서
	sprintf(timeBuf, "[%d-%d-%d %d:%d:%d] ",
		tm.tm_year + 1900, tm.tm_mon + 1, tm.tm_mday,
		tm.tm_hour, tm.tm_min, tm.tm_sec);
	strcpy(totalBuf, timeBuf);

	// 포맷을 얻어
	va_start(ap, _fmt);
	vsprintf(totalBuf + strlen(totalBuf), _fmt, ap);
	va_end(ap);

	// 기록한다.
	fwrite(totalBuf, strlen(totalBuf), 1, errPtr);
}

void LogManager::ConnectFileWrite(const char* _fmt, ...)
{
	char totalBuf[BUFSIZE] = { 0, };
	char timeBuf[HALF_BUFSIZE] = { 0, };
	time_t t = time(NULL);
	struct tm tm = *localtime(&t);
	va_list ap;

	// 시간 정보를 추가해서
	sprintf(timeBuf, "[%d-%d-%d %d:%d:%d] ",
		tm.tm_year + 1900, tm.tm_mon + 1, tm.tm_mday,
		tm.tm_hour, tm.tm_min, tm.tm_sec);
	strcpy(totalBuf, timeBuf);

	// 포맷을 얻어
	va_start(ap, _fmt);
	vsprintf(totalBuf + strlen(totalBuf), _fmt, ap);
	va_end(ap);

	// 기록한다.
	fwrite(totalBuf, strlen(totalBuf), 1, connectPtr);
}

void LogManager::WSAOverlappedResultPrintf(const char* _msg, WSAOVERLAPPED_EX* _overlapped)
{
	// ClientInfo를 캐스팅 해서 얻은 후
	C_ClientInfo* ptr = (C_ClientInfo*)_overlapped->ptr;

	// 오류 코드를 발생시켜서
	DWORD temp1, temp2;
	WSAGetOverlappedResult(ptr->GetSocket(), &_overlapped->overlapped,
		&temp1, FALSE, &temp2);

	// 이를 다시 에러로 호출한다.
	LogManager::GetInstance()->ErrorPrintf(_msg);
}