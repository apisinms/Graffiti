using System.Collections;
using System.Collections.Generic;
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
public partial class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;
    public readonly string[] weaponsTag = new string[3];
    public int myIndex { get; set; } //가독성을위해 하나 더만들어줌
    public GameObject[] obj_weaponPrefabsList = new GameObject[6];

    #region WEAPONS_TYPE
    public _WEAPONS[] mainWeapon { get; set; }
    public _WEAPONS[] subWeapon { get; set; }
    public GameObject[] obj_mainWeapon { get; set; }
    public GameObject[] obj_subWeapon { get; set; }

    #region AR
    public struct _INFO_AR
    {
        public int curAmmo;
        public int maxAmmo;
        public float fireRate;
        public float damage;
        public float accuracy;
        public float range;
    }
    public _INFO_AR[] infoAR { get; set; }
    #endregion

    #region SG
    public struct _INFO_SG
    {
        public int curAmmo;
        public int maxAmmo;
        public float fireRate;
        public float damage;
        public float accuracy;
        public float range;
    }
    public _INFO_SG[] infoSG { get; set; }
    #endregion

    #region SMG
    public struct _INFO_SMG
    {
        public int curAmmo;
        public int maxAmmo;
        public float fireRate;
        public float damage;
        public float accuracy;
        public float range;
    }
    public _INFO_SMG[] infoSMG { get; set; }
    #endregion
    #endregion


    void Awake()
    {
        if (instance == null)
            instance = this;

        myIndex = GameManager.instance.myIndex;
        weaponsTag[0] = "Ar"; weaponsTag[1] = "Sg"; weaponsTag[2] = "Smg";

        Initialization(C_Global.MAX_PLAYER);
        Initialization_Attribute(C_Global.MAX_PLAYER);
    }

    void Initialization(int _num)
    {
        obj_mainWeapon = new GameObject[_num];
        obj_subWeapon = new GameObject[_num];
        mainWeapon = new _WEAPONS[_num];
        subWeapon = new _WEAPONS[_num];

        curCor = null;
        cor_ActionBullet = ActionBullet();

        // !!!!!!!!!!! 서버에서 받은데이터로 초기화해야함  임의로 속성값부여해둠. !!!!!!!!!!
        for (int i = 0; i < _num; i++)
        {
            if (myIndex == i) //내인덱스들의 초기화
            {
                int index;
                mainWeapon[i] = _WEAPONS.AR; //셀렉트웨폰에서 선택했던 무기를 서버에서 받아야함.
                subWeapon[i] = _WEAPONS.GRENADE;

                index = (int)mainWeapon[i]; //주무기의 생성
                obj_mainWeapon[i] = Instantiate(obj_weaponPrefabsList[index], PlayersManager.instance.obj_players[myIndex].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
                obj_mainWeapon[i].transform.SetParent(PlayersManager.instance.obj_players[myIndex].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));

                //index = (int)subWeapon[i] - 1; //보조무기의 생성
                //obj_subWeapon[i] = Instantiate(obj_weaponPrefabsList[index], PlayersManager.instance.obj_players[myIndex].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
                //obj_subWeapon[i].transform.SetParent(PlayersManager.instance.obj_players[myIndex].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
            }
            else //나머지 3명 인덱스의 초기화
            {
                int index;
                mainWeapon[i] = _WEAPONS.AR; //셀렉트웨폰에서 선택했던 무기를 서버에서 받아야함.
                subWeapon[i] = _WEAPONS.GRENADE;

                index = (int)mainWeapon[i]; //주무기의 생성
                obj_mainWeapon[i] = Instantiate(obj_weaponPrefabsList[index], PlayersManager.instance.obj_players[i].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
                obj_mainWeapon[i].transform.SetParent(PlayersManager.instance.obj_players[i].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));

               // index = (int)subWeapon[i] - 1; //보조무기의 생성
                //obj_subWeapon[i] = Instantiate(obj_weaponPrefabsList[index], PlayersManager.instance.obj_players[i].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
                //obj_subWeapon[i].transform.SetParent(PlayersManager.instance.obj_players[i].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
            }
        }
    }
    void Initialization_Attribute(int _num)
    {
        infoAR    = new _INFO_AR[_num];
        infoSG    = new _INFO_SG[_num];
        infoSMG = new _INFO_SMG[_num];
        //rg_bullet = new Rigidbody[30];
        sample = new GameObject[30];
        for (int i = 0; i < _num; i++)
        {
            infoAR[i].curAmmo  = 30;
            infoAR[i].maxAmmo = 30;
            infoAR[i].fireRate     = 0.1f;
            infoAR[i].damage    = 1.0f;
            infoAR[i].accuracy    = 0.07f;
            infoAR[i].range       = 20.0f;

            infoSG[i].curAmmo   = 2;
            infoSG[i].maxAmmo  = 2;
            infoSG[i].fireRate      = 1.0f;
            infoSG[i].damage     = 1.5f;
            infoSG[i].accuracy    = 0.25f;
            infoSG[i].range        = 11.0f;

            infoSMG[i].curAmmo  = 25;
            infoSMG[i].maxAmmo = 25;
            infoSMG[i].fireRate     = 2.0f;
            infoSMG[i].damage    = 0.5f;
            infoSMG[i].accuracy   = 0.6f;
            infoSMG[i].range       = 0.6f;
        }
    }
}
