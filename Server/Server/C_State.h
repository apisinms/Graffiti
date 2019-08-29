#pragma once
class C_ClientInfo;

class C_State
{
public:
	virtual void Init() = 0;
	virtual void End() = 0;
	virtual void Read(C_ClientInfo* _ptr) = 0;
	virtual void Write(C_ClientInfo* _ptr) = 0;
};