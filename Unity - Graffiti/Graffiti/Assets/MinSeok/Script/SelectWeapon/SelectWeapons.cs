using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectWeapons : MonoBehaviour {

    public GameObject panel_mainWeapon, panel_subWeapon; //스크롤될 패널 2개
    public Button[] btn_mainWeapons = new Button[3]; //주무기 버튼 3개
    public Button[] btn_subWeapons = new Button[2]; //보조무기 버튼 3개
    public Button btn_return; //돌아가기 버튼
    public Text txt_selectTime; int selectTime = 30; //제한시간 텍스트
    int[] myWeapon = new int[2]; //선택된 무기

    void Awake()
    {
        //주무기 버튼 등록
        btn_mainWeapons[0].onClick.AddListener(() => BtnSelectWeapons(0));
        btn_mainWeapons[1].onClick.AddListener(() => BtnSelectWeapons(1));
        btn_mainWeapons[2].onClick.AddListener(() => BtnSelectWeapons(2));

        //보조무기 버튼등록
        btn_subWeapons[0].onClick.AddListener(() => BtnSelectWeapons(3));
        btn_subWeapons[1].onClick.AddListener(() => BtnSelectWeapons(4));

        //돌아가기 버튼등록
        btn_return.onClick.AddListener(() => BtnSelectWeapons(-1));
    }

    void Start()
    {
        StartCoroutine(AppearMainWeapon());
    }

    void Update()
    {
        txt_selectTime.text = selectTime.ToString() + "초";
        
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            selectTime--;
        }
    }
    public void BtnSelectWeapons(int _type) //어떤무기를 선택했는가.
    {
        switch(_type)
        {
            case -1:
                Debug.Log("뒤로가기 선택");
                break;
            case 0:
                Debug.Log("ar선택");
                myWeapon[0] = 0;
                break;
            case 1:
                Debug.Log("sg선택");
                myWeapon[0] = 1;
                break;
            case 2:
                Debug.Log("smg선택");
                myWeapon[0] = 2;
                break;
            case 3:
                Debug.Log("trap선택");
                myWeapon[1] = 3;
                break;
            case 4:
                Debug.Log("grenade선택");
                myWeapon[1] = 4;
                break;
        }

        if (_type >= 0 && _type <= 2) //주무기 선택시
        {
            for (int i = 0; i < btn_mainWeapons.Length; i++)
            {
                btn_mainWeapons[i].interactable = false; //버튼 비활성화
            }
            StartCoroutine(AppearSubWeapon(false)); //보조무기 패널이 스크롤됨.
        }
        else if(_type >= 3 && _type <= 4) //보조무기 선택시
        {
            for (int i = 0; i < btn_subWeapons.Length; i++)
            {
                btn_subWeapons[i].interactable = false;
            }
            btn_return.interactable = false;
            SceneManager.LoadScene("MainGame"); //메인타이틀로 입장
        }
        else //뒤로가기
        {
            StartCoroutine(AppearSubWeapon(true));
        }
    }

    IEnumerator AppearMainWeapon() //주무기 스크린인지
    {
        while (true)
        {
            //오차극복
            if (panel_mainWeapon.transform.localPosition.x >= 0 && panel_mainWeapon.transform.localPosition.x <= 4)
            {
                break;
            }

            //패널 화면을 좌측으로 스크롤
            panel_mainWeapon.transform.localPosition =
                new Vector2(Mathf.Lerp(panel_mainWeapon.transform.localPosition.x, 0, Time.smoothDeltaTime * 7.0f), 0);

            Debug.Log(panel_mainWeapon.transform.localPosition.x + "     " + panel_mainWeapon.transform.localPosition.y);

            yield return null;
        }

        for(int i=0; i<btn_mainWeapons.Length; i++)
        {
            btn_mainWeapons[i].interactable = true; //버튼 활성화
        }
        Debug.Log("코루틴 탈출!");
        yield break;
    }

    IEnumerator AppearSubWeapon(bool isReverse) //서브무기 스크린인지.
    {
        switch(isReverse)
        {
            case true:
                for (int i = 0; i < btn_subWeapons.Length; i++)
                {
                    btn_subWeapons[i].interactable = false;
                }
                btn_return.interactable = false;

                while (true)
                {
                    if (panel_subWeapon.transform.localPosition.x >= 1918 && panel_subWeapon.transform.localPosition.x <= 1920)
                    {
                        break;
                    }

                    panel_subWeapon.transform.localPosition =
                        new Vector2(Mathf.Lerp(panel_subWeapon.transform.localPosition.x, 1920, Time.smoothDeltaTime * 10.0f), 0);

                    Debug.Log(panel_subWeapon.transform.localPosition.x + "     " + panel_subWeapon.transform.localPosition.y);

                    yield return null;
                }
                Debug.Log("반전 코루틴 탈출!");

                //메인무기선택 버튼들을 다시 활성화
                for (int i = 0; i < btn_mainWeapons.Length; i++)
                {
                    btn_mainWeapons[i].interactable = true; //버튼 활성화
                }
                break;
            case false:
                while (true)
                {
                    if (panel_subWeapon.transform.localPosition.x >= 0 && panel_subWeapon.transform.localPosition.x <= 4)
                    {
                        break;
                    }

                    panel_subWeapon.transform.localPosition =
                        new Vector2(Mathf.Lerp(panel_subWeapon.transform.localPosition.x, 0, Time.smoothDeltaTime * 7.0f), 0);

                    Debug.Log(panel_subWeapon.transform.localPosition.x + "     " + panel_subWeapon.transform.localPosition.y);

                    yield return null;
                }

                for (int i = 0; i < btn_subWeapons.Length; i++)
                {
                    btn_subWeapons[i].interactable = true;
                }
                btn_return.interactable = true;
                Debug.Log("코루틴 탈출!");
                break;
        }


        yield break;
    }

}
