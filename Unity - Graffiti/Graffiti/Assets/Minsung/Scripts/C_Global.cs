using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class C_Global
{
	public const int BUFSIZE = 4096;   // 버퍼 사이즈


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
