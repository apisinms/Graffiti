using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class C_Global
{
	public const int IDSIZE = 255;
	public const int PWSIZE = 255;
	public const int NICKNAMESIZE = 255;
	public const int BUFSIZE = 4096;   // 버퍼 사이즈

	public struct QueueInfo
	{
		byte[] packet;
		public byte[] GetPacket { get { return packet; } }

		public QueueInfo(byte[] _packet) { packet = _packet; }
	}
}
