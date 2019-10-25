using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

/*
public enum _WEAPONS : sbyte
{
    NODATA = -1,

    // main weapons
    AR,
    SG,
    SMG,
    MAIN_MAX_LENGTH,

    // sub weapons
    TRAP,
    GRENADE,
    SUB_MAX_LENGTH,
}
*/
public interface IMainWeaponType
{
    IEnumerator ActionFire(int _index);
    void CheckFireRange(GameObject _obj_bullet, BulletCollision._BULLET_CLONE_INFO _info_bullet, int _index);
    void ReloadAmmo(int _index);
    void ApplyDamage(int _type, int _index);
}

public class WeaponManager : MonoBehaviour, IMainWeaponType
{
    public const int WEAPON_NAME_SIZE = 32; // 무기 이름 길이

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WeaponInfo
    {
        [MarshalAs(UnmanagedType.I4)]
        public int num;            // 무기 번호

        [MarshalAs(UnmanagedType.I4)]
        public int numOfPattern;   // 패턴 갯수

		[MarshalAs(UnmanagedType.I4)]
		public int bulletPerShot;  // 한 번 쏘면 몇 발 나가는지

		[MarshalAs(UnmanagedType.I4)]
        public int maxAmmo;        // 최대 총알

        [MarshalAs(UnmanagedType.R4)]
        public float fireRate;     // 발사 속도

        [MarshalAs(UnmanagedType.R4)]
        public float damage;       // 데미지

        [MarshalAs(UnmanagedType.R4)]
        public float accuracy;     // 정확도

        [MarshalAs(UnmanagedType.R4)]
        public float range;        // 사정거리

        [MarshalAs(UnmanagedType.R4)]
        public float speed;        // 탄속

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = WEAPON_NAME_SIZE)]
        public string weaponName; // 무기 이름

        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(WeaponInfo))];

            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (WeaponInfo)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(WeaponInfo));
            gch.Free();
        }
    };

    public static WeaponManager instance;
    public IMainWeaponType[] mainWeaponType { get; set; }
    public Component[] cn_mainWeaponList { get; set; }
    public GameObject obj_mainWeaponList;
    public GameObject[] obj_weaponPrefabsList = new GameObject[6];

    public readonly string[] weaponsTag = new string[3];
    public int myIndex { get; set; } //가독성을위해 하나 더만들어줌

    #region WEAPONS_COR
    public Coroutine[] curMainActionCor { get; set; }
    #endregion

    #region WEAPONS_TYPE
    public _WEAPONS[] mainWeapon { get; set; }
    public _WEAPONS[] subWeapon { get; set; }
    public GameObject[] obj_mainWeapon { get; set; }
    public GameObject[] obj_subWeapon { get; set; }
    #endregion

    // 공용으로 사용하는 무기의 속성(정보)
    #region WEAPON_INFO
    public WeaponInfo weaponInfoAR;      // AR 정보(공용)
    public WeaponInfo weaponInfoSG;      // SG 정보(공용)
    public WeaponInfo weaponInfoSMG;	// SMG 정보(공용)
	#endregion

	// 누가, 몇 발의 총알을 맞았는지 담을 구조체. (로컬 플레이어만 쓴다)
	private NetworkManager.BulletCollisionChecker colChecker = new NetworkManager.BulletCollisionChecker();

	void Awake()
    {
        if (instance == null)
            instance = this;

        myIndex = GameManager.instance.myIndex;
        mainWeaponType = new IMainWeaponType[C_Global.MAX_PLAYER];

        weaponsTag[0] = "Ar"; weaponsTag[1] = "Sg"; weaponsTag[2] = "Smg";
        
        cn_mainWeaponList = obj_mainWeaponList.GetComponents<Component>();
    
        Initialization(C_Global.MAX_PLAYER);
    }

    void Start()
    {
#if NETWORK
        WeaponInfo[] weapons = BridgeClientToServer.instance.GetTempWeapons;

        for (int i = 0; i < weapons.Length; i++)
        {
            switch (weapons[i].weaponName.ToString())
            {
                case "AR":
                    weaponInfoAR = weapons[i];
                    break;

                case "SG":
                    weaponInfoSG = weapons[i];
                    break;

                case "SMG":
                    weaponInfoSMG = weapons[i];
                    break;

                ///////// 보조무기
                case "TRAP":
                    break;

                case "GRENADE":
                    break;
            }
        }
	//// 여기 else문에 비 네트워크시 적용할 무기 정보 넣으면 됨
#else

#endif
	}

    void Initialization(int _num)
    {
        obj_mainWeapon = new GameObject[_num];
        obj_subWeapon = new GameObject[_num];
        mainWeapon = new _WEAPONS[_num];
        subWeapon = new _WEAPONS[_num];
        curMainActionCor = new Coroutine[_num];

        for (int i = 0; i < _num; i++)
            curMainActionCor[i] = null;
    }

    public void SetMainWeapon(IMainWeaponType _weaponType, int _index)
    {
        mainWeaponType[_index] = _weaponType; //내 상태 업데이트.

        int weaponIndex = (int)mainWeapon[_index]; //주무기의 생성

        if(obj_mainWeapon[_index] != null) //가지고있던 총오브젝트를 지우고
            Destroy(obj_mainWeapon[_index]);
      
        obj_mainWeapon[_index] = Instantiate(obj_weaponPrefabsList[weaponIndex], PlayersManager.instance.obj_players[_index].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
		obj_mainWeapon[_index].transform.SetParent(PlayersManager.instance.obj_players[_index].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
    }

    /* 보조무기 마찬가지로
    public void SetSubWeapon(IMainWeaponType _weaponType, int _index)
    {
        //index = (int)subWeapon[i] - 1; //보조무기의 생성
        //obj_subWeapon[i] = Instantiate(obj_weaponPrefabsList[index], PlayersManager.instance.obj_players[myIndex].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
        //obj_subWeapon[i].transform.SetParent(PlayersManager.instance.obj_players[myIndex].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
    }
    */

    public IEnumerator ActionFire(int _index) //선택된 총의 파이어액션
    {
        yield return mainWeaponType[_index].ActionFire(_index);
    }

    public void CheckFireRange(GameObject _obj_bullet, BulletCollision._BULLET_CLONE_INFO _info_bulletint, int _index) //선택된 총의 사거리검사
    {
        mainWeaponType[_index].CheckFireRange(_obj_bullet, _info_bulletint, _index);
    }

    public void ReloadAmmo(int _index)
    {
        mainWeaponType[_index].ReloadAmmo(_index);
    }

    public void ApplyDamage(int _type, int _index) //해당 총별로 총에 맞았을때 데미지를 실질적으로 깎음.
    {
        mainWeaponType[_index].ApplyDamage(_type, _index);
    }

	/////////////////////////////// BulletCollisionChecker 구조체 전용
	public NetworkManager.BulletCollisionChecker GetCollisionChecker()
	{
		return colChecker;
	}

	public void SetCollisionChecker(string _playerTag)
	{
		int hitPlayerNum = SetPlayerBit(_playerTag);   // 1. 맞은 플레이어 비트 활성화

		if (hitPlayerNum != -1)
		{
			IncPlayerHitCountBit(hitPlayerNum);
		}
	}

    // byte형의 플레이어 비트를 활성화 함
    public int SetPlayerBit(string _playerTag)
    {
        int hitPlayerNum = -1;
        switch (_playerTag)
        {
            case "Player1":
                {
                    colChecker.playerBit |= (byte)C_Global.PLAYER_BIT.PLAYER_1;
                    hitPlayerNum = 1;
                }
                break;

            case "Player2":
                {
                    colChecker.playerBit |= (byte)C_Global.PLAYER_BIT.PLAYER_2;
                    hitPlayerNum = 2;
                }
                break;

            case "Player3":
                {
                    colChecker.playerBit |= (byte)C_Global.PLAYER_BIT.PLAYER_3;
                    hitPlayerNum = 3;
                }
                break;

            case "Player4":
                {
                    colChecker.playerBit |= (byte)C_Global.PLAYER_BIT.PLAYER_4;
                    hitPlayerNum = 4;
                }
                break;
        }

        return hitPlayerNum;
    }
    /*
	public int SetPlayerBit(string _playerTag)
	{
		int hitPlayerNum = -1;
		switch (_playerTag)
		{
			case "Player1":
				{
					// 나랑 같은 팀이 아니면 세팅
					if ((myIndex + 1) != 2)
					{
					colChecker.playerBit |= (byte)C_Global.PLAYER_BIT.PLAYER_1;
					hitPlayerNum = 1;
					}
				}
				break;


			case "Player2":
				{
					// 나랑 같은 팀이 아니면 세팅
					if ((myIndex + 1) != 1)
					{
						colChecker.playerBit |= (byte)C_Global.PLAYER_BIT.PLAYER_2;
						hitPlayerNum = 2;
					}
				}
				break;

			case "Player3":
				{
					// 나랑 같은 팀이 아니면 세팅
					if ((myIndex + 1) != 4)
					{
						colChecker.playerBit |= (byte)C_Global.PLAYER_BIT.PLAYER_3;
						hitPlayerNum = 3;
					}
				}
				break;

			case "Player4":
				{
					// 나랑 같은 팀이 아니면 세팅
					if ((myIndex + 1) != 3)
					{
						colChecker.playerBit |= (byte)C_Global.PLAYER_BIT.PLAYER_4;
						hitPlayerNum = 4;
					}
				}
				break;
		}

		return hitPlayerNum;
	}
    */

    // player의 HitCountBit(맞은 횟수 비트)를 증가한다.
    public void IncPlayerHitCountBit(int _hitPlayerNum)
    {
        int Shifter = 8 * (C_Global.MAX_PLAYER - _hitPlayerNum);    // 이동 연산에 필요한 값

        byte bitEraseMask = 0x00;   // 8비트 지우기 마스크(00000000)


        // 1. 기존에 해당 플레이어의 hit횟수를 가져온다.
        int beforeCount = (colChecker.playerHitCountBit >> Shifter);    // 한 대 맞은 누적 카운트를 저장하고

        // 2. 기존 플레이어의 hit 횟수를 0으로 만든다(해당 플레이어의 범위만)
        colChecker.playerHitCountBit &= (bitEraseMask << Shifter);

        beforeCount++;  // 한 대 더 맞았으니 누적한다.

        // 32비트를 4명의 플레이어가 8비트 단위로 끊어서 저장한다.
        colChecker.playerHitCountBit |= (beforeCount << Shifter);
    }

    // 총알 충돌 체커 구조체 초기화
    public void ResetCollisionChecker()
	{
		colChecker.playerBit = 0;
		colChecker.playerHitCountBit = 0;
	}
}
