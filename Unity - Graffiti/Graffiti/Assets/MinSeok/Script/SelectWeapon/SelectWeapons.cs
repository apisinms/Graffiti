using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectWeapons : MonoBehaviour {

    public GameObject panel_mainWeapon, panel_subWeapon; //스크롤될 패널 2개
    public Button[] btn_mainWeapons = new Button[3]; //주무기 버튼 3개
    public Button[] btn_subWeapons = new Button[2]; //보조무기 버튼 3개
    int[] myWeapon = new int[2]; //선택된 무기
    string mainW;
    string subW;

    void Awake()
    {
        //주무기 버튼 등록
        btn_mainWeapons[0].onClick.AddListener(() => BtnSelectWeapons(0));
        btn_mainWeapons[1].onClick.AddListener(() => BtnSelectWeapons(1));
        btn_mainWeapons[2].onClick.AddListener(() => BtnSelectWeapons(2));

        //보조무기 버튼등록
        btn_subWeapons[0].onClick.AddListener(() => BtnSelectWeapons(3));
        btn_subWeapons[1].onClick.AddListener(() => BtnSelectWeapons(4));
    }

    void Start()
    {
        StartCoroutine(AppearMainWeapon());
    }

    public void BtnSelectWeapons(int _type) //어떤무기를 선택했는가.
    {

        switch(_type)
        {
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
        if (_type != 3 && _type != 4) //주무기 선택시
        {
            for (int i = 0; i < btn_mainWeapons.Length; i++)
            {
                btn_mainWeapons[i].interactable = false; //버튼 비활성화
            }
            StartCoroutine(AppearSubWeapon()); //보조무기 패널이 스크롤됨.
        }
        else //보조무기 선택시
        {
            for (int i = 0; i < btn_subWeapons.Length; i++)
            {
                btn_subWeapons[i].interactable = false;
            }

            NetworkManager.GetInstance.MayIItemSelect(myWeapon[0], myWeapon[1]);


            if (NetworkManager.GetInstance.CheckItemSelectFail() == true)
                Debug.Log("itemselect 실패");

            if (NetworkManager.GetInstance.CheckItemSelectSuccess() == true)
            {
                Debug.Log(myWeapon[0]);
                Debug.Log(myWeapon[1]);
                Debug.Log("itemselect 성공");
            }

        }
    }

    IEnumerator AppearMainWeapon() //주무기 스크린인지
    {
        while (true)
        {
            //오차극복
            if (panel_mainWeapon.transform.localPosition.x >= 0 && panel_mainWeapon.transform.localPosition.x <= 2)
            {
                break;
            }

            //패널 화면을 좌측으로 스크롤
            panel_mainWeapon.transform.localPosition =
                new Vector2(Mathf.Lerp(panel_mainWeapon.transform.localPosition.x, 0, Time.smoothDeltaTime * 5.0f), 0);

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

    IEnumerator AppearSubWeapon() //서브무기 스크린인지.
    {
        while (true)
        {
            if (panel_subWeapon.transform.localPosition.x >= 0 && panel_subWeapon.transform.localPosition.x <= 2)
            {
                break;
            }

            panel_subWeapon.transform.localPosition =
                new Vector2(Mathf.Lerp(panel_subWeapon.transform.localPosition.x, 0, Time.smoothDeltaTime * 5.0f), 0);

            Debug.Log(panel_subWeapon.transform.localPosition.x + "     " + panel_subWeapon.transform.localPosition.y);

            yield return null;
        }

        for (int i = 0; i < btn_subWeapons.Length; i++)
        {
            btn_subWeapons[i].interactable = true;
        }
        Debug.Log("코루틴 탈출!");
        yield break;
    }

}
