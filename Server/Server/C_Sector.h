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

	list<PlayerInfo*> playerList;
	vector<SectorInstance*> adjacencySector;	// 인접 섹터 목록
};

// 개별로 존재하는 섹터 인스턴스를 총괄하는 섹터클래스
class C_Sector
{
private:
	static const int ROW   = 4;	// 행
	static const int COL   = 4;	// 열

	const double GRID_SIZE = 2.5;	// 격자 사이즈

	//SectorInstance sectors[ROW][COL];
	SectorInstance** sectors;	// 전체 섹터

public:
	C_Sector();
	~C_Sector();
};