#pragma once
#include "C_LinkedList.h"
#define MAX_JOIN	100
#define MAX_CLIENT	100
#define MAX_LOGIN	100

template <typename T>
class C_List
{
private:
	C_LinkedList<T>* list;

public:
	C_List();
	~C_List();
	bool Insert(T _info);	// ����Ʈ�� �����ϴ� �Լ�
	bool Delete(T _info);	// ����Ʈ���� �����ϴ� �Լ�
	bool Remove(T _info);	// ����Ʈ���� ��� + �����ͤ������ϴ� �Լ�
	int GetCount();			// ������ �������ִ� �Լ�
	bool SearchCheck();		// �˻������� �˻��ϴ� �Լ�
	void SearchStart();		// �˻��� �����ϴ� �Լ�(Ǫ��)
	T SearchData();			// �˻��ؼ� �����͸� �����ϴ� �Լ�
	void SearchEnd();		// �˻��� �����ϴ� �Լ�(��)
};
//////////////////// �޼��� ���� ///////////////////////////
template <typename T>
C_List<T>::C_List()
{
	list = new C_LinkedList<T>();
}

template <typename T>
C_List<T>::~C_List()
{
	delete list;
}


template <typename T>
bool C_List<T>::Insert(T _info)
{
	return list->Insert(_info);
}

template <typename T>
bool C_List<T>::Delete(T _info)
{
	return list->Delete(_info);
}

template <typename T>
bool C_List<T>::Remove(T _info)
{
	return list->Remove(_info);
}


template <typename T>
int C_List<T>::GetCount()
{
	return list->GetCount();
}

template <typename T>
bool C_List<T>::SearchCheck()
{
	return list->SearchCheck();
}

template <typename T>
void C_List<T>::SearchStart()
{
	list->SearchStart();
}

template <typename T>
T C_List<T>::SearchData()
{
	return list->SearchData();
}

template <typename T>
void C_List<T>::SearchEnd()
{
	list->SearchEnd();
}
