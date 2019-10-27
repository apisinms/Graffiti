#include "stdafx.h"
#include "MatchManager.h"
#include "RoomManager.h"
#include "C_ClientInfo.h"
#include "InGameManager.h"

MatchManager* MatchManager::instance;

MatchManager* MatchManager::GetInstance()
{
	// �ν��Ͻ��� ���ٸ� �ν��Ͻ��� �����ϰ� �����Ѵ�.
	if (instance == nullptr)
		instance = new MatchManager();

	return instance;
}

void MatchManager::Init()
{
	waitList = new list<C_ClientInfo*>[RoomInfo::GameType::_MAX_GAMETYPE];
}

void MatchManager::End()
{
	delete[]waitList;
}

void MatchManager::Destroy()
{
	delete instance;
}

void MatchManager::WaitListRemove(C_ClientInfo* _ptr)
{
	IC_CS cs;

	int gameType = _ptr->GetGameType();
	if (gameType != -1)
	{
		waitList[gameType].remove(_ptr);
		_ptr->SetGameType(-1);	// -1�� ����

		switch ((RoomInfo::GameType)gameType)
		{
			case RoomInfo::GameType::_2vs2:
			{
				wprintf(L"2:2 ��⸮��Ʈ ���� ���� : %d\n", (int)waitList[gameType].size());
			}
			break;

			case RoomInfo::GameType::_1vs1:
			{
				wprintf(L"1:1 ��⸮��Ʈ ���� ���� : %d\n", (int)waitList[gameType].size());
			}
			break;
		}
	}
}

bool MatchManager::MatchProcess(C_ClientInfo* _ptr)
{
	IC_CS cs;

	int gameType = _ptr->GetGameType();
	waitList[gameType].emplace_back(_ptr);	// ����Ʈ�� �ڷ� �ְ�

	// DB���� �ҷ��� ������� ����
	int MaxPlayerOfThisGameType = InGameManager::GetInstance()->GetGameInfo(gameType)->maxPlayer;

	switch ((RoomInfo::GameType)gameType)
	{
		case RoomInfo::GameType::_2vs2:
		{
			printf("2:2 ��� ����Ʈ�� ���� ���� ������ : %d\n", (int)waitList[gameType].size());
		}
		break;

		case RoomInfo::GameType::_1vs1:
		{
			printf("1:1 ��� ����Ʈ�� ���� ���� ������ : %d\n", (int)waitList[gameType].size());
		}
		break;
	}

	
	// �ο����� á�ٸ�
	if (waitList[gameType].size() >= MaxPlayerOfThisGameType)
	{
		// ���� ��� ����Ʈ�� �����ؼ� ���� ����
		return RoomManager::GetInstance()->CreateRoom(waitList[gameType], MaxPlayerOfThisGameType);
	}

	// ���� ��Ī�� �������� ��Ȳ�̶��
	else
		return false;
}

