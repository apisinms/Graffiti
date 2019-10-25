using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        Initialization_Bullet(C_Global.MAX_PLAYER);
        Initialization_Shell(C_Global.MAX_PLAYER);
        Initialization_Effect(C_Global.MAX_PLAYER);
    }

    private void Start()
    {
        for (int i = 0; i < C_Global.MAX_PLAYER; i++) //모든플레이어들의 총알풀 생성.
        {
            CreateBulletPool(i, 30);
            CreateShellPool(i, 30);
        }

        CreateCollisionEffectPool(100);
    }
}
