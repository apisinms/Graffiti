#include "C_ClientInfo.h"
#include "LoginManager.h"
#include "LogManager.h"
#include "C_List.h"
#include "UtilityManager.h"

LoginManager* LoginManager::instance;			// 초기화

LoginManager* LoginManager::GetInstance()
{
	// 인스턴스가 없다면 인스턴스를 생성하고 리턴한다.
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

	// 문자열 길이
	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	// 문자열(유니코드)
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

	// 문자열 1 받음
	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;



	memcpy(&str2size, ptr, sizeof(str2size));
	ptr = ptr + sizeof(str2size);

	// 문자열 2 받음
	memcpy(_str2, ptr, str2size);
	ptr = ptr + str2size;



	memcpy(&str3size, ptr, sizeof(str3size));
	ptr = ptr + sizeof(str3size);

	// 문자열 3받음
	memcpy(_str3, ptr, str3size);
	ptr = ptr + str3size;
}
void LoginManager::UnPackPacket(char* _getBuf, TCHAR* _str1, TCHAR* _str2)
{
	int str1size, str2size;
	char* ptr = _getBuf + sizeof(PROTOCOL_LOGIN);


	memcpy(&str1size, ptr, sizeof(str1size));
	ptr = ptr + sizeof(str1size);

	// 문자열 1 받음
	memcpy(_str1, ptr, str1size);
	ptr = ptr + str1size;



	memcpy(&str2size, ptr, sizeof(str2size));
	ptr = ptr + sizeof(str2size);

	// 문자열 2 받음
	memcpy(_str2, ptr, str2size);
	ptr = ptr + str2size;
}

void LoginManager::GetProtocol(PROTOCOL_LOGIN& _protocol)
{
	// major state를 제외한(클라는 state를 안보내니까(혹시나 추후에 보내게되면 이부분을 수정)) protocol을 가져오기 위해서 상위 10비트 위치에 마스크를 만듦
#ifdef __64BIT__
	__int64 mask = ((__int64)0x1f << (64 - 10));
#endif

#ifdef __32BIT__
	int mask = ((int)0x1f << (32 - 10));
#endif

	// 마스크에 걸러진 1개의 프로토콜이 저장된다. 
	PROTOCOL_LOGIN protocol = (PROTOCOL_LOGIN)(_protocol & (PROTOCOL_LOGIN)mask);

	switch (protocol)
	{
	case JOIN_PROTOCOL:
		break;
	case LOGIN_PROTOCOL:
		break;
	}

	// 아웃풋용 인자이므로 저장해준다.
	// 나중에 한번더 저장해주는 이유는 나중에 추가로 받을 수 있는 result 에 대해서 protocol 을 살려놓기 위해 
	_protocol = protocol;
}
LoginManager::PROTOCOL_LOGIN LoginManager::SetProtocol(STATE_PROTOCOL _state, PROTOCOL_LOGIN _protocol, RESULT_LOGIN _result)
{
	// 완성된 프로토콜을 리턴 
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

	_ptr->GetPacket(bitProtocol, _buf);	// 우선 걸러지지않은 프로토콜을 가져온다.

	// 진짜 프로토콜을 가져와 준다.(안에서 프로토콜 AND 검사)
	PROTOCOL_LOGIN realProtocol = (PROTOCOL_LOGIN)bitProtocol;
	GetProtocol(realProtocol);

	return realProtocol;
}

