#pragma once
#include "C_ComfortableCS.h"

template<typename T>
class C_SyncCS
{
private:
	static C_ComfortableCS comfortableCS;
	friend class IC_CS;	

public:
	class IC_CS
	{
	public:
		IC_CS()
		{
			comfortableCS.Enter();
		}

		~IC_CS()
		{
			comfortableCS.Leave();
		}
	};
};

template<typename T>
C_ComfortableCS C_SyncCS<T>::comfortableCS;