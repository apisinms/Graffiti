using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : UnityEngine.MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int vertical = 0, horizontal = 0;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            horizontal = 1;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            horizontal = -1;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            vertical = 1;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            vertical = -1;

        transform.Translate(new Vector3(2f * horizontal, 0f, 2f * vertical));
    }
}
