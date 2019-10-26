using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IJoystickControll
{
	void DragStart();
	void Drag(BaseEventData _Data);
	void DragEnd();
}

public class JoystickManager : MonoBehaviour
{
	private int myIndex { get; set; }

	void Awake()
	{
		myIndex = GameManager.instance.myIndex;

        //WeaponManager.instance.mainWeapon[myIndex] = _WEAPONS.SMG;
        //WeaponManager.instance.SetMainWeapon(Main_SMG.GetMainWeaponInstance(), myIndex);
#if !NETWORK
        ChangeGun(1);
#endif
    }

    public void ChangeGun(int _index)
	{
        #if !NETWORK
		switch (_index)
		{
			case 1:
				WeaponManager.instance.mainWeapon[myIndex] = _WEAPONS.AR;
				WeaponManager.instance.SetMainWeapon(Main_AR.GetMainWeaponInstance(), myIndex);

				UIManager.instance.weaponInfo.img_mainW.sprite = UIManager.instance.spr_mainW[0];
				UIManager.instance.weaponInfo.txt_ammoState.text = WeaponManager.instance.weaponInfoAR.maxAmmo.ToString();
				break;
			case 2:
				WeaponManager.instance.mainWeapon[myIndex] = _WEAPONS.SG;
				WeaponManager.instance.SetMainWeapon(Main_SG.GetMainWeaponInstance(), myIndex);

				UIManager.instance.weaponInfo.img_mainW.sprite = UIManager.instance.spr_mainW[1];
				UIManager.instance.weaponInfo.txt_ammoState.text = WeaponManager.instance.weaponInfoSG.maxAmmo.ToString();
				break;
			case 3:
				WeaponManager.instance.mainWeapon[myIndex] = _WEAPONS.SMG;
				WeaponManager.instance.SetMainWeapon(Main_SMG.GetMainWeaponInstance(), myIndex);

				UIManager.instance.weaponInfo.img_mainW.sprite = UIManager.instance.spr_mainW[2];
				UIManager.instance.weaponInfo.txt_ammoState.text = WeaponManager.instance.weaponInfoSMG.maxAmmo.ToString();
				break;
		}

		if (EffectManager.instance.ps_tmpMuzzle[myIndex].body.body != null)
			Destroy(EffectManager.instance.ps_tmpMuzzle[myIndex].body.body.gameObject);

		switch (WeaponManager.instance.mainWeapon[myIndex])
		{
			case _WEAPONS.AR:
				EffectManager.instance.ps_tmpMuzzle[myIndex].body.body = Instantiate(EffectManager.instance.ps_muzzlePrefebsList[0], PlayersManager.instance.tf_players[myIndex].transform) as ParticleSystem;
				break;
			case _WEAPONS.SG:
				EffectManager.instance.ps_tmpMuzzle[myIndex].body.body = Instantiate(EffectManager.instance.ps_muzzlePrefebsList[1], PlayersManager.instance.tf_players[myIndex].transform) as ParticleSystem;
				break;
			case _WEAPONS.SMG:
				EffectManager.instance.ps_tmpMuzzle[myIndex].body.body = Instantiate(EffectManager.instance.ps_muzzlePrefebsList[2], PlayersManager.instance.tf_players[myIndex].transform) as ParticleSystem;
				break;
		}

		EffectManager.instance.ps_tmpMuzzle[myIndex].glow.body = EffectManager.instance.ps_tmpMuzzle[myIndex].body.body.transform.GetChild(0).GetComponent<ParticleSystem>();
		EffectManager.instance.ps_tmpMuzzle[myIndex].spike.body = EffectManager.instance.ps_tmpMuzzle[myIndex].body.body.transform.GetChild(1).GetComponent<ParticleSystem>();
		EffectManager.instance.ps_tmpMuzzle[myIndex].flare.body = EffectManager.instance.ps_tmpMuzzle[myIndex].body.body.transform.GetChild(2).GetComponent<ParticleSystem>();

		EffectManager.instance.ps_tmpMuzzle[myIndex].body.option = EffectManager.instance.ps_tmpMuzzle[myIndex].body.body.main;
		EffectManager.instance.ps_tmpMuzzle[myIndex].glow.option = EffectManager.instance.ps_tmpMuzzle[myIndex].glow.body.main;
		EffectManager.instance.ps_tmpMuzzle[myIndex].spike.option = EffectManager.instance.ps_tmpMuzzle[myIndex].spike.body.main;
		EffectManager.instance.ps_tmpMuzzle[myIndex].flare.option = EffectManager.instance.ps_tmpMuzzle[myIndex].flare.body.main;
#endif
	}
	/*
    void Update()
    {

            //    if (Input.GetKeyDown(KeyCode.Space))

            //Debug.Log(MyPlayerManager.instance.myActionState);
            //Debug.Log(PlayersManager.instance.direction[PlayersManager.instance.myIndex]);
            //       new Vector3(Mathf.Cos(MyPlayerManager.instance.obj_myPlayer.transform.localEulerAngles.y),
            //       0.0f, Mathf.Sin(MyPlayerManager.instance.obj_myPlayer.transform.localEulerAngles.y)));
            //ClientSectorManager.instance.GetMyArea();
            //  Debug.Log(PlayersManager.instance.direction[PlayersManager.instance.myIndex]);
            //Debug.Log(PlayersManager.instance.direction2[PlayersManager.instance.myIndex]);

            //Debug.Log(PlayersManager.instance.actionState[PlayersManager.instance.myIndex]);

            //  Debug.Log(new Vector3(0, Mathf.Atan2(PlayersManager.instance.direction[PlayersManager.instance.myIndex].x,
            //    PlayersManager.instance.direction[PlayersManager.instance.myIndex].z) * Mathf.Rad2Deg, 0));
            //Debug.Log(PlayersManager.instance.obj_players[PlayersManager.instance.myIndex].transform.);
            //Debug.Log(PlayersManager.instance.obj_players[PlayersManager.instance.myIndex].transform.localEulerAngles.y);

            //tf_player.eulerAngles = new Vector3(0, Mathf.Atan2(playerDir.x, playerDir.y) * Mathf.Rad2Deg, 0); /
            /*
            Debug.Log(

                new Vector3(0, Mathf.Atan2(PlayersManager.instance.direction[PlayersManager.instance.myIndex].x,
                PlayersManager.instance.direction[PlayersManager.instance.myIndex].z) * Mathf.Rad2Deg, 0)

                );
              */

	//Debug.Log(PlayersManager.instance.stateInfo[myIndex].actionState);



	/*
	switch (PlayersManager.instance.stateInfo[myIndex].actionState)
	{
		case _ACTION_STATE.IDLE:
			PlayersManager.instance.Action_Idle();
			break;
		case _ACTION_STATE.CIR:          
			PlayersManager.instance.Action_CircuitNormal();
			break;
		case _ACTION_STATE.AIM:
			PlayersManager.instance.Action_AimingNormal();
			break;
		case _ACTION_STATE.CIR_AIM:
			PlayersManager.instance.Action_AimingWithCircuit();
			break;
		case _ACTION_STATE.CIR_AIM_SHOT:
			break;
	}

}
*/

}