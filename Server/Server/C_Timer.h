#pragma once
#include <chrono>
using namespace std::chrono;
class C_Timer
{
private:
	time_point<system_clock> startTime;
	time_point<system_clock> endTime;
	bool run = false;

public:
	void Start();
	void Stop();

	double ElapsedMilliseconds();
	double ElapsedSeconds();

	bool IsRunning() { return run; }
};

