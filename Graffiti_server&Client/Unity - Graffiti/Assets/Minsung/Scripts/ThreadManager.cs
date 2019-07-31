using System;
using System.Threading;
using System.IO;

/// <summary>
/// 쓰레드들을 관리하는 쓰레드매니저(싱글톤)
/// </summary>
public class ThreadManager
{
	private Thread recvThread = null;			// 수신전용 쓰레드
	//private BinaryWriter bw   = null;			// 이 클라의 쓰기 스트림		
	private BinaryReader br   = null;			// 이 클라의 읽기 스트림
	private object key        = new object();	// 동기화에 사용할 key

	private static ThreadManager instance;	// 싱글톤을 위해 만드는 인스턴스
	public static ThreadManager GetInstance
	{
		get
		{
			if (instance == null)
			{
				instance = new ThreadManager();
				//instance.bw = NetworkManager.instance.BinaryWriter;
				instance.br = NetworkManager.instance.BinaryReader;
			}


			return instance;
		}
	}

	// 초기화
	public void Init()
	{
		// recv쓰레드를 초기화하고 실행한다.
		if(recvThread == null)
		{
			recvThread = new Thread(new ThreadStart(RecvThread));
			recvThread.Start();
		}
	}

	// 마무리
	public void End()
	{
		//recvThread.Join();
		recvThread.Abort();
	}

	private void RecvThread()
	{
		int readSize = 0;
		byte[] recvBuf;
		while(true)
		{
			lock (key)	// 동기화 적용
			{
				// 먼저 사이즈를 얻음
				readSize = br.ReadInt32();

				// 얻은 사이즈만큼 할당하고 스트림에서 읽어옴
				recvBuf = new byte[readSize];
				recvBuf = br.ReadBytes(readSize);
				
				// 복호화 진행
				C_Encrypt.instance.Decrypt(recvBuf, recvBuf, readSize);

				// 복호화된 패킷을 그 사이즈만큼 새롭게 생성해서 저장하고
				byte[] packet = new byte[readSize];
				Buffer.BlockCopy(recvBuf, 0, packet, 0, readSize);

				// 큐에 담는다.
				NetworkManager.instance.Queue.Enqueue(new C_Global.QueueInfo(packet));
			}
		}
	}
}

