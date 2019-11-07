#include "stdafx.h"
#include "C_ClientInfo.h"
#include "LoginManager.h"

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

	// 문자열 길이
	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	// 문자열(유니코드)
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

	// 문자열1 길이
	memcpy(ptr, &strsize1, sizeof(strsize1));
	ptr = ptr + sizeof(strsize1);
	_size = _size + sizeof(strsize1);

	// 문자열1(유니코드)
	memcpy(ptr, _str1, strsize1);
	ptr = ptr + strsize1;
	_size = _size + strsize1;



	// 문자열2 길이
	memcpy(ptr, &strsize2, sizeof(strsize2));
	ptr = ptr + sizeof(strsize2);
	_size = _size + sizeof(strsize2);

	// 문자열2(유니코드)
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
	__int64 mask = ((__int64)PROTOCOL_OFFSET << (64 - PROTOCOL_MASK));

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
	__int64 bitProtocol = 0;
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

	char buf[BUFSIZE] = { 0, };		// 버퍼
	int packetSize = 0;     // 총 사이즈

	// 회원가입이 되는지 확인해본다.
	UserInfo tmpUserInfo;
	memset(&tmpUserInfo, 0, sizeof(UserInfo));
	UnPackPacket(_buf, tmpUserInfo.id, tmpUserInfo.pw, tmpUserInfo.nickname);

	// 새롭게 동적할당
	UserInfo* userInfo = new UserInfo(tmpUserInfo);

	// 이 유저 정보에 대한 회원가입 결과를 얻어옴
	joinResult = CheckJoin(_ptr, userInfo);

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
		DatabaseManager::GetInstance()->InsertUserInfo(userInfo);	// DB에 추가
		joinList.emplace_back(userInfo);	// 회원가입 리스트에 추가
		wprintf(L"회원가입 성공 유저정보 : ID:%s, NICK:%s, PW:%s\n", userInfo->id, userInfo->nickname, userInfo->pw);
	}
	break;
	}

	// 프로토콜, 패킷 Pack 및 전송
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

	// 새롭게 동적할당
	UserInfo* userInfo = new UserInfo(tmpUserInfo);

	// 로그인 결과를 토대로 메시지를 조립한다.
	loginResult = CheckLogin(_ptr, userInfo);	// 내부에서 가입 목록을 훑어 로그인 가능하면 그 로그인 정보를 가져온다.

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
		PackPacket(buf, msg, userInfo->nickname, packetSize);	// 로그인 성공시에는 닉네임도 함께 보내준다.

		// 유저의 정보를 로그인 리스트에 추가.
		_ptr->SetUserInfo(userInfo);	// 유저정보 세팅
		loginList.emplace_back(userInfo);
		wprintf(L"로그인 성공 유저정보 : ID:%s, NICK:%s, PW:%s\n", userInfo->id, userInfo->nickname, userInfo->pw);
	}
	break;
	}

	// 프로토콜 패킹 및 전송
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

LoginManager::RESULT_LOGIN LoginManager::CheckJoin(C_ClientInfo* _ptr, UserInfo* _userInfo)
{
	IC_CS cs;	// 검색중이니까 동기화 시작

	RESULT_LOGIN joinResult = RESULT_LOGIN::JOIN_SUCCESS;	// 만약 같은 아이디 있으면 ID_EXIST로 세팅될거임

	UserInfo* userInfo = nullptr;
	for (auto iter = joinList.begin(); iter != joinList.end(); ++iter)
	{
		userInfo = *iter;

		// 만약 일치하는 ID를 찾는다면 회원가입 실패이다.
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
	IC_CS cs;	// 검색중이니까 동기화 시작

	RESULT_LOGIN loginResult = RESULT_LOGIN::NODATA;

	////////////////// 1. 우선 로그인 리스트에서 로그인 된 사용자가 있는지 확인한다. ////////////////
	UserInfo* userInfo = nullptr;
	for (auto iter = loginList.begin(); iter != loginList.end(); ++iter)
	{
		userInfo = *iter;

		// 아이디가 같은 경우면 이미 접속한 유저가 있는 경우이다.
		if (_tcscmp(userInfo->id, _userInfo->id) == 0)
		{
			loginResult = RESULT_LOGIN::ID_EXIST;
			return loginResult;		// 바로 리턴해줌
		}
	}

	///////////////// 2. 이번엔 가입된 목록에서 조사해야함 //////////////////////////////
	userInfo = nullptr;
	for (auto iter = joinList.begin(); iter != joinList.end(); ++iter)
	{
		userInfo = *iter;

		// 비밀번호가 다르면 '비밀번호 틀림'으로 실패다
		if (_tcscmp(userInfo->id, _userInfo->id) == 0 &&
			_tcscmp(userInfo->pw, _userInfo->pw) != 0)
		{
			loginResult = RESULT_LOGIN::PW_ERROR;
			return loginResult;		// 바로 리턴해줌
		}

		// 아이디와 비밀번호가 모두 같다면 로그인 성공이다.
		if (_tcscmp(userInfo->id, _userInfo->id) == 0 &&
			_tcscmp(userInfo->pw, _userInfo->pw) == 0)
		{
			_tcscpy_s(_userInfo->nickname, NICKNAMESIZE, userInfo->nickname);	// 닉네임 정보는 클라가 안줬으니 DB에서 로드한걸로 저장시키고

			loginResult = RESULT_LOGIN::LOGIN_SUCCESS;
			return loginResult;		// 바로 리턴해줌
		}
	}

	// 이전까지 찾은게 아무것도 없다면 ID가 존재하지 않는 것이다.
	if (loginResult == RESULT_LOGIN::NODATA)
	{
		loginResult = RESULT_LOGIN::ID_ERROR;
	}

	return loginResult;		// ID_ERROR 리턴
}


bool LoginManager::LoginListDelete(UserInfo* _userInfo)
{
	// 지우고 난 뒤에 사이즈가 줄었으면 정상 삭제임
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