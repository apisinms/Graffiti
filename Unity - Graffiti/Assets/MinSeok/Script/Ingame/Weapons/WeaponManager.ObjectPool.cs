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
       // obj_bulletClone.transform.GetChild(0).GetComponent<TrailRenderer>().Clear();
        return obj_bulletClone;
    }

    public void ReturnBulletToPool(GameObject _obj_bullet, int _index)
    {
        //물리초기화.
        _obj_bullet.GetComponent<Rigidbody>().isKinematic = true;
        _obj_bullet.GetComponent<Rigidbody>().isKinematic = false; 
        _obj_bullet.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous; //바뀐옵션을 다시적용
        _obj_bullet.GetComponent<TrailRenderer>().Clear(); //궤적 렌더러의 위치를 초기화

        //다시 비활성화후 트랜스폼 원상복구후 풀로 복귀.
        _obj_bullet.SetActive(false);
        _obj_bullet.transform.SetParent(PlayersManager.instance.obj_players[_index].transform);
        _obj_bullet.transform.localPosition = tf_firstPos[_index].localPosition;
        _obj_bullet.transform.localRotation = tf_firstPos[_index].localRotation;
        _obj_bullet.transform.localScale = tf_firstPos[_index].localScale;
        list_bulletPool[_index].Add(_obj_bullet);
    }
}
