using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public int myIndex { get; set; }
    public _ACTION_STATE[] prevActionState { get; set; }

    private void Awake()
    {
        myIndex = GameManager.instance.myIndex;
        prevActionState = new _ACTION_STATE[C_Global.MAX_PLAYER];
    }
    private void Start()
    {
        //총알이 만들어지고 발사전 첫프레임에 쏜 플레이어의 상태를 저장시킴. 이렇게안하면 총알이 날아가는중에
        //쏜플레이어 상태가 샷이 아니게되면(쏘고 바로조준떼거나) 사정거리 적용이 풀려버려서 우주끝까지 총알이날아감
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
            prevActionState[i] = PlayersManager.instance.actionState[i];
    }
    private void Update()
    {
        for (int i = 0; i < C_Global.MAX_PLAYER; i++)
        {
            //총알이 쐇을때의 플레이어 상태가 샷일때만 사정거리 검사진행
            if (prevActionState[i] != _ACTION_STATE.SHOT && prevActionState[i] != _ACTION_STATE.CIR_AIM_SHOT)
                continue;

            CheckBulletRange(i);
        }
    }

    public void CheckBulletRange(int _index)
    {
        switch (WeaponManager.instance.mainWeapon[_index])
            {
                case _WEAPONS.AR:
                    {
                        if (Vector3.Distance(this.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= WeaponManager.instance.infoAR[_index].range)
                                WeaponManager.instance.ReturnBulletToPool(gameObject, _index);             
                    }
                    break;

                case _WEAPONS.SG:
                    {
                        if (Vector3.Distance(this.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= WeaponManager.instance.infoSG[_index].range)
                            WeaponManager.instance.ReturnBulletToPool(gameObject, _index);
                    }
                    break;

                case _WEAPONS.SMG:
                    {
                        if (Vector3.Distance(this.transform.position, PlayersManager.instance.obj_players[_index].transform.position) >= WeaponManager.instance.infoSMG[_index].range)
                            WeaponManager.instance.ReturnBulletToPool(gameObject, _index);
                    }
                    break;
            }   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
            return;

        // Debug.Log(other.name + "과(와) 충돌하였다111111111111111111111111111.", this);

        if (this.transform.parent.CompareTag("BulletPool1"))
            WeaponManager.instance.ReturnBulletToPool(gameObject, 0);

        else if (this.transform.parent.CompareTag("BulletPool2"))
            WeaponManager.instance.ReturnBulletToPool(gameObject, 1);

        else if (this.transform.parent.CompareTag("BulletPool3"))
            WeaponManager.instance.ReturnBulletToPool(gameObject, 2);

        else if (this.transform.parent.CompareTag("BulletPool4"))
            WeaponManager.instance.ReturnBulletToPool(gameObject, 3);
    }
}
