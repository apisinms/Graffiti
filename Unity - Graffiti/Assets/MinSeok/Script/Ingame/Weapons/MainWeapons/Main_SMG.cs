using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_SMG : MonoBehaviour, IMainWeaponType
{
    public static Main_SMG instance;
    private WeaponManager weaponManager;
    private int myIndex { get; set; }

    #region SMG
    public struct _PLAYER_SMG_INFO
    {
        public Vector3[] vt_bulletPattern;
        public int bulletPatternIndex;
        public int prevBulletPatternIndex;
        public int curAmmo;
        public bool isReloading;
        public float reloadTime;
        public string mainWname;
    }

    public _PLAYER_SMG_INFO[] playerSMGInfo { get; set; }
    #endregion

    private void Start()
    {
        weaponManager = WeaponManager.instance;
        myIndex = GameManager.instance.myIndex;

        playerSMGInfo = new _PLAYER_SMG_INFO[GameManager.instance.gameInfo.maxPlayer];

#if !NETWORK
        weaponManager.weaponInfoSMG.maxAmmo = 17;
        weaponManager.weaponInfoSMG.fireRate = 0.07f;
        weaponManager.weaponInfoSMG.damage = 0.02f;
        weaponManager.weaponInfoSMG.accuracy = 0.1f;
        weaponManager.weaponInfoSMG.range = 15.0f;
        weaponManager.weaponInfoSMG.speed = 1200.0f;
		weaponManager.weaponInfoSMG.reloadTime = 2.0f;
#endif

		for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
        {
            playerSMGInfo[i].vt_bulletPattern = new Vector3[3];
            playerSMGInfo[i].bulletPatternIndex = 2;
            playerSMGInfo[i].prevBulletPatternIndex = 1;
            playerSMGInfo[i].curAmmo = weaponManager.weaponInfoSMG.maxAmmo;
            playerSMGInfo[i].reloadTime = weaponManager.weaponInfoSMG.reloadTime;
            playerSMGInfo[i].mainWname = "SMG";
        }
    }

    public static Main_SMG GetMainWeaponInstance()
    {
        if (instance == null)
            instance = (Main_SMG)WeaponManager.instance.cn_mainWeaponList[3]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

        return instance;
    }

    public IEnumerator ActionFire()
    {
        EffectManager.instance.ps_tmpMuzzle[myIndex].body.option.simulationSpeed = 4.0f;
        EffectManager.instance.ps_tmpMuzzle[myIndex].glow.option.simulationSpeed = 4.0f;
        EffectManager.instance.ps_tmpMuzzle[myIndex].spike.option.simulationSpeed = 4.0f;
        EffectManager.instance.ps_tmpMuzzle[myIndex].flare.option.simulationSpeed = 4.0f;

        while (true)
        {
            if (playerSMGInfo[myIndex].curAmmo <= 0)
            {
                ReloadAmmoProcess(myIndex);
                yield break;
            }

            EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, myIndex);

            var clone = PoolManager.instance.GetBulletFromPool(myIndex);
            var shellClone = PoolManager.instance.GetShellFromPool(myIndex);
            PoolManager.instance.StartCoroutine(PoolManager.instance.CheckShellEnd(shellClone, myIndex));

            Transform tf_clone = clone.transform;

            playerSMGInfo[myIndex].vt_bulletPattern[0].x = tf_clone.forward.x - (tf_clone.right.x * weaponManager.weaponInfoSMG.accuracy);
            playerSMGInfo[myIndex].vt_bulletPattern[0].z = tf_clone.forward.z - (tf_clone.right.z * weaponManager.weaponInfoSMG.accuracy);
            playerSMGInfo[myIndex].vt_bulletPattern[1].x = tf_clone.forward.x;
            playerSMGInfo[myIndex].vt_bulletPattern[1].z = tf_clone.forward.z;
            playerSMGInfo[myIndex].vt_bulletPattern[2].x = tf_clone.forward.x + (tf_clone.right.x * weaponManager.weaponInfoSMG.accuracy);
            playerSMGInfo[myIndex].vt_bulletPattern[2].z = tf_clone.forward.z + (tf_clone.right.z * weaponManager.weaponInfoSMG.accuracy);

            tf_clone.localRotation = Quaternion.LookRotation(playerSMGInfo[myIndex].vt_bulletPattern[playerSMGInfo[myIndex].bulletPatternIndex]);
            clone.GetComponent<Rigidbody>().AddForce(playerSMGInfo[myIndex].vt_bulletPattern[playerSMGInfo[myIndex].bulletPatternIndex] * weaponManager.weaponInfoSMG.speed, ForceMode.Acceleration);
            AudioManager.Instance.Play(2);

            playerSMGInfo[myIndex].curAmmo--;
            UIManager.instance.SetAmmoStateTxt(playerSMGInfo[myIndex].curAmmo);
  
            switch (playerSMGInfo[myIndex].bulletPatternIndex)
            {
                case 0:
                    playerSMGInfo[myIndex].bulletPatternIndex = 1;
                    break;
                case 1:
                    if (playerSMGInfo[myIndex].prevBulletPatternIndex == 1)
                    {
                        playerSMGInfo[myIndex].bulletPatternIndex = 0;
                        playerSMGInfo[myIndex].prevBulletPatternIndex = 2;
                    }
                    else if (playerSMGInfo[myIndex].prevBulletPatternIndex == 2)
                    {
                        playerSMGInfo[myIndex].bulletPatternIndex = 2;
                        playerSMGInfo[myIndex].prevBulletPatternIndex = 1;
                    }
                    break;
                case 2:
                    playerSMGInfo[myIndex].bulletPatternIndex = 1;
                    break;
            }

            if (playerSMGInfo[myIndex].curAmmo <= 0)
            {
                ReloadAmmoProcess(myIndex);
                yield break;
            }

            yield return YieldInstructionCache.WaitForSeconds(weaponManager.weaponInfoSMG.fireRate);
        }
    }

    public IEnumerator ActionFire(int _index)
	{
        EffectManager.instance.ps_tmpMuzzle[_index].body.option.simulationSpeed = 4.0f;
		EffectManager.instance.ps_tmpMuzzle[_index].glow.option.simulationSpeed = 4.0f;
		EffectManager.instance.ps_tmpMuzzle[_index].spike.option.simulationSpeed = 4.0f;
		EffectManager.instance.ps_tmpMuzzle[_index].flare.option.simulationSpeed = 4.0f;

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

            var clone = PoolManager.instance.GetBulletFromPool(_index);
			var shellClone = PoolManager.instance.GetShellFromPool(_index);
			PoolManager.instance.StartCoroutine(PoolManager.instance.CheckShellEnd(shellClone, _index));

			Transform tf_clone = clone.transform;

			playerSMGInfo[_index].vt_bulletPattern[0].x = tf_clone.forward.x - (tf_clone.right.x * weaponManager.weaponInfoSMG.accuracy);
			playerSMGInfo[_index].vt_bulletPattern[0].z = tf_clone.forward.z - (tf_clone.right.z * weaponManager.weaponInfoSMG.accuracy);
			playerSMGInfo[_index].vt_bulletPattern[1].x = tf_clone.forward.x;
			playerSMGInfo[_index].vt_bulletPattern[1].z = tf_clone.forward.z;
			playerSMGInfo[_index].vt_bulletPattern[2].x = tf_clone.forward.x + (tf_clone.right.x * weaponManager.weaponInfoSMG.accuracy);
			playerSMGInfo[_index].vt_bulletPattern[2].z = tf_clone.forward.z + (tf_clone.right.z * weaponManager.weaponInfoSMG.accuracy);

			tf_clone.localRotation = Quaternion.LookRotation(playerSMGInfo[_index].vt_bulletPattern[playerSMGInfo[_index].bulletPatternIndex]);
			clone.GetComponent<Rigidbody>().AddForce(playerSMGInfo[_index].vt_bulletPattern[playerSMGInfo[_index].bulletPatternIndex] * weaponManager.weaponInfoSMG.speed, ForceMode.Acceleration);
			AudioManager.Instance.Play(2);

			switch (playerSMGInfo[_index].bulletPatternIndex)
			{
				case 0:
					playerSMGInfo[_index].bulletPatternIndex = 1;
					break;
				case 1:
					if (playerSMGInfo[_index].prevBulletPatternIndex == 1)
					{
						playerSMGInfo[_index].bulletPatternIndex = 0;
						playerSMGInfo[_index].prevBulletPatternIndex = 2;
					}
					else if (playerSMGInfo[_index].prevBulletPatternIndex == 2)
					{
						playerSMGInfo[_index].bulletPatternIndex = 2;
						playerSMGInfo[_index].prevBulletPatternIndex = 1;
					}
					break;
				case 2:
					playerSMGInfo[_index].bulletPatternIndex = 1;
					break;
			}
            
            #if NETWORK
            if (NetworkManager.instance.GetReloadState(_index) == true)
            {
                StartDecreaseReloadGage(_index);
                yield break;
            }
            #endif

            yield return YieldInstructionCache.WaitForSeconds(weaponManager.weaponInfoSMG.fireRate);
		}
	}

	public void CheckFireRange(GameObject _obj_bullet, BulletCollision._BULLET_CLONE_INFO _info_bullet, int _index)
    {
        if (Vector3.Distance(_obj_bullet.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= Main_SMG.instance.weaponManager.weaponInfoSMG.range)
            PoolManager.instance.ReturnGunToPool(_obj_bullet, _info_bullet, _index);
    }

    private void StartDecreaseReloadGage(int _index)
    {
        EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, _index);
        if (BridgeClientToServer.instance.isStartReloadGageCor[_index] == false)
        {
            UIManager.instance.StartCoroutine(UIManager.instance.DecreaseReloadGageImg(weaponManager.weaponInfoSMG.reloadTime, _index));
            BridgeClientToServer.instance.isStartReloadGageCor[_index] = true;
        }
    }

    public float GetReloadTime(int _index)
    {
        return playerSMGInfo[_index].reloadTime;
    }

    public string GetWeaponName(int _index)
    {
        return playerSMGInfo[_index].mainWname;

    }

    public Sprite GetWeaponSprite(int _index)
    {
        return UIManager.instance.spr_mainW[2];
    }

    public void SupplyAmmo(int _index)
    {
        playerSMGInfo[_index].curAmmo = weaponManager.weaponInfoSMG.maxAmmo;
        UIManager.instance.SetAmmoStateTxt(playerSMGInfo[_index].curAmmo);
    }

    public void ReloadAmmoProcess(int _index)
    {
        /*
        #if NETWORK
        NetworkManager.instance.SendIngamePacket();
        #endif
        */

        if (playerSMGInfo[_index].curAmmo >= weaponManager.weaponInfoSMG.maxAmmo
            || weaponManager.isReloading == true) //풀탄창이면 재장전안함
            return;

        AudioManager.Instance.Play(8);
        StartCoroutine(Cor_ReloadAmmo(_index));
    }

    public IEnumerator Cor_ReloadAmmo(int _index)
    {
        playerSMGInfo[_index].curAmmo = 0; //어차피 장전중엔 총을못쏘므로 총알을 0으로 만들어줌

        yield return YieldInstructionCache.WaitForSeconds(0.05f);
        EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, _index);

        if (weaponManager.isReloading == false)
        {
			weaponManager.isReloading = true;
            #if NETWORK
			NetworkManager.instance.SendIngamePacket();
            #endif

            UIManager.instance.StartCoroutine(UIManager.instance.DecreaseReloadGageImg(weaponManager.weaponInfoSMG.reloadTime, _index));

            yield return YieldInstructionCache.WaitForSeconds(weaponManager.weaponInfoSMG.reloadTime);

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

        UIManager.instance.hp[_type].img_front.fillAmount -= 0.03f; //데미지만큼 피를깎음.
        UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHPImg(_type, 0.03f));
        /*
        switch (_type)
        {          
            case 0:
                UIManager.instance.myHP.img_front.fillAmount -= weaponManager.weaponInfoSMG.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoSMG.damage));
                break;
            case 1:
                UIManager.instance.teamHP.img_front.fillAmount -= weaponManager.weaponInfoSMG.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoSMG.damage));
                break;
            case 2:
                UIManager.instance.enemyHP[0].img_front.fillAmount -= weaponManager.weaponInfoSMG.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoSMG.damage));
                break;
            case 3:
                UIManager.instance.enemyHP[1].img_front.fillAmount -= weaponManager.weaponInfoSMG.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoSMG.damage));
                break;
        }
        */
    }
}