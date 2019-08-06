//#pragma once
////#include "C_SyncCS.h"
////#include "C_Global.h"
////#include "C_ClientInfo.h"
////#include "C_List.h"
////#include "LogManager.h"
//
//class C_Socket;
//class C_ClientInfo;
//
//class C_Select : public C_SyncCS<C_Select>
//{
//protected:
//	FD_SET rSet, wSet, eSet;
//	C_Socket* listenSock;					// 리스닝 소켓 포인터
//public:
//	virtual C_ClientInfo* FDAccept(C_Socket* _listenSock) = 0;
//	virtual int FDRead(C_ClientInfo* _client) = 0;
//	virtual int FDWrite(C_ClientInfo* _client) = 0;
//	virtual void FDSet(C_Socket* _sock, int _type) = 0;
//	virtual void FDReset() = 0;
//	void Select_Run();
//	int Select(bool _isRead, bool _isWrite, bool _isException, timeval* _timeout)
//	{
//		int retval = select(0,
//			_isRead == true ? &rSet : nullptr,
//			_isWrite == true ? &wSet : nullptr,
//			_isException == true ? &eSet : nullptr,
//			_timeout);
//
//		if (retval == SOCKET_ERROR)
//			LogManager::GetInstance()->ErrQuitMsgBox("Select()");
//
//		return retval;
//	}
//};