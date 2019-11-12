using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_SG : MonoBehaviour, IMainWeaponType
{
    public static Main_SG instance;
    private WeaponManager weaponManager;
    private int myIndex { get; set; }

    #region SG
    public struct _PLAYER_SG_INFO
    {
        public GameObject[] obj_bulletClone;
        public Transform[] tf_bulletClone;
        public Vector3[,] vt_bulletPattern;
        public int bulletPatternIndex;
        public int curAmmo;
        public float reloadTime;
        public string mainWname;
    }

    public _PLAYER_SG_INFO[] playerSGInfo { get; set; }
    #endregion

    private void Start()
    {
        weaponManager = WeaponManager.instance;
        myIndex = GameManager.instance.myIndex;

        playerSGInfo = new _PLAYER_SG_INFO[GameManager.instance.gameInfo.maxPlayer];

#if !NETWORK
        weaponManager.weaponInfoSG.maxAmmo = 2;
        weaponManager.weaponInfoSG.fireRate = 0.14f;
        weaponManager.weaponInfoSG.damage = 0.04f;
        weaponManager.weaponInfoSG.accuracy = 0.06f;
        weaponManager.weaponInfoSG.range = 10.0f;
        weaponManager.weaponInfoSG.speed = 2000.0f;
		weaponManager.weaponInfoSG.reloadTime = 2.0f;
#endif

		for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
        {
            playerSGInfo[i].obj_bulletClone = new GameObject[5];
            playerSGInfo[i].tf_bulletClone = new Transform[5];
            playerSGInfo[i].vt_bulletPattern = new Vector3[2, 5];
            playerSGInfo[i].bulletPatternIndex = 0;
            playerSGInfo[i].curAmmo = weaponManager.weaponInfoSG.maxAmmo;
            playerSGInfo[i].reloadTime = weaponManager.weaponInfoSG.reloadTime;
            playerSGInfo[i].mainWname = "SG";
        }
    }

    public static Main_SG GetMainWeaponInstance()
    {
        if (instance == null)
            instance = (Main_SG)WeaponManager.instance.cn_mainWeaponList[2]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

        return instance;
    }

    public IEnumerator ActionFire()
    {
        if (playerSGInfo[myIndex].curAmmo <= 0)
        {
            ReloadAmmoProcess(myIndex);
            yield break;
        }

        EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, myIndex);

        for (int i = 0; i < 5; i++)
        {
            playerSGInfo[myIndex].obj_bulletClone[i] = PoolManager.instance.GetBulletFromPool(myIndex);
            playerSGInfo[myIndex].tf_bulletClone[i] = playerSGInfo[myIndex].obj_bulletClone[i].transform;
        }

        var shellClone = PoolManager.instance.GetShellFromPool(myIndex);
        PoolManager.instance.StartCoroutine(PoolManager.instance.CheckShellEnd(shellClone, myIndex));

        playerSGInfo[myIndex].vt_bulletPattern[0, 0].x = (playerSGInfo[myIndex].tf_bulletClone[0].forward.x - playerSGInfo[myIndex].tf_bulletClone[0].right.x * 0.05f) - (playerSGInfo[myIndex].tf_bulletClone[0].right.x * 0.2f);
        playerSGInfo[myIndex].vt_bulletPattern[0, 0].z = (playerSGInfo[myIndex].tf_bulletClone[0].forward.z - playerSGInfo[myIndex].tf_bulletClone[0].right.z * 0.05f) - (playerSGInfo[myIndex].tf_bulletClone[0].right.z * 0.2f);
        playerSGInfo[myIndex].vt_bulletPattern[0, 1].x = (playerSGInfo[myIndex].tf_bulletClone[1].forward.x - playerSGInfo[myIndex].tf_bulletClone[1].right.x * 0.05f) - (playerSGInfo[myIndex].tf_bulletClone[1].right.x * 0.1f);
        playerSGInfo[myIndex].vt_bulletPattern[0, 1].z = (playerSGInfo[myIndex].tf_bulletClone[1].forward.z - playerSGInfo[myIndex].tf_bulletClone[1].right.z * 0.05f) - (playerSGInfo[myIndex].tf_bulletClone[1].right.z * 0.1f);
        playerSGInfo[myIndex].vt_bulletPattern[0, 2].x = (playerSGInfo[myIndex].tf_bulletClone[2].forward.x - playerSGInfo[myIndex].tf_bulletClone[2].right.x * 0.05f);
        playerSGInfo[myIndex].vt_bulletPattern[0, 2].z = (playerSGInfo[myIndex].tf_bulletClone[2].forward.z - playerSGInfo[myIndex].tf_bulletClone[2].right.z * 0.05f);
        playerSGInfo[myIndex].vt_bulletPattern[0, 3].x = (playerSGInfo[myIndex].tf_bulletClone[3].forward.x - playerSGInfo[myIndex].tf_bulletClone[3].right.x * 0.05f) + (playerSGInfo[myIndex].tf_bulletClone[3].right.x * 0.1f);
        playerSGInfo[myIndex].vt_bulletPattern[0, 3].z = (playerSGInfo[myIndex].tf_bulletClone[3].forward.z - playerSGInfo[myIndex].tf_bulletClone[3].right.z * 0.05f) + (playerSGInfo[myIndex].tf_bulletClone[3].right.z * 0.1f);
        playerSGInfo[myIndex].vt_bulletPattern[0, 4].x = (playerSGInfo[myIndex].tf_bulletClone[4].forward.x - playerSGInfo[myIndex].tf_bulletClone[4].right.x * 0.05f) + (playerSGInfo[myIndex].tf_bulletClone[4].right.x * 0.2f);
        playerSGInfo[myIndex].vt_bulletPattern[0, 4].z = (playerSGInfo[myIndex].tf_bulletClone[4].forward.z - playerSGInfo[myIndex].tf_bulletClone[4].right.z * 0.05f) + (playerSGInfo[myIndex].tf_bulletClone[4].right.z * 0.2f);

        playerSGInfo[myIndex].vt_bulletPattern[1, 0].x = (playerSGInfo[myIndex].tf_bulletClone[0].forward.x + playerSGInfo[myIndex].tf_bulletClone[0].right.x * 0.05f) - (playerSGInfo[myIndex].tf_bulletClone[0].right.x * 0.2f);
        playerSGInfo[myIndex].vt_bulletPattern[1, 0].z = (playerSGInfo[myIndex].tf_bulletClone[0].forward.z + playerSGInfo[myIndex].tf_bulletClone[0].right.z * 0.05f) - (playerSGInfo[myIndex].tf_bulletClone[0].right.z * 0.2f);
        playerSGInfo[myIndex].vt_bulletPattern[1, 1].x = (playerSGInfo[myIndex].tf_bulletClone[1].forward.x + playerSGInfo[myIndex].tf_bulletClone[1].right.x * 0.05f) - (playerSGInfo[myIndex].tf_bulletClone[1].right.x * 0.1f);
        playerSGInfo[myIndex].vt_bulletPattern[1, 1].z = (playerSGInfo[myIndex].tf_bulletClone[1].forward.z + playerSGInfo[myIndex].tf_bulletClone[1].right.z * 0.05f) - (playerSGInfo[myIndex].tf_bulletClone[1].right.z * 0.1f);
        playerSGInfo[myIndex].vt_bulletPattern[1, 2].x = (playerSGInfo[myIndex].tf_bulletClone[2].forward.x + playerSGInfo[myIndex].tf_bulletClone[2].right.x * 0.05f);
        playerSGInfo[myIndex].vt_bulletPattern[1, 2].z = (playerSGInfo[myIndex].tf_bulletClone[2].forward.z + playerSGInfo[myIndex].tf_bulletClone[2].right.z * 0.05f);
        playerSGInfo[myIndex].vt_bulletPattern[1, 3].x = (playerSGInfo[myIndex].tf_bulletClone[3].forward.x + playerSGInfo[myIndex].tf_bulletClone[3].right.x * 0.05f) + (playerSGInfo[myIndex].tf_bulletClone[3].right.x * 0.1f);
        playerSGInfo[myIndex].vt_bulletPattern[1, 3].z = (playerSGInfo[myIndex].tf_bulletClone[3].forward.z + playerSGInfo[myIndex].tf_bulletClone[3].right.z * 0.05f) + (playerSGInfo[myIndex].tf_bulletClone[3].right.z * 0.1f);
        playerSGInfo[myIndex].vt_bulletPattern[1, 4].x = (playerSGInfo[myIndex].tf_bulletClone[4].forward.x + playerSGInfo[myIndex].tf_bulletClone[4].right.x * 0.05f) + (playerSGInfo[myIndex].tf_bulletClone[4].right.x * 0.2f);
        playerSGInfo[myIndex].vt_bulletPattern[1, 4].z = (playerSGInfo[myIndex].tf_bulletClone[4].forward.z + playerSGInfo[myIndex].tf_bulletClone[4].right.z * 0.05f) + (playerSGInfo[myIndex].tf_bulletClone[4].right.z * 0.2f);

        AudioManager.Instance.Play(1);
        for (int i = 0; i < playerSGInfo[myIndex].obj_bulletClone.Length; i++)
        {
            playerSGInfo[myIndex].tf_bulletClone[i].localRotation = Quaternion.LookRotation(playerSGInfo[myIndex].vt_bulletPattern[playerSGInfo[myIndex].bulletPatternIndex, i]);
            playerSGInfo[myIndex].obj_bulletClone[i].GetComponent<Rigidbody>().AddForce(playerSGInfo[myIndex].vt_bulletPattern[playerSGInfo[myIndex].bulletPatternIndex, i] * weaponManager.weaponInfoSG.speed, ForceMode.Acceleration);
        }
        playerSGInfo[myIndex].curAmmo--;
        UIManager.instance.SetAmmoStateTxt(playerSGInfo[myIndex].curAmmo);

        #if NETWORK
        NetworkManager.instance.SendIngamePacket();
        #endif

        switch (playerSGInfo[myIndex].bulletPatternIndex)
        {
            case 0:
                playerSGInfo[myIndex].bulletPatternIndex = 1;
                break;
            case 1:
                playerSGInfo[myIndex].bulletPatternIndex = 0;
                break;
        }

        if (playerSGInfo[myIndex].curAmmo <= 0)
        {
            ReloadAmmoProcess(myIndex);
            yield break;
        }
        yield break;
    }

    public IEnumerator ActionFire(int _index)
	{
        if (_index == myIndex)
            yield break;
        
        #if NETWORK
        if (NetworkManager.instance.GetReloadState(_index) == true)
        {
            StartDecreaseReloadGage(_index);
            yield break;
        }
        #endif

        EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, _index);

        for (int i = 0; i < 5; i++)
		{
			playerSGInfo[_index].obj_bulletClone[i] = PoolManager.instance.GetBulletFromPool(_index);
			playerSGInfo[_index].tf_bulletClone[i] = playerSGInfo[_index].obj_bulletClone[i].transform;
		}

		var shellClone = PoolManager.instance.GetShellFromPool(_index);
		PoolManager.instance.StartCoroutine(PoolManager.instance.CheckShellEnd(shellClone, _index));

		playerSGInfo[_index].vt_bulletPattern[0, 0].x = (playerSGInfo[_index].tf_bulletClone[0].forward.x - playerSGInfo[_index].tf_bulletClone[0].right.x * 0.05f) - (playerSGInfo[_index].tf_bulletClone[0].right.x * 0.2f);
		playerSGInfo[_index].vt_bulletPattern[0, 0].z = (playerSGInfo[_index].tf_bulletClone[0].forward.z - playerSGInfo[_index].tf_bulletClone[0].right.z * 0.05f) - (playerSGInfo[_index].tf_bulletClone[0].right.z * 0.2f);
		playerSGInfo[_index].vt_bulletPattern[0, 1].x = (playerSGInfo[_index].tf_bulletClone[1].forward.x - playerSGInfo[_index].tf_bulletClone[1].right.x * 0.05f) - (playerSGInfo[_index].tf_bulletClone[1].right.x * 0.1f);
		playerSGInfo[_index].vt_bulletPattern[0, 1].z = (playerSGInfo[_index].tf_bulletClone[1].forward.z - playerSGInfo[_index].tf_bulletClone[1].right.z * 0.05f) - (playerSGInfo[_index].tf_bulletClone[1].right.z * 0.1f);
		playerSGInfo[_index].vt_bulletPattern[0, 2].x = (playerSGInfo[_index].tf_bulletClone[2].forward.x - playerSGInfo[_index].tf_bulletClone[2].right.x * 0.05f);
		playerSGInfo[_index].vt_bulletPattern[0, 2].z = (playerSGInfo[_index].tf_bulletClone[2].forward.z - playerSGInfo[_index].tf_bulletClone[2].right.z * 0.05f);
		playerSGInfo[_index].vt_bulletPattern[0, 3].x = (playerSGInfo[_index].tf_bulletClone[3].forward.x - playerSGInfo[_index].tf_bulletClone[3].right.x * 0.05f) + (playerSGInfo[_index].tf_bulletClone[3].right.x * 0.1f);
		playerSGInfo[_index].vt_bulletPattern[0, 3].z = (playerSGInfo[_index].tf_bulletClone[3].forward.z - playerSGInfo[_index].tf_bulletClone[3].right.z * 0.05f) + (playerSGInfo[_index].tf_bulletClone[3].right.z * 0.1f);
		playerSGInfo[_index].vt_bulletPattern[0, 4].x = (playerSGInfo[_index].tf_bulletClone[4].forward.x - playerSGInfo[_index].tf_bulletClone[4].right.x * 0.05f) + (playerSGInfo[_index].tf_bulletClone[4].right.x * 0.2f);
		playerSGInfo[_index].vt_bulletPattern[0, 4].z = (playerSGInfo[_index].tf_bulletClone[4].forward.z - playerSGInfo[_index].tf_bulletClone[4].right.z * 0.05f) + (playerSGInfo[_index].tf_bulletClone[4].right.z * 0.2f);

		playerSGInfo[_index].vt_bulletPattern[1, 0].x = (playerSGInfo[_index].tf_bulletClone[0].forward.x + playerSGInfo[_index].tf_bulletClone[0].right.x * 0.05f) - (playerSGInfo[_index].tf_bulletClone[0].right.x * 0.2f);
		playerSGInfo[_index].vt_bulletPattern[1, 0].z = (playerSGInfo[_index].tf_bulletClone[0].forward.z + playerSGInfo[_index].tf_bulletClone[0].right.z * 0.05f) - (playerSGInfo[_index].tf_bulletClone[0].right.z * 0.2f);
		playerSGInfo[_index].vt_bulletPattern[1, 1].x = (playerSGInfo[_index].tf_bulletClone[1].forward.x + playerSGInfo[_index].tf_bulletClone[1].right.x * 0.05f) - (playerSGInfo[_index].tf_bulletClone[1].right.x * 0.1f);
		playerSGInfo[_index].vt_bulletPattern[1, 1].z = (playerSGInfo[_index].tf_bulletClone[1].forward.z + playerSGInfo[_index].tf_bulletClone[1].right.z * 0.05f) - (playerSGInfo[_index].tf_bulletClone[1].right.z * 0.1f);
		playerSGInfo[_index].vt_bulletPattern[1, 2].x = (playerSGInfo[_index].tf_bulletClone[2].forward.x + playerSGInfo[_index].tf_bulletClone[2].right.x * 0.05f);
		playerSGInfo[_index].vt_bulletPattern[1, 2].z = (playerSGInfo[_index].tf_bulletClone[2].forward.z + playerSGInfo[_index].tf_bulletClone[2].right.z * 0.05f);
		playerSGInfo[_index].vt_bulletPattern[1, 3].x = (playerSGInfo[_index].tf_bulletClone[3].forward.x + playerSGInfo[_index].tf_bulletClone[3].right.x * 0.05f) + (playerSGInfo[_index].tf_bulletClone[3].right.x * 0.1f);
		playerSGInfo[_index].vt_bulletPattern[1, 3].z = (playerSGInfo[_index].tf_bulletClone[3].forward.z + playerSGInfo[_index].tf_bulletClone[3].right.z * 0.05f) + (playerSGInfo[_index].tf_bulletClone[3].right.z * 0.1f);
		playerSGInfo[_index].vt_bulletPattern[1, 4].x = (playerSGInfo[_index].tf_bulletClone[4].forward.x + playerSGInfo[_index].tf_bulletClone[4].right.x * 0.05f) + (playerSGInfo[_index].tf_bulletClone[4].right.x * 0.2f);
		playerSGInfo[_index].vt_bulletPattern[1, 4].z = (playerSGInfo[_index].tf_bulletClone[4].forward.z + playerSGInfo[_index].tf_bulletClone[4].right.z * 0.05f) + (playerSGInfo[_index].tf_bulletClone[4].right.z * 0.2f);

        AudioManager.Instance.Play(1);
		for (int i = 0; i < playerSGInfo[_index].obj_bulletClone.Length; i++)
		{
			playerSGInfo[_index].tf_bulletClone[i].localRotation = Quaternion.LookRotation(playerSGInfo[_index].vt_bulletPattern[playerSGInfo[_index].bulletPatternIndex, i]);
			playerSGInfo[_index].obj_bulletClone[i].GetComponent<Rigidbody>().AddForce(playerSGInfo[_index].vt_bulletPattern[playerSGInfo[_index].bulletPatternIndex, i] * weaponManager.weaponInfoSG.speed, ForceMode.Acceleration);
		}

        switch (playerSGInfo[_index].bulletPatternIndex)
        {
            case 0:
                playerSGInfo[_index].bulletPatternIndex = 1;
                break;
            case 1:
                playerSGInfo[_index].bulletPatternIndex = 0;
                break;
        }
        yield break;
	}

	public void CheckFireRange(GameObject _obj_bullet, BulletCollision._BULLET_CLONE_INFO _info_bullet, int _index)
    {
        if (Vector3.Distance(_obj_bullet.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= Main_SG.instance.weaponManager.weaponInfoSG.range)
            PoolManager.instance.ReturnGunToPool(_obj_bullet, _info_bullet, _index);
    }

    private void StartDecreaseReloadGage(int _index)
    {
        if (BridgeClientToServer.instance.isStartReloadGageCor[_index] == false)
        {
            UIManager.instance.StartCoroutine(UIManager.instance.Cor_DecreaseReloadGageImg(weaponManager.weaponInfoSG.reloadTime, _index));
            BridgeClientToServer.instance.isStartReloadGageCor[_index] = true;
        }
    }

    public float GetReloadTime(int _index)
    {
        return playerSGInfo[_index].reloadTime;
    }

    public string GetWeaponName(int _index)
    {
        return playerSGInfo[_index].mainWname;
    }

    public Sprite GetWeaponSprite(int _index)
    {
        return UIManager.instance.spr_mainW[1];
    }

    public void SupplyAmmo(int _index)
    {
        playerSGInfo[_index].curAmmo = weaponManager.weaponInfoSG.maxAmmo;
        UIManager.instance.SetAmmoStateTxt(playerSGInfo[_index].curAmmo);
    }

    public void ReloadAmmoProcess(int _index)
    {
        #if NETWORK
        NetworkManager.instance.SendIngamePacket();
        #endif

        if (playerSGInfo[_index].curAmmo >= weaponManager.weaponInfoSG.maxAmmo
            || weaponManager.isReloading == true) //풀탄창이면 재장전안함
            return;

        AudioManager.Instance.Play(8);
        StartCoroutine(Cor_ReloadAmmo(_index));
    }

    public IEnumerator Cor_ReloadAmmo(int _index)
    {
        playerSGInfo[_index].curAmmo = 0; //어차피 장전중엔 총을못쏘므로 총알을 0으로 만들어줌

        if (weaponManager.isReloading == false) //weaponManager.isReloading[_index] == false
        {
            weaponManager.isReloading = true;     
            #if NETWORK
            NetworkManager.instance.SendIngamePacket();
            #endif

            UIManager.instance.StartCoroutine(UIManager.instance.Cor_DecreaseReloadGageImg(weaponManager.weaponInfoSG.reloadTime, _index));

            yield return YieldInstructionCache.WaitForSeconds(weaponManager.weaponInfoSG.reloadTime);

            SupplyAmmo(_index);
            AudioManager.Instance.Play(9);

            weaponManager.isReloading = false;
            #if NETWORK
            NetworkManager.instance.SendIngamePacket();
            #endif

            //장전 이전상태가 사격중이였을경우 계속 이어서쏨
            if (PlayersManager.instance.actionState[_index] == _ACTION_STATE.SHOT ||
                PlayersManager.instance.actionState[_index] == _ACTION_STATE.CIR_AIM_SHOT || RightJoystick.RightTouch == true)
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
            PlayersManager.instance.Action_Death(_type);
            return;
        }

        UIManager.instance.hp[_type].img_front.fillAmount -= 0.06f; //데미지만큼 피를깎음.
        UIManager.instance.StartCoroutine(UIManager.instance.Cor_DecreaseMiddleHPImg(_type, 0.06f));
        /*
        switch (_type)
        {
            case 0:
                UIManager.instance.myHP.img_front.fillAmount -= weaponManager.weaponInfoSG.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoSG.damage));
                break;
            case 1:
                UIManager.instance.teamHP.img_front.fillAmount -= weaponManager.weaponInfoSG.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoSG.damage));
                break;
            case 2:
                UIManager.instance.enemyHP[0].img_front.fillAmount -= weaponManager.weaponInfoSG.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoSG.damage));
                break;
            case 3:
                UIManager.instance.enemyHP[1].img_front.fillAmount -= weaponManager.weaponInfoSG.damage; //데미지만큼 피를깎음.
                UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, weaponManager.weaponInfoSG.damage));
                break;
        }
        */
    }
}