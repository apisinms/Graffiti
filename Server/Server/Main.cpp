#include "stdafx.h"
#include "MainManager.h"
#include "C_Sector.h"

void myPrint(const char* _msg)
{
	puts(_msg);
}

int main()
{
	MainManager* manager = MainManager::GetInstance();

	manager->Init();
	manager->Run();
}