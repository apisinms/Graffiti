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
    #endregion

    void Awake()
    {
        if (this == null)
            instance = this;

        myIndex = GameManager.instance.myIndex;
        weaponsTag[0] = "Ar"; weaponsTag[1] = "Sg"; weaponsTag[2] = "Smg";

        Initialization(C_Global.MAX_PLAYER);
    }

    void Initialization(int _num)
    {
        obj_mainWeapon = new GameObject[C_Global.MAX_PLAYER];
        obj_subWeapon = new GameObject[C_Global.MAX_PLAYER];
        mainWeapon = new _WEAPONS[C_Global.MAX_PLAYER];
        subWeapon = new _WEAPONS[C_Global.MAX_PLAYER];

        // !!!!!!!!!!! 서버에서 받은데이터로 초기화해야함  임의로 속성값부여해둠. !!!!!!!!!!
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            if (myIndex == i) //내인덱스들의 초기화
            {
                int index;
                mainWeapon[i] = _WEAPONS.AR; //셀렉트웨폰에서 선택했던 무기를 서버에서 받아야함.
                subWeapon[i] = _WEAPONS.GRENADE;

                index = (int)mainWeapon[i]; //주무기의 생성
                obj_mainWeapon[i] = Instantiate(obj_weaponPrefabsList[index], PlayersManager.instance.obj_players[myIndex].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
                obj_mainWeapon[i].transform.SetParent(PlayersManager.instance.obj_players[myIndex].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));

                index = (int)subWeapon[i] - 1; //보조무기의 생성
                obj_subWeapon[i] = Instantiate(obj_weaponPrefabsList[index], PlayersManager.instance.obj_players[myIndex].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
                obj_subWeapon[i].transform.SetParent(PlayersManager.instance.obj_players[myIndex].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
            }
            else //나머지 3명 인덱스의 초기화
            {
                int index;
                mainWeapon[i] = _WEAPONS.AR; //셀렉트웨폰에서 선택했던 무기를 서버에서 받아야함.
                subWeapon[i] = _WEAPONS.GRENADE;

                index = (int)mainWeapon[i]; //주무기의 생성
                obj_mainWeapon[i] = Instantiate(obj_weaponPrefabsList[index], PlayersManager.instance.obj_players[i].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
                obj_mainWeapon[i].transform.SetParent(PlayersManager.instance.obj_players[i].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));

                index = (int)subWeapon[i] - 1; //보조무기의 생성
                obj_subWeapon[i] = Instantiate(obj_weaponPrefabsList[index], PlayersManager.instance.obj_players[i].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
                obj_subWeapon[i].transform.SetParent(PlayersManager.instance.obj_players[i].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0));
            }
        }
    }
}
