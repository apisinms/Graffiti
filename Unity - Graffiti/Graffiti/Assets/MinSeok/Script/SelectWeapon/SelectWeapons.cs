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
	// Main Weapons
	AR = 0,
	SG,
	SMG,

	// Sub Weapons
	TRAP,
	GRENADE,
}

public class SelectWeapons : MonoBehaviour
{

	public GameObject panel_mainWeapon, panel_subWeapon; //스크롤될 패널 2개
	public Button[] btn_mainWeapons = new Button[3]; //주무기 버튼 3개
	public Button[] btn_subWeapons = new Button[2]; //보조무기 버튼 3개
	public Button btn_return; //돌아가기 버튼
	public Text txt_selectTime; int selectTime = 30; //제한시간 텍스트
	private _WEAPONS myMainWeapon;
	private _WEAPONS mySubWeapon;

	void Awake()
	{
		//주무기 버튼 등록
		btn_mainWeapons[0].onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.MAIN, _WEAPONS.AR));
		btn_mainWeapons[1].onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.MAIN, _WEAPONS.SG));
		btn_mainWeapons[2].onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.MAIN, _WEAPONS.SMG));

		//보조무기 버튼등록
		btn_subWeapons[0].onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.SUB, _WEAPONS.TRAP));
		btn_subWeapons[1].onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.SUB, _WEAPONS.GRENADE));

		//돌아가기 버튼등록
		btn_return.onClick.AddListener(() => BtnSelectWeapons(_WEAPONS_TYPE.NODATA, 0));
	}

	void Start()
	{
		StartCoroutine(AppearMainWeapon());
	}

	void Update()
	{
		txt_selectTime.text = selectTime.ToString() + "초";

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			selectTime--;
		}
	}
	public void BtnSelectWeapons(_WEAPONS_TYPE _type, _WEAPONS _name) //어떤무기를 선택했는가.
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

				for (int i = 0; i < btn_mainWeapons.Length; i++)
				{
					btn_mainWeapons[i].interactable = false; //버튼 비활성화
				}
				StartCoroutine(AppearSubWeapon(false)); //보조무기 패널이 스크롤됨.
				break;

			case _WEAPONS_TYPE.SUB:
				mySubWeapon = _name;
				Debug.Log(mySubWeapon + "선택");

				for (int i = 0; i < btn_subWeapons.Length; i++)
				{
					btn_subWeapons[i].interactable = false;
				}
				btn_return.interactable = false;



				/// 테스트로 보내본다

				for (int i = 0; i < btn_subWeapons.Length; i++)
					btn_subWeapons[i].interactable = false;

				NetworkManager.instance.MayIItemSelect((sbyte)myMainWeapon, (sbyte)mySubWeapon);

				if (NetworkManager.instance.CheckItemSelectSuccess() == true)
				{
					Debug.Log(myMainWeapon);
					Debug.Log(mySubWeapon);
					Debug.Log("itemselect 성공");
				}

				else
					Debug.Log("itemselect 실패");

				SceneManager.LoadScene("MainGame"); //메인타이틀로 입장
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

}



/// 무기선택시간 30초가 지났을 때 이 부분이 실행되면 됨

//		 else //보조무기 선택시
//        {
//            for (int i = 0; i<btn_subWeapons.Length; i++)
//            {
//                btn_subWeapons[i].interactable = false;
//            }

//NetworkManager.GetInstance.MayIItemSelect(myWeapon[0], myWeapon[1]);


//            if (NetworkManager.GetInstance.CheckItemSelectFail() == true)
//                Debug.Log("itemselect 실패");

//            if (NetworkManager.GetInstance.CheckItemSelectSuccess() == true)
//            {
//                Debug.Log(myWeapon[0]);
//                Debug.Log(myWeapon[1]);
//                Debug.Log("itemselect 성공");
//            }

//}
