#pragma once

// 개별로 존재하는 섹터 인스턴스
struct SectorInstance
{
	friend class C_Sector;
private:
	int num;							// 섹터 번호
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

private:
	byte FlagPlayerBit(vector<SectorInstance*> _enterSector);

public:
	C_Sector();
	~C_Sector();

	// 위치정보를 토대로 섹터 인덱스를 얻는다.
	inline bool GetIndex(const INDEX _beforeIdx, INDEX& _newIdx, double _posX, double _posZ)
	{
		// 맵의 끝 부분을 넘어갔다면 이전 인덱스를 그대로 저장시키고, false를 리턴한다.
		if (sectors[0][0].leftTop.x > _posX || sectors[0][0].leftTop.z < _posZ ||
			sectors[ROW - 1][COL - 1].rightBottom.x < _posX ||
			sectors[ROW - 1][COL - 1].rightBottom.z > _posZ)
		{
			printf("좌표오류!x:%f, z:%f\n", _posX, _posZ);
			_newIdx = _beforeIdx;
			return false;
		}

		_newIdx.i = (int)(abs(_posZ / GRID_SIZE));
		_newIdx.j = (int)(abs(_posX / GRID_SIZE));

		printf("GetIndex:%d, %d\n", _newIdx.i, _newIdx.j);

		// 섹터 범위 안에 있는 인덱스이면 true리턴.
		if (_newIdx.i >= 0 && _newIdx.i < ROW
			&&	_newIdx.j >= 0 && _newIdx.j < COL)
		{
			return true;
		}

		// 그것도 아니면 false임(2중으로 예외체크)
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
			printf("GetLeftTop 인덱스에러 %d, %d\n", _index.i, _index.j);
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
			printf("GetLeftTop 인덱스에러 %d, %d\n", _index.i, _index.j);
			COORD_DOUBLE ret;
			return ret;
		}
	}

	// 인덱스를 토대로 해당 섹터 + 인접 섹터의 플레이어 리스트를 하나로 병합하여 리턴해줌
	list<C_ClientInfo*> GetSectorPlayerList(INDEX _Idx);

	byte GetMovedSectorPlayerList(INDEX _beforeIdx, INDEX _curIdx,
		list<C_ClientInfo*>& _enterList, list<C_ClientInfo*>& _exitList);

	void Add(C_ClientInfo* _player, INDEX& _index);
	void Delete(C_ClientInfo* _player, INDEX _index);

	// 자신의 섹터에서 나감 처리
	inline void LeaveProcess(C_ClientInfo* _player, INDEX _index)
	{
		sectors[_index.i][_index.j].playerList.remove(_player);
	}
};