#pragma once
// ���ø� Ŭ���� CNode(��ũ�� ����Ʈ���� ���� ��� �� Ŭ����)
template <typename T>
class C_Node
{
	T	mData;			// ����� ���� ������
	C_Node<T>*	mNext;	// �� ����� ������ ����Ű�� ������(���� ��嵵 CNode Ŭ�����̹Ƿ�, CNode<T>* ���°� �´�)

public:
	// ������(���� 0)
	C_Node()
	{
		memset(&mData, 0, sizeof(mData));	// ����� �����͸� �ʱ�ȭ�ϰ�
		mNext = nullptr;					// ������ ����Ű�� �����͸� nullptr�� �Ѵ�.
	}
	// ������(���� 1, �Ѱܿ� �����͸� mData�� �ʱ�ȭ�ϰ� mNext �����ʹ� null�� �Ѵ�)
	C_Node(T _data) : mData(_data), mNext(nullptr) {}

	~C_Node() {}	// �⺻ �Ҹ���

	// �����͸� �����ϴ� �Լ�
	T GetmData(){return mData;}

	// ������ ����Ű�� �����͸� �����ϴ� �Լ�
	C_Node<T>*  GetmNext(){return mNext;}

	// �����͸� �����ϴ� �Լ�
	void SetmData(T _data){mData = _data;}

	// ������ ����Ű�� �����͸� �����ϴ� �Լ�
	void SetmNext(C_Node<T>* _next){mNext = _next;}
};
