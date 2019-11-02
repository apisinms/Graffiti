using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using KetosGames.SceneTransition;

public class InputInfoManager : UnityEngine.MonoBehaviour
{
	public GameObject panel_login, panel_join; //로그인창, 회원가입창

	//로그인, 회원가입 입력필드
	public TMP_InputField inputField_login_id, 
        inputField_login_pw,
		inputField_join_id, 
        inputField_join_nick, 
        inputField_join_pw;

	public GameObject btn_login_enter, btn_join_enter, btn_GuestLogin; //확인버튼들
	public TMP_Text txt_login_result, txt_join_result; //로그인, 가입 오류시 문자출력

	private NetworkManager networkManager;

	void Start()
	{
		networkManager = NetworkManager.instance;
	}

	//void Update()
	//{
	//	if (inputField_login_id.isFocused == true) //포커스가 입력창에 맞춰졌을때 탭키눌러서 자동으로 다음껄로 넘어가게
	//	{
	//		if (Input.GetKeyDown(KeyCode.Tab))
	//		{
	//			inputField_login_pw.Select();
	//		}
	//	}

	//	else if (inputField_login_pw.isFocused == true) //포커스가 pw입력창이면, 탭키로 확인으로 바꿈
	//	{
	//		if (Input.GetKeyDown(KeyCode.Tab))
	//		{
	//			btn_login_enter.Select();
	//		}
	//	}

 //       if (inputField_join_nick.isFocused == true)
 //       {
 //           if (Input.GetKeyDown(KeyCode.Tab))
 //           {
 //               inputField_join_id.Select();
 //           }
 //       }

 //       if (inputField_join_id.isFocused == true)
	//	{
	//		if (Input.GetKeyDown(KeyCode.Tab))
	//		{
	//			inputField_join_pw.Select();
	//		}
	//	}

	//	if (inputField_join_pw.isFocused == true)
	//	{
	//		if (Input.GetKeyDown(KeyCode.Tab))
	//		{
	//			btn_join_enter.Select();
	//		}
	//	}
	//}

	public void EnablePanel(int _type) //로그인창이냐 회원가입창이냐.
	{

		switch (_type)
		{
			case 1: //가입창 클릭
                if(panel_join.gameObject.activeSelf == true) // 가입창이 이미 켜져있다면
                {
                    panel_join.SetActive(false);

                    // 게스트 로그인 버튼 켜주기 
                    btn_GuestLogin.gameObject.SetActive(true);

                }
                else if(panel_login.gameObject.activeSelf == true) // 로그인 창이 켜져있다면
                {
                    panel_login.SetActive(false);
                    panel_join.SetActive(true);
                    inputField_join_nick.text = "";
                    inputField_join_id.text = "";
                    inputField_join_pw.text = "";
                    txt_join_result.text = "";
                    btn_GuestLogin.gameObject.SetActive(false);
                }
                else
                {
                    inputField_join_nick.text = "";
                    inputField_join_id.text = "";
                    inputField_join_pw.text = "";
                    txt_join_result.text = "";
                    panel_join.SetActive(true);
                    btn_GuestLogin.gameObject.SetActive(false);
                }
				break;
            case 2: //로그인 창 클릭
                if (panel_login.gameObject.activeSelf == true) // 로그인 창이 이미 켜져있다면
                {
                    panel_login.SetActive(false);

                    // 게스트 로그인 버튼 켜주기 
                    btn_GuestLogin.gameObject.SetActive(true);
                }
                else if (panel_join.gameObject.activeSelf == true) // 가입창이 켜져있다면
                {
                    panel_join.SetActive(false);
                    panel_login.SetActive(true);
                    inputField_login_id.text = "";
                    inputField_login_pw.text = "";
                    txt_login_result.text = "";
                    btn_GuestLogin.gameObject.SetActive(false);

                }
                else
                {
                    inputField_login_id.text = "";
                    inputField_login_pw.text = "";
                    txt_login_result.text = "";
                    panel_login.SetActive(true);
                    btn_GuestLogin.gameObject.SetActive(false);
                }
                break;
        }
	}

