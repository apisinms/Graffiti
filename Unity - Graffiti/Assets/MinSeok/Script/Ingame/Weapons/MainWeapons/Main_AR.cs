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
        public float reloadTime;
        public string mainWname;
    }
    public _PLAYER_AR_INFO[] playerARInfo { get; set; }
    #endregion

    // 로컬에서도 테스트 해봐야되니까 일단 Start에서 초기화는 해줌
    private void Start()
    {
        weaponManager = WeaponManager.instance;
        myIndex = GameManager.instance.myIndex;

        playerARInfo = new _PLAYER_AR_INFO[GameManager.instance.gameInfo.maxPlayer];

#if !NETWORK
		weaponManager.weaponInfoAR.maxAmmo = 20;
        weaponManager.weaponInfoAR.fireRate = 0.14f;
        weaponManager.weaponInfoAR.damage = 0.03f;
        weaponManager.weaponInfoAR.accuracy = 0.06f;
        weaponManager.weaponInfoAR.range = 20.0f;
        weaponManager.weaponInfoAR.speed = 2000.0f;
		weaponManager.weaponInfoAR.reloadTime = 3.0f;
#endif

		for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
        {
            playerARInfo[i].vt_bulletPattern = new Vector3[3];
            playerARInfo[i].bulletPatternIndex = 1;
            playerARInfo[i].prevBulletPatternIndex = 2;
            playerARInfo[i].curAmmo = weaponManager.weaponInfoAR.maxAmmo;
            playerARInfo[i].reloadTime = weaponManager.weaponInfoAR.reloadTime;
            playerARInfo[i].mainWname = "AR";
        }
    }

    public static Main_AR GetMainWeaponInstance()
    {
        if (instance == null)
            instance = (Main_AR)WeaponManager.instance.cn_mainWeaponList[1]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

        return instance;
    }

    public IEnumerator ActionFire()
    {     
        while (true)
        {
            if (playerARInfo[myIndex].curAmmo <= 0)
            {
                ReloadAmmoProcess(myIndex);
                yield break;
            }

            EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, myIndex);
            var bulletClone = PoolManager.instance.GetBulletFromPool(myIndex);
            var shellClone = PoolManager.instance.GetShellFromPool(myIndex);
            PoolManager.instance.StartCoroutine(PoolManager.instance.CheckShellEnd(shellClone, myIndex));

            Transform tf_clone = bulletClone.transform;

            playerARInfo[myIndex].vt_bulletPattern[0].x = tf_clone.forward.x - (tf_clone.right.x * weaponManager.weaponInfoAR.accuracy);
            playerARInfo[myIndex].vt_bulletPattern[0].z = tf_clone.forward.z - (tf_clone.right.z * weaponManager.weaponInfoAR.accuracy);
            playerARInfo[myIndex].vt_bulletPattern[1].x = tf_clone.forward.x;
            playerARInfo[myIndex].vt_bulletPattern[1].z = tf_clone.forward.z;
            playerARInfo[myIndex].vt_bulletPattern[2].x = tf_clone.forward.x + (tf_clone.right.x * weaponManager.weaponInfoAR.accuracy);
            playerARInfo[myIndex].vt_bulletPattern[2].z = tf_clone.forward.z + (tf_clone.right.z * weaponManager.weaponInfoAR.accuracy);

            tf_clone.localRotation = Quaternion.LookRotation(playerARInfo[myIndex].vt_bulletPattern[playerARInfo[myIndex].bulletPatternIndex]);
            bulletClone.GetComponent<Rigidbody>().AddForce(playerARInfo[myIndex].vt_bulletPattern[playerARInfo[myIndex].bulletPatternIndex] * weaponManager.weaponInfoAR.speed, ForceMode.Acceleration);
            AudioManager.Instance.Play(0);

            playerARInfo[myIndex].curAmmo--;
            UIManager.instance.SetAmmoStateTxt(playerARInfo[myIndex].curAmmo);

            switch (playerARInfo[myIndex].bulletPatternIndex)
            {
                case 0:
                    playerARInfo[myIndex].bulletPatternIndex = 1;
                    //Debug.Log("중");
                    break;
                case 1:
                    if (playerARInfo[myIndex].prevBulletPatternIndex == 1)
                    {
                        playerARInfo[myIndex].bulletPatternIndex = 0;
                        playerARInfo[myIndex].prevBulletPatternIndex = 2;
                        //Debug.Log("좌");
                    }
                    else if (playerARInfo[myIndex].prevBulletPatternIndex == 2)
                    {
                        playerARInfo[myIndex].bulletPatternIndex = 2;
                        playerARInfo[myIndex].prevBulletPatternIndex = 1;
                        //Debug.Log("우");
                    }
                    break;
                case 2:
                    playerARInfo[myIndex].bulletPatternIndex = 1;
                    //Debug.Log("중");
                    break;
            }

            if (playerARInfo[myIndex].curAmmo <= 0)
            {
                ReloadAmmoProcess(myIndex);
                yield break;
            }

            yield return YieldInstructionCache.WaitForSeconds(weaponManager.weaponInfoAR.fireRate);
        }
    }

    public IEnumerator ActionFire(int _index)
	{
        if (_index == myIndex)
            yield break;

        while (true)
		{
            #if NETWORK
            if (NetworkManager.instance.GetReloadState(_index) == true)
            {
                StartDecreaseReloadGage(_index);
                yield break;
            }
            #endif

            EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, _index);
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
            
            #if NETWORK
            if (NetworkManager.instance.GetReloadState(_index) == true)
            {
                StartDecreaseReloadGage(_index);
                yield break;
            }
            #endif

            yield return YieldInstructionCache.WaitForSeconds(weaponManager.weaponInfoAR.fireRate);
		}
	}

	public void CheckFireRange(GameObject _obj_bullet, BulletCollision._BULLET_CLONE_INFO _info_bullet, int _index)
    {
        if (Vector3.Distance(_obj_bullet.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= Main_AR.instance.weaponManager.weaponInfoAR.range)
            PoolManager.instance.ReturnGunToPool(_obj_bullet, _info_bullet, _index);
    }

    private void StartDecreaseReloadGage(int _index)
    {
        EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, _index);
        if (BridgeClientToServer.instance.isStartReloadGageCor[_index] == false)
        {
            UIManager.instance.StartCoroutine(UIManager.instance.DecreaseReloadGageImg(weaponManager.weaponInfoAR.reloadTime, _index));
            BridgeClientToServer.instance.isStartReloadGageCor[_index] = true;
        }
    }

    public float GetReloadTime(int _index)
    {
        return playerARInfo[_index].reloadTime;
    }

    public string GetWeaponName(int _index)
    {
        return playerARInfo[_index].mainWname;
    }

    public Sprite GetWeaponSprite(int _index)
    {
        return UIManager.instance.spr_mainW[0];
    }

    public void SupplyAmmo(int _index)
    {
        playerARInfo[_index].curAmmo = weaponManager.weaponInfoAR.maxAmmo;
        UIManager.instance.SetAmmoStateTxt(playerARInfo[_index].curAmmo);
    }

    public void ReloadAmmoProcess(int _index)
    {
        #if NETWORK
        NetworkManager.instance.SendIngamePacket();
        #endif

        if (playerARInfo[_index].curAmmo >= weaponManager.weaponInfoAR.maxAmmo) //풀탄창이면 재장전안함
            return;

        AudioManager.Instance.Play(8);
        StartCoroutine(Cor_ReloadAmmo(_index));
    }

    public IEnumerator Cor_ReloadAmmo(int _index)
    {
        playerARInfo[_index].curAmmo = 0; //어차피 장전중엔 총을못쏘므로 총알을 0으로 만들어줌

        yield return YieldInstructionCache.WaitForSeconds(0.05f);
        EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, _index);

        if (weaponManager.isReloading == false)
        {
            weaponManager.isReloading = true;
            #if NETWORK
			NetworkManager.instance.SendIngamePacket();
            #endif

            UIManager.instance.StartCoroutine(UIManager.instance.DecreaseReloadGageImg(weaponManager.weaponInfoAR.reloadTime, _index));

            yield return YieldInstructionCache.WaitForSeconds(weaponManager.weaponInfoAR.reloadTime);

            SupplyAmmo(_index);
            AudioManager.Instance.Play(9);

            weaponManager.isReloading = false;
            #if NETWORK
			NetworkManager.instance.SendIngamePacket();
            #endif

            //장전 이전상태가 사격중이였을경우 계속 이어서쏨
            if (PlayersManager.instance.actionState[_index] == _ACTION_STATE.SHOT ||
                PlayersManager.instance.actionState[_index] == _ACTION_STATE.CIR_AIM_SHOT)
            {
                StateManager.instance.Shot(false);
                #if NETWORK
                NetworkManager.instance.SendIngamePacket();
                #endif
                StateManager.instance.Shot(true);
                #if NETWORK
                NetworkManager.instance.SendIngamePacket();
                #endif
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