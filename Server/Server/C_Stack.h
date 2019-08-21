#pragma once
// ���ø� Ŭ���� CStack
template <class T, int _size>
class C_Stack
{
public:
	C_Stack();	// �⺻ ������
	bool pop(T& element);	// ��(������) �Լ�
	bool push(T element);	// Ǫ��(�ֱ�) �Լ�
	bool isEmpty();			// ������ �������
	int currSize();			// ���� ���� ����� ��������
	bool isFull();			// ������ �� á����
	bool copytop(T& element);	// top�� �ִ� element�� copy��

private:
	int size;	// ������ ������
	int top;	// ������ �ֻ�� ��ġ
	T *s_ptr;	// ���� ���ÿ� �����ų ������. Ÿ���� ���ø��̴�.
};

// ������
template <class T, int _size>
C_Stack<T, _size>::C_Stack()
{
	size = _size;	// ����� �Ѱܿ��� �Ķ���ͷ� �����Ѵ�.
	top = -1;		// ó������ top�� �����Ƿ� -1�̴�.
	s_ptr = new T[size];	// �Ѱܿ��� Ÿ��, ������� �����Ҵ��Ѵ�.
}

// Ǫ�� �Լ�
template <class T, int _size>
bool C_Stack<T, _size>::push(T element)
{
	// �� ���� �ƴϸ�
	if (!isFull())
	{
		s_ptr[++top] = element; //  top�� 1 ������Ű��, s_ptr���ٰ� ���� top��ġ�� ��Ҹ� �����Ѵ�.
		return true;	// true ����
	}
	return false;	// false ����
}

// �� �Լ�
template <class T, int _size>
bool C_Stack<T, _size>::pop(T& element)
{
	// ������ ����ִ� �� �ƴ϶��
	if (!isEmpty())
	{
		element = s_ptr[top--];	// ����Ÿ�� �Ķ���� element�� ���� top ��ġ�� �����͸� �����Ű��, top�� 1 �����Ѵ�.
		return true; // true ����
	}

	return false;	// false ����
}

// ���� �Լ�
template <class T, int _size>
bool C_Stack<T, _size>::copytop(T& element)
{
	// �� �� �ƴϸ�
	if (!isEmpty())
	{
		// ������ �ֻ�ܿ� �ִ� �����͸� element�� �����ϰ� true ����
		element = s_ptr[top];
		return true;
	}
	return false;	// �ƴϸ� false ����
}

// ������ ���� á�°�?
template <class T, int _size>
bool C_Stack<T, _size>::isFull()
{
	// top�� zero base index�̹Ƿ� size�� ���� ���� -1�� ���ؾ��Ѵ�. �̶� ���ٸ� ������ ���� ���ִ� ���̴�.
	if (top == size - 1)
		return true;	// �������� true

	else
		return false;	// �ƴϸ� false
}

// ������ ����°�?
template <class T, int _size>
bool C_Stack<T, _size> ::isEmpty()
{
	// �����ڿ��� -1�� �ʱ�ȭ ���ֹǷ� top�� -1�̶�� ����ִ°��̴�.(�Ǵ� top�� 0���� pop�ϸ� -1�� �ǹǷ� �װ͵� ����ִ� ��)
	if (top == -1)
		return true;	// ��� true

	else
		return false;	// �ƴϸ� false
}

// ���� ���� ������ ����
template <class T, int _size>
int C_Stack<T, _size>::currSize()
{
	return (top + 1);	// top�� zero base index�̹Ƿ� +1 �ؼ� ����
}