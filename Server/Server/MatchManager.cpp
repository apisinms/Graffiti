#include "MatchManager.h"
#include "C_ClientInfo.h"

MatchManager* MatchManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
	if (instance == nullptr)
		instance = new MatchManager();

	return instance;
}

void MatchManager::Init()
{
	instance->waitList = new C_List<C_ClientInfo*>();
}
void MatchManager::End()
{
	delete waitList;
}

void MatchManager::Destroy()
{
	delete instance;
}

bool MatchManager::MatchProcess(C_ClientInfo* _ptr)
{
	// 4���̻��� �ƴٸ� ���� �� �տ��� �� ��, ���� 2���� �� ������ ����
	if (waitList->GetCount() >= 4)
	{
		// ����Ʈ�� �ִ� 4���� ������ ���� �����
		// ���� ���� ���� ȭ������

		return true;
	}

	// ���� ��Ī�� �������� ��Ȳ�̶��
	else
	{
		// ��⸮��Ʈ�� �ְ� false ����
		waitList->Insert(_ptr);	
		return false;
	}
}