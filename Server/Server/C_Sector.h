#pragma once

// ������ �����ϴ� ���� �ν��Ͻ�
struct SectorInstance
{
public:
	struct POINT
	{
		double x;
		double z;
	};

	SectorInstance::POINT leftTop;				// �»� ��ǥ
	SectorInstance::POINT rightBottom;			// ���� ��ǥ

	list<C_ClientInfo*>playerList;				// ���Ϳ� �����ϴ� �÷��̾� ����Ʈ
	//list<C_Item*>itemList;					// ���Ϳ� �����ϴ� ������ ����Ʈ(�߰��ؾ���)
	//list<C_Bullet*>bulletList;				// ���Ϳ� �����ϴ� �Ѿ� ����Ʈ(�߰����� �ָ���)

	vector<SectorInstance*> adjacencySector;	// ���� ���� ���
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
};