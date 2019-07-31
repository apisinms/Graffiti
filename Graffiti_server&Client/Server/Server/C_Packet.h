#pragma once
#include "C_Socket.h"

class C_Packet : public C_Socket
{
public:
#ifdef __64BIT__
	void GetPacket(__int64& _protocol, char* _getBuf);
	void SetPacket(__int64& _protocol, char* _setBuf, int _packetSize);
	bool SendPacket(__int64 _ptotocol, char* _buf, int _size);
#endif

#ifdef __32BIT__
	void GetPacket(int& _protocol, char* _getBuf);
	void SetPacket(int& _protocol, char* _setBuf, int _packetSize);
	bool SendPacket(int _ptotocol, char* _buf, int _size);
#endif
};