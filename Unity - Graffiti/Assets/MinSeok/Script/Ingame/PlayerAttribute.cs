using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttribute : MonoBehaviour
{
    public static PlayerAttribute instance;

    [HideInInspector] public int myLocalNum;
    [System.NonSerialized] public float speed = 3.0f;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

}
