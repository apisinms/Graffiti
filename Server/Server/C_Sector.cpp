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
		memset(sectors[i], 0, sizeof(SectorInstance) * COL);
	}
	

	SectorInstance::POINT LT;		// 좌상
	LT.x = 0;
	LT.z = 0;

	SectorInstance::POINT RB;		// 우하
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