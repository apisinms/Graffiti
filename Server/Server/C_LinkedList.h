#pragma once
#define STACK_SIZE 5
#include "C_Stack.h"
#include "C_Node.h"

// ���ø� Ŭ���� CLinkedList
template <typename T>
class C_LinkedList
{
	C_Node<T>*	mHead;	// ��ũ�� ����Ʈ�� ���� ��� ������(����)
	C_Node<T>*	mTail;	// ��ũ�� ����Ʈ�� �� ��� ������

	C_Stack<C_Node<T>*, STACK_SIZE>* mSearchStack;	// �˻��ϱ� ���� ���������͸� ������(Ÿ���� ��� ������, ������ 5��) �� �������� �뵵�� mSearchNode�� �ߺ� ����� �����̴�. �� Depth�� �ִ� 5
	C_Node<T>*	mSearchNode; // �˻� ��� ������

	int			mCount;		// ��ũ�� ����Ʈ�� ����� ��� ����

public:
	C_LinkedList();		// ������
	~C_LinkedList();		// �Ҹ���

	C_Node<T>*  GetmHead(){ return mHead; }	// ���� ��� ������ ��ȯ
	C_Node<T>*  GetmTail(){ return mTail; }	// �� ��� ������ ��ȯ

	bool	Insert(T  _data);	// ����
	bool	Delete(T  _data);	// ��常 ����
	bool    Remove(T _data);	// ��� + ������ ����
	int		GetCount();			// ���� ����
	
	void	SearchStart();		// �˻� ����
	bool    SearchCheck();		// �˻��� �������̾������� üũ
	T		SearchData();		// �˻��ؼ� �����͸� ����
	void	SearchEnd();		// �˻� ����
};

// �˻��� �������̾������� üũ
template <typename T>
bool C_LinkedList<T>::SearchCheck()
{
	// �������̾��ٸ� true�� ����
	if (mSearchNode != nullptr)
		return true;

	// �ƴϸ� false�� ����
	else
		return false;
}

// �˻��� ����
template <typename T>
void C_LinkedList<T>::SearchStart()
{
	mSearchStack->push(mSearchNode);	// ���� �˻����� ���ÿ� ���� �˻��� �����ϴ� ��� �����͸� ������ѵ�
	mSearchNode = nullptr;				// �˻� ���� null�� ������
}

// �˻��ؼ� �����͸� ����
template <typename T>
T C_LinkedList<T>::SearchData()
{
	// ���࿡ �˻� ��尡 nullptr�̸�
	if (mSearchNode == nullptr)
		mSearchNode = this->GetmHead();	// ���� ��带 mSearchNode�� ���������

	mSearchNode = mSearchNode->GetmNext();	// mSearchNode�� ���� ���� �̵���Ŵ

	// ���࿡ �˻� ��尡 nullptr�� �ƴ϶��
	if (mSearchNode != nullptr)
		return mSearchNode->GetmData();		// �˻� ����� �����͸� ������.

	return nullptr;		// ���� ���� �̵��Ѱ� nullptr�̶�� nullptr�� ������.                                        
}

// �˻��� ����
template <typename T>
void C_LinkedList<T>::SearchEnd()
{
	mSearchStack->pop(mSearchNode);	// SearchStart���� ���ÿ� �����ߴ� mSearchNode�� �ٽ� �����ͼ� ������
}

// ������
template <typename T>
C_LinkedList<T>::C_LinkedList()
{
	mHead = new C_Node<T>();	// ���� ��带 �����.
	mTail = mHead;			// ���� = ��	�̴� ó������.
	mSearchNode = nullptr;	// �˻� �����͸� nullptr��
	mSearchStack = new C_Stack<C_Node<T>*, STACK_SIZE>();	// �˻� ���� �����͸� �ν��Ͻ�ȭ �Ѵ�. (Ÿ���� CNode<T> *, ������ STACK_SIZE����ŭ)
	mCount = 0;				// ������ 0���� �Ѵ�.
}

