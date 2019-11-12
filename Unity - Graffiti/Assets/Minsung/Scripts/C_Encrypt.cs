using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Encrypt : UnityEngine.MonoBehaviour
{
	const int C1 = 52845;
	const int C2 = 22719;
	const int KEY = 78695;


	public static C_Encrypt instance = null;

	private void Start()
	{
		if (instance == null)
			instance = this;

		else
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}


	public bool Encrypt(byte[] _src, byte[] _dest, int _size)
	{
		int key = KEY;

		if (_src == null || _dest == null || _size <= 0)
			return false;


		for (int i = 0; i < _size; i++)
		{
			_dest[i] = (byte)(_src[i] ^ key >> 8);
			key = ((sbyte)_dest[i] + key) * C1 + C2;    // key값을 다시 연산할때에는 sbyte로 변환한 뒤에 연산해야 음수값이 제대로 들어갈 수 있다.
		}

		return true;
	}

	public bool Decrypt(byte[] _src, byte[] _dest, int _size)
	{
		byte prevBlock;
		int key = KEY;


		if (_src == null || _dest == null || _size <= 0)
			return false;

		for (int i = 0; i < _size; i++)
		{
			prevBlock = _src[i];
			_dest[i] = (byte)(_src[i] ^ key >> 8);
			key = ((sbyte)prevBlock + key) * C1 + C2;
		}

		return true;
	}
}
