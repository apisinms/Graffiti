using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class C_Global
{
    public const int BUFSIZE = 4096;   // 버퍼 사이즈
    //public const int MAX_PLAYER = 4;    // 플레이어 수
    public const int MAX_CHARACTER = 4;            // 최대 캐릭터 수
    public const float interpolation_LeftRot = 12.5f;
    public const float interpolation_RightRot = 10.0f;
    public const float interpolation_Pos = 0.175f;
    //public const float interpolation_Rot = 40.0f;
    public const float amingSpeed = 0.35f;

    public const float camPosY = 7.0f;
    public const float camPosZ = -5.3f;

    public const float packetInterval = 0.1f;
    public const float carHitDrag = 1.0f;
    public const float normalDrag = 20.0f;

    // 큐에 들어갈 정보
    public struct QueueInfo
    {
        public byte[] packet;

        public QueueInfo(byte[] _packet)
        {
            packet = _packet;
        }
    }

	public enum PLAYER_BIT : byte
	{
		PLAYER_1 = (1 << 3),
		PLAYER_2 = (1 << 2),
		PLAYER_3 = (1 << 1),
		PLAYER_4 = (1 << 0),
	}

    public enum GameType : int
    {
        _2vs2,
        _1vs1,
    }
}