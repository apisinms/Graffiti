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
    _TMP[] tmp = new _TMP[4];

    public struct _TMP2
    {
        public string from;
        public Sprite spr;
        public string to;
    }

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

        tmp[2].obj_parent = obj_killLogs[2].gameObject;
        tmp[2].txt_from = tmp[2].obj_parent.transform.GetChild(0).GetComponent<Text>();
        tmp[2].img_weapon = tmp[2].obj_parent.transform.GetChild(1).GetComponent<Image>();
        tmp[2].txt_to = tmp[2].obj_parent.transform.GetChild(2).GetComponent<Text>();

        tmp[3].obj_parent = obj_killLogs[3].gameObject;
        tmp[3].txt_from = tmp[3].obj_parent.transform.GetChild(0).GetComponent<Text>();
        tmp[3].img_weapon = tmp[3].obj_parent.transform.GetChild(1).GetComponent<Image>();
        tmp[3].txt_to = tmp[3].obj_parent.transform.GetChild(2).GetComponent<Text>();


        tmp[0].txt_from.text = null;
        tmp[0].img_weapon.sprite = null;
        tmp[0].txt_to.text = null;

        tmp[1].txt_from.text = null;
        tmp[1].img_weapon.sprite = null;
        tmp[1].txt_to.text = null;

        tmp[2].txt_from.text = null;
        tmp[2].img_weapon.sprite = null;
        tmp[2].txt_to.text = null;

        tmp[3].txt_from.text = null;
        tmp[3].img_weapon.sprite = null;
        tmp[3].txt_to.text = null;
    }

    Queue<_TMP2> queueTmp = new Queue<_TMP2>();
    _TMP2 sample = new _TMP2();
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(tmpCor());
        }

        else if (Input.GetKeyDown(KeyCode.A))
        {
            sample.from = "1";
            sample.spr = spr_tmp;
            sample.to = "2";

            queueTmp.Enqueue(sample);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            sample.from = "3";
            sample.spr = spr_tmp;
            sample.to = "4";

            queueTmp.Enqueue(sample);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            sample.from = "5";
            sample.spr = spr_tmp;
            sample.to = "6";

            queueTmp.Enqueue(sample);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            sample.from = "7";
            sample.spr = spr_tmp;
            sample.to = "8";

            queueTmp.Enqueue(sample);
        }
    }

    int index = 0;
    int num = 0;
    IEnumerator tmpCor()
    {
        while (true)
        {
            if (queueTmp.Count > 0)
            {
                _TMP2 test = queueTmp.Dequeue();

                if (tmp[0].obj_parent.activeSelf == false)
                    index = 0;

                tmp[index].txt_from.text = test.from;
                tmp[index].img_weapon.sprite = test.spr;
                tmp[index].txt_to.text = test.to;
                tmp[index].obj_parent.SetActive(true);

                StartCoroutine(turnOff(tmp[index]));
                index++;
            }
            yield return null;
        }
    }

    IEnumerator turnOff(_TMP _tmp)
    {
        yield return YieldInstructionCache.WaitForSeconds(1.0f);
        _tmp.obj_parent.SetActive(false);
    }

}

