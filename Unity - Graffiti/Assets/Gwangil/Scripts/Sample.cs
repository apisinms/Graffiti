using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : Singleton<Sample>
{
    private void Awake()
    {
       if(Sample.Instance != null)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
