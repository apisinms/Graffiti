#include "stdafx.h"
#include "C_ClientInfo.h"

LogManager* LogManager::instance;	// �ʱ�ȭ
FILE* LogManager::errPtr = nullptr;
FILE* LogManager::connectPtr = nullptr;

LogManager* LogManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
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

	// �����޽��� ����
	va_list ap;
	LPVOID lpMsgBuf;
	FormatMessageA(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, WSAGetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPSTR)&lpMsgBuf, 0, NULL);

	// ���� ���ڿ� �߰�
	va_start(ap, _fmt);
	vsprintf(totalBuf + strlen(totalBuf), _fmt, ap);
	va_end(ap);

	// ���� ���ڿ� �߰�
	strcat(totalBuf, (const char*)lpMsgBuf);

	// ���� �޽����ڽ� ���
	MessageBoxA(NULL, (LPCSTR)totalBuf, _fmt, MB_ICONERROR);

	// �α׿� ���
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

	// ���� �� �Լ����� ���Ͽ� ����ؾ��Ѵٸ� �� ��ġ�� ���ڹ��ڿ�+�������ڿ��� ���ؼ� ErrorFileWrite()�� ȣ���ϸ� �ȴ�.

	// �ð��� �� totalBuf�� �����ϰ�
	sprintf(timeBuf, "[%d-%d-%d %d:%d:%d] ",
		tm.tm_year + 1900, tm.tm_mon + 1, tm.tm_mday,
		tm.tm_hour, tm.tm_min, tm.tm_sec);
	strcpy(totalBuf, timeBuf);

	// ���ڷ� ���޵� ���ڿ��� �߰��Ѵ�.
	va_start(ap, _fmt);
	vsprintf(totalBuf + strlen(totalBuf), _fmt, ap);
	va_end(ap);

	// ��ü���� ���������� �߰��Ѵ�.
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

	// �ð� ������ �߰��ؼ�
	sprintf(timeBuf, "[%d-%d-%d %d:%d:%d] ",
		tm.tm_year + 1900, tm.tm_mon + 1, tm.tm_mday,
		tm.tm_hour, tm.tm_min, tm.tm_sec);
	strcpy(totalBuf, timeBuf);

	// ������ ���
	va_start(ap, _fmt);
	vsprintf(totalBuf + strlen(totalBuf), _fmt, ap);
	va_end(ap);

	// ����Ѵ�.
	fwrite(totalBuf, strlen(totalBuf), 1, errPtr);
}

void LogManager::ConnectFileWrite(const char* _fmt, ...)
{
	char totalBuf[BUFSIZE] = { 0, };
	char timeBuf[HALF_BUFSIZE] = { 0, };
	time_t t = time(NULL);
	struct tm tm = *localtime(&t);
	va_list ap;

	// �ð� ������ �߰��ؼ�
	sprintf(timeBuf, "[%d-%d-%d %d:%d:%d] ",
		tm.tm_year + 1900, tm.tm_mon + 1, tm.tm_mday,
		tm.tm_hour, tm.tm_min, tm.tm_sec);
	strcpy(totalBuf, timeBuf);

	// ������ ���
	va_start(ap, _fmt);
	vsprintf(totalBuf + strlen(totalBuf), _fmt, ap);
	va_end(ap);

	// ����Ѵ�.
	fwrite(totalBuf, strlen(totalBuf), 1, connectPtr);
}

void LogManager::WSAOverlappedResultPrintf(const char* _msg, WSAOVERLAPPED_EX* _overlapped)
{
	// ClientInfo�� ĳ���� �ؼ� ���� ��
	C_ClientInfo* ptr = (C_ClientInfo*)_overlapped->ptr;

	// ���� �ڵ带 �߻����Ѽ�
	DWORD temp1, temp2;
	WSAGetOverlappedResult(ptr->GetSocket(), &_overlapped->overlapped,
		&temp1, FALSE, &temp2);

	// �̸� �ٽ� ������ ȣ���Ѵ�.
	LogManager::GetInstance()->ErrorPrintf(_msg);
}