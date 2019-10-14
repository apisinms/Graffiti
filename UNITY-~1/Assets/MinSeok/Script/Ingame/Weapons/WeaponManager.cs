using System.Collections;
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
