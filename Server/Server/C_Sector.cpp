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
		//memset(sectors[i], 0, sizeof(SectorInstance) * COL);
	}
	

	COORD_DOUBLE LT;		// �»�
	LT.x = 0;
	LT.z = 0;

	COORD_DOUBLE RB;		// ����
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

list<C_ClientInfo*> C_Sector::GetMergedPlayerList(INDEX& _idx, byte& _playerBit)
{
	list<C_ClientInfo*> mergedList;		// ������ ���ļ� ���Ͻ�ų ����Ʈ

	// 1. �ϴ� �� ������ �÷��̾� ����Ʈ ���� �߰�
	if (!sectors[_idx.i][_idx.j].playerList.empty())
	{
		// ���纻 ���� �׳��� ����Ʈ�� ���ս�Ų��.(�� �κе� �����丵�� �����ϸ� ��������� ȣ������ �ʴ� �������� �غ���)
		list<C_ClientInfo*> copyList(sectors[_idx.i][_idx.j].playerList);
		mergedList.merge(copyList);
	}

	// 2. ���� ������ �÷��̾� ����Ʈ�� �߰�
	SectorInstance* sector;
	for (int i = 0; i < sectors[_idx.i][_idx.j].adjacencySector.size(); i++)
	{
		sector = sectors[_idx.i][_idx.j].adjacencySector.at(i);

		if (!sector->playerList.empty())
		{
			list<C_ClientInfo*> copyList(sector->playerList);
			mergedList.merge(copyList);
		}
	}

	_playerBit = 0;
	// 3. ���� �ٹ� ���Ϳ� �ִ� �÷��̾���� bit�� Ȱ��ȭ ���Ѽ� ������ ������.
	for (auto iter = mergedList.begin(); iter != mergedList.end(); ++iter)
	{
		// �÷��̾� �ѹ��� �о bit�� Ȱ��ȭ ��Ŵ(ex : 1011 << 1,3,4 �÷��̾ ���� ���Ϳ� ����)
		switch (((C_ClientInfo*)(*iter))->GetPosition()->playerNum)
		{
		case 1:
			_playerBit |= PLAYER_1;
			break;

		case 2:
			_playerBit |= PLAYER_2;
			break;

		case 3:
			_playerBit |= PLAYER_3;
			break;

		case 4:
			_playerBit |= PLAYER_4;
			break;
		}
	}

	return mergedList;	// ��ģ�Ÿ� �������ش�.
}

void C_Sector::Add(C_ClientInfo* _player, INDEX& _index)
{
	sectors[_index.i][_index.j].playerList.emplace_front(_player);
}

void C_Sector::Delete(C_ClientInfo* _player, INDEX _index)
{
	// ���� ������ ����Ʈ ����
	list<C_ClientInfo*> list = sectors[_index.i][_index.j].playerList;

	// ����Ʈ�� ��ȸ�ϸ�
	int i = 0;
	for (auto iter = list.begin(); iter != list.end(); ++iter, i++)
	{
		// �ڽ��� ã�� ���
		if (*iter == _player)
		{
			list.erase(iter++);			// ���� �÷��̾� ����Ʈ���� ����(iterator������ �ݵ�� iter++�� ����� ��!)
			break;
		}
	}
}

