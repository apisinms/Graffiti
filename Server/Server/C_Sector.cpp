#include "stdafx.h"
#include "C_Sector.h"


/// 일단 맵 크기가 x 8, z 6이라고 가정하고, 그리드 x 2.5, z 2.5를 가정해서 섹터를 생성한다.
C_Sector::C_Sector()
{
	// 섹터 더블포인터 동적할당
	sectors = new SectorInstance*[ROW];
	for (int i = 0; i < ROW; i++)
	{
		sectors[i] = new SectorInstance[COL];
	}
	

	COORD_DOUBLE LT;		// 좌상
	LT.x = 0;
	LT.z = 0;

	COORD_DOUBLE RB;		// 우하
	RB.x = GRID_SIZE;
	RB.z = -(GRID_SIZE);

	int i, j;
	int num = 1;

///////////// 각 섹터의 좌상, 우하 좌표 셋팅 /////////////
	for (i = 0; i < ROW; i++)
	{
		for (j = 0; j < COL; j++)
		{
			sectors[i][j].num = num++;	// 번호 설정

			sectors[i][j].leftTop     = LT;
			sectors[i][j].rightBottom = RB;

			LT.x += GRID_SIZE;
			RB.x += GRID_SIZE;
		}

		LT.x = 0;
		RB.x = GRID_SIZE;

		LT.z -= GRID_SIZE;
		RB.z -= GRID_SIZE;
	}


////////////// 인접 섹터 추가 //////////////
	int x = 0;
	int y = 0;

	// 모든 섹터에 대해서 반복한다.
	while (x < ROW)
	{
		for (i = 0; i < ROW; i++)
		{
			for (j = 0; j < COL; j++)
			{
				// 인접하지 않은 섹터는 건너 뜀(차이가 2이상 나면 인접하지 않음)
				if (abs(x - i) >= 2 || abs(y - j) >= 2)
					continue;

				// 본인 섹터는 건너 뜀
				if (x == i && y == j)
					continue;

				// 인접 섹터에 추가
				sectors[x][y].adjacencySector.emplace_back(&sectors[i][j]);
			}
		}

		y++;	// 일단 열을 증가시키고

		// 인덱스가 4가 넘어가면 안되므로 행을 1증가시키고 열은 0으로 셋팅한다.
		if (y % 4 == 0)
		{
			x++;
			y = 0;
		}
	}
}

C_Sector::~C_Sector()
{
	// 우선 행 갯수만큼 순차적으로 지워주고
	for (int i = 0; i < ROW; i++)
		delete[] sectors[i];

	// 전체를 지워줌
	delete[]sectors;
}

list<C_ClientInfo*> C_Sector::GetSectorPlayerList(INDEX _idx)
{
	list<C_ClientInfo*> mergedList;     // 통으로 합쳐서 리턴시킬 리스트
	
	if ((_idx.i < 0) || (_idx.i >= ROW)
		|| (_idx.j < 0) || (_idx.j) >= COL)
	{
		printf("Delete()에서 옳지 않은 인덱스 발견\t%d,%d\n", _idx.i, _idx.j);
		return mergedList;
	}

	// 1. 일단 이 섹터의 플레이어 리스트 먼저 추가
	if (!sectors[_idx.i][_idx.j].playerList.empty())
	{
		// 복사본 만들어서 그놈을 리스트랑 병합시킨다.(이 부분도 리팩토링이 가능하면 복사생성자 호출하지 않는 방향으로 해보자)
		sectors[_idx.i][_idx.j].playerList.sort();   // merge전에 정렬해야함(여기선 원본정렬)
		list<C_ClientInfo*> copyList(sectors[_idx.i][_idx.j].playerList);
		mergedList.merge(copyList);
	}

	// 2. 인접 섹터의 플레이어 리스트도 추가
	SectorInstance* sector = nullptr;
	for (int i = 0; i < sectors[_idx.i][_idx.j].adjacencySector.size(); i++)
	{
		sector = sectors[_idx.i][_idx.j].adjacencySector.at(i);

		if (!sector->playerList.empty())
		{
			sector->playerList.sort();   // merge전에 정렬해야함(여기선 원본정렬)
			list<C_ClientInfo*> copyList(sector->playerList);
			mergedList.merge(copyList);
		}
	}

	return mergedList;   // 합친거를 리턴해준다.
}

