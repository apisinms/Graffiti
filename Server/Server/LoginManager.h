#pragma once
#include "DatabaseManager.h"
class C_ClientInfo;

#define ID_ERROR_MSG		TEXT("���� ���̵��Դϴ�.\n")
#define PW_ERROR_MSG		TEXT("�н����尡 Ʋ�Ƚ��ϴ�.\n")
#define LOGIN_SUCCESS_MSG	TEXT("�α��ο� �����߽��ϴ�.\n")
#define ID_EXIST_MSG		TEXT("�̹� �ִ� ���̵� �Դϴ�.\n")
#define ALREADY_LOGIN_MSG	TEXT("�̹� ������ ���̵� �Դϴ�.\n")
#define JOIN_SUCCESS_MSG	TEXT("���Կ� �����߽��ϴ�.\n")
#define LOGOUT_SUCCESS_MSG	TEXT("�α׾ƿ��� �����߽��ϴ�.\n")
#define LOGOUT_FAIL_MSG		TEXT("�α׾ƿ��� �����߽��ϴ�.\n")

class LoginManager
{
	// state + protocol + result ������ ���������� ����ȴ�.

#ifdef __64BIT__
	// 64��Ʈ �������� ������
	enum PROTOCOL_LOGIN : __int64
	{
		JOIN_PROTOCOL = ((__int64)0x1 << 58),
		LOGIN_PROTOCOL = ((__int64)0x1 << 57),
		//LOGOUT_PROTOCOL = ((__int64)0x1 << 56),
	};

	// 64��Ʈ �������� Ȯ�� ������ 
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
	C_List<UserInfo*>* joinList;	// ȸ�����Ե� ������ ������ ��Ÿ���� ����Ʈ
	C_List<UserInfo*>* loginList;	// ȸ�����Ե� ������ ������ ��Ÿ���� ����Ʈ


private:
	RESULT_LOGIN CheckJoin(C_ClientInfo* _ptr);
	RESULT_LOGIN CheckLogin(C_ClientInfo* _ptr);


	void PackPacket(char* _setptr, TCHAR* _str1, int& _size);
	void UnPackPacket(char* _getBuf, TCHAR* _str1, TCHAR* _str2, TCHAR* _str3);
	void UnPackPacket(char* _getBuf, TCHAR* _str1, TCHAR* _str2);

	void GetProtocol(PROTOCOL_LOGIN& _protocol);
	PROTOCOL_LOGIN SetProtocol(STATE_PROTOCOL _state, PROTOCOL_LOGIN _protocol, RESULT_LOGIN _result);

	PROTOCOL_LOGIN GetBufferAndProtocol(C_ClientInfo* _ptr, char* _buf);	// GetPacket�� GetProtocol�� ���������� ȣ���Ͽ� Protocol�� buf�� �ѹ��� ������ �Լ�

	bool JoinProcess(C_ClientInfo* _ptr, char* buf);		// ȸ������ ó��
	bool LoginProcess(C_ClientInfo* _ptr, char* buf);		// �α��� ó��
	//bool LogoutProcess(C_ClientInfo* _ptr);					// �α׾ƿ� ó��

public:
	static LoginManager* GetInstance();
	static void Destroy();

	void Init();
	void End();

	bool CanIJoin(C_ClientInfo* _ptr);		// ȸ�������� �Ǵ���
	bool CanILogin(C_ClientInfo* _ptr);		// �α����� �Ǵ���
	//bool CanILogout(C_ClientInfo* _ptr);	// �α׾ƿ��� �Ǵ���

	bool LoginListDelete(C_ClientInfo* _ptr);	// �ܺο��� ȣ���ϴ�, �α��� ��Ͽ��� �� Ŭ�� �����޶�� �Լ�
};
