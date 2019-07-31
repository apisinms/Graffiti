using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum _TEAM
{
    RED = 0,
    BLUE
}

public class Move : MonoBehaviour {

    public GameObject[] redTeam = new GameObject[2];
    public GameObject[] blueTeam = new GameObject[2];

   // GameObject obj_me;
    Camera camera_main;

    int speed = 4;
    Animator am_run;

    public static _TEAM myTeam;
    Move tmp = null;

    

    void Awake()
    {
        tmp = redTeam[0].GetComponent<Move>();
        am_run = GetComponent<Animator>();
        myTeam = _TEAM.RED;
      
    }
    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        float keyHorizontal = Input.GetAxisRaw("Horizontal");
        float keyVertical = Input.GetAxisRaw("Vertical");
       
        transform.Translate(Vector3.right * speed * Time.smoothDeltaTime * (keyHorizontal ), Space.World);
        transform.Translate(Vector3.forward * speed * Time.smoothDeltaTime * (keyVertical ), Space.World);

        if (keyHorizontal == 0 && keyVertical == 0)
        {
            am_run.SetBool("idle_to_run", false);
        }
        else
        {
            am_run.SetBool("idle_to_run", true);
        }

    }
}
