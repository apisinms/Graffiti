using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Animator am;

    // Start is called before the first frame update
    void Start()
    {
        am.Play("run", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
