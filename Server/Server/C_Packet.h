#pragma once
#include "C_Socket.h"

class C_Packet : public C_Socket
{
public:
	void GetPacket(__int64& _protocol, char* _getBuf);
	void SetPacket(__int64& _protocol, char* _setBuf, int _packetSize);
	bool SendPacket(__int64 _ptotocol, char* _buf, int _size);
};