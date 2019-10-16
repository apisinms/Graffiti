#pragma once
class RandomManager
{
private:
	RandomManager() {}
	~RandomManager() {}
	static RandomManager* instance;
	
private:
	mt19937 random;

public:
	static RandomManager* GetInstance();
	static void Destroy();

public:
	void Init();
	void End();

	void SetSeed(int _seed);
	int GetIntNumRandom();
	int GetIntNumRandom(int _min, int _max);
	double GetRealNumRandom();
	double GetRealNumRandom(double _min, double _max);

};

