#pragma once

// 개별로 존재하는 섹터 인스턴스
struct SectorInstance
{
	friend class C_Sector;
private:
	COORD_DOUBLE leftTop;				// 좌상 좌표
	COORD_DOUBLE rightBottom;			// 우하 좌표

	vector<SectorInstance*> adjacencySector;	// 인접 섹터 목록

	list<C_ClientInfo*>playerList;				// 섹터에 존재하는 플레이어 리스트
	//list<C_Item*>itemList;					// 섹터에 존재하는 아이템 리스트(추가해야함)
	//list<C_Bullet*>bulletList;				// 섹터에 존재하는 총알 리스트(추가할지 애매함)

};

// 개별로 존재하는 섹터 인스턴스를 총괄하는 섹터클래스
class C_Sector
{
private:
	static const int ROW   = 4;	// 행
	static const int COL   = 4;	// 열

	const double GRID_SIZE = 2.5;	// 격자 사이즈

	SectorInstance** sectors;	// 전체 섹터

public:
	C_Sector();
	~C_Sector();

	// 위치정보를 토대로 섹터 인덱스를 얻는다.
	inline INDEX GetIndex(double _posX, double _posZ)
	{
		INDEX index;

		index.i = (int)(abs(_posZ / GRID_SIZE));
		index.j = (int)(abs(_posX / GRID_SIZE));

		printf("현재 인덱스 : %d, %d\n", index.i, index.j);

		return index;
	}

	list<C_ClientInfo*> GetMergedPlayerList(INDEX _idx);	// 인덱스를 토대로 해당 섹터 + 인접 섹터의 플레이어 리스트를 하나로 병합하여 리턴해줌

	void Add(C_ClientInfo* _player, INDEX& _index);
	void Delete(C_ClientInfo* _player, INDEX _index);
};