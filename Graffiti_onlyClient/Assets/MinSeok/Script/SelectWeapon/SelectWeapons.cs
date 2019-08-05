using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public enum _WEAPONS_TYPE
{
    NODATA = -1,
    MAIN,
    SUB
}
public enum _WEAPONS
{
    NODATA = -1,

    // main weapons
    AR,
    SG,
    SMG,
    MAIN_MAX_LENGTH,

    // sub weapons
    TRAP,
    GRENADE,
    SUB_MAX_LENGTH,
}


public class SelectWeapons : MonoBehaviour {

    public GameObject panel_mainWeapon, panel_subWeapon; //스크롤될 패널 2개;
    public Image[] img_checkMark = new Image[2]; //선택무기 체크마크 주무기용, 보조무기용

    public Button[] btn_mainWeapons = new Button[3]; //주무기 버튼 3개
    private Vector3[] vt_mainWeapons = new Vector3[3]; //주무기 버튼 3개의 포지션.

    public Button[] btn_subWeapons = new Button[2]; //보조무기 버튼 3개
    private Vector3[] vt_subWeapons = new Vector3[2]; //보조무기 버튼3개의 포지션

    public Button btn_return; //돌아가기 버튼

    public Text txt_selectTime; float selectTime = 30.0f; //제한시간 텍스트

    private _WEAPONS myMainWeapon;
    private _WEAPONS mySubWeapon;

    void Awake()
    {
        
        //Vector형으로 각 무기 버튼들의 포지션을 저장해둠
        for(int i=0; i< vt_mainWeapons.Length; i++)
        {
            vt_mainWeapons[i] = btn_mainWeapons[i].transform.localPosition;
        }
        for(int i=0; i< vt_subWeapons.Length; i++)
        {
            vt_subWeapons[i] = btn_subWeapons[i].transform.localPosition;
        }
        
        //주무기 버튼 등록
        btn_mainWeapons[0].onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.MAIN, _WEAPONS.AR,
            vt_mainWeapons[0]));
        btn_mainWeapons[1].onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.MAIN, _WEAPONS.SG,
            vt_mainWeapons[1]));
        btn_mainWeapons[2].onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.MAIN, _WEAPONS.SMG,
            vt_mainWeapons[2]));

        //보조무기 버튼등록
        btn_subWeapons[0].onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.SUB, _WEAPONS.TRAP,
            vt_subWeapons[0]));
        btn_subWeapons[1].onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.SUB, _WEAPONS.GRENADE,
            vt_subWeapons[1]));

        //돌아가기 버튼등록
        btn_return.onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.NODATA, 0, vt_subWeapons[0]));

        //무기 노선택.
        myMainWeapon = _WEAPONS.NODATA;
        mySubWeapon = _WEAPONS.NODATA;
    }

    void Start()
    {
        StartCoroutine(AppearMainWeapon());
        InvokeRepeating("SelectWeaponTimer", 0.0f, 1.0f); //1초마다 시간을 깎는 인보크
    }

    void Update()
    {
        /*
        if (selectTime > 0)
        {
            txt_selectTime.text = ((int)selectTime).ToString() + "초";
            selectTime -= (Time.smoothDeltaTime * 1.0f);
        }
        */
    }

    public void SelectWeaponTimer() //30초 시간제한거는 인보크함수
    {
        if(selectTime <= 0) 
        {
            Debug.Log("무기선택 종료!!!");

            //주무기 보조무기 랜덤선택
            myMainWeapon = (_WEAPONS)Random.RandomRange(0, (int)_WEAPONS.MAIN_MAX_LENGTH);
            mySubWeapon = (_WEAPONS)Random.RandomRange((int)_WEAPONS.MAIN_MAX_LENGTH+1, (int)_WEAPONS.SUB_MAX_LENGTH);

            SceneManager.LoadScene("MainGame"); //메인타이틀로 입장
            CancelInvoke("SelectWeaponTimer");
            //제한시간 종료시 아래 작성

        }
        txt_selectTime.text = "게임시작까지 " + ((int)selectTime).ToString() + "초";
        selectTime--; // (Time.smoothDeltaTime * 1.0f);
    }

    
    public void BtnSelectWeapons(_WEAPONS_TYPE _type, _WEAPONS _name, Vector3 _checkMarkPos) //어떤무기를 선택했는가.
    {
        switch(_type)
        {
            case _WEAPONS_TYPE.NODATA:
                Debug.Log("뒤로가기 선택");
                StartCoroutine(AppearSubWeapon(true));
                break;
            case _WEAPONS_TYPE.MAIN:
                myMainWeapon = _name;
                Debug.Log(myMainWeapon + "선택");

                img_checkMark[0].fillAmount = 1.0f; 
                img_checkMark[0].transform.localPosition = _checkMarkPos; //체크마크 표시

                for (int i = 0; i < btn_mainWeapons.Length; i++)
                {
                   btn_mainWeapons[i].interactable = false; //버튼 비활성화
                }
                StartCoroutine(AppearSubWeapon(false)); //보조무기 패널이 스크롤됨.
                break;
            case _WEAPONS_TYPE.SUB:
                mySubWeapon = _name;
                Debug.Log(mySubWeapon + "선택");

                img_checkMark[1].fillAmount = 1.0f;
                img_checkMark[1].transform.localPosition = _checkMarkPos;

                /*
                for (int i = 0; i < btn_subWeapons.Length; i++)
                {
                    btn_subWeapons[i].interactable = false;
                }
                btn_return.interactable = false;
                */

            //    SceneManager.LoadScene("MainGame"); //메인타이틀로 입장
                break;
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

           // Debug.Log(panel_mainWeapon.transform.localPosition.x + "     " + panel_mainWeapon.transform.localPosition.y);

            yield return null;
        }

        for(int i=0; i<btn_mainWeapons.Length; i++)
        {
            btn_mainWeapons[i].interactable = true; //버튼 활성화
        }
        Debug.Log("주무기 코루틴 탈출!");
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

               //     Debug.Log(panel_subWeapon.transform.localPosition.x + "     " + panel_subWeapon.transform.localPosition.y);

                    yield return null;
                }
                Debug.Log("부무기 반전 코루틴 탈출!");

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

                 //   Debug.Log(panel_subWeapon.transform.localPosition.x + "     " + panel_subWeapon.transform.localPosition.y);

                    yield return null;
                }

                for (int i = 0; i < btn_subWeapons.Length; i++)
                {
                    btn_subWeapons[i].interactable = true;
                }
                btn_return.interactable = true;
                Debug.Log("부무기 코루틴 탈출!");
                break;
        }
        yield break;
    }

}
