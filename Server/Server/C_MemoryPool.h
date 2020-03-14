#pragma once
#include "C_SyncCS.h"
#include <assert.h>

// 템플릿 클래스 CMemoryPool, 할당의 단위는 50바이트씩
template <class T, int ALLOC_BLOCK_SIZE = 50>
class C_MemoryPool : public C_SyncCS<T>
{
public:
	static VOID* operator new(std::size_t allocLength)
	{
		C_SyncCS<T>::IC_CS();

		//assert 함수는 괄호안의 식이 거짓이면 프로그램을 종료하는 함수이다.
		assert(sizeof(T) == allocLength);		// 할당을 원하는 크기(파라미터)가 템플릿 클래스 크기와 일치하지 않으면 assert
		assert(sizeof(T) >= sizeof(UCHAR*));	// 템플릿 클래스 크기가 UCHAR*(1바이트) 보다 작으면 assert

		// mFreePointer가 nullptr이면 allocBlock()를 호출해서 실제로 블록을 할당한다.
		if (!mFreePointer)
			allocBlock();

		// 할당된 포인터를 받아서 mFreePointer가 할당된 다음 블록을 가리키게 한다.
		UCHAR *ReturnPointer = mFreePointer;
		mFreePointer = *reinterpret_cast<UCHAR**>(ReturnPointer);

		//printf("new 호출됨\n");

		return ReturnPointer;
	}

	static VOID	operator delete(VOID* deletePointer)
	{
		C_SyncCS<T>::IC_CS();

		*reinterpret_cast<UCHAR**>(deletePointer) = mFreePointer;	// 삭제될 블록의 다음 블록을 가리키는 위치에 mFreePointer 위치를 가리키게한다.
		mFreePointer = static_cast<UCHAR*>(deletePointer);	// 삭제될 위치를 mFreePointer가 가리킨다.
	
		//printf("delete 호출됨\n");
	}

private:
	static VOID	allocBlock()
	{
		// 클래스 크기 * 할당 블록 단위(50바이트)를 동적할당하여 mFreePointer가 가리키게한다.
		mFreePointer		= new UCHAR[sizeof(T) * ALLOC_BLOCK_SIZE];

		// 처음 위치를 가리키는 더블포인터 Current를 만든다. (참고로 reinterpret_cast는 강제 형변환이며, 서로 다른 포인터 타입간에 변환이 가능하다)
		UCHAR **Current = reinterpret_cast<UCHAR **>(mFreePointer);
		UCHAR *Next		= mFreePointer;	// 다음 위치를 가리키는 용도로 쓸 포인터 Next

		// 할당된 블록 단위 만큼 돌면서
		for (INT i=0;i<ALLOC_BLOCK_SIZE-1;++i)
		{
			Next		+= sizeof(T);	// 처음 -> 다음위치로 또 다음->다음다음 위치로 계속 이동
			*Current	= Next;			// 현재 블록의 다음 블록인 Next의 위치를 첫 포인터에 저장한다.
			Current		= reinterpret_cast<UCHAR**>(Next);	// 그리고 다시 Next의 더블 포인터로 캐스팅하여 Current로 조정한다.(다음 블록의 포인터에 다음의다음 블록 포인터를 저장하기 위해서 위치를 옮기는 것)
		}

		*Current = 0;
	}

private:
	static UCHAR	*mFreePointer;

protected:
	~C_MemoryPool()
	{
	}
};

template <class T, int ALLOC_BLOCK_SIZE>
UCHAR* C_MemoryPool<T, ALLOC_BLOCK_SIZE>::mFreePointer;
