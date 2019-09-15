using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WeaponManager : MonoBehaviour
{
    public GameObject obj_bulletPool;
    public List<GameObject> list_bulletPool = new List<GameObject>(); //총알을 가져오는 풀
    public Transform tf_firstPos { get; set; }
    //public Rigidbody[] rg_bullet { get; set; }

    public void CreateBulletPool(int _bulletNum)
    {
        tf_firstPos = obj_weaponPrefabsList[5].transform;

        for (int i=0; i<_bulletNum; i++) //알을 만들어놓고 리스트에 박아둠
        {
            var obj_bulletClone = Instantiate(obj_weaponPrefabsList[5], PlayersManager.instance.obj_players[myIndex].transform) as GameObject;
            //rg_bullet[i] = obj_bulletClone.GetComponent<Rigidbody>();
            obj_bulletClone.SetActive(false);
            list_bulletPool.Add(obj_bulletClone);
        }
        
        for(int i=0; i<list_bulletPool.Count; i++)
            sample[i] = list_bulletPool[i];
    }

    public GameObject GetBulletFromPool()
    {
        GameObject obj_bulletClone = null;

        for (int i = 0; i < list_bulletPool.Count; i++)
        {
            if (list_bulletPool[i].activeSelf == false) //넣어둔 총알을 가져옴.
            {
                obj_bulletClone = list_bulletPool[i];
                obj_bulletClone.transform.SetParent(obj_bulletPool.transform);
                obj_bulletClone.SetActive(true);
                list_bulletPool.RemoveAt(i); //가져온 인덱스제거.
                break;
            }
        }
        return obj_bulletClone;
    }

    public void ReturnBulletToPool(GameObject _obj_bullet)
    {
      //  int index;
        var obj_bulletClone = _obj_bullet;
        obj_bulletClone.SetActive(false);
        obj_bulletClone.GetComponent<Rigidbody>().isKinematic = true; //남아있는 물리를 제거.

        //총알위치 원상복구.
        obj_bulletClone.transform.SetParent(PlayersManager.instance.obj_players[myIndex].transform);
        obj_bulletClone.transform.localPosition = tf_firstPos.localPosition;
        obj_bulletClone.transform.localEulerAngles = tf_firstPos.localEulerAngles;

        obj_bulletClone.GetComponent<Rigidbody>().isKinematic = false; //다시끔
        obj_bulletClone.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous; //바뀐옵션을 다시적용
        list_bulletPool.Add(obj_bulletClone);
    }
}
