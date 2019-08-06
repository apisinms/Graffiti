#include "C_ClientInfo.h"
#include "LoginManager.h"
#include "LogManager.h"
#include "C_List.h"
#include "UtilityManager.h"

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
	loginList = new C_List<UserInfo*>();
	joinList = new C_List<UserInfo*>();

	UserInfo* ptr;
	while ((ptr = DatabaseManager::GetInstance()->LoadData()) != nullptr)
		joinList->Insert(ptr);
}
void LoginManager::End()
{
	delete loginList;
	delete joinList;
}

void LoginManager::Destroy()
{
	delete instance;
}
void LoginManager::PackPacket(char* _setptr, TCHAR* _str1, int& _size)
{
	char* ptr = _setptr;
	int strsize1 = _tcslen(_str1) * sizeof(TCHAR);
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
#ifdef __64BIT__
	__int64 mask = ((__int64)0x1f << (64 - 10));
#endif

#ifdef __32BIT__
	int mask = ((int)0x1f << (32 - 10));
#endif

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
#ifdef __64BIT__
	__int64 bitProtocol = 0;
#endif

#ifdef __32BIT__
	int bitProtocol = 0;
#endif

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

	char buf[BUFSIZE];		// ����
	int packetSize = 0;     // �� ������

	// ȸ�������� �Ǵ��� Ȯ���غ���.
	UserInfo userInfo;
	memset(&userInfo, 0, sizeof(userInfo));
	UnPackPacket(_buf, userInfo.id, userInfo.pw, userInfo.nickname);

	// ���Ӱ� �����Ҵ��ؼ� ���������� �߰�����
	UserInfo* ptr = new UserInfo(userInfo);

	_ptr->SetUserInfo(ptr);

	joinResult = CheckJoin(_ptr);

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
		DatabaseManager::GetInstance()->InsertData(ptr);
		joinList->Insert(_ptr->GetUserInfo());
	}
	break;
	}

	// �������� ���� 
	protocol = SetProtocol(LOGIN_STATE, PROTOCOL_LOGIN::JOIN_PROTOCOL, joinResult);
	ZeroMemory(buf, sizeof(BUFSIZE));
	// ��ŷ �� ����

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
	char buf[BUFSIZE];
	int packetSize;

	UserInfo tmpInfo;
	memset(&tmpInfo, 0, sizeof(tmpInfo));
	UnPackPacket(_buf, tmpInfo.id, tmpInfo.pw);

	// ���Ӱ� �����Ҵ��ؼ� ���������� �߰�����
	UserInfo* ptr = new UserInfo(tmpInfo);

	_ptr->SetUserInfo(ptr);

	// �α��� ����� ���� �޽����� �����Ѵ�.
	loginResult = CheckLogin(_ptr);	// ���ο��� ���� ����� �Ⱦ� �α��� �����ϸ� �� �α��� ������ �����´�.

	switch (loginResult)
	{

	case RESULT_LOGIN::ID_EXIST:
		_tcscpy_s(msg, MSGSIZE, ALREADY_LOGIN_MSG);
		break;

	case RESULT_LOGIN::ID_ERROR:
		_tcscpy_s(msg, MSGSIZE, ID_ERROR_MSG);
		break;

	case RESULT_LOGIN::PW_ERROR:
		_tcscpy_s(msg, MSGSIZE, PW_ERROR_MSG);
		break;

	case RESULT_LOGIN::LOGIN_SUCCESS:
	{
		_tcscpy_s(msg, MSGSIZE, LOGIN_SUCCESS_MSG);

		// ������ ������ �α��� ����Ʈ�� �߰�.
		loginList->Insert(_ptr->GetUserInfo());
		wprintf(L"�α��� ���� �������� : %s, %s\n", _ptr->GetUserInfo()->id, _ptr->GetUserInfo()->pw);
	}
	break;
	}


	// �������� ���� 
	protocol = SetProtocol(LOGIN_STATE, PROTOCOL_LOGIN::LOGIN_PROTOCOL, loginResult);
	ZeroMemory(buf, sizeof(BUFSIZE));
	// ��ŷ �� ����
	PackPacket(buf, msg, packetSize);
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