bool LoginManager::JoinProcess(C_ClientInfo* _ptr, char* _buf)
{
	RESULT_LOGIN joinResult = RESULT_LOGIN::NODATA;
	TCHAR msg[MSGSIZE] = { 0, };
	PROTOCOL_LOGIN protocol;

	char buf[BUFSIZE];		// 버퍼
	int packetSize = 0;     // 총 사이즈

	// 회원가입이 되는지 확인해본다.
	UserInfo userInfo;
	memset(&userInfo, 0, sizeof(userInfo));
	UnPackPacket(_buf, userInfo.id, userInfo.pw, userInfo.nickname);

	// 새롭게 동적할당해서 유저정보에 추가해줌
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
		// 성공 메시지 조립 후
		_tcscpy_s(msg, MSGSIZE, JOIN_SUCCESS_MSG);

		// DB에 추가하고, 회원가입 리스트에 넣는다.
		DatabaseManager::GetInstance()->InsertData(ptr);
		joinList->Insert(_ptr->GetUserInfo());
	}
	break;
	}

	// 프로토콜 세팅 
	protocol = SetProtocol(LOGIN_STATE, PROTOCOL_LOGIN::JOIN_PROTOCOL, joinResult);
	ZeroMemory(buf, sizeof(BUFSIZE));
	// 패킹 및 전송

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

	// 새롭게 동적할당해서 유저정보에 추가해줌
	UserInfo* ptr = new UserInfo(tmpInfo);

	_ptr->SetUserInfo(ptr);

	// 로그인 결과를 토대로 메시지를 조립한다.
	loginResult = CheckLogin(_ptr);	// 내부에서 가입 목록을 훑어 로그인 가능하면 그 로그인 정보를 가져온다.

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

		// 유저의 정보를 로그인 리스트에 추가.
		loginList->Insert(_ptr->GetUserInfo());
		wprintf(L"로그인 성공 유저정보 : %s, %s\n", _ptr->GetUserInfo()->id, _ptr->GetUserInfo()->pw);
	}
	break;
	}


	// 프로토콜 세팅 
	protocol = SetProtocol(LOGIN_STATE, PROTOCOL_LOGIN::LOGIN_PROTOCOL, loginResult);
	ZeroMemory(buf, sizeof(BUFSIZE));
	// 패킹 및 전송
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
//	// 삭제에 성공했다면 로그아웃 성공
//	if (loginList->Delete(_ptr->GetUserInfo()) == true)
//	{
//		_ptr->SetUserInfo(nullptr);
//		_tcscpy_s(msg, MSGSIZE, LOGOUT_SUCCESS_MSG);
//		logoutResult = RESULT_LOGIN::LOGOUT_SUCCESS;
//	}
//
//	// 아니면 로그아웃 실패
//	else
//	{
//		_tcscpy_s(msg, MSGSIZE, LOGOUT_FAIL_MSG);
//		logoutResult = RESULT_LOGIN::LOGOUT_FAIL;
//	}
//
//
//	// 프로토콜 세팅
//	protocol = SetProtocol(LOGIN_STATE, PROTOCOL_LOGIN::LOGOUT_PROTOCOL, logoutResult);
//
//	ZeroMemory(buf, sizeof(BUFSIZE));
//
//	// 패킹 및 전송
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
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_LOGIN protocol = GetBufferAndProtocol(_ptr, buf);

	if (protocol == JOIN_PROTOCOL)
		return JoinProcess(_ptr, buf);

	return false;
}
bool LoginManager::CanILogin(C_ClientInfo* _ptr)
{
	char buf[BUFSIZE] = { 0, }; // 암호화가 끝난 패킷을 가지고 있을 버프 
	PROTOCOL_LOGIN protocol = GetBufferAndProtocol(_ptr, buf);

	if (protocol == LOGIN_PROTOCOL)
		return LoginProcess(_ptr, buf);

	return false;
}
//bool LoginManager::CanILogout(C_ClientInfo* _ptr)
//{
//	// 외부에서 이 CanILogout()을 호출할 때에는, 이미 Protocol을 받아서 걸러내어 호출하는 것이므로 검사할 필요없이 바로 LogoutProcess를 호출하면 된다.
//
//	if (LogoutProcess(_ptr) == true)
//		return true;
//
//	return false;
//}

