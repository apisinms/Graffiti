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
    }

    public _PLAYER_SG_INFO[] playerSGInfo { get; set; }
    #endregion

    private void Start()
    {
        weaponManager = WeaponManager.instance;
        myIndex = GameManager.instance.myIndex;

        playerSGInfo = new _PLAYER_SG_INFO[C_Global.MAX_PLAYER];

#if !NETWORK
        weaponManager.weaponInfoSG.maxAmmo = 2;
        weaponManager.weaponInfoSG.fireRate = 0.14f;
        weaponManager.weaponInfoSG.damage = 0.04f;
        weaponManager.weaponInfoSG.accuracy = 0.06f;
        weaponManager.weaponInfoSG.range = 10.0f;
        weaponManager.weaponInfoSG.speed = 2000.0f;
		weaponManager.weaponInfoSG.reloadTime = 2.0f;
#endif

		for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            playerSGInfo[i].obj_bulletClone = new GameObject[5];
            playerSGInfo[i].tf_bulletClone = new Transform[5];
            playerSGInfo[i].vt_bulletPattern = new Vector3[2, 5];
            playerSGInfo[i].bulletPatternIndex = 0;
            playerSGInfo[i].curAmmo = weaponManager.weaponInfoSG.maxAmmo;
        }
    }

    public static Main_SG GetMainWeaponInstance()
    {
        if (instance == null)
            instance = (Main_SG)WeaponManager.instance.cn_mainWeaponList[2]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

        return instance;
    }

	public IEnumerator ActionFire(int _index)
	{
		if (_index == myIndex)
		{
			if (playerSGInfo[_index].curAmmo <= 0)
			{
				ReloadAmmo(_index);
				yield break;
			}
			EffectManager.instance.PlayEffect(_EFFECT_TYPE.MUZZLE, myIndex);
		}

		if (NetworkManager.instance.GetReloadState(_index) == true)
		{
			EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, _index);
			yield break;
		}

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

		if (_index == myIndex)
		{
			playerSGInfo[_index].curAmmo--;
			UIManager.instance.SetAmmoStateTxt(playerSGInfo[_index].curAmmo);
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

		if (_index == myIndex)
		{
			if (playerSGInfo[_index].curAmmo <= 0)
			{
				ReloadAmmo(_index);
				yield break;
			}
		}

		if (NetworkManager.instance.GetReloadState(_index) == true)
		{
			EffectManager.instance.StopEffect(_EFFECT_TYPE.MUZZLE, _index);
			yield break;
		}

		yield break;
	}

	public void CheckFireRange(GameObject _obj_bullet, BulletCollision._BULLET_CLONE_INFO _info_bullet, int _index)
    {
        if (Vector3.Distance(_obj_bullet.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= Main_SG.instance.weaponManager.weaponInfoSG.range)
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
        playerSGInfo[_index].curAmmo = 0; //어차피 장전중엔 총을못쏘므로 총알을 0으로 만들어줌

        if (weaponManager.isReloading == false)
        {
			weaponManager.isReloading = true;

			/// 여기에서 패킷 보내면 됨
			NetworkManager.instance.SendIngamePacket(weaponManager.GetCollisionChecker());

			Debug.Log("SG총알없음. 장전중");
            UIManager.instance.StartCoroutine(UIManager.instance.DecreaseReloadTimeImg(weaponManager.weaponInfoSG.reloadTime));

            yield return YieldInstructionCache.WaitForSeconds(weaponManager.weaponInfoSG.reloadTime);
            playerSGInfo[_index].curAmmo = weaponManager.weaponInfoSG.maxAmmo;
            UIManager.instance.SetAmmoStateTxt(playerSGInfo[_index].curAmmo);
            AudioManager.Instance.Play(9);
            Debug.Log("SG장전완료");
			weaponManager.isReloading = false;

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
            PlayersManager.instance.Action_Death(_type);
            return;
        }

        UIManager.instance.hp[_type].img_front.fillAmount -= 0.06f; //데미지만큼 피를깎음.
        UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHPImg(_type, 0.06f));
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