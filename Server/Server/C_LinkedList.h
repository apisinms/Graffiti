#pragma once
#define STACK_SIZE 5
#include "C_Stack.h"
#include "C_Node.h"

// 템플릿 클래스 CLinkedList
template <typename T>
class C_LinkedList
{
	C_Node<T>*	mHead;	// 링크드 리스트의 시작 노드 포인터(더미)
	C_Node<T>*	mTail;	// 링크드 리스트의 끝 노드 포인터

	C_Stack<C_Node<T>*, STACK_SIZE>* mSearchStack;	// 검색하기 위한 스택포인터를 생성함(타입은 노드 포인터, 갯수는 5개) 이 포인터의 용도는 mSearchNode의 중복 사용을 위함이다. 그 Depth가 최대 5
	C_Node<T>*	mSearchNode; // 검색 노드 포인터

	int			mCount;		// 링크드 리스트에 저장된 노드 갯수

public:
	C_LinkedList();		// 생성자
	~C_LinkedList();		// 소멸자

	C_Node<T>*  GetmHead(){ return mHead; }	// 시작 노드 포인터 반환
	C_Node<T>*  GetmTail(){ return mTail; }	// 끝 노드 포인터 반환

	bool	Insert(T  _data);	// 삽입
	bool	Delete(T  _data);	// 노드만 삭제
	bool    Remove(T _data);	// 노드 + 데이터 삭제
	int		GetCount();			// 갯수 리턴
	
	void	SearchStart();		// 검색 시작
	bool    SearchCheck();		// 검색을 진행중이었는지를 체크
	T		SearchData();		// 검색해서 데이터를 얻어옴
	void	SearchEnd();		// 검색 종료
};

// 검색을 진행중이었는지를 체크
template <typename T>
bool C_LinkedList<T>::SearchCheck()
{
	// 진행중이었다면 true를 리턴
	if (mSearchNode != nullptr)
		return true;

	// 아니면 false를 리턴
	else
		return false;
}

// 검색을 시작
template <typename T>
void C_LinkedList<T>::SearchStart()
{
	mSearchStack->push(mSearchNode);	// 현재 검색전용 스택에 현재 검색을 진행하던 노드 포인터를 저장시켜둠
	mSearchNode = nullptr;				// 검색 노드는 null로 설정함
}

// 검색해서 데이터를 얻음
template <typename T>
T C_LinkedList<T>::SearchData()
{
	// 만약에 검색 노드가 nullptr이면
	if (mSearchNode == nullptr)
		mSearchNode = this->GetmHead();	// 시작 노드를 mSearchNode에 저장시켜줌

	mSearchNode = mSearchNode->GetmNext();	// mSearchNode를 다음 노드로 이동시킴

	// 만약에 검색 노드가 nullptr이 아니라면
	if (mSearchNode != nullptr)
		return mSearchNode->GetmData();		// 검색 노드의 데이터를 리턴함.

	return nullptr;		// 다음 노드로 이동한게 nullptr이라면 nullptr을 리턴함.                                        
}

// 검색을 종료
template <typename T>
void C_LinkedList<T>::SearchEnd()
{
	mSearchStack->pop(mSearchNode);	// SearchStart에서 스택에 저장했던 mSearchNode를 다시 꺼내와서 저장함
}

// 생성자
template <typename T>
C_LinkedList<T>::C_LinkedList()
{
	mHead = new C_Node<T>();	// 시작 노드를 만든다.
	mTail = mHead;			// 시작 = 끝	이다 처음에는.
	mSearchNode = nullptr;	// 검색 포인터를 nullptr로
	mSearchStack = new C_Stack<C_Node<T>*, STACK_SIZE>();	// 검색 스택 포인터를 인스턴스화 한다. (타입은 CNode<T> *, 갯수는 STACK_SIZE개만큼)
	mCount = 0;				// 갯수는 0으로 한다.
}

// 소멸자
template <typename T>
C_LinkedList<T>::~C_LinkedList()
{
	C_Node<T>* ptr = mHead->GetmNext();	// 모든 포인터를 제거하기 위해 ptr에 시작 노드 포인터를 넣는다.
	delete mHead;						// ptr에 시작 노드 주소가 들어가있으니, mHead 포인터를 반납한다.

	// mHead에 ptr에 저장된 포인터를 넣고, mHead는 삭제만하고, ptr은 계속 다음으로 넘어간다.
	// 그래서 ptr이 nullptr이 되면 모든 노드 포인터가 삭제된 것이다.
	while (ptr != nullptr)
	{
		mHead = ptr;
		ptr = ptr->GetmNext();
		delete mHead;
	}
}

// 삽입 함수
template <typename T>
bool C_LinkedList<T>::Insert(T _data)
{
	C_Node<T>* ptr = new C_Node<T>(_data);	// 저장하고자 하는 데이터를 ptr에 생성시키고
	mTail->SetmNext(ptr);					// 끝 노드 다음에 이 ptr을 연결시킨다.
	mTail = ptr;							// 이제 끝 노드가 ptr이 된다.
	mCount++;								// 갯수를 하나 증가
	return true;							// 삽입 성공 리턴
}

// 노드만 삭제 함수
template <typename T>
bool C_LinkedList<T>::Delete(T _data)
{
	C_Node<T>* pre = mHead;	// 일단 시작 노드 포인터를 pre라는 이름의 포인터 변수에 저장해둔다.
	C_Node<T>* ptr = pre->GetmNext();	// ptr에는 pre(지금은 시작)의 다음 노드를 저장한다.
	bool flag = false;	// 삭제 성공 플래그

	// while문은 ptr이 nullptr에 도달할 때까지 반복한다.
	while (ptr != nullptr)
	{
		// 삭제를 원하고자 하는 데이터를 찾았다면
		if (ptr->GetmData() == _data)
		{
			flag = true;	// 삭제 플래그 활성화

			// 검색포인터가 ptr이라면
			if (mSearchNode == ptr)
				mSearchNode = pre;	// 검색 포인터에 pre를 넣고

			pre->SetmNext(ptr->GetmNext());	// pre의 다음 노드를 ptr의 다음 노드로 설정해준다(그래야 삭제되는놈 이전 노드를 삭제되는 노드의 다음 노드로 설정해서 연결 오류가 없게한다)

			// 만약 삭제하는 놈이 끝 노드였다면
			if (ptr->GetmNext() == nullptr)
				mTail = pre;	// pre를 끝 노드로 설정해준다.

			delete ptr;	// 해당 노드를 삭제하고
			break;		// while문을 빠져 나간다.
		}

		pre = ptr;	// pre는 ptr노드가 되고
		ptr = ptr->GetmNext();	// ptr은 ptr의 다음 노드가 된다.(따라서 pre는 항상 ptr보다 이전 노드 포인터가 된다)
	}

	// 삭제 플래그가 활성화 됐다면 갯수를 줄이고 true 리턴
	if (flag)
	{
		mCount--;
		return true;
	}

	return false;	// 아니면 못찾은거니 false 리턴

}

// 노드 + 데이터 삭제 함수
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

			delete ptr->GetmData();	// 데이터를 먼저 삭제하고
			delete ptr;				// 노드도 삭제한다.
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

// 리스트에 저장된 노드 갯수 리턴 함수
template <typename T>
int C_LinkedList<T>::GetCount()
{
	return mCount;
}