LoginManager::RESULT_LOGIN LoginManager::CheckJoin(C_ClientInfo* _ptr)
{
	RESULT_LOGIN joinResult = RESULT_LOGIN::NODATA;

	// ���� ȸ�� ����Ʈ�� �˻����� �ƴ϶��
	if (joinList->SearchCheck() == false)
	{
		UserInfo* info = nullptr;	// SearchData�� ���Ϲ޾Ƽ� ����Ʈ�� ��ȸ�ϴ� �뵵�� ����

		joinList->SearchStart();	// �˻� ����
		while (1)
		{
			info = joinList->SearchData();	// �����͸� �޾Ƽ�
			if (info == nullptr)	// �����Ͱ� nullptr�̸� ����Ʈ�� ������ ���̻� ����.
			{
				// ������ �߰��Ѱ� �ƹ��͵� ���ٸ�, ȸ�������� �����ϴ�.
				if (joinResult == RESULT_LOGIN::NODATA)
					joinResult = RESULT_LOGIN::JOIN_SUCCESS;

				break;
			}

			// ���̵� ���� ���� ȸ������ �ȵȴ�.
			if (_tcscmp(info->id, _ptr->GetUserInfo()->id) == 0)
			{
				joinResult = RESULT_LOGIN::ID_EXIST;
				break;
			}
		}
		joinList->SearchEnd();	// �˻� ����
	}

	// ���� ȸ�����Կ� �����ߴٸ�
	if (joinResult != RESULT_LOGIN::JOIN_SUCCESS)
	{
		// ȸ������ �õ��� �� �Է��� ���� ���� �����ش�.
		delete _ptr->GetUserInfo();
		_ptr->SetUserInfo(nullptr);
	}

	return joinResult;
}
LoginManager::RESULT_LOGIN LoginManager::CheckLogin(C_ClientInfo* _ptr)
{
	RESULT_LOGIN loginResult = RESULT_LOGIN::NODATA;

	////////////////// �켱 �α��� ����Ʈ���� �α��� �� ����ڰ� �ִ��� Ȯ���Ѵ�. ////////////////

	// ���� �α��� ����Ʈ�� �˻����� �ƴ϶��
	if (loginList->SearchCheck() == false)
	{
		UserInfo* info = nullptr;	// SearchData�� ���Ϲ޾Ƽ� ����Ʈ�� ��ȸ�ϴ� �뵵�� ����

		loginList->SearchStart();	// �˻� ����
		while (1)
		{
			info = loginList->SearchData();	// �����͸� �޾Ƽ�

			// �����Ͱ� nullptr�̸� Ż�� �����̴�.
			if (info == nullptr)
				break;

			// ���̵� ���� ���� �̹� ������ ������ �ִ� ����̴�.
			if (_tcscmp(info->id, _ptr->GetUserInfo()->id) == 0)
			{
				loginResult = RESULT_LOGIN::ID_EXIST;
				break;
			}
		}
		loginList->SearchEnd();	// �˻� ����
	}

	// �˻��� ����ġ�� ������ ��, �α����� �� �� ���� ������̶�� �� ���� �ٷ� �����Ѵ�.
	if (loginResult != RESULT_LOGIN::NODATA)
	{
		// �α��� �õ��� �� �Է��� ���� ���� �����ش�.
		delete _ptr->GetUserInfo();
		_ptr->SetUserInfo(nullptr);

		return loginResult;
	}

	/////////////////////////////////////// �̹��� ���Ե� ��Ͽ��� �����ؾ��� //////////////////////////////

		// ���� �� ����Ʈ�� �˻����� �ƴ϶��
	if (joinList->SearchCheck() == false)
	{
		UserInfo* info = nullptr;	// SearchData�� ���Ϲ޾Ƽ� ����Ʈ�� ��ȸ�ϴ� �뵵�� ����

		joinList->SearchStart();	// �˻� ����
		while (1)
		{
			info = joinList->SearchData();	// �����͸� �޾Ƽ�

			// �����Ͱ� nullptr�̸� Ż�� �����̴�.
			if (info == nullptr)
			{
				// �������� ã���� �ƹ��͵� ���ٸ� ID�� �������� �ʴ� ���̴�.
				if (loginResult == RESULT_LOGIN::NODATA)
					loginResult = RESULT_LOGIN::ID_ERROR;

				break;
			}

			// ��й�ȣ�� �ٸ��� '��й�ȣ Ʋ��'���� ���д�
			if (_tcscmp(info->id, _ptr->GetUserInfo()->id) == 0 &&
				_tcscmp(info->pw, _ptr->GetUserInfo()->pw) != 0)
			{
				loginResult = RESULT_LOGIN::PW_ERROR;
				break;
			}

			// ���̵�� ��й�ȣ�� ��� ���ٸ� �α��� �����̴�.
			if (_tcscmp(info->id, _ptr->GetUserInfo()->id) == 0 &&
				_tcscmp(info->pw, _ptr->GetUserInfo()->pw) == 0)
			{
				//delete _ptr->GetUserInfo();	// ������ �Է��� �α��� ������ ����������
				_ptr->SetUserInfo(info);	// nickname �������� ���Ե� �����͸� ���Ӱ� ����Ű���Ѵ�.

				loginResult = RESULT_LOGIN::LOGIN_SUCCESS;
				break;
			}
		}
		joinList->SearchEnd();	// �˻� ����
	}

	// �α��� ������ �ƴ϶��
	if (loginResult != RESULT_LOGIN::LOGIN_SUCCESS)
	{
		// �α��� �õ��� �� �Է��� ���� ���� �����ش�.
		delete _ptr->GetUserInfo();
		_ptr->SetUserInfo(nullptr);
	}

	return loginResult;
}


bool LoginManager::LoginListDelete(C_ClientInfo* _ptr)
{
	return loginList->Delete(_ptr->GetUserInfo());
}