using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//                   PoolManager.Grenade
public partial class PoolManager : MonoBehaviour
{
    #region BULLET_GRENADE
    public GameObject[] obj_grenadePool;
    public List<GameObject>[] list_grenadePool { get; set; } //총알을 가져오는 풀
    public Transform[] tf_grenadeFirstPos { get; set; }
    public string[] grenadeTag;
    #endregion

    private void Initialization_Grenade(int _num)
    {
        list_grenadePool = new List<GameObject>[_num];
        tf_grenadeFirstPos = new Transform[_num];

        for (int i = 0; i < _num; i++)
            list_grenadePool[i] = new List<GameObject>();

        grenadeTag = new string[GameManager.instance.gameInfo.maxPlayer];

        for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
        {
            grenadeTag[i] = "Grenade" + (i + 1).ToString();
        }
    }

    public void CreateGrenadePool(int _index, int _num)
    {
        tf_grenadeFirstPos[_index] = WeaponManager.instance.obj_weaponPrefabsList[4].transform;

        for (int i = 0; i < _num; i++) //알과 탄피를 만들어놓고 리스트에 박아둠
        {
            var obj_grenadeClone = Instantiate(WeaponManager.instance.obj_weaponPrefabsList[4], PlayersManager.instance.tf_players[_index].transform) as GameObject;
            obj_grenadeClone.SetActive(false);
            obj_grenadeClone.gameObject.tag = grenadeTag[_index];
            list_grenadePool[_index].Add(obj_grenadeClone);
        }
    }

    public GameObject GetGrenadeFromPool(int _index)
    {
        GameObject obj_grenadeClone = null;

        for (int i = 0; i < list_grenadePool[_index].Count; i++)
        {
            if (list_grenadePool[_index][i].activeSelf == false) //넣어둔 수류탄을 가져옴
            {
                obj_grenadeClone = list_grenadePool[_index][i];
                obj_grenadeClone.transform.SetParent(obj_grenadePool[_index].transform);
                obj_grenadeClone.SetActive(true);
                list_grenadePool[_index].RemoveAt(i); //가져온 인덱스의 불릿제거.
                break;
            }
        }
        return obj_grenadeClone;
    }

    public void ReturnGrenadeToPool(GameObject _obj_grenade, GrenadeCollision._GRENADE_CLONE_INFO _info_grenade, int _index)
    {
        if (list_grenadePool[_index].Count > 2)
            return;


        //애드포스중인 수류탄을 정지시킨후
        _info_grenade.rb_grenadeClone.velocity = Vector3.zero;
        _info_grenade.rb_grenadeClone.angularVelocity = Vector3.zero;
        _info_grenade.tr_grenadeClone.Clear(); //트레일 렌더러 초기화

        //다시 비활성화후 트랜스폼 원상복구후 풀로 복귀.
        _obj_grenade.SetActive(false);
        _info_grenade.tf_grenadeClone.SetParent(PlayersManager.instance.tf_players[_index].transform);
        _info_grenade.tf_grenadeClone.localPosition = tf_grenadeFirstPos[_index].localPosition;
        _info_grenade.tf_grenadeClone.localRotation = tf_grenadeFirstPos[_index].localRotation;
        _info_grenade.tf_grenadeClone.localScale = tf_grenadeFirstPos[_index].localScale;
        list_grenadePool[_index].Add(_obj_grenade);
        
    }

}