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
		//memset(sectors[i], 0, sizeof(SectorInstance) * COL);
	}
	

	COORD_DOUBLE LT;		// 좌상
	LT.x = 0;
	LT.z = 0;

	COORD_DOUBLE RB;		// 우하
	RB.x = GRID_SIZE;
	RB.z = -(GRID_SIZE);

	int i, j;

///////////// 각 섹터의 좌상, 우하 좌표 셋팅 /////////////
	for (i = 0; i < ROW; i++)
	{
		for (j = 0; j < COL; j++)
		{
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

list<C_ClientInfo*> C_Sector::GetUniquePlayerList(INDEX _IdxA, INDEX _IdxB)
{
	list<C_ClientInfo*> mergedList;				// 통으로 합쳐서 리턴시킬 리스트
	list<C_ClientInfo*> copyList;				// 복사용
	vector<SectorInstance*> uniqueAdjSector;	// 겹치지 않는 인접 섹터

	// 일단 자기 섹터부터 때려박는다.
	uniqueAdjSector.emplace_back(sectors[_IdxA.i][_IdxA.j]);
	uniqueAdjSector.emplace_back(sectors[_IdxB.i][_IdxB.j]);

	// 그 다음 모든 인접 섹터를 다 때려 박는다.
	int i;
	for (i = 0; i < sectors[_IdxA.i][_IdxA.j].adjacencySector.size(); i++)
		uniqueAdjSector.emplace_back(sectors[_IdxA.i][_IdxA.j].adjacencySector.at(i));

	for (i = 0; i < sectors[_IdxB.i][_IdxB.j].adjacencySector.size(); i++)
		uniqueAdjSector.emplace_back(sectors[_IdxB.i][_IdxB.j].adjacencySector.at(i));

	// 정렬한 후에 인접 원소를 모두 유니크하게 만든다.
	sort(uniqueAdjSector.begin(), uniqueAdjSector.end());
	unique(uniqueAdjSector.begin(), uniqueAdjSector.end());

	// 이제 반복문을 돌면서 플레이어 리스트가 비지 않은 섹터를 merge해준다.(merge전 정렬 필수)
	for (i = 0; i < uniqueAdjSector.size(); i++)
	{
		if (!uniqueAdjSector[i]->playerList.empty())
		{
			uniqueAdjSector[i]->playerList.sort();
			copyList = uniqueAdjSector[i]->playerList;
			mergedList.merge(copyList);
		}
	}

	return mergedList;   // 합친거를 리턴해준다.
}

void C_Sector::Add(C_ClientInfo* _player, INDEX& _index)
{
	INDEX index = _player->GetIndex();

	if ((_index.i < 0) || (_index.i >= ROW)
		|| (_index.j < 0) || (_index.j) >= COL)
	{
		printf("Add()에서 옳지 않은 인덱스 발견\t%d,%d", _index.i, _index.j);
		return;
	}

   sectors[_index.i][_index.j].playerList.emplace_front(_player);
}

void C_Sector::Delete(C_ClientInfo* _player, INDEX _index)
{
	if ((_index.i < 0) || (_index.i >= ROW)
		|| (_index.j < 0) || (_index.j) >= COL)
	{
		printf("Delete()에서 옳지 않은 인덱스 발견\t%d,%d", _index.i, _index.j);
		return;
	}

   //// 리스트를 순회하며
   //for (auto iter = sectors[_index.i][_index.j].playerList.begin(); 
   //   iter != sectors[_index.i][_index.j].playerList.end(); ++iter)
   //{
   //   // 자신을 찾은 경우
   //   if (*iter == _player)
   //   {
   //      sectors[_index.i][_index.j].playerList.erase(iter++);         // 방의 플레이어 리스트에서 제거(iterator때문에 반드시 iter++을 해줘야 함!)
   //      break;
   //   }
   //}
	//printf("size=%d\n", sectors[_index.i][_index.j].playerList.size());

	sectors[_index.i][_index.j].playerList.remove(_player);
}