using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//                   PoolManager.Bullet
public partial class PoolManager : MonoBehaviour
{
    #region BULLET_POOL
    public GameObject[] obj_bulletPool;
    public List<GameObject>[] list_bulletPool { get; set; } //총알을 가져오는 풀
    public Transform[] tf_bulletFirstPos { get; set; }
    public readonly string[] bulletTag = new string[C_Global.MAX_PLAYER];
    #endregion

    #region SHELL_POOL
    public GameObject[] obj_shellPool;
    public List<ParticleSystem>[] list_shellPool { get; set; } //탄피를 가져오는 풀
    public Transform[] tf_shellFirstPos { get; set; }
    public readonly string[] shellTag = new string[C_Global.MAX_PLAYER];
    #endregion

    private void Initialization_Bullet(int _num)
    {
        list_bulletPool = new List<GameObject>[_num];
        tf_bulletFirstPos = new Transform[_num];

        for (int i = 0; i < _num; i++)
            list_bulletPool[i] = new List<GameObject>();

        bulletTag[0] = "Bullet1"; bulletTag[1] = "Bullet2"; bulletTag[2] = "Bullet3"; bulletTag[3] = "Bullet4";
    }

    private void Initialization_Shell(int _num)
    {
        list_shellPool = new List<ParticleSystem>[_num];
        tf_shellFirstPos = new Transform[_num];

        for (int i = 0; i < _num; i++)
            list_shellPool[i] = new List<ParticleSystem>();

        shellTag[0] = "Shell1"; shellTag[1] = "Shell2"; shellTag[2] = "Shell3"; shellTag[3] = "Shell4";
    }

    public void CreateBulletPool(int _index, int _num)
    {
        tf_bulletFirstPos[_index] = WeaponManager.instance.obj_weaponPrefabsList[5].transform;

        for (int i = 0; i < _num; i++) //알과 탄피를 만들어놓고 리스트에 박아둠
        {
            var obj_bulletClone = Instantiate(WeaponManager.instance.obj_weaponPrefabsList[5], PlayersManager.instance.tf_players[_index].transform) as GameObject;
            obj_bulletClone.SetActive(false);
            obj_bulletClone.gameObject.tag = bulletTag[_index];
            list_bulletPool[_index].Add(obj_bulletClone);
        }
    }

    public void CreateShellPool(int _index, int _num)
    {
        tf_shellFirstPos[_index] = EffectManager.instance.ps_sparkPrefebsList[3].transform;

        for (int i = 0; i < _num; i++) //알과 탄피를 만들어놓고 리스트에 박아둠
        {
            var ps_shellClone = Instantiate(EffectManager.instance.ps_sparkPrefebsList[3], PlayersManager.instance.tf_players[_index].transform) as ParticleSystem;
            ps_shellClone.Stop(); ps_shellClone.Clear();
            ps_shellClone.gameObject.SetActive(false);
            ps_shellClone.gameObject.tag = shellTag[_index];
            list_shellPool[_index].Add(ps_shellClone);
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

    public ParticleSystem GetShellFromPool(int _index)
    {
        ParticleSystem ps_shellClone = null;
        for (int i = 0; i < list_shellPool[_index].Count; i++)
        {
            if (list_shellPool[_index][i].gameObject.activeSelf == false && list_shellPool[_index][i].isPlaying == false) //넣어둔 탄피를 가져옴.
            {
                ps_shellClone = list_shellPool[_index][i];
                ps_shellClone.gameObject.transform.SetParent(obj_shellPool[_index].transform);
                ps_shellClone.gameObject.SetActive(true);
                ps_shellClone.Play();
                list_shellPool[_index].RemoveAt(i); //가져온 인덱스의 불릿제거.
                break;
            }
        }
        return ps_shellClone;
    }

    public void ReturnGunToPool(GameObject _obj_bullet, BulletCollision._BULLET_CLONE_INFO _info_bullet, int _index)
    {
        if (list_bulletPool[_index].Count > 30)
            return;

        //애드포스중인 총알을 정지시킨후
        _info_bullet.rb_bulletClone.velocity = Vector3.zero;
        _info_bullet.rb_bulletClone.angularVelocity = Vector3.zero;

        //다시 비활성화후 트랜스폼 원상복구후 풀로 복귀.
        _obj_bullet.SetActive(false);
        _info_bullet.tf_bulletClone.SetParent(PlayersManager.instance.tf_players[_index].transform);
        _info_bullet.tf_bulletClone.localPosition = tf_bulletFirstPos[_index].localPosition;
        _info_bullet.tf_bulletClone.localRotation = tf_bulletFirstPos[_index].localRotation;
        _info_bullet.tf_bulletClone.localScale = tf_bulletFirstPos[_index].localScale;
        list_bulletPool[_index].Add(_obj_bullet);
    }

    public void ReturnShellToPool(ParticleSystem _ps_shell, int _index)
    {
        if (list_shellPool[_index].Count > 30)
            return;

        //다시 비활성화후 트랜스폼 원상복구후 풀로 복귀.
        _ps_shell.Stop();
        _ps_shell.Clear();
        _ps_shell.gameObject.SetActive(false);
        _ps_shell.gameObject.transform.SetParent(PlayersManager.instance.tf_players[_index].transform);
        _ps_shell.gameObject.transform.localPosition = tf_shellFirstPos[_index].localPosition;
        _ps_shell.gameObject.transform.localRotation = tf_shellFirstPos[_index].localRotation;
        _ps_shell.gameObject.transform.localScale = tf_shellFirstPos[_index].localScale;
        list_shellPool[_index].Add(_ps_shell);
    }

    public IEnumerator CheckShellEnd(ParticleSystem _ps_shellClone, int _index)
    {
        yield return YieldInstructionCache.WaitForSeconds(1.0f);

        PoolManager.instance.ReturnShellToPool(_ps_shellClone, _index);
    }
}
