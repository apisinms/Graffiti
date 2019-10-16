#include "stdafx.h"
#include "RandomManager.h"
#include <float.h>

RandomManager* RandomManager::instance = nullptr;

RandomManager* RandomManager::GetInstance()
{
	if (instance == nullptr)
	{
		instance = new RandomManager();
	}

	return instance;
}

void RandomManager::Destroy()
{
	delete instance;
}

void RandomManager::Init()
{
	random_device rn;
	random.seed(rn());
}

void RandomManager::End()
{

}

void RandomManager::SetSeed(int _seed)
{
	random.seed(_seed);
}

int RandomManager::GetIntNumRandom()
{
	uniform_int_distribution<int> intRandom(INT_MIN, INT_MAX);
	return intRandom(random);
}

int RandomManager::GetIntNumRandom(int _min, int _max)
{
	uniform_int_distribution<int> intRandom(_min, _max);
	return intRandom(random);
}

double RandomManager::GetRealNumRandom()
{
	uniform_real_distribution<> realRandom(DBL_MIN, DBL_MAX);
	return realRandom(random);
}

double RandomManager::GetRealNumRandom(double _min, double _max)
{
	uniform_real_distribution<> realRandom(_min, _min);
	return realRandom(random);
}
