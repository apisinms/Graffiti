#pragma once
#include "DatabaseManager.h"
class C_ClientInfo;

#define ID_ERROR_MSG		TEXT("없는 아이디입니다.\n")
#define PW_ERROR_MSG		TEXT("패스워드가 틀렸습니다.\n")
#define LOGIN_SUCCESS_MSG	TEXT("로그인에 성공했습니다.\n")
#define ID_EXIST_MSG		TEXT("이미 있는 아이디 입니다.\n")
#define ALREADY_LOGIN_MSG	TEXT("이미 접속한 아이디 입니다.\n")
#define JOIN_SUCCESS_MSG	TEXT("가입에 성공했습니다.\n")
#define LOGOUT_SUCCESS_MSG	TEXT("로그아웃에 성공했습니다.\n")
#define LOGOUT_FAIL_MSG		TEXT("로그아웃에 실패했습니다.\n")

class LoginManager
{
	// state + protocol + result 순서로 프로토콜이 저장된다.

#ifdef __64BIT__
	// 64비트 프로토콜 열거형
	enum PROTOCOL_LOGIN : __int64
	{
		JOIN_PROTOCOL = ((__int64)0x1 << 58),
		LOGIN_PROTOCOL = ((__int64)0x1 << 57),
		//LOGOUT_PROTOCOL = ((__int64)0x1 << 56),
	};

	// 64비트 프로토콜 확장 열거형 
	enum RESULT_LOGIN : __int64
	{
		JOIN_SUCCESS = ((__int64)0x1 << 53),
		LOGIN_SUCCESS = ((__int64)0x1 << 53),
		LOGOUT_SUCCESS = ((__int64)0x1 << 53),
		LOGOUT_FAIL = ((__int64)0x1 << 52),	// Logout Fail

		// Join & Login result
		ID_EXIST = ((__int64)0x1 << 52),
		ID_ERROR = ((__int64)0x1 << 51),
		PW_ERROR = ((__int64)0x1 << 50),


		NODATA = ((__int64)0x1 << 49)
	};
#endif

#ifdef __32BIT__
	enum PROTOCOL_LOGIN : int
	{
		JOIN_PROTOCOL = ((int)0x1 << 26),
		LOGIN_PROTOCOL = ((int)0x1 << 25),
		LOGOUT_PROTOCOL = ((int)0x1 << 24),
	};

	enum RESULT_LOGIN : int
	{
		JOIN_SUCCESS   = ((int)0x1 << 21),
		LOGIN_SUCCESS  = ((int)0x1 << 21),
		LOGOUT_SUCCESS = ((int)0x1 << 21),
		LOGOUT_FAIL    = ((int)0x1 << 20),	// Logout Fail

		// Join Fail
		ID_EXIST = ((int)0x1 << 20),

		// Login Fail
		ID_ERROR = ((int)0x1 << 19),
		PW_ERROR = ((int)0x1 << 18),


		NODATA = ((int)0x1 << 17)
	};
#endif


private:
	LoginManager() {};
	~LoginManager() {};
	static LoginManager* instance;
	C_List<UserInfo*>* joinList;	// 회원가입된 유저의 정보를 나타내는 리스트
	C_List<UserInfo*>* loginList;	// 회원가입된 유저의 정보를 나타내는 리스트


private:
	RESULT_LOGIN CheckJoin(C_ClientInfo* _ptr);
	RESULT_LOGIN CheckLogin(C_ClientInfo* _ptr);


	void PackPacket(char* _setptr, TCHAR* _str1, int& _size);
	void UnPackPacket(char* _getBuf, TCHAR* _str1, TCHAR* _str2, TCHAR* _str3);
	void UnPackPacket(char* _getBuf, TCHAR* _str1, TCHAR* _str2);

	void GetProtocol(PROTOCOL_LOGIN& _protocol);
	PROTOCOL_LOGIN SetProtocol(STATE_PROTOCOL _state, PROTOCOL_LOGIN _protocol, RESULT_LOGIN _result);

	PROTOCOL_LOGIN GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// GetPacket과 GetProtocol을 내부적으로 호출하여 Protocol과 buf를 한번에 얻어오는 함수

	bool JoinProcess(C_ClientInfo* _ptr, char* buf);		// 회원가입 처리
	bool LoginProcess(C_ClientInfo* _ptr, char* buf);		// 로그인 처리
	//bool LogoutProcess(C_ClientInfo* _ptr);					// 로그아웃 처리

public:
	static LoginManager* GetInstance();
	static void Destroy();

	void Init();
	void End();

	bool CanIJoin(C_ClientInfo* _ptr);		// 회원가입이 되는지
	bool CanILogin(C_ClientInfo* _ptr);		// 로그인이 되는지
	//bool CanILogout(C_ClientInfo* _ptr);	// 로그아웃이 되는지

	bool LoginListDelete(C_ClientInfo* _ptr);	// 외부에서 호출하는, 로그인 목록에서 이 클라를 지워달라는 함수
};
