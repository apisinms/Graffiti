#include "stdafx.h"
#include "C_ClientInfo.h"
#include "LoginManager.h"

LoginManager* LoginManager::instance;			// �ʱ�ȭ

LoginManager* LoginManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
	if (instance == nullptr)
		instance = new LoginManager();

	return instance;
}

void LoginManager::Init()
{
	UserInfo* ptr;
	while ((ptr = DatabaseManager::GetInstance()->LoadUserInfo()) != nullptr)
	{
		joinList.emplace_back(ptr);
	}
}
void LoginManager::End()
{
}

void LoginManager::Destroy()
{
	delete instance;
}
void LoginManager::PackPacket(char* _setptr, TCHAR* _str1, int& _size)
{
	char* ptr = _setptr;
	int strsize1 = (int)_tcslen(_str1) * sizeof(TCHAR);
	_size = 0;

	// ���ڿ� ����
	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	// ���ڿ�(�����ڵ�)
	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	_size = _size + strsize1;
}
void LoginManager::PackPacket(char* _setptr, TCHAR* _str1, TCHAR* _str2, int& _size)
{
	char* ptr = _setptr;
	int strsize1 = (int)_tcslen(_str1) * sizeof(TCHAR);
	int strsize2 = (int)_tcslen(_str2) * sizeof(TCHAR);
	_size = 0;

	// ���ڿ�1 ����
	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	// ���ڿ�1(�����ڵ�)
	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	_size = _size + strsize1;



	// ���ڿ�2 ����
	memcpy(ptr, &strsize2, sizeof(strsize2));
	ptr = ptr + sizeof(strsize2);
	_size = _size + sizeof(strsize2);

	// ���ڿ�2(�����ڵ�)
	memcpy(ptr, _str2, strsize2);
	ptr = ptr + strsize2;
	_size = _size + strsize2;
}
void LoginManager::UnPackPacket(char* _getBuf, TCHAR* _str1, TCHAR* _str2, TCHAR* _str3)
{
	int str1size, str2size, str3size;
	char* ptr = _getBuf + sizeof(PROTOCOL_LOGIN);

	memcpy(&str1size, ptr, sizeof(str1size));
	ptr = ptr + sizeof(str1size);

	// ���ڿ� 1 ����
	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;



	memcpy(&str2size, ptr, sizeof(str2size));
	ptr = ptr + sizeof(str2size);

	// ���ڿ� 2 ����
	memcpy(_str2, ptr, str2size);
	ptr = ptr + str2size;



	memcpy(&str3size, ptr, sizeof(str3size));
	ptr = ptr + sizeof(str3size);

	// ���ڿ� 3����
	memcpy(_str3, ptr, str3size);
	ptr = ptr + str3size;
}
void LoginManager::UnPackPacket(char* _getBuf, TCHAR* _str1, TCHAR* _str2)
{
	int str1size, str2size;
	char* ptr = _getBuf + sizeof(PROTOCOL_LOGIN);


	memcpy(&str1size, ptr, sizeof(str1size));
	ptr = ptr + sizeof(str1size);

	// ���ڿ� 1 ����
	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;



	memcpy(&str2size, ptr, sizeof(str2size));
	ptr = ptr + sizeof(str2size);

	// ���ڿ� 2 ����
	memcpy(_str2, ptr, str2size);
	ptr = ptr + str2size;
}

void LoginManager::GetProtocol(PROTOCOL_LOGIN& _protocol)
{
	// major state�� ������(Ŭ��� state�� �Ⱥ����ϱ�(Ȥ�ó� ���Ŀ� �����ԵǸ� �̺κ��� ����)) protocol�� �������� ���ؼ� ���� 10��Ʈ ��ġ�� ����ũ�� ����
	__int64 mask = ((__int64)PROTOCOL_OFFSET << (64 - PROTOCOL_MASK));

	// ����ũ�� �ɷ��� 1���� ���������� ����ȴ�. 
	PROTOCOL_LOGIN protocol = (PROTOCOL_LOGIN)(_protocol & (PROTOCOL_LOGIN)mask);

	switch (protocol)
	{
	case JOIN_PROTOCOL:
		break;
	case LOGIN_PROTOCOL:
		break;
	}

	// �ƿ�ǲ�� �����̹Ƿ� �������ش�.
	// ���߿� �ѹ��� �������ִ� ������ ���߿� �߰��� ���� �� �ִ� result �� ���ؼ� protocol �� ������� ���� 
	_protocol = protocol;
}
LoginManager::PROTOCOL_LOGIN LoginManager::SetProtocol(STATE_PROTOCOL _state, PROTOCOL_LOGIN _protocol, RESULT_LOGIN _result)
{
	// �ϼ��� ���������� ���� 
	PROTOCOL_LOGIN protocol = (PROTOCOL_LOGIN)0;
	protocol = (PROTOCOL_LOGIN)(_state | _protocol | _result);
	return protocol;
}

