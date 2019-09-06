#pragma once

// ������ �����ϴ� ���� �ν��Ͻ�
struct SectorInstance
{
	friend class C_Sector;
private:
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

public:
	C_Sector();
	~C_Sector();

	// ��ġ������ ���� ���� �ε����� ��´�.
	inline INDEX GetIndex(double _posX, double _posZ)
	{
		INDEX index;

		index.i = (int)(abs(_posZ / GRID_SIZE));
		index.j = (int)(abs(_posX / GRID_SIZE));

		printf("���� �ε��� : %d, %d\n", index.i, index.j);

		return index;
	}

	list<C_ClientInfo*> GetMergedPlayerList(INDEX _idx);	// �ε����� ���� �ش� ���� + ���� ������ �÷��̾� ����Ʈ�� �ϳ��� �����Ͽ� ��������

	void Add(C_ClientInfo* _player, INDEX& _index);
	void Delete(C_ClientInfo* _player, INDEX _index);
};