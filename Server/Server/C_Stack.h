#pragma once
// 템플릿 클래스 CStack
template <class T, int _size>
class C_Stack
{
public:
	C_Stack();	// 기본 생성자
	bool pop(T& element);	// 팝(빼내기) 함수
	bool push(T element);	// 푸쉬(넣기) 함수
	bool isEmpty();			// 스택이 비었는지
	int currSize();			// 현재 스택 사이즈를 리턴해줌
	bool isFull();			// 스택이 꽉 찼는지
	bool copytop(T& element);	// top에 있는 element를 copy함

private:
	int size;	// 스택의 사이즈
	int top;	// 스택의 최상단 위치
	T *s_ptr;	// 실제 스택에 저장시킬 데이터. 타입은 템플릿이다.
};

// 생성자
template <class T, int _size>
C_Stack<T, _size>::C_Stack()
{
	size = _size;	// 사이즈를 넘겨오는 파라미터로 설정한다.
	top = -1;		// 처음에는 top이 없으므로 -1이다.
	s_ptr = new T[size];	// 넘겨오는 타입, 사이즈로 동적할당한다.
}

// 푸쉬 함수
template <class T, int _size>
bool C_Stack<T, _size>::push(T element)
{
	// 꽉 찬게 아니면
	if (!isFull())
	{
		s_ptr[++top] = element; //  top을 1 증가시키고, s_ptr에다가 현재 top위치에 요소를 저장한다.
		return true;	// true 리턴
	}
	return false;	// false 리턴
}

// 팝 함수
template <class T, int _size>
bool C_Stack<T, _size>::pop(T& element)
{
	// 스택이 비어있는 게 아니라면
	if (!isEmpty())
	{
		element = s_ptr[top--];	// 참조타입 파라미터 element에 현재 top 위치의 데이터를 저장시키고, top을 1 감소한다.
		return true; // true 리턴
	}

	return false;	// false 리턴
}

// 복사 함수
template <class T, int _size>
bool C_Stack<T, _size>::copytop(T& element)
{
	// 빈 게 아니면
	if (!isEmpty())
	{
		// 스택의 최상단에 있는 데이터를 element에 저장하고 true 리턴
		element = s_ptr[top];
		return true;
	}
	return false;	// 아니면 false 리턴
}

// 스택이 가득 찼는가?
template <class T, int _size>
bool C_Stack<T, _size>::isFull()
{
	// top은 zero base index이므로 size와 비교할 때는 -1로 비교해야한다. 이때 같다면 스택이 가득 차있는 것이다.
	if (top == size - 1)
		return true;	// 가득차면 true

	else
		return false;	// 아니면 false
}

// 스택이 비었는가?
template <class T, int _size>
bool C_Stack<T, _size> ::isEmpty()
{
	// 생성자에서 -1로 초기화 해주므로 top이 -1이라면 비어있는것이다.(또는 top이 0에서 pop하면 -1이 되므로 그것도 비어있는 것)
	if (top == -1)
		return true;	// 비면 true

	else
		return false;	// 아니면 false
}

// 현재 스택 사이즈 리턴
template <class T, int _size>
int C_Stack<T, _size>::currSize()
{
	return (top + 1);	// top이 zero base index이므로 +1 해서 리턴
}