LoginManager::PROTOCOL_LOGIN LoginManager::GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf)
{
	__int64 bitProtocol = 0;
	_ptr->GetPacket(bitProtocol, _buf);	// �켱 �ɷ��������� ���������� �����´�.

	// ��¥ ���������� ������ �ش�.(�ȿ��� �������� AND �˻�)
	PROTOCOL_LOGIN realProtocol = (PROTOCOL_LOGIN)bitProtocol;
	GetProtocol(realProtocol);

	return realProtocol;
}

bool LoginManager::JoinProcess(C_ClientInfo* _ptr, char* _buf)
{
	RESULT_LOGIN joinResult = RESULT_LOGIN::NODATA;
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_LOGIN protocol;

	char buf[BUFSIZE] = { 0, };		// ����
	int packetSize = 0;     // �� ������

	// ȸ�������� �Ǵ��� Ȯ���غ���.
	UserInfo tmpUserInfo;
	memset(&tmpUserInfo, 0, sizeof(UserInfo));
	UnPackPacket(_buf, tmpUserInfo.id, tmpUserInfo.pw, tmpUserInfo.nickname);

	// ���Ӱ� �����Ҵ�
	UserInfo* userInfo = new UserInfo(tmpUserInfo);

	// �� ���� ������ ���� ȸ������ ����� ����
	joinResult = CheckJoin(_ptr, userInfo);

	switch (joinResult)
	{
	case RESULT_LOGIN::ID_EXIST:
		_tcscpy_s(msg, MSGSIZE, ID_EXIST_MSG);
		break;


	case RESULT_LOGIN::JOIN_SUCCESS:
	{
		// ���� �޽��� ���� ��
		_tcscpy_s(msg, MSGSIZE, JOIN_SUCCESS_MSG);

		// DB�� �߰��ϰ�, ȸ������ ����Ʈ�� �ִ´�.
		DatabaseManager::GetInstance()->InsertUserInfo(userInfo);	// DB�� �߰�
		joinList.emplace_back(userInfo);	// ȸ������ ����Ʈ�� �߰�
		wprintf(L"ȸ������ ���� �������� : ID:%s, NICK:%s, PW:%s\n", userInfo->id, userInfo->nickname, userInfo->pw);
	}
	break;
	}

	// ��������, ��Ŷ Pack �� ����
	protocol = SetProtocol(LOGIN_STATE, PROTOCOL_LOGIN::JOIN_PROTOCOL, joinResult);
	PackPacket(buf, msg, packetSize);
	_ptr->SendPacket(protocol, buf, packetSize);

	if (joinResult == RESULT_LOGIN::JOIN_SUCCESS)
		return true;

	return false;
}
bool LoginManager::LoginProcess(C_ClientInfo* _ptr, char* _buf)
{
	RESULT_LOGIN loginResult = RESULT_LOGIN::NODATA;

	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_LOGIN protocol;
	char buf[BUFSIZE] = { 0, };
	int packetSize = 0;

	UserInfo tmpUserInfo;
	memset(&tmpUserInfo, 0, sizeof(UserInfo));
	UnPackPacket(_buf, tmpUserInfo.id, tmpUserInfo.pw);

	// ���Ӱ� �����Ҵ�
	UserInfo* userInfo = new UserInfo(tmpUserInfo);

	// �α��� ����� ���� �޽����� �����Ѵ�.
	loginResult = CheckLogin(_ptr, userInfo);	// ���ο��� ���� ����� �Ⱦ� �α��� �����ϸ� �� �α��� ������ �����´�.

	switch (loginResult)
	{

	case RESULT_LOGIN::ID_EXIST:
		_tcscpy_s(msg, MSGSIZE, ALREADY_LOGIN_MSG);
		PackPacket(buf, msg, packetSize);
		break;

	case RESULT_LOGIN::ID_ERROR:
		_tcscpy_s(msg, MSGSIZE, ID_ERROR_MSG);
		PackPacket(buf, msg, packetSize);
		break;

	case RESULT_LOGIN::PW_ERROR:
		_tcscpy_s(msg, MSGSIZE, PW_ERROR_MSG);
		PackPacket(buf, msg, packetSize);
		break;

	case RESULT_LOGIN::LOGIN_SUCCESS:
	{
		_tcscpy_s(msg, MSGSIZE, LOGIN_SUCCESS_MSG);
		PackPacket(buf, msg, userInfo->nickname, packetSize);	// �α��� �����ÿ��� �г��ӵ� �Բ� �����ش�.

		// ������ ������ �α��� ����Ʈ�� �߰�.
		_ptr->SetUserInfo(userInfo);	// �������� ����
		loginList.emplace_back(userInfo);
		wprintf(L"�α��� ���� �������� : ID:%s, NICK:%s, PW:%s\n", userInfo->id, userInfo->nickname, userInfo->pw);
	}
	break;
	}

	// �������� ��ŷ �� ����
	protocol = SetProtocol(LOGIN_STATE, PROTOCOL_LOGIN::LOGIN_PROTOCOL, loginResult);
	_ptr->SendPacket(protocol, buf, packetSize);

	if (loginResult == RESULT_LOGIN::LOGIN_SUCCESS)
		return true;

	return false;
}
//bool LoginManager::LogoutProcess(C_ClientInfo* _ptr)
//{
//	TCHAR msg[MSGSIZE] = { 0, };
//	PROTOCOL_LOGIN protocol;
//	char buf[BUFSIZE];
//	int packetSize;
//
//	RESULT_LOGIN logoutResult = RESULT_LOGIN::NODATA;
//
//	// ������ �����ߴٸ� �α׾ƿ� ����
//	if (loginList->Delete(_ptr->GetUserInfo()) == true)
//	{
//		_ptr->SetUserInfo(nullptr);
//		_tcscpy_s(msg, MSGSIZE, LOGOUT_SUCCESS_MSG);
//		logoutResult = RESULT_LOGIN::LOGOUT_SUCCESS;
//	}
//
//	// �ƴϸ� �α׾ƿ� ����
//	else
//	{
//		_tcscpy_s(msg, MSGSIZE, LOGOUT_FAIL_MSG);
//		logoutResult = RESULT_LOGIN::LOGOUT_FAIL;
//	}
//
//
//	// �������� ����
//	protocol = SetProtocol(LOGIN_STATE, PROTOCOL_LOGIN::LOGOUT_PROTOCOL, logoutResult);
//
//	ZeroMemory(buf, sizeof(BUFSIZE));
//
//	// ��ŷ �� ����
//	PackPacket(buf, msg, packetSize);
//	_ptr->SendPacket(protocol, buf, packetSize);
//
//
//	if (logoutResult == RESULT_LOGIN::LOGOUT_SUCCESS)
//		return true;
//
//	return false;
//}