	public void BtnEnterLogin() //로그인 정보 입력후 확인버튼
	{
		//공백이 없을시에만
		if (inputField_login_id.text.Length == 0 || inputField_login_pw.text.Length == 0)
		{
			txt_login_result.text = "공백입력은 불가."; 
			return;
		}

		//Debug.Log("입력한 로그인 ID: " + inputField_login_id.text);
		//Debug.Log("입력한 로그인 PW: " + inputField_login_pw.text);

		networkManager.SysMsg = string.Empty;

		// 우선 로그인이 가능한지 서버로 정보를 보낸다.
		networkManager.MayILogin(
			inputField_login_id.text, 
			inputField_login_pw.text);

		btn_login_enter.SetActive(false);	// 버튼 비활성
		StartCoroutine(LoginCheck());	// 로그인 검사 코루틴 시작
	}

	private IEnumerator LoginCheck()
	{
		while (true)
		{
			if (networkManager.SysMsg == "")
				yield return null;

			else
			{
				string retMsg = networkManager.SysMsg;
				Debug.Log("로그인 결과 : " + retMsg);

                //로그인정보가 틀렸다면 아래작성
                if (networkManager.CheckLogin_IDError() == true)
                {
                    inputField_login_id.text = "";
                    inputField_login_pw.text = "";
                    txt_login_result.text = retMsg;
                }

                else if (networkManager.CheckLogin_PWError() == true)
                {
                    inputField_login_id.text = "";
                    inputField_login_pw.text = "";
                    txt_login_result.text = retMsg;
                }

                else if (networkManager.CheckLogin_IDExist() == true)
                {
                    inputField_login_id.text = "";
                    inputField_login_pw.text = "";
                    txt_login_result.text = retMsg;
                }

                //로그인이 성공하면 아래작성
                else if (networkManager.CheckLoginSuccess() == true)
                    SceneLoader.LoadScene("LobbyMenuScene");
                    //LoadingSceneManager.LoadScene("LobbyMenuScene", false);
                    //SceneManager.LoadScene("LobbyMenuScene"); //메인타이틀로 입장

				btn_login_enter.SetActive(true);	// 버튼 다시 원상 복구
				yield break;
			}
		}
	}

	public void BtnEnterJoin() //회원가입 정보 입력후 확인버튼
	{
		//공백이 없을시에만
		if (inputField_join_id.text.Length == 0 || inputField_join_pw.text.Length == 0 || inputField_join_nick.text.Length == 0)
		{
			txt_join_result.text = "공백입력은 불가.";
			return;
		}

		//Debug.Log("입력한 회원가입 ID: " + inputField_join_id.text);
		//Debug.Log("입력한 회원가입 PW: " + inputField_join_pw.text);
		//Debug.Log("입력한 회원가입 NICKNAME: " + inputField_join_nick.text);

		networkManager.SysMsg = string.Empty;

		// 회원가입이 가능한지 서버로 보낸다.
		networkManager.MayIJoin(
			inputField_join_id.text, 
			inputField_join_pw.text, 
			inputField_join_nick.text);


		btn_join_enter.SetActive(false);
		StartCoroutine(JoinCheck());	// 회원가입 확인 코루틴 실행
	}

	private IEnumerator JoinCheck()
	{
		while (true)
		{
			if (networkManager.SysMsg == "")
				yield return null;

			else
			{
				string retMsg = networkManager.SysMsg;
				//Debug.Log("회원가입 결과 : " + retMsg);

				// 회원가입 정보가 틀렸다면 아래작성
				if (networkManager.CheckJoin_IDExist() == true)
				{
                    inputField_join_nick.text = "";
                    inputField_join_id.text = "";
                    inputField_join_pw.text = "";
                    txt_join_result.text = retMsg;
					inputField_join_id.Select();
				}

				//회원가입에 성공하면 아래작성
				else if (networkManager.CheckJoin_Success() == true)
				{
                    //EnablePanel(1);
                    inputField_join_nick.text = "";
                    inputField_join_id.text = "";
                    inputField_join_pw.text = "";
                    txt_join_result.text = retMsg;
				}

				btn_join_enter.SetActive(true); // 버튼 원복
				yield break;
			}
		}
	}

	public void BtnQuit() //로그인 패널에서 종료를 누르면
	{
		networkManager.Disconnect();
		Application.Quit();
	}

}