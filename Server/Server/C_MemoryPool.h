#pragma once
#include "C_SyncCS.h"
#include <assert.h>

// ���ø� Ŭ���� CMemoryPool, �Ҵ��� ������ 50����Ʈ��
template <class T, int ALLOC_BLOCK_SIZE = 50>
class C_MemoryPool : public C_SyncCS<T>
{
public:
	static VOID* operator new(std::size_t allocLength)
	{
		C_SyncCS<T>::IC_CS();

		//assert �Լ��� ��ȣ���� ���� �����̸� ���α׷��� �����ϴ� �Լ��̴�.
		assert(sizeof(T) == allocLength);		// �Ҵ��� ���ϴ� ũ��(�Ķ����)�� ���ø� Ŭ���� ũ��� ��ġ���� ������ assert
		assert(sizeof(T) >= sizeof(UCHAR*));	// ���ø� Ŭ���� ũ�Ⱑ UCHAR*(1����Ʈ) ���� ������ assert

		// mFreePointer�� nullptr�̸� allocBlock()�� ȣ���ؼ� ������ ����� �Ҵ��Ѵ�.
		if (!mFreePointer)
			allocBlock();

		// �Ҵ�� �����͸� �޾Ƽ� mFreePointer�� �Ҵ�� ���� ����� ����Ű�� �Ѵ�.
		UCHAR *ReturnPointer = mFreePointer;
		mFreePointer = *reinterpret_cast<UCHAR**>(ReturnPointer);

		//printf("new ȣ���\n");

		return ReturnPointer;
	}

	static VOID	operator delete(VOID* deletePointer)
	{
		C_SyncCS<T>::IC_CS();

		*reinterpret_cast<UCHAR**>(deletePointer) = mFreePointer;	// ������ ����� ���� ����� ����Ű�� ��ġ�� mFreePointer ��ġ�� ����Ű���Ѵ�.
		mFreePointer = static_cast<UCHAR*>(deletePointer);	// ������ ��ġ�� mFreePointer�� ����Ų��.
	
		//printf("delete ȣ���\n");
	}

private:
	static VOID	allocBlock()
	{
		// Ŭ���� ũ�� * �Ҵ� ��� ����(50����Ʈ)�� �����Ҵ��Ͽ� mFreePointer�� ����Ű���Ѵ�.
		mFreePointer		= new UCHAR[sizeof(T) * ALLOC_BLOCK_SIZE];

		// ó�� ��ġ�� ����Ű�� ���������� Current�� �����. (����� reinterpret_cast�� ���� ����ȯ�̸�, ���� �ٸ� ������ Ÿ�԰��� ��ȯ�� �����ϴ�)
		UCHAR **Current = reinterpret_cast<UCHAR **>(mFreePointer);
		UCHAR *Next		= mFreePointer;	// ���� ��ġ�� ����Ű�� �뵵�� �� ������ Next

		// �Ҵ�� ��� ���� ��ŭ ���鼭
		for (INT i=0;i<ALLOC_BLOCK_SIZE-1;++i)
		{
			Next		+= sizeof(T);	// ó�� -> ������ġ�� �� ����->�������� ��ġ�� ��� �̵�
			*Current	= Next;			// ���� ����� ���� ����� Next�� ��ġ�� ù �����Ϳ� �����Ѵ�.
			Current		= reinterpret_cast<UCHAR**>(Next);	// �׸��� �ٽ� Next�� ���� �����ͷ� ĳ�����Ͽ� Current�� �����Ѵ�.(���� ����� �����Ϳ� �����Ǵ��� ��� �����͸� �����ϱ� ���ؼ� ��ġ�� �ű�� ��)
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
