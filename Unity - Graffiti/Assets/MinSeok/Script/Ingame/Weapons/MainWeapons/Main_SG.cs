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

        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            playerSGInfo[i].obj_bulletClone = new GameObject[5];
            playerSGInfo[i].tf_bulletClone = new Transform[5];
            playerSGInfo[i].vt_bulletPattern = new Vector3[2, 5];
            playerSGInfo[i].bulletPatternIndex = 0;
            playerSGInfo[i].curAmmo = 2;
        }

#if !NETWORK
        weaponManager.weaponInfoSG.maxAmmo = 30;
        weaponManager.weaponInfoSG.fireRate = 0.14f;
        weaponManager.weaponInfoSG.damage = 0.04f;
        weaponManager.weaponInfoSG.accuracy = 0.06f;
        weaponManager.weaponInfoSG.range = 20.0f;
        weaponManager.weaponInfoSG.speed = 2000.0f;
#endif
    }

    public static Main_SG GetMainWeaponInstance()
    {
        if (instance == null)
            instance = (Main_SG)WeaponManager.instance.cn_mainWeaponList[2]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

        return instance;
    }

    public IEnumerator ActionFire(int _index)
    {
        for (int i = 0; i < 5; i++)
        {
            playerSGInfo[_index].obj_bulletClone[i] = PoolManager.instance.GetBulletFromPool(_index);
            playerSGInfo[_index].tf_bulletClone[i] = playerSGInfo[_index].obj_bulletClone[i].transform;
        }

        playerSGInfo[_index].vt_bulletPattern[0, 0].x = (playerSGInfo[_index].tf_bulletClone[0].forward.x - playerSGInfo[_index].tf_bulletClone[0].right.x * 0.2f) - (playerSGInfo[_index].tf_bulletClone[0].right.x * 0.2f);
        playerSGInfo[_index].vt_bulletPattern[0, 0].z = (playerSGInfo[_index].tf_bulletClone[0].forward.z - playerSGInfo[_index].tf_bulletClone[0].right.z * 0.2f) - (playerSGInfo[_index].tf_bulletClone[0].right.z * 0.2f);
        playerSGInfo[_index].vt_bulletPattern[0, 1].x = (playerSGInfo[_index].tf_bulletClone[1].forward.x - playerSGInfo[_index].tf_bulletClone[1].right.x * 0.2f) - (playerSGInfo[_index].tf_bulletClone[1].right.x * 0.1f);
        playerSGInfo[_index].vt_bulletPattern[0, 1].z = (playerSGInfo[_index].tf_bulletClone[1].forward.z - playerSGInfo[_index].tf_bulletClone[1].right.z * 0.2f) - (playerSGInfo[_index].tf_bulletClone[1].right.z * 0.1f);
        playerSGInfo[_index].vt_bulletPattern[0, 2].x = (playerSGInfo[_index].tf_bulletClone[2].forward.x - playerSGInfo[_index].tf_bulletClone[2].right.x * 0.2f);
        playerSGInfo[_index].vt_bulletPattern[0, 2].z = (playerSGInfo[_index].tf_bulletClone[2].forward.z - playerSGInfo[_index].tf_bulletClone[2].right.z * 0.2f);
        playerSGInfo[_index].vt_bulletPattern[0, 3].x = (playerSGInfo[_index].tf_bulletClone[3].forward.x - playerSGInfo[_index].tf_bulletClone[3].right.x * 0.2f) + (playerSGInfo[_index].tf_bulletClone[3].right.x * 0.1f);
        playerSGInfo[_index].vt_bulletPattern[0, 3].z = (playerSGInfo[_index].tf_bulletClone[3].forward.z - playerSGInfo[_index].tf_bulletClone[3].right.z * 0.2f) + (playerSGInfo[_index].tf_bulletClone[3].right.z * 0.1f);
        playerSGInfo[_index].vt_bulletPattern[0, 4].x = (playerSGInfo[_index].tf_bulletClone[4].forward.x - playerSGInfo[_index].tf_bulletClone[4].right.x * 0.2f) + (playerSGInfo[_index].tf_bulletClone[4].right.x * 0.2f);
        playerSGInfo[_index].vt_bulletPattern[0, 4].z = (playerSGInfo[_index].tf_bulletClone[4].forward.z - playerSGInfo[_index].tf_bulletClone[4].right.z * 0.2f) + (playerSGInfo[_index].tf_bulletClone[4].right.z * 0.2f);

        playerSGInfo[_index].vt_bulletPattern[1, 0].x = (playerSGInfo[_index].tf_bulletClone[0].forward.x + playerSGInfo[_index].tf_bulletClone[0].right.x * 0.2f) - (playerSGInfo[_index].tf_bulletClone[0].right.x * 0.2f);
        playerSGInfo[_index].vt_bulletPattern[1, 0].z = (playerSGInfo[_index].tf_bulletClone[0].forward.z + playerSGInfo[_index].tf_bulletClone[0].right.z * 0.2f) - (playerSGInfo[_index].tf_bulletClone[0].right.z * 0.2f);
        playerSGInfo[_index].vt_bulletPattern[1, 1].x = (playerSGInfo[_index].tf_bulletClone[1].forward.x + playerSGInfo[_index].tf_bulletClone[1].right.x * 0.2f) - (playerSGInfo[_index].tf_bulletClone[1].right.x * 0.1f);
        playerSGInfo[_index].vt_bulletPattern[1, 1].z = (playerSGInfo[_index].tf_bulletClone[1].forward.z + playerSGInfo[_index].tf_bulletClone[1].right.z * 0.2f) - (playerSGInfo[_index].tf_bulletClone[1].right.z * 0.1f);
        playerSGInfo[_index].vt_bulletPattern[1, 2].x = (playerSGInfo[_index].tf_bulletClone[2].forward.x + playerSGInfo[_index].tf_bulletClone[2].right.x * 0.2f);
        playerSGInfo[_index].vt_bulletPattern[1, 2].z = (playerSGInfo[_index].tf_bulletClone[2].forward.z + playerSGInfo[_index].tf_bulletClone[2].right.z * 0.2f);
        playerSGInfo[_index].vt_bulletPattern[1, 3].x = (playerSGInfo[_index].tf_bulletClone[3].forward.x + playerSGInfo[_index].tf_bulletClone[3].right.x * 0.2f) + (playerSGInfo[_index].tf_bulletClone[3].right.x * 0.1f);
        playerSGInfo[_index].vt_bulletPattern[1, 3].z = (playerSGInfo[_index].tf_bulletClone[3].forward.z + playerSGInfo[_index].tf_bulletClone[3].right.z * 0.2f) + (playerSGInfo[_index].tf_bulletClone[3].right.z * 0.1f);
        playerSGInfo[_index].vt_bulletPattern[1, 4].x = (playerSGInfo[_index].tf_bulletClone[4].forward.x + playerSGInfo[_index].tf_bulletClone[4].right.x * 0.2f) + (playerSGInfo[_index].tf_bulletClone[4].right.x * 0.2f);
        playerSGInfo[_index].vt_bulletPattern[1, 4].z = (playerSGInfo[_index].tf_bulletClone[4].forward.z + playerSGInfo[_index].tf_bulletClone[4].right.z * 0.2f) + (playerSGInfo[_index].tf_bulletClone[4].right.z * 0.2f);

        EffectManager.instance.ps_tmpMuzzle[_index].body.option.loop = false;
        EffectManager.instance.ps_tmpMuzzle[_index].glow.option.loop = false;
        EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.loop = false;
        EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.loop = false;
        EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.loop = false;
        EffectManager.instance.ps_tmpMuzzle[_index].spark.option.loop = false;

        EffectManager.instance.ps_tmpMuzzle[_index].body.option.duration = 0.15f;
        EffectManager.instance.ps_tmpMuzzle[_index].glow.option.duration = 0.15f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.duration = 0.15f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.duration = 0.15f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.duration = 0.15f;
        EffectManager.instance.ps_tmpMuzzle[_index].spark.option.duration = 0.15f;

        EffectManager.instance.ps_tmpMuzzle[_index].body.option.simulationSpeed = 1.7f;
        EffectManager.instance.ps_tmpMuzzle[_index].glow.option.simulationSpeed = 1.7f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.simulationSpeed = 1.7f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.simulationSpeed = 1.7f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.simulationSpeed = 1.7f;
        EffectManager.instance.ps_tmpMuzzle[_index].spark.option.simulationSpeed = 1.7f;

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
        /*
        infoSG[myIndex].vt_bulletDir[0].x = PlayersManager.instance.direction2[myIndex].x - (infoSG[myIndex].tf_bulletClone[0].right.x * 0.2f);
        infoSG[myIndex].vt_bulletDir[0].z = PlayersManager.instance.direction2[myIndex].z - (infoSG[myIndex].tf_bulletClone[0].right.z * 0.2f);
        infoSG[myIndex].vt_bulletDir[1].x = PlayersManager.instance.direction2[myIndex].x - (infoSG[myIndex].tf_bulletClone[1].right.x * 0.1f);
        infoSG[myIndex].vt_bulletDir[1].z = PlayersManager.instance.direction2[myIndex].z - (infoSG[myIndex].tf_bulletClone[1].right.z * 0.1f);
        infoSG[myIndex].vt_bulletDir[2].x = PlayersManager.instance.direction2[myIndex].x;
        infoSG[myIndex].vt_bulletDir[2].z = PlayersManager.instance.direction2[myIndex].z;
        infoSG[myIndex].vt_bulletDir[3].x = PlayersManager.instance.direction2[myIndex].x + (infoSG[myIndex].tf_bulletClone[3].right.x * 0.1f);
        infoSG[myIndex].vt_bulletDir[3].z = PlayersManager.instance.direction2[myIndex].z + (infoSG[myIndex].tf_bulletClone[3].right.z * 0.1f);
        infoSG[myIndex].vt_bulletDir[4].x = PlayersManager.instance.direction2[myIndex].x + (infoSG[myIndex].tf_bulletClone[4].right.x * 0.2f);
        infoSG[myIndex].vt_bulletDir[4].z = PlayersManager.instance.direction2[myIndex].z + (infoSG[myIndex].tf_bulletClone[4].right.z * 0.2f);
        */
        StateManager.instance.Shot(false);
        yield break;
    }

    public void CheckFireRange(GameObject _obj_bullet, int _index)
    {
        if (Vector3.Distance(_obj_bullet.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= Main_SG.instance.weaponManager.weaponInfoSG.range)
            PoolManager.instance.ReturnBulletToPool(_obj_bullet, _index);
    }

    public void ApplyDamage(int _type, int _index) 
    {
        UIManager.instance.hp[_type].img_front.fillAmount -= 0.06f; //데미지만큼 피를깎음.
        UIManager.instance.StartCoroutine(UIManager.instance.DecreaseMiddleHP(_type, 0.06f));
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