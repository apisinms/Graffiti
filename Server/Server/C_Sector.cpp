#include "stdafx.h"
#include "C_Sector.h"

/// �ϴ� �� ũ�Ⱑ x 8, z 6�̶�� �����ϰ�, �׸��� x 2.5, z 2.5�� �����ؼ� ���͸� �����Ѵ�.
C_Sector::C_Sector()
{
	// ���� ���������� �����Ҵ�
	sectors = new SectorInstance*[ROW];
	for (int i = 0; i < ROW; i++)
	{
		sectors[i] = new SectorInstance[COL];
		memset(sectors[i], 0, sizeof(SectorInstance) * COL);
	}
	

	SectorInstance::POINT LT;		// �»�
	LT.x = 0;
	LT.z = 0;

	SectorInstance::POINT RB;		// ����
	RB.x = GRID_SIZE;
	RB.z = -(GRID_SIZE);

	int i, j;

///////////// �� ������ �»�, ���� ��ǥ ���� /////////////
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


////////////// ���� ���� �߰� //////////////
	int x = 0;
	int y = 0;

	// ��� ���Ϳ� ���ؼ� �ݺ��Ѵ�.
	while (x < ROW)
	{
		for (i = 0; i < ROW; i++)
		{
			for (j = 0; j < COL; j++)
			{
				// �������� ���� ���ʹ� �ǳ� ��(���̰� 2�̻� ���� �������� ����)
				if (abs(x - i) >= 2 || abs(y - j) >= 2)
					continue;

				// ���� ���ʹ� �ǳ� ��
				if (x == i && y == j)
					continue;

				// ���� ���Ϳ� �߰�
				sectors[x][y].adjacencySector.emplace_back(&sectors[i][j]);
			}
		}

		y++;	// �ϴ� ���� ������Ű��

		// �ε����� 4�� �Ѿ�� �ȵǹǷ� ���� 1������Ű�� ���� 0���� �����Ѵ�.
		if (y % 4 == 0)
		{
			x++;
			y = 0;
		}
	}
}

C_Sector::~C_Sector()
{
	// �켱 �� ������ŭ ���������� �����ְ�
	for (int i = 0; i < ROW; i++)
		delete[] sectors[i];

	// ��ü�� ������
	delete[]sectors;
}