bool LoginManager::CanIJoin(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_LOGIN protocol = GetBufferAndProtocol(_ptr, buf);

	if (protocol == JOIN_PROTOCOL)
		return JoinProcess(_ptr, buf);

	return false;
}
bool LoginManager::CanILogin(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // ��ȣȭ�� ���� ��Ŷ�� ������ ���� ���� 
	PROTOCOL_LOGIN protocol = GetBufferAndProtocol(_ptr, buf);

	if (protocol == LOGIN_PROTOCOL)
		return LoginProcess(_ptr, buf);

	return false;
}
//bool LoginManager::CanILogout(C_ClientInfo* _ptr)
//{
//	// �ܺο��� �� CanILogout()�� ȣ���� ������, �̹� Protocol�� �޾Ƽ� �ɷ����� ȣ���ϴ� ���̹Ƿ� �˻��� �ʿ���� �ٷ� LogoutProcess�� ȣ���ϸ� �ȴ�.
//
//	if (LogoutProcess(_ptr) == true)
//		return true;
//
//	return false;
//}

LoginManager::RESULT_LOGIN LoginManager::CheckJoin(C_ClientInfo* _ptr, UserInfo* _userInfo)
{
	IC_CS cs;	// �˻����̴ϱ� ����ȭ ����

	RESULT_LOGIN joinResult = RESULT_LOGIN::JOIN_SUCCESS;	// ���� ���� ���̵� ������ ID_EXIST�� ���õɰ���

	UserInfo* userInfo = nullptr;
	for (auto iter = joinList.begin(); iter != joinList.end(); ++iter)
	{
		userInfo = *iter;

		// ���� ��ġ�ϴ� ID�� ã�´ٸ� ȸ������ �����̴�.
		if (_tcscmp(userInfo->id, _userInfo->id) == 0)
		{
			joinResult = RESULT_LOGIN::ID_EXIST;
			break;
		}
	}

	return joinResult;
}
LoginManager::RESULT_LOGIN LoginManager::CheckLogin(C_ClientInfo* _ptr, UserInfo* _userInfo)
{
	IC_CS cs;	// �˻����̴ϱ� ����ȭ ����

	RESULT_LOGIN loginResult = RESULT_LOGIN::NODATA;

	////////////////// 1. �켱 �α��� ����Ʈ���� �α��� �� ����ڰ� �ִ��� Ȯ���Ѵ�. ////////////////
	UserInfo* userInfo = nullptr;
	for (auto iter = loginList.begin(); iter != loginList.end(); ++iter)
	{
		userInfo = *iter;

		// ���̵� ���� ���� �̹� ������ ������ �ִ� ����̴�.
		if (_tcscmp(userInfo->id, _userInfo->id) == 0)
		{
			loginResult = RESULT_LOGIN::ID_EXIST;
			return loginResult;		// �ٷ� ��������
		}
	}

	///////////////// 2. �̹��� ���Ե� ��Ͽ��� �����ؾ��� //////////////////////////////
	userInfo = nullptr;
	for (auto iter = joinList.begin(); iter != joinList.end(); ++iter)
	{
		userInfo = *iter;

		// ��й�ȣ�� �ٸ��� '��й�ȣ Ʋ��'���� ���д�
		if (_tcscmp(userInfo->id, _userInfo->id) == 0 &&
			_tcscmp(userInfo->pw, _userInfo->pw) != 0)
		{
			loginResult = RESULT_LOGIN::PW_ERROR;
			return loginResult;		// �ٷ� ��������
		}

		// ���̵�� ��й�ȣ�� ��� ���ٸ� �α��� �����̴�.
		if (_tcscmp(userInfo->id, _userInfo->id) == 0 &&
			_tcscmp(userInfo->pw, _userInfo->pw) == 0)
		{
			_tcscpy_s(_userInfo->nickname, NICKNAMESIZE, userInfo->nickname);	// �г��� ������ Ŭ�� �������� DB���� �ε��Ѱɷ� �����Ű��

			loginResult = RESULT_LOGIN::LOGIN_SUCCESS;
			return loginResult;		// �ٷ� ��������
		}
	}

	// �������� ã���� �ƹ��͵� ���ٸ� ID�� �������� �ʴ� ���̴�.
	if (loginResult == RESULT_LOGIN::NODATA)
	{
		loginResult = RESULT_LOGIN::ID_ERROR;
	}

	return loginResult;		// ID_ERROR ����
}


bool LoginManager::LoginListDelete(UserInfo* _userInfo)
{
	// ����� �� �ڿ� ����� �پ����� ���� ������
	int beforeSize = (int)loginList.size();

	loginList.remove(_userInfo);

	if (_userInfo != nullptr)
	{
		delete _userInfo;
		_userInfo = nullptr;
	}

	if (beforeSize < (int)loginList.size())
	{
		return true;
	}

	return false;
}