LoginManager::RESULT_LOGIN LoginManager::CheckJoin(C_ClientInfo* _ptr)
{
	RESULT_LOGIN joinResult = RESULT_LOGIN::NODATA;

	// 만약 회원 리스트가 검색중이 아니라면
	if (joinList->SearchCheck() == false)
	{
		UserInfo* info = nullptr;	// SearchData로 리턴받아서 리스트를 순회하는 용도의 변수

		joinList->SearchStart();	// 검색 시작
		while (1)
		{
			info = joinList->SearchData();	// 데이터를 받아서
			if (info == nullptr)	// 데이터가 nullptr이면 리스트에 내용이 더이상 없다.
			{
				// 이전에 발견한게 아무것도 없다면, 회원가입이 가능하다.
				if (joinResult == RESULT_LOGIN::NODATA)
					joinResult = RESULT_LOGIN::JOIN_SUCCESS;

				break;
			}

			// 아이디가 같은 경우면 회원가입 안된다.
			if (_tcscmp(info->id, _ptr->GetUserInfo()->id) == 0)
			{
				joinResult = RESULT_LOGIN::ID_EXIST;
				break;
			}
		}
		joinList->SearchEnd();	// 검색 종료
	}

	// 만약 회원가입에 실패했다면
	if (joinResult != RESULT_LOGIN::JOIN_SUCCESS)
	{
		// 회원가입 시도할 때 입력한 유저 정보 지워준다.
		delete _ptr->GetUserInfo();
		_ptr->SetUserInfo(nullptr);
	}

	return joinResult;
}
LoginManager::RESULT_LOGIN LoginManager::CheckLogin(C_ClientInfo* _ptr)
{
	RESULT_LOGIN loginResult = RESULT_LOGIN::NODATA;

	////////////////// 우선 로그인 리스트에서 로그인 된 사용자가 있는지 확인한다. ////////////////

	// 만약 로그인 리스트가 검색중이 아니라면
	if (loginList->SearchCheck() == false)
	{
		UserInfo* info = nullptr;	// SearchData로 리턴받아서 리스트를 순회하는 용도의 변수

		loginList->SearchStart();	// 검색 시작
		while (1)
		{
			info = loginList->SearchData();	// 데이터를 받아서

			// 데이터가 nullptr이면 탈출 조건이다.
			if (info == nullptr)
				break;

			// 아이디가 같은 경우면 이미 접속한 유저가 있는 경우이다.
			if (_tcscmp(info->id, _ptr->GetUserInfo()->id) == 0)
			{
				loginResult = RESULT_LOGIN::ID_EXIST;
				break;
			}
		}
		loginList->SearchEnd();	// 검색 종료
	}

	// 검색을 끝마치고 나왔을 때, 로그인을 할 수 없는 결과값이라면 그 값을 바로 리턴한다.
	if (loginResult != RESULT_LOGIN::NODATA)
	{
		// 로그인 시도할 때 입력한 유저 정보 지워준다.
		delete _ptr->GetUserInfo();
		_ptr->SetUserInfo(nullptr);

		return loginResult;
	}

	/////////////////////////////////////// 이번엔 가입된 목록에서 조사해야함 //////////////////////////////

		// 만약 이 리스트가 검색중이 아니라면
	if (joinList->SearchCheck() == false)
	{
		UserInfo* info = nullptr;	// SearchData로 리턴받아서 리스트를 순회하는 용도의 변수

		joinList->SearchStart();	// 검색 시작
		while (1)
		{
			info = joinList->SearchData();	// 데이터를 받아서

			// 데이터가 nullptr이면 탈출 조건이다.
			if (info == nullptr)
			{
				// 이전까지 찾은게 아무것도 없다면 ID가 존재하지 않는 것이다.
				if (loginResult == RESULT_LOGIN::NODATA)
					loginResult = RESULT_LOGIN::ID_ERROR;

				break;
			}

			// 비밀번호가 다르면 '비밀번호 틀림'으로 실패다
			if (_tcscmp(info->id, _ptr->GetUserInfo()->id) == 0 &&
				_tcscmp(info->pw, _ptr->GetUserInfo()->pw) != 0)
			{
				loginResult = RESULT_LOGIN::PW_ERROR;
				break;
			}

			// 아이디와 비밀번호가 모두 같다면 로그인 성공이다.
			if (_tcscmp(info->id, _ptr->GetUserInfo()->id) == 0 &&
				_tcscmp(info->pw, _ptr->GetUserInfo()->pw) == 0)
			{
				//delete _ptr->GetUserInfo();	// 기존에 입력한 로그인 정보는 지워버리고
				_ptr->SetUserInfo(info);	// nickname 정보까지 포함된 포인터를 새롭게 가리키게한다.

				loginResult = RESULT_LOGIN::LOGIN_SUCCESS;
				break;
			}
		}
		joinList->SearchEnd();	// 검색 종료
	}

	// 로그인 성공이 아니라면
	if (loginResult != RESULT_LOGIN::LOGIN_SUCCESS)
	{
		// 로그인 시도할 때 입력한 유저 정보 지워준다.
		delete _ptr->GetUserInfo();
		_ptr->SetUserInfo(nullptr);
	}

	return loginResult;
}


bool LoginManager::LoginListDelete(C_ClientInfo* _ptr)
{
	return loginList->Delete(_ptr->GetUserInfo());
}