byte C_Sector::GetMovedSectorPlayerList(INDEX _beforeIdx, INDEX _curIdx,
	list<C_ClientInfo*>& _enterList, list<C_ClientInfo*>& _exitList)
{
	if ((_beforeIdx.i < 0) || (_beforeIdx.i >= ROW)
		|| (_beforeIdx.j < 0) || (_beforeIdx.j) >= COL
		|| (_curIdx.i < 0) || (_curIdx.i >= ROW)
		|| (_curIdx.j < 0) || (_curIdx.j) >= COL)
	{
		printf("GetMovedSectorPlayerList()에서 옳지 않은 인덱스 발견\n");
		return false;
	}

	// 1. 각각 퇴장할 섹터, 입장한 섹터를 저장(해당섹터+인접섹터)
	vector<SectorInstance*> exitVector(sectors[_beforeIdx.i][_beforeIdx.j].adjacencySector);
	exitVector.emplace_back(&sectors[_beforeIdx.i][_beforeIdx.j]);

	vector<SectorInstance*> enterVector(sectors[_curIdx.i][_curIdx.j].adjacencySector);
	enterVector.emplace_back(&sectors[_curIdx.i][_curIdx.j]);

	// 1-1. 먼저 exitVector를 지워줄것이기 때문에 지워지기 전의 원본을 가지고 있어야 한다. (exitVector원본)
	vector<SectorInstance*> originalExitVector(exitVector);

	// 1-2. 입장한 인접섹터에 대한 원본을 가지고 있어야 playerBit를 활성화 시킬 수 있다. (enterVector원본)
	vector<SectorInstance*> originalEnterVector(enterVector);

	// 2. 먼저 공통되는 부분을 지워서 퇴장한 벡터먼저 구한다.
	for (auto iter = enterVector.begin(); iter != enterVector.end(); ++iter)
		exitVector.erase(remove(exitVector.begin(), exitVector.end(), *iter), exitVector.end());

	for (auto iter = originalExitVector.begin(); iter != originalExitVector.end(); ++iter)
		enterVector.erase(remove(enterVector.begin(), enterVector.end(), *iter), enterVector.end());

	// 3. 마지막으로 입장 리스트에 추가
	_enterList.clear();
	list<C_ClientInfo*>::iterator iter = _enterList.begin();
	for (int i = 0; i < enterVector.size(); i++)
	{
		if (!enterVector[i]->playerList.empty())
			iter = _enterList.insert(iter, enterVector[i]->playerList.begin(), enterVector[i]->playerList.end());
	}

	// 3. 마지막으로 퇴장 리스트에 추가
	_exitList.clear();
	iter = _exitList.begin();
	for (int i = 0; i < exitVector.size(); i++)
	{
		if (!exitVector[i]->playerList.empty())
			iter = _exitList.insert(iter, exitVector[i]->playerList.begin(), exitVector[i]->playerList.end());
	}

	// 4. 입장한 섹터에 있는 플레이어 비트를 가져와서 리턴
	return FlagPlayerBit(originalEnterVector);
}

void C_Sector::Add(C_ClientInfo* _player, INDEX& _index)
{
	if ((_index.i < 0) || (_index.i >= ROW)
		|| (_index.j < 0) || (_index.j) >= COL)
	{
		printf("Add()에서 옳지 않은 인덱스 발견\t%d,%d\n", _index.i, _index.j);
		return;
	}

   sectors[_index.i][_index.j].playerList.emplace_back(_player);
}

void C_Sector::Remove(C_ClientInfo* _player, INDEX _index)
{
	if ((_index.i < 0) || (_index.i >= ROW)
		|| (_index.j < 0) || (_index.j) >= COL)
	{
		printf("Delete()에서 옳지 않은 인덱스 발견\t%d,%d\n", _index.i, _index.j);
		return;
	}

	sectors[_index.i][_index.j].playerList.remove(_player);
}

byte C_Sector::FlagPlayerBit(vector<SectorInstance*>&_enterSector)
{
	byte playerBit = 0;

	C_ClientInfo* otherPlayer = nullptr;
	// 입장한 인접 섹터를 다 돌면서
	for (int i = 0; i < _enterSector.size(); i++)
	{
		// 해당 플레이어 리스트가 비지 않았다면
		if (!_enterSector[i]->playerList.empty())
		{
			// 그 플레이어 리스트를 다 돌면서 어느 플레이어가 있는지 bit를 활성화 시킨다.
			for (auto iter = _enterSector[i]->playerList.begin(); iter != _enterSector[i]->playerList.end(); ++iter)
			{
				otherPlayer = *iter;

				// 플레이어 넘버를 읽어서 bit를 활성화 시킴(ex : 1011 << 1,3,4 플레이어가 같은 섹터에 있음)
				switch (otherPlayer->GetPlayerInfo()->GetPlayerNum())
				{
				case 1:
					playerBit |= PLAYER_1;
					break;

				case 2:
					playerBit |= PLAYER_2;
					break;

				case 3:
					playerBit |= PLAYER_3;
					break;

				case 4:
					playerBit |= PLAYER_4;
					break;
				}
			}
		}
	}

	return playerBit;
}

byte C_Sector::FlagPlayerBit(list<C_ClientInfo*>& _playerList)
{
	byte playerBit = 0;

	C_ClientInfo* otherPlayer = nullptr;
	// 입장한 인접 섹터를 다 돌면서 어느 플레이어가 있는지 bit를 활성화 시킨다.
	for (auto iter = _playerList.begin(); iter != _playerList.end(); ++iter)
	{
		otherPlayer = *iter;

		// 플레이어 넘버를 읽어서 bit를 활성화 시킴(ex : 1011 << 1,3,4 플레이어가 같은 섹터에 있음)
		switch (otherPlayer->GetPlayerInfo()->GetPlayerNum())
		{
		case 1:
			playerBit |= PLAYER_1;
			break;

		case 2:
			playerBit |= PLAYER_2;
			break;

		case 3:
			playerBit |= PLAYER_3;
			break;

		case 4:
			playerBit |= PLAYER_4;
			break;
		}
	}

	return playerBit;
}