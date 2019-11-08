#include "stdafx.h"
#include "C_Timer.h"


void C_Timer::Start()
{
	startTime = system_clock::now();
	run = true;
}

void C_Timer::Stop()
{
	endTime = system_clock::now();
	run = false;
}

double C_Timer::ElapsedMilliseconds()
{
	time_point<system_clock> nowEndTime;

	if (run == true)
	{
		nowEndTime = system_clock::now();
	}
	else
	{
		nowEndTime = endTime;
	}

	return (double)duration_cast<milliseconds>(nowEndTime - startTime).count();
}

double C_Timer::ElapsedSeconds()
{
	return ElapsedMilliseconds() / 1000.0;
}
