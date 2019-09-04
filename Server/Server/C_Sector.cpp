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

list<C_ClientInfo*> C_Sector::GetMergedPlayerList(INDEX _idx)
{
   list<C_ClientInfo*> mergedList;      // 통으로 합쳐서 리턴시킬 리스트

   // 1. 일단 이 섹터의 플레이어 리스트 먼저 추가
   if (!sectors[_idx.i][_idx.j].playerList.empty())
   {
      // 복사본 만들어서 그놈을 리스트랑 병합시킨다.(이 부분도 리팩토링이 가능하면 복사생성자 호출하지 않는 방향으로 해보자)
      sectors[_idx.i][_idx.j].playerList.sort();   // merge전에 정렬해야함(여기선 원본정렬)
      list<C_ClientInfo*> copyList(sectors[_idx.i][_idx.j].playerList);
      mergedList.merge(copyList);
   }

   // 2. 인접 섹터의 플레이어 리스트도 추가
   SectorInstance* sector;
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

void C_Sector::Add(C_ClientInfo* _player, INDEX& _index)
{
   sectors[_index.i][_index.j].playerList.emplace_front(_player);
}

void C_Sector::Delete(C_ClientInfo* _player, INDEX _index)
{
   // 리스트를 순회하며
   for (auto iter = sectors[_index.i][_index.j].playerList.begin(); 
      iter != sectors[_index.i][_index.j].playerList.end(); ++iter)
   {
      // 자신을 찾은 경우
      if (*iter == _player)
      {
         sectors[_index.i][_index.j].playerList.erase(iter++);         // 방의 플레이어 리스트에서 제거(iterator때문에 반드시 iter++을 해줘야 함!)
         break;
      }
   }
}