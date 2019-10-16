using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : Singleton<ObjectPool>
{
    public List<PooledObject> objectPool = new List<PooledObject>();

    void Awake()
    {
        for (int ix = 0; ix < objectPool.Count; ++ix)
        {
            objectPool[ix].Initialize(transform);
        }
    }

    /*
     * itemName : 반환할 객체의 pool 오브젝트 이름
       item : 반환할 객체 – 게임 오브젝트
       parent : 부모 계층 관계를 설정할 정보
     */
    // 특정 게임 오브젝트의 자식으로 지정하고 싶을땐 parent 정보를 전달
    public bool PushToPool(int index, GameObject item, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(index);
        if (pool == null)
            return false;

        pool.PushToPool(item, parent == null ? transform : parent);
        return true;
    }

    /*
     * itemName : 요청할 객체의 pool 오브젝트 이름
       parent : 부모 계층 관계를 설정할 정보
     */
    // 특정 게임 오브젝트의 자식으로 지정하고 싶을땐 parent 정보를 전달
    public GameObject PopFromPool(int index, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(index);
        if (pool == null)
            return null;

        return pool.PopFromPool(parent);
    }

    // 파라메터와 같은 인덱스 가진 오브젝트 풀을 검색하고 결과를 리턴 
    PooledObject GetPoolItem(int index)
    {
        for (int ix = 0; ix < objectPool.Count; ++ix)
        {
            if (ix == index)
                return objectPool[ix];
        }

        Debug.LogWarning("There's no matched pool list.");
        return null;
    }
}