// �Ҹ���
template <typename T>
C_LinkedList<T>::~C_LinkedList()
{
	C_Node<T>* ptr = mHead->GetmNext();	// ��� �����͸� �����ϱ� ���� ptr�� ���� ��� �����͸� �ִ´�.
	delete mHead;						// ptr�� ���� ��� �ּҰ� ��������, mHead �����͸� �ݳ��Ѵ�.

	// mHead�� ptr�� ����� �����͸� �ְ�, mHead�� �������ϰ�, ptr�� ��� �������� �Ѿ��.
	// �׷��� ptr�� nullptr�� �Ǹ� ��� ��� �����Ͱ� ������ ���̴�.
	while (ptr != nullptr)
	{
		mHead = ptr;
		ptr = ptr->GetmNext();
		delete mHead;
	}
}

// ���� �Լ�
template <typename T>
bool C_LinkedList<T>::Insert(T _data)
{
	C_Node<T>* ptr = new C_Node<T>(_data);	// �����ϰ��� �ϴ� �����͸� ptr�� ������Ű��
	mTail->SetmNext(ptr);					// �� ��� ������ �� ptr�� �����Ų��.
	mTail = ptr;							// ���� �� ��尡 ptr�� �ȴ�.
	mCount++;								// ������ �ϳ� ����
	return true;							// ���� ���� ����
}

// ��常 ���� �Լ�
template <typename T>
bool C_LinkedList<T>::Delete(T _data)
{
	C_Node<T>* pre = mHead;	// �ϴ� ���� ��� �����͸� pre��� �̸��� ������ ������ �����صд�.
	C_Node<T>* ptr = pre->GetmNext();	// ptr���� pre(������ ����)�� ���� ��带 �����Ѵ�.
	bool flag = false;	// ���� ���� �÷���

	// while���� ptr�� nullptr�� ������ ������ �ݺ��Ѵ�.
	while (ptr != nullptr)
	{
		// ������ ���ϰ��� �ϴ� �����͸� ã�Ҵٸ�
		if (ptr->GetmData() == _data)
		{
			flag = true;	// ���� �÷��� Ȱ��ȭ

			// �˻������Ͱ� ptr�̶��
			if (mSearchNode == ptr)
				mSearchNode = pre;	// �˻� �����Ϳ� pre�� �ְ�

			pre->SetmNext(ptr->GetmNext());	// pre�� ���� ��带 ptr�� ���� ���� �������ش�(�׷��� �����Ǵ³� ���� ��带 �����Ǵ� ����� ���� ���� �����ؼ� ���� ������ �����Ѵ�)

			// ���� �����ϴ� ���� �� ��忴�ٸ�
			if (ptr->GetmNext() == nullptr)
				mTail = pre;	// pre�� �� ���� �������ش�.

			delete ptr;	// �ش� ��带 �����ϰ�
			break;		// while���� ���� ������.
		}

		pre = ptr;	// pre�� ptr��尡 �ǰ�
		ptr = ptr->GetmNext();	// ptr�� ptr�� ���� ��尡 �ȴ�.(���� pre�� �׻� ptr���� ���� ��� �����Ͱ� �ȴ�)
	}

	// ���� �÷��װ� Ȱ��ȭ �ƴٸ� ������ ���̰� true ����
	if (flag)
	{
		mCount--;
		return true;
	}

	return false;	// �ƴϸ� ��ã���Ŵ� false ����

}

// ��� + ������ ���� �Լ�
template <typename T>
bool C_LinkedList<T>::Remove(T _data)
{
	C_Node<T>* pre = mHead;
	C_Node<T>* ptr = pre->GetmNext();
	bool flag = false;

	while (ptr != nullptr)
	{
		if (ptr->GetmData() == _data)
		{
			flag = true;
			if (mSearchNode == ptr)
				mSearchNode = pre;

			pre->SetmNext(ptr->GetmNext());
			if (ptr->GetmNext() == nullptr)
				mTail = pre;

			delete ptr->GetmData();	// �����͸� ���� �����ϰ�
			delete ptr;				// ��嵵 �����Ѵ�.
			break;
		}

		pre = ptr;
		ptr = ptr->GetmNext();

	}

	if (flag)
	{

		mCount--;
		return true;
	}

	return false;

}

// ����Ʈ�� ����� ��� ���� ���� �Լ�
template <typename T>
int C_LinkedList<T>::GetCount()
{
	return mCount;
}
