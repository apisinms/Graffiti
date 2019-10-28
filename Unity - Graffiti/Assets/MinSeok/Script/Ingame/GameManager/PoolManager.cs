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

        Initialization_Bullet(GameManager.instance.gameInfo.maxPlayer);
        Initialization_Shell(GameManager.instance.gameInfo.maxPlayer);
        Initialization_Effect(GameManager.instance.gameInfo.maxPlayer);
    }

    private void Start()
    {
        for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++) //모든플레이어들의 총알풀 생성.
        {
            //얘네는 개인용으로다 잇음
            CreateBulletPool(i, 30);
            CreateShellPool(i, 30);
        }

        //이건 공용임
        CreateCollisionEffectPool(100);
    }
}