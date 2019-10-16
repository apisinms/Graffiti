using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PooledObject
{
    public int index;
    public GameObject prefab = null;
    public int poolCount = 0;
    [SerializeField]
    private List<GameObject> poolList = new List<GameObject>();

    // PolledObject 객체를 초기화 할때 처음 한번만 호출
    // PoolCount 에 지정한 수 만큼 객체를 생성해서 poolList 에 추가하는 역할
    public void Initialize(Transform parent = null)
    {
        for (int ix = 0; ix < poolCount; ++ix)
        {
            poolList.Add(CreateItem(parent));
        }
    }

    // 사용한 객체를 풀에 다시 반환할때 사용할 함수 부모 Transform 정보가 필요할 경우엔 같이 전달
    public void PushToPool(GameObject item, Transform parent = null)
    {
        item.transform.SetParent(parent);
        item.SetActive(false);
        poolList.Add(item);
    }

    // 저장해둔 오브젝트가 남아있는지 확인 후 없으면 새로 생성해서 추가
    public GameObject PopFromPool(Transform parent = null)
    {
        if (poolList.Count == 0)
            poolList.Add(CreateItem(parent));
        GameObject item = poolList[0];
        poolList.RemoveAt(0);
        return item;
    }

    // prefab 변수에 지정된 게임 오브젝트를 생성하는 역할
    private GameObject CreateItem(Transform parent = null)
    {
        GameObject item = Object.Instantiate(prefab) as GameObject;
        item.transform.SetParent(parent);
        item.SetActive(false);
        return item;
    }
}

