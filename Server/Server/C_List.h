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
	bool Insert(T _info);	// 리스트에 삽입하는 함수
	bool Delete(T _info);	// 리스트에서 삭제하는 함수
	bool Remove(T _info);	// 리스트에서 노드 + 데이터ㅏ삭제하는 함수
	int GetCount();			// 갯수를 리턴해주는 함수
	bool SearchCheck();		// 검색중인지 검사하는 함수
	void SearchStart();		// 검색을 시작하는 함수(푸쉬)
	T SearchData();			// 검색해서 데이터를 리턴하는 함수
	void SearchEnd();		// 검색을 종료하는 함수(팝)
};
//////////////////// 메서드 정의 ///////////////////////////
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
