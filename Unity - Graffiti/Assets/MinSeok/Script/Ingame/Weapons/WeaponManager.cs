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
    void CheckFireRange(GameObject _obj_bullet, int _index);
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
    public WeaponInfo weaponInfoSMG;   // SMG 정보(공용)
    #endregion

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

    public void CheckFireRange(GameObject _obj_bullet, int _index) //선택된 총의 사거리검사
    {
        mainWeaponType[_index].CheckFireRange(_obj_bullet, _index);
    }
}
