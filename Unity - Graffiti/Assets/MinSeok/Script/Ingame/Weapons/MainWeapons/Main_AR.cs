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
        public bool isReloading;
    }

    public _PLAYER_AR_INFO[] playerARInfo { get; set; }
    #endregion

    // 로컬에서도 테스트 해봐야되니까 일단 Start에서 초기화는 해줌
    private void Start()
    {
        weaponManager = WeaponManager.instance;
        myIndex = GameManager.instance.myIndex;

        playerARInfo = new _PLAYER_AR_INFO[C_Global.MAX_PLAYER];

#if !NETWORK
		weaponManager.weaponInfoAR.maxAmmo = 20;
        weaponManager.weaponInfoAR.fireRate = 0.14f;
        weaponManager.weaponInfoAR.damage = 0.03f;
        weaponManager.weaponInfoAR.accuracy = 0.06f;
        weaponManager.weaponInfoAR.range = 20.0f;
        weaponManager.weaponInfoAR.speed = 2000.0f;
#endif

        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            playerARInfo[i].vt_bulletPattern = new Vector3[3];
            playerARInfo[i].bulletPatternIndex = 1;
            playerARInfo[i].prevBulletPatternIndex = 2;
            playerARInfo[i].curAmmo = weaponManager.weaponInfoAR.maxAmmo;
        }
    }

    public static Main_AR GetMainWeaponInstance()
    {
        if (instance == null)
            instance = (Main_AR)WeaponManager.instance.cn_mainWeaponList[1]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

        return instance;
    }

    public IEnumerator ActionFire(int _index)
    {
        if (playerARInfo[_index].curAmmo <= 0)
        {
            ReloadAmmo(_index);
            yield break;
        }

        EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, myIndex);

        while (true)
        {
            if (playerARInfo[_index].curAmmo <= 0)
            {
                ReloadAmmo(_index);
                yield break;
            }

            var bulletClone = PoolManager.instance.GetBulletFromPool(_index);
            var shellClone = PoolManager.instance.GetShellFromPool(_index);
            PoolManager.instance.StartCoroutine(PoolManager.instance.CheckShellEnd(shellClone, _index));

            Transform tf_clone = bulletClone.transform;

            playerARInfo[_index].vt_bulletPattern[0].x = tf_clone.forward.x - (tf_clone.right.x * weaponManager.weaponInfoAR.accuracy);
            playerARInfo[_index].vt_bulletPattern[0].z = tf_clone.forward.z - (tf_clone.right.z * weaponManager.weaponInfoAR.accuracy);
            playerARInfo[_index].vt_bulletPattern[1].x = tf_clone.forward.x;
            playerARInfo[_index].vt_bulletPattern[1].z = tf_clone.forward.z;
            playerARInfo[_index].vt_bulletPattern[2].x = tf_clone.forward.x + (tf_clone.right.x * weaponManager.weaponInfoAR.accuracy);
            playerARInfo[_index].vt_bulletPattern[2].z = tf_clone.forward.z + (tf_clone.right.z * weaponManager.weaponInfoAR.accuracy);

            tf_clone.localRotation = Quaternion.LookRotation(playerARInfo[_index].vt_bulletPattern[playerARInfo[_index].bulletPatternIndex]);
            bulletClone.GetComponent<Rigidbody>().AddForce(playerARInfo[_index].vt_bulletPattern[playerARInfo[_index].bulletPatternIndex] * weaponManager.weaponInfoAR.speed, ForceMode.Acceleration);
            AudioManager.Instance.Play(0);
            playerARInfo[_index].curAmmo--;
            UIManager.instance.SetAmmoStateTxt(playerARInfo[_index].curAmmo);

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

    public void CheckFireRange(GameObject _obj_bullet, BulletCollision._BULLET_CLONE_INFO _info_bullet, int _index)
    {
        if (Vector3.Distance(_obj_bullet.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= Main_AR.instance.weaponManager.weaponInfoAR.range)
            PoolManager.instance.ReturnGunToPool(_obj_bullet, _info_bullet, _index);
    }

    public void ReloadAmmo(int _index)
    {
        EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, myIndex);
        AudioManager.Instance.Play(8);
        StartCoroutine(Cor_ReloadAmmo(_index));
    }

    public IEnumerator Cor_ReloadAmmo(int _index)
    {
        playerARInfo[_index].curAmmo = 0; //어차피 장전중엔 총을못쏘므로 총알을 0으로 만들어줌

        if (playerARInfo[_index].isReloading == false)
        {
            playerARInfo[_index].isReloading = true;

            Debug.Log("AR총알없음. 장전중");
            UIManager.instance.StartCoroutine(UIManager.instance.DecreaseReloadTimeImg(3.0f));

            yield return YieldInstructionCache.WaitForSeconds(3.0f);
            playerARInfo[_index].curAmmo = weaponManager.weaponInfoAR.maxAmmo;
            UIManager.instance.SetAmmoStateTxt(playerARInfo[_index].curAmmo);
            AudioManager.Instance.Play(9);
            Debug.Log("AR장전완료");
            playerARInfo[_index].isReloading = false;

            //장전 이전상태가 사격중이였을경우 계속 이어서쏨
            if (PlayersManager.instance.actionState[_index] == _ACTION_STATE.SHOT ||
                PlayersManager.instance.actionState[_index] == _ACTION_STATE.CIR_AIM_SHOT)
            {
                StateManager.instance.Shot(false);
                StateManager.instance.Shot(true);
            }
        }
    }

    public void ApplyDamage(int _type, int _index)
    {
        if (UIManager.instance.hp[_type].img_front.fillAmount <= 0)
        {
            PlayersManager.instance.actionState[_type] = _ACTION_STATE.DEATH;
            PlayersManager.instance.obj_players[_type].GetComponent<CapsuleCollider>().isTrigger = true;
            PlayersManager.instance.Action_Death(_type);
            return;
        }

        UIManager.instance.hp[_type].img_front.fillAmount -= 0.04f; //데미지만큼 피를깎음.
        UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHPImg(_type, 0.04f));
    }
}