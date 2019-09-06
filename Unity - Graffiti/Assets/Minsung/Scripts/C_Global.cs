using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class C_Global
{
	public const int BUFSIZE = 4096;   // 버퍼 사이즈
	public const int MAX_PLAYER = 4;    // 플레이어 수

	public const float interpolation_Pos = 0.175f;
	public const float interpolation_Rot = 20.0f;
	public const float amingSpeed = 0.35f;


	// 큐에 들어갈 정보
	public struct QueueInfo
	{
		public byte[] packet;

		public QueueInfo(byte[] _packet)
		{
			packet = _packet;
		}
	}
}
