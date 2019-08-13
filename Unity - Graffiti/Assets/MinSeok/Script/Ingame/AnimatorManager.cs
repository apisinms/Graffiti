using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    private GameObject player;
    public Animator am_playerMovement { get; set; }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag(PlayerAttribute.instance.playerTag[0]);
        am_playerMovement = player.GetComponent<Animator>();
    }
}
