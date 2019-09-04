using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class C_Global
{
	public const int BUFSIZE = 4096;   // 버퍼 사이즈
	public const int MAX_PLAYER = 4;	// 플레이어 수


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
