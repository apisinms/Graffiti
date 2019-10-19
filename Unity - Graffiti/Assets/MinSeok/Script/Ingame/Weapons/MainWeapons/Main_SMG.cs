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
    }

    public _PLAYER_SMG_INFO[] playerSMGInfo { get; set; }
    #endregion

    private void Start()
    {
        weaponManager = WeaponManager.instance;
        myIndex = GameManager.instance.myIndex;

        playerSMGInfo = new _PLAYER_SMG_INFO[C_Global.MAX_PLAYER];

        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            playerSMGInfo[i].vt_bulletPattern = new Vector3[3];
            playerSMGInfo[i].bulletPatternIndex = 2;
            playerSMGInfo[i].prevBulletPatternIndex = 1;
            playerSMGInfo[i].curAmmo = 25;

        }

        weaponManager.weaponInfoSMG.maxAmmo = 25;
        weaponManager.weaponInfoSMG.fireRate = 0.07f;
        weaponManager.weaponInfoSMG.damage = 0.02f;
        weaponManager.weaponInfoSMG.accuracy = 0.1f;
        weaponManager.weaponInfoSMG.range = 15.0f;
        weaponManager.weaponInfoSMG.speed = 1200.0f;
    }

    public static Main_SMG GetMainWeaponInstance()
    {
        if (instance == null)
            instance = (Main_SMG)WeaponManager.instance.cn_mainWeaponList[3]; //StateManager.instance.obj_stateList.GetComponent<State_Circuit>();

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

        EffectManager.instance.ps_tmpMuzzle[_index].body.option.simulationSpeed = 1.5f;
        EffectManager.instance.ps_tmpMuzzle[_index].glow.option.simulationSpeed = 1.5f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane2.option.simulationSpeed = 1.5f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane3.option.simulationSpeed = 1.5f;
        EffectManager.instance.ps_tmpMuzzle[_index].plane4.option.simulationSpeed = 1.5f;
        EffectManager.instance.ps_tmpMuzzle[_index].spark.option.simulationSpeed = 1.5f;

        while (true)
        {
            var clone = PoolManager.instance.GetBulletFromPool(_index);
            Transform tf_clone = clone.transform;

            playerSMGInfo[_index].vt_bulletPattern[0].x = tf_clone.forward.x - (tf_clone.right.x * weaponManager.weaponInfoSMG.accuracy);
            playerSMGInfo[_index].vt_bulletPattern[0].z = tf_clone.forward.z - (tf_clone.right.z * weaponManager.weaponInfoSMG.accuracy);
            playerSMGInfo[_index].vt_bulletPattern[1].x = tf_clone.forward.x;
            playerSMGInfo[_index].vt_bulletPattern[1].z = tf_clone.forward.z;
            playerSMGInfo[_index].vt_bulletPattern[2].x = tf_clone.forward.x + (tf_clone.right.x * weaponManager.weaponInfoSMG.accuracy);
            playerSMGInfo[_index].vt_bulletPattern[2].z = tf_clone.forward.z + (tf_clone.right.z * weaponManager.weaponInfoSMG.accuracy);

            tf_clone.localRotation = Quaternion.LookRotation(playerSMGInfo[_index].vt_bulletPattern[playerSMGInfo[_index].bulletPatternIndex]);
            clone.GetComponent<Rigidbody>().AddForce(playerSMGInfo[_index].vt_bulletPattern[playerSMGInfo[_index].bulletPatternIndex] * weaponManager.weaponInfoSMG.speed, ForceMode.Acceleration);

            switch (playerSMGInfo[_index].bulletPatternIndex)
            {
                case 0:
                    playerSMGInfo[_index].bulletPatternIndex = 1;
                    //Debug.Log("중");
                    break;
                case 1:
                    if (playerSMGInfo[_index].prevBulletPatternIndex == 1)
                    {
                        playerSMGInfo[_index].bulletPatternIndex = 0;
                        playerSMGInfo[_index].prevBulletPatternIndex = 2;
                        //Debug.Log("좌");
                    }
                    else if (playerSMGInfo[_index].prevBulletPatternIndex == 2)
                    {
                        playerSMGInfo[_index].bulletPatternIndex = 2;
                        playerSMGInfo[_index].prevBulletPatternIndex = 1;
                        //Debug.Log("우");
                    }
                    break;
                case 2:
                    playerSMGInfo[_index].bulletPatternIndex = 1;
                    //Debug.Log("중");
                    break;
            }

            /*
             infoAR[myIndex].vt_bulletPattern[0].x = PlayersManager.instance.direction2[myIndex].x - (tf_clone.right.x * infoAR[myIndex].accuracy);
            infoAR[myIndex].vt_bulletPattern[0].z = PlayersManager.instance.direction2[myIndex].z - (tf_clone.right.z * infoAR[myIndex].accuracy);
            infoAR[myIndex].vt_bulletPattern[1].x = PlayersManager.instance.direction2[myIndex].x;
            infoAR[myIndex].vt_bulletPattern[1].z = PlayersManager.instance.direction2[myIndex].z;
            infoAR[myIndex].vt_bulletPattern[2].x = PlayersManager.instance.direction2[myIndex].x + (tf_clone.right.x * infoAR[myIndex].accuracy);
            infoAR[myIndex].vt_bulletPattern[2].z = PlayersManager.instance.direction2[myIndex].z + (tf_clone.right.z * infoAR[myIndex].accuracy);
            */
            yield return YieldInstructionCache.WaitForSeconds(weaponManager.weaponInfoSMG.fireRate);
        }
    }

    public void CheckFireRange(GameObject _obj_bullet, int _index)
    {
        if (Vector3.Distance(_obj_bullet.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= Main_SMG.instance.weaponManager.weaponInfoSMG.range)
            PoolManager.instance.ReturnBulletToPool(_obj_bullet, _index);
    }

    public void ApplyDamage(int _type, int _index)
    {
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
    }
}