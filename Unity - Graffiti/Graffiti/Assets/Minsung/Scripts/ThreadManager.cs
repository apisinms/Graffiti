using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;
using UnityEngine;

public class ThreadManager
{
	private Thread recvThread;
	private static ThreadManager instance;

	private byte[] recvBuf;
	private BinaryReader br;

	private readonly object lockObj = new object();

	public static ThreadManager GetInstance
	{
		get
		{
			if (instance == null)
				instance = new ThreadManager();
				//instance = ScriptableObject.CreateInstance<ThreadManager>();

			return instance;
		}
	}

	public void Init()
	{
		recvBuf = new byte[C_Global.BUFSIZE];
		br = NetworkManager.GetInstance.GetBinaryReader;
		recvThread = new Thread(new ThreadStart(RecvThread));
		recvThread.Start();
	}

	public void End()
	{
		//recvThread.Join();
		recvThread.Abort();
	}

	private void RecvThread()
	{
		int readSize = 0;
		while (true)
		{
			lock (lockObj)
			{
				// 서버로부터 결과 얻기
				readSize = br.ReadInt32();      // 먼저 총 size를 얻어온다.
				recvBuf = br.ReadBytes(readSize);   // 그리고 받아야할 size만큼 byte[]를 얻어온다.

				// 얻은 버퍼를 복호화 진행
				byte[] decryptBuf = new byte[C_Global.BUFSIZE];
				C_Encrypt.GetInstance.Decrypt(recvBuf, decryptBuf, readSize);

				// 복호화된 패킷을 packet이라는 이름의 배열에 저장하고, 이를 큐에 넣음
				byte[] packet = new byte[readSize];
				Buffer.BlockCopy(decryptBuf, 0, packet, 0, readSize);
				NetworkManager.GetInstance.GetQueue.Enqueue(new C_Global.QueueInfo(packet));    // 큐에 집어넣기

				/// 만약 종료 프로토콜같은게 들어왔다면 break한다.
			}
		}
	}
}
