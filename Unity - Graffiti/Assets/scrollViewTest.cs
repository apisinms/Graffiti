using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrollViewTest : MonoBehaviour
{
    public GameObject[] obj_killLogs;
    public Sprite spr_tmp;

    public struct _TMP
    {
        public GameObject obj_parent;
        public Text txt_from;
        public Image img_weapon;
        public Text txt_to;
    }
    _TMP[] tmp = new _TMP[2];

    // Start is called before the first frame update
    void Start()
    {
        tmp[0].obj_parent = obj_killLogs[0].gameObject;
        tmp[0].txt_from = tmp[0].obj_parent.transform.GetChild(0).GetComponent<Text>();
        tmp[0].img_weapon = tmp[0].obj_parent.transform.GetChild(1).GetComponent<Image>();
        tmp[0].txt_to = tmp[0].obj_parent.transform.GetChild(2).GetComponent<Text>();

        tmp[1].obj_parent = obj_killLogs[1].gameObject;
        tmp[1].txt_from = tmp[1].obj_parent.transform.GetChild(0).GetComponent<Text>();
        tmp[1].img_weapon = tmp[1].obj_parent.transform.GetChild(1).GetComponent<Image>();
        tmp[1].txt_to = tmp[1].obj_parent.transform.GetChild(2).GetComponent<Text>();


        tmp[0].txt_from.text = "김민석";
        tmp[0].img_weapon.sprite = spr_tmp;
        tmp[0].txt_to.text = "김광일";

        tmp[1].txt_from.text = "김민성";
        tmp[1].img_weapon.sprite = spr_tmp;
        tmp[1].txt_to.text = "황태준";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {

        }
        else if (Input.GetKeyDown(KeyCode.S))
        {

        }

    }
    IEnumerator tmpCor()
    {
        while(true)
        {

        }
    }
}
