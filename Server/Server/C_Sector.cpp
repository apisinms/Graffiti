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
	int num = 1;

///////////// �� ������ �»�, ���� ��ǥ ���� /////////////
	for (i = 0; i < ROW; i++)
	{
		for (j = 0; j < COL; j++)
		{
			sectors[i][j].num = num++;	// ��ȣ ����

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

list<C_ClientInfo*> C_Sector::GetSectorPlayerList(INDEX _idx)
{
	list<C_ClientInfo*> mergedList;     // ������ ���ļ� ���Ͻ�ų ����Ʈ

	// 1. �ϴ� �� ������ �÷��̾� ����Ʈ ���� �߰�
	if (!sectors[_idx.i][_idx.j].playerList.empty())
	{
		// ���纻 ���� �׳��� ����Ʈ�� ���ս�Ų��.(�� �κе� �����丵�� �����ϸ� ��������� ȣ������ �ʴ� �������� �غ���)
		sectors[_idx.i][_idx.j].playerList.sort();   // merge���� �����ؾ���(���⼱ ��������)
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
			sector->playerList.sort();   // merge���� �����ؾ���(���⼱ ��������)
			list<C_ClientInfo*> copyList(sector->playerList);
			mergedList.merge(copyList);
		}
	}

	return mergedList;   // ��ģ�Ÿ� �������ش�.
}

byte C_Sector::GetMovedSectorPlayerList(INDEX _beforeIdx, INDEX _curIdx,
	list<C_ClientInfo*>& _enterList, list<C_ClientInfo*>& _exitList)
{
	// 1. ���� ������ ����, ������ ���͸� ����(�ش缽��+��������)
	vector<SectorInstance*> exitVector(sectors[_beforeIdx.i][_beforeIdx.j].adjacencySector);
	exitVector.push_back(&sectors[_beforeIdx.i][_beforeIdx.j]);

	vector<SectorInstance*> enterVector(sectors[_curIdx.i][_curIdx.j].adjacencySector);
	enterVector.push_back(&sectors[_curIdx.i][_curIdx.j]);

	// 1-1. ���� exitVector�� �����ٰ��̱� ������ �������� ���� ������ ������ �־�� �Ѵ�. (exitVector����)
	vector<SectorInstance*> originalExitVector(exitVector);

	// 1-2. ������ �������Ϳ� ���� ������ ������ �־�� playerBit�� Ȱ��ȭ ��ų �� �ִ�. (enterVector����)
	vector<SectorInstance*> originalEnterVector(enterVector);

	// 2. ���� ����Ǵ� �κ��� ������ ������ ���͸��� ���Ѵ�.
	for (auto i = enterVector.begin(); i < enterVector.end(); ++i)
		exitVector.erase(remove(exitVector.begin(), exitVector.end(), *i), exitVector.end());

	for (auto i = originalExitVector.begin(); i < originalExitVector.end(); ++i)
		enterVector.erase(remove(enterVector.begin(), enterVector.end(), *i), enterVector.end());

	// 3. ���������� ���� ����Ʈ�� �߰�
	_enterList.clear();
	list<C_ClientInfo*>::iterator iter = _enterList.begin();
	for (int i = 0; i < enterVector.size(); i++)
	{
		if (!enterVector[i]->playerList.empty())
			iter = _enterList.insert(iter, enterVector[i]->playerList.begin(), enterVector[i]->playerList.end());
	}

	// 3. ���������� ���� ����Ʈ�� �߰�
	_exitList.clear();
	iter = _exitList.begin();
	for (int i = 0; i < exitVector.size(); i++)
	{
		if (!exitVector[i]->playerList.empty())
			iter = _exitList.insert(iter, exitVector[i]->playerList.begin(), exitVector[i]->playerList.end());
	}

	// 4. ������ ���Ϳ� �ִ� �÷��̾� ��Ʈ�� �����ͼ� ����
	return FlagPlayerBit(originalEnterVector);
}

void C_Sector::Add(C_ClientInfo* _player, INDEX& _index)
{
	INDEX index = _player->GetIndex();

	if ((_index.i < 0) || (_index.i >= ROW)
		|| (_index.j < 0) || (_index.j) >= COL)
	{
		printf("Add()���� ���� ���� �ε��� �߰�\t%d,%d", _index.i, _index.j);
		return;
	}

   sectors[_index.i][_index.j].playerList.emplace_front(_player);
}

void C_Sector::Delete(C_ClientInfo* _player, INDEX _index)
{
	if ((_index.i < 0) || (_index.i >= ROW)
		|| (_index.j < 0) || (_index.j) >= COL)
	{
		printf("Delete()���� ���� ���� �ε��� �߰�\t%d,%d", _index.i, _index.j);
		return;
	}

   //// ����Ʈ�� ��ȸ�ϸ�
   //for (auto iter = sectors[_index.i][_index.j].playerList.begin(); 
   //   iter != sectors[_index.i][_index.j].playerList.end(); ++iter)
   //{
   //   // �ڽ��� ã�� ���
   //   if (*iter == _player)
   //   {
   //      sectors[_index.i][_index.j].playerList.erase(iter++);         // ���� �÷��̾� ����Ʈ���� ����(iterator������ �ݵ�� iter++�� ����� ��!)
   //      break;
   //   }
   //}
	//printf("size=%d\n", sectors[_index.i][_index.j].playerList.size());

	sectors[_index.i][_index.j].playerList.remove(_player);
}

//////// private method
byte C_Sector::FlagPlayerBit(vector<SectorInstance*>_enterSector)
{
	byte playerBit = 0;

	// ������ ���� ���͸� �� ���鼭
	for (int i = 0; i < _enterSector.size(); i++)
	{
		// �ش� �÷��̾� ����Ʈ�� ���� �ʾҴٸ�
		if (!_enterSector[i]->playerList.empty())
		{
			// �� �÷��̾� ����Ʈ�� �� ���鼭 ��� �÷��̾ �ִ��� bit�� Ȱ��ȭ ��Ų��.
			for (auto iter = _enterSector[i]->playerList.begin(); iter != _enterSector[i]->playerList.end(); ++iter)
			{
				// �÷��̾� �ѹ��� �о bit�� Ȱ��ȭ ��Ŵ(ex : 1011 << 1,3,4 �÷��̾ ���� ���Ϳ� ����)
				switch (((C_ClientInfo*)(*iter))->GetPosition()->playerNum)
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