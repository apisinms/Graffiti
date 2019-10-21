using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_AR : MonoBehaviour, IMainWeaponType
{
    public static Main_AR instance;
    private WeaponManager weaponManager;
    private int myIndex { get; set; }

    #region AR
    public struct _PLAYER_AR_INFO   // 플레이어 각각 가지고 있어야되는 정보
    {
        public Vector3[] vt_bulletPattern;
        public int bulletPatternIndex;
        public int prevBulletPatternIndex;

        public int curAmmo;
    }

    public _PLAYER_AR_INFO[] playerARInfo { get; set; }
    #endregion

    // 로컬에서도 테스트 해봐야되니까 일단 Start에서 초기화는 해줌
    private void Start()
    {
        weaponManager = WeaponManager.instance;
        myIndex = GameManager.instance.myIndex;

        playerARInfo = new _PLAYER_AR_INFO[C_Global.MAX_PLAYER];

        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            playerARInfo[i].vt_bulletPattern = new Vector3[3];
            playerARInfo[i].bulletPatternIndex = 1;
            playerARInfo[i].prevBulletPatternIndex = 2;
            playerARInfo[i].curAmmo = 30;
        }
#if !NETWORK
		weaponManager.weaponInfoAR.maxAmmo = 30;
        weaponManager.weaponInfoAR.fireRate = 0.14f;
        weaponManager.weaponInfoAR.damage = 0.03f;
        weaponManager.weaponInfoAR.accuracy = 0.06f;
        weaponManager.weaponInfoAR.range = 20.0f;
        weaponManager.weaponInfoAR.speed = 2000.0f;
#endif
    }

    public static Main_AR GetMainWeaponInstance()
    {
        if (instance == null)
            instance = (Main_AR)WeaponManager.instance.cn_mainWeaponList[1]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

        return instance;
    }

    public IEnumerator ActionFire(int _index)
    {
        EffectManager.instance.ps_tmpMuzzle[_index].body.option.loop = true;
        EffectManager.instance.ps_tmpMuzzle[_index].glow.option.loop = true;
        EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.loop = true;
        EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.loop = true;
        EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.loop = true;
        EffectManager.instance.ps_tmpMuzzle[_index].spark.option.loop = true;

        EffectManager.instance.ps_tmpMuzzle[_index].body.option.duration = 1.0f;
        EffectManager.instance.ps_tmpMuzzle[_index].glow.option.duration = 1.0f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.duration = 1.0f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.duration = 1.0f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.duration = 1.0f;
        EffectManager.instance.ps_tmpMuzzle[_index].spark.option.duration = 1.0f;

        EffectManager.instance.ps_tmpMuzzle[_index].body.option.simulationSpeed = 0.8f;
        EffectManager.instance.ps_tmpMuzzle[_index].glow.option.simulationSpeed = 0.8f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.simulationSpeed = 0.8f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.simulationSpeed = 0.8f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.simulationSpeed = 0.8f;
        EffectManager.instance.ps_tmpMuzzle[_index].spark.option.simulationSpeed = 0.8f;

        while (true)
        {
            var clone = PoolManager.instance.GetBulletFromPool(_index);
            Transform tf_clone = clone.transform;

            playerARInfo[_index].vt_bulletPattern[0].x = tf_clone.forward.x - (tf_clone.right.x * weaponManager.weaponInfoAR.accuracy);
            playerARInfo[_index].vt_bulletPattern[0].z = tf_clone.forward.z - (tf_clone.right.z * weaponManager.weaponInfoAR.accuracy);
            playerARInfo[_index].vt_bulletPattern[1].x = tf_clone.forward.x;
            playerARInfo[_index].vt_bulletPattern[1].z = tf_clone.forward.z;
            playerARInfo[_index].vt_bulletPattern[2].x = tf_clone.forward.x + (tf_clone.right.x * weaponManager.weaponInfoAR.accuracy);
            playerARInfo[_index].vt_bulletPattern[2].z = tf_clone.forward.z + (tf_clone.right.z * weaponManager.weaponInfoAR.accuracy);

            tf_clone.localRotation = Quaternion.LookRotation(playerARInfo[_index].vt_bulletPattern[playerARInfo[_index].bulletPatternIndex]);
            clone.GetComponent<Rigidbody>().AddForce(playerARInfo[_index].vt_bulletPattern[playerARInfo[_index].bulletPatternIndex] * weaponManager.weaponInfoAR.speed, ForceMode.Acceleration);
            AudioManager.Instance.Play(0);

            switch (playerARInfo[_index].bulletPatternIndex)
            {
                case 0:
                    playerARInfo[_index].bulletPatternIndex = 1;
                    //Debug.Log("중");
                    break;
                case 1:
                    if (playerARInfo[_index].prevBulletPatternIndex == 1)
                    {
                        playerARInfo[_index].bulletPatternIndex = 0;
                        playerARInfo[_index].prevBulletPatternIndex = 2;
                        //Debug.Log("좌");
                    }
                    else if (playerARInfo[_index].prevBulletPatternIndex == 2)
                    {
                        playerARInfo[_index].bulletPatternIndex = 2;
                        playerARInfo[_index].prevBulletPatternIndex = 1;
                        //Debug.Log("우");
                    }
                    break;
                case 2:
                    playerARInfo[_index].bulletPatternIndex = 1;
                    //Debug.Log("중");
                    break;
            }

            yield return YieldInstructionCache.WaitForSeconds(weaponManager.weaponInfoAR.fireRate);

        }
    }

    public void CheckFireRange(GameObject _obj_bullet, int _index)
    {
        if (Vector3.Distance(_obj_bullet.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= Main_AR.instance.weaponManager.weaponInfoAR.range)
            PoolManager.instance.ReturnBulletToPool(_obj_bullet, _index);
    }

    public void ApplyDamage(int _type, int _index)
    {
        UIManager.instance.hp[_type].img_front.fillAmount -= 0.04f; //데미지만큼 피를깎음.
        UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, 0.04f));
        /*
        switch(_type)
        {
            case 0:
                UIManager.instance.myHP.img_front.fillAmount -= weaponManager.weaponInfoAR.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoAR.damage));
                break;
            case 1:
                UIManager.instance.teamHP.img_front.fillAmount -= weaponManager.weaponInfoAR.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoAR.damage));
                break;
            case 2:
                UIManager.instance.enemyHP[0].img_front.fillAmount -= weaponManager.weaponInfoAR.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoAR.damage));
                break;
            case 3:
                UIManager.instance.enemyHP[1].img_front.fillAmount -= weaponManager.weaponInfoAR.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoAR.damage));
                break;
        }
        */
    }
}