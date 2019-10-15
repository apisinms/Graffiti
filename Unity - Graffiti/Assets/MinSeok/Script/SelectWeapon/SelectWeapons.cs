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
public enum _WEAPONS : sbyte
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

public class SelectWeapons : UnityEngine.MonoBehaviour
{
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

    private NetworkManager networkManager;

	void Awake()
	{
        //Vector형으로 각 무기 버튼들의 포지션을 저장해둠
        for (int i = 0; i < vt_mainWeapons.Length; i++)
        {
            vt_mainWeapons[i] = btn_mainWeapons[i].transform.localPosition;
        }
        for (int i = 0; i < vt_subWeapons.Length; i++)
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
		networkManager = NetworkManager.instance;
		StartCoroutine(AppearMainWeapon());
	}

    public void BtnSelectWeapons(_WEAPONS_TYPE _type, _WEAPONS _name, Vector3 _checkMarkPos) //어떤무기를 선택했는가.
    {
		switch (_type)
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

		for (int i = 0; i < btn_mainWeapons.Length; i++)
		{
			btn_mainWeapons[i].interactable = true; //버튼 활성화
		}
		Debug.Log("주무기 코루틴 탈출!");

		StartCoroutine(SelectWeaponTimer());    // 여기에서 타이머 코루틴 시작
		yield break;
	}

	IEnumerator AppearSubWeapon(bool isReverse) //서브무기 스크린인지.
	{
		switch (isReverse)
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

	IEnumerator SelectWeaponTimer()
	{
		while(true)
		{
			// 타이머가 끝났다면
			if (networkManager.CheckTimerEnd() == true)
			{
                if (myMainWeapon == _WEAPONS.NODATA) //주무기 미선택시 주무기 랜덤선택
                    myMainWeapon = (_WEAPONS)Random.RandomRange(0, (int)_WEAPONS.MAIN_MAX_LENGTH);

                if (mySubWeapon == _WEAPONS.NODATA) //부무기 미선택시 부무기 랜덤선택
                    mySubWeapon = (_WEAPONS)Random.RandomRange((int)_WEAPONS.MAIN_MAX_LENGTH + 1, (int)_WEAPONS.SUB_MAX_LENGTH);

                // 선택한 무기를 서버로 보내고
                networkManager.MayISelectWeapon((sbyte)myMainWeapon, (sbyte)mySubWeapon);

				// 무기 보낸 결과 대기 코루틴 호출 후 코루틴 탈출
				StartCoroutine(CheckWeaponSend());
				yield break;
			}

			// 그게 아니면 서버가 보내온 시간으로 텍스트 설정.
			if (networkManager.CheckTimer(txt_selectTime.text) == true)
				txt_selectTime.text = networkManager.SysMsg;

			yield return null;
		}
	}

	// 서버로 보냈던 무기선택 결과를 대기한다.
	IEnumerator CheckWeaponSend()
	{
		while (true)
		{
			if (networkManager.CheckWeaponSelectSuccess() == true)
			{
				Debug.Log(myMainWeapon);
				Debug.Log(mySubWeapon);
				Debug.Log("itemselect 성공");

				SceneManager.LoadScene("MainGame"); //메인타이틀로 입장

				yield break;
			}

			else
				yield return null;
		}
	}
}
