using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//                   PoolManager.Effect
public partial class PoolManager : MonoBehaviour
{
    #region COLLISION_EFFECT_POOL
    public GameObject[] obj_collisionEffectPool;
    public List<ParticleSystem>[] list_collisionEffectPool { get; set; } //총알을 가져오는 풀
    public readonly string[] collisionEffectTag = new string[2];
    #endregion

    public void Initialization_Effect(int _num)
    {
        list_collisionEffectPool = new List<ParticleSystem>[2];

        for (int i = 0; i < 2; i++)
            list_collisionEffectPool[i] = new List<ParticleSystem>();

        collisionEffectTag[0] = "Concrete1"; collisionEffectTag[1] = "Iron1";
    }

    public void CreateCollisionEffectPool(int _num)
    {
        ParticleSystem[] ps_effectClone = new ParticleSystem[obj_collisionEffectPool.Length];

        // 0은 콘크리트, 1은 아이언이펙트
        for (int i = 0; i < obj_collisionEffectPool.Length; i++) //알을 만들어놓고 리스트에 박아둠
        {
            for (int j = 0; j < _num; j++)
            {
                ps_effectClone[i] = Instantiate(EffectManager.instance.ps_sparkPrefebsList[i], obj_collisionEffectPool[i].transform) as ParticleSystem;
                ps_effectClone[i].Stop();
                ps_effectClone[i].Clear();
                ps_effectClone[i].gameObject.SetActive(false);
                ps_effectClone[i].gameObject.tag = collisionEffectTag[i];
                list_collisionEffectPool[i].Add(ps_effectClone[i]);
            }
        }
    }

    public ParticleSystem GetCollisionEffectFromPool(string _CollisionTag, Vector3 _playPos, Vector3 _playRot)
    {
        ParticleSystem ps_effectClone = null;

        for(int i=0; i < obj_collisionEffectPool.Length; i++)
        {
            if (!obj_collisionEffectPool[i].CompareTag(_CollisionTag))
                continue;

            for (int j = 0; j < list_collisionEffectPool[i].Count; j++)
            {
                if (list_collisionEffectPool[i][j].gameObject.activeSelf == false && list_collisionEffectPool[i][j].isPlaying == false) //넣어둔 총알을 가져옴.
                {
                    ps_effectClone = list_collisionEffectPool[i][j];
                    ps_effectClone.gameObject.SetActive(true);
                    list_collisionEffectPool[i].RemoveAt(j); //가져온 인덱스의 불릿제거.
                    break;
                }
            }
        }

        ps_effectClone.gameObject.transform.position = _playPos;
        ps_effectClone.gameObject.transform.rotation = Quaternion.LookRotation(_playRot);
        ps_effectClone.Stop();
        ps_effectClone.Clear();
        ps_effectClone.Play();

        return ps_effectClone;
    }

    public void ReturnCollisionEffectToPool(ParticleSystem _ps_effect)
    {
        for (int i = 0; i < obj_collisionEffectPool.Length; i++)
        {
            if (!obj_collisionEffectPool[i].CompareTag(_ps_effect.transform.parent.tag))
                continue;

           // if (list_collisionEffectPool[i].Count > 60 || _ps_effect.isPlaying == false)
             //   return;
         //   else
            {
                //다시 비활성화후 트랜스폼 원상복구후 풀로 복귀.
                //_ps_effect.Stop();
               // _ps_effect.Clear();
                _ps_effect.gameObject.SetActive(false);
                list_collisionEffectPool[i].Add(_ps_effect);
                break;
            }
        }
    }
}
