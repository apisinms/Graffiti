#pragma once

// 개별로 존재하는 섹터 인스턴스
struct SectorInstance
{
public:
	struct POINT
	{
		double x;
		double z;
	};

	SectorInstance::POINT leftTop;				// 좌상 좌표
	SectorInstance::POINT rightBottom;			// 우하 좌표

	list<C_ClientInfo*>playerList;				// 섹터에 존재하는 플레이어 리스트
	//list<C_Item*>itemList;					// 섹터에 존재하는 아이템 리스트(추가해야함)
	//list<C_Bullet*>bulletList;				// 섹터에 존재하는 총알 리스트(추가할지 애매함)

	vector<SectorInstance*> adjacencySector;	// 인접 섹터 목록
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
};