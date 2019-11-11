using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sample : MonoBehaviour
{
    public Animator[] am_sprEffect; 
  
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("asd");
            am_sprEffect[0].SetTrigger("win_1");
            am_sprEffect[1].SetTrigger("win_1");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            am_sprEffect[2].SetTrigger("lose_1");
            am_sprEffect[3].SetTrigger("lose_1");
        }
    }
}
