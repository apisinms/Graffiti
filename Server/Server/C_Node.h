#pragma once
// 템플릿 클래스 CNode(링크드 리스트에서 노드로 사용 될 클래스)
template <typename T>
class C_Node
{
	T	mData;			// 노드의 실제 데이터
	C_Node<T>*	mNext;	// 이 노드의 다음을 가리키는 포인터(다음 노드도 CNode 클래스이므로, CNode<T>* 형태가 맞다)

public:
	// 생성자(인자 0)
	C_Node()
	{
		memset(&mData, 0, sizeof(mData));	// 노드의 데이터를 초기화하고
		mNext = nullptr;					// 다음을 가리키는 포인터를 nullptr로 한다.
	}
	// 생성자(인자 1, 넘겨온 데이터를 mData로 초기화하고 mNext 포인터는 null로 한다)
	C_Node(T _data) : mData(_data), mNext(nullptr) {}

	~C_Node() {}	// 기본 소멸자

	// 데이터를 리턴하는 함수
	T GetmData(){return mData;}

	// 다음을 가리키는 포인터를 리턴하는 함수
	C_Node<T>*  GetmNext(){return mNext;}

	// 데이터를 셋팅하는 함수
	void SetmData(T _data){mData = _data;}

	// 다음을 가리키는 포인터를 셋팅하는 함수
	void SetmNext(C_Node<T>* _next){mNext = _next;}
};
