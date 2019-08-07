using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public GameObject player;
    protected Animator am_playerMovement;

    protected virtual void Awake()
    {
        am_playerMovement = player.GetComponent<Animator>();
    }
}
