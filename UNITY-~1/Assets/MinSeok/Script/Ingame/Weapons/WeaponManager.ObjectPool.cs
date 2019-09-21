using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WeaponManager : MonoBehaviour
{
    #region BULLET_POOL
    public GameObject[] obj_bulletPool;
    public List<GameObject>[] list_bulletPool; //총알을 가져오는 풀
    public Transform[] tf_firstPos { get; set; }
    #endregion

    public void CreateBulletPool(int _index, int _bulletNum)
    {
        tf_firstPos[_index] = obj_weaponPrefabsList[5].transform;

        for (int i=0; i<_bulletNum; i++) //알을 만들어놓고 리스트에 박아둠
        {
            var obj_bulletClone = Instantiate(obj_weaponPrefabsList[5], PlayersManager.instance.obj_players[_index].transform) as GameObject;
            obj_bulletClone.SetActive(false);
            list_bulletPool[_index].Add(obj_bulletClone);
        }
    }

    public GameObject GetBulletFromPool(int _index)
    {
        GameObject obj_bulletClone = null;

        for (int i = 0; i < list_bulletPool[_index].Count; i++)
        {
            if (list_bulletPool[_index][i].activeSelf == false) //넣어둔 총알을 가져옴.
            {
                obj_bulletClone = list_bulletPool[_index][i];
                obj_bulletClone.transform.SetParent(obj_bulletPool[_index].transform);
                obj_bulletClone.SetActive(true);
                list_bulletPool[_index].RemoveAt(i); //가져온 인덱스의 불릿제거.
                break;
            }
        }
        return obj_bulletClone;
    }

    public void ReturnBulletToPool(GameObject _obj_bullet, int _index)
    {
        var obj_bulletClone = _obj_bullet;
        obj_bulletClone.SetActive(false);
        obj_bulletClone.GetComponent<Rigidbody>().isKinematic = true; //남아있는 물리를 제거.

        //총알위치 원상복구.
        obj_bulletClone.transform.SetParent(PlayersManager.instance.obj_players[_index].transform);
        obj_bulletClone.transform.localPosition = tf_firstPos[_index].localPosition;
        obj_bulletClone.transform.localEulerAngles = tf_firstPos[_index].localEulerAngles;

        obj_bulletClone.GetComponent<Rigidbody>().isKinematic = false; //다시끔
        obj_bulletClone.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous; //바뀐옵션을 다시적용
        list_bulletPool[_index].Add(obj_bulletClone);
    }
}
