#pragma once

// ������ �����ϴ� ���� �ν��Ͻ�
struct SectorInstance
{
	friend class C_Sector;
private:
	int num;							// ���� ��ȣ
	COORD_DOUBLE leftTop;				// �»� ��ǥ
	COORD_DOUBLE rightBottom;			// ���� ��ǥ

	vector<SectorInstance*> adjacencySector;	// ���� ���� ���

	list<C_ClientInfo*>playerList;				// ���Ϳ� �����ϴ� �÷��̾� ����Ʈ
	//list<C_Item*>itemList;					// ���Ϳ� �����ϴ� ������ ����Ʈ(�߰��ؾ���)
	//list<C_Bullet*>bulletList;				// ���Ϳ� �����ϴ� �Ѿ� ����Ʈ(�߰����� �ָ���)

};

// ������ �����ϴ� ���� �ν��Ͻ��� �Ѱ��ϴ� ����Ŭ����
class C_Sector
{
private:
	static const int ROW   = 4;	// ��
	static const int COL   = 4;	// ��

	const double GRID_SIZE = 2.5;	// ���� ������


	SectorInstance** sectors;	// ��ü ����

private:
	byte FlagPlayerBit(vector<SectorInstance*> _enterSector);

public:
	C_Sector();
	~C_Sector();

	// ��ġ������ ���� ���� �ε����� ��´�.
	inline bool GetIndex(const INDEX _beforeIdx, INDEX& _newIdx, double _posX, double _posZ)
	{
		// ���� �� �κ��� �Ѿ�ٸ� ���� �ε����� �״�� �����Ű��, false�� �����Ѵ�.
		if (sectors[0][0].leftTop.x > _posX || sectors[0][0].leftTop.z < _posZ ||
			sectors[ROW - 1][COL - 1].rightBottom.x < _posX ||
			sectors[ROW - 1][COL - 1].rightBottom.z > _posZ)
		{
			printf("��ǥ����!x:%f, z:%f\n", _posX, _posZ);
			_newIdx = _beforeIdx;
			return false;
		}

		_newIdx.i = (int)(abs(_posZ / GRID_SIZE));
		_newIdx.j = (int)(abs(_posX / GRID_SIZE));

		printf("GetIndex:%d, %d\n", _newIdx.i, _newIdx.j);

		// ���� ���� �ȿ� �ִ� �ε����̸� true����.
		if (_newIdx.i >= 0 && _newIdx.i < ROW
			&&	_newIdx.j >= 0 && _newIdx.j < COL)
		{
			return true;
		}

		// �װ͵� �ƴϸ� false��(2������ ����üũ)
		return false;
	}
	inline COORD_DOUBLE GetLeftTop(INDEX _index)
	{
		if (_index.i >= 0 && _index.i < ROW
			&&	_index.j >= 0 && _index.j < COL)
		{
			return sectors[_index.i][_index.j].leftTop;
		}
		
		else
		{
			printf("GetLeftTop �ε������� %d, %d\n", _index.i, _index.j);
			COORD_DOUBLE ret;
			return ret;
		}
	}
	inline COORD_DOUBLE GetRightBottom(INDEX _index)
	{
		if (_index.i >= 0 && _index.i < ROW
			&&	_index.j >= 0 && _index.j < COL)
		{
			return sectors[_index.i][_index.j].rightBottom;
		}

		else
		{
			printf("GetLeftTop �ε������� %d, %d\n", _index.i, _index.j);
			COORD_DOUBLE ret;
			return ret;
		}
	}

	// �ε����� ���� �ش� ���� + ���� ������ �÷��̾� ����Ʈ�� �ϳ��� �����Ͽ� ��������
	list<C_ClientInfo*> GetSectorPlayerList(INDEX _Idx);

	byte GetMovedSectorPlayerList(INDEX _beforeIdx, INDEX _curIdx,
		list<C_ClientInfo*>& _enterList, list<C_ClientInfo*>& _exitList);

	void Add(C_ClientInfo* _player, INDEX& _index);
	void Delete(C_ClientInfo* _player, INDEX _index);

	// �ڽ��� ���Ϳ��� ���� ó��
	inline void LeaveProcess(C_ClientInfo* _player, INDEX _index)
	{
		sectors[_index.i][_index.j].playerList.remove(_player);
	}
};