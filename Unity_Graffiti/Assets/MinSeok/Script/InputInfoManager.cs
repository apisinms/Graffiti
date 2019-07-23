using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputInfoManager : MonoBehaviour
{

	public GameObject panel_login, panel_join; //로그인창, 회원가입창

	//로그인, 회원가입 입력필드
	public InputField inputField_login_id, inputField_login_pw, 
		inputField_join_id, inputField_join_pw;

	public Button btn_login_enter, btn_join_enter; //확인버튼들]
	public Text txt_result; //로그인, 가입 오류시 문자출력

	NetworkManager networkManager;

	void Start()
	{
		networkManager = NetworkManager.GetInstance;
		if (networkManager == null)
			Debug.Log("서버에 접속할 수 없음\n");

		inputField_login_id.Select(); //맨처음 로그인아이디 입력창 자동선택
	}

	void Update()
	{
		if(inputField_login_id.isFocused == true) //포커스가 입력창에 맞춰졌을때 탭키눌러서 자동으로 다음껄로 넘어가게
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				inputField_login_pw.Select();
			}
		}
		else if(inputField_login_pw.isFocused == true) //포커스가 pw입력창이면, 탭키로 확인으로 바꿈
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				btn_login_enter.Select();
			}
		}

		
		if (inputField_join_id.isFocused == true)
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				inputField_join_pw.Select();
			}
		}
		if (inputField_join_pw.isFocused == true)
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				btn_join_enter.Select();
			}
		}
	  
	}

	public void EnablePanel(int _type) //로그인창이냐 회원가입창이냐.
	{
		txt_result.text = null;

		switch (_type)    
		{
			case 1: //1은 로그인창에서 회원가입누를때 가입창으로.     
				inputField_login_id.text = null;
				inputField_login_pw.text = null;
				panel_login.SetActive(false);
				panel_join.SetActive(true);
				inputField_join_id.Select();
				break;
			case 2: //2는 회원가입창에서 닫기버튼 누르면 다시 로그인창으로
				inputField_join_id.text = null;
				inputField_join_pw.text = null;
				panel_join.SetActive(false);
				panel_login.SetActive(true);
				inputField_login_id.Select();
				break;
		}
	}

	public void BtnEnterLogin() //로그인 정보 입력후 확인버튼
	{
		Debug.Log("입력한 로그인 ID: " + inputField_login_id.text);
		Debug.Log("입력한 로그인 PW: " + inputField_login_pw.text);

		string retMsg = NetworkManager.GetInstance.MayILogin(inputField_login_id.text, inputField_login_pw.text);

		Debug.Log("로그인 결과 : " + retMsg);
		txt_result.text = retMsg;

		//로그인정보가 틀렸다면 아래작성
		//   txt_result.text = "로그인불가. 다시 입력 하시오"; //예시로 해놓은거임

		//   inputField_login_id.text = null;
		//   inputField_login_pw.text = null;
		inputField_login_id.Select();

		//로그인이 성공하면 아래작성
		//txt_result.text = "로그인성공";
		SceneManager.LoadScene("MainMenu"); //메인타이틀로 입장
	}

	public void BtnEnterJoin() //회원가입 정보 입력후 확인버튼
	{
		Debug.Log("입력한 회원가입 ID: " + inputField_join_id.text);
		Debug.Log("입력한 회원가입 PW: " + inputField_join_pw.text);

		//회원가입 정보가 틀렸다면 아래작성
		txt_result.text = "회원가입불가. 다시 입력 하시오"; //예시로 해놓은거임
	  //  inputField_join_id.text = null;
	   // inputField_join_pw.text = null;

		//회원가입에 성공하면 아래작성
		//txt_result.text = "회원가입에 성공하였습니다.";

		inputField_join_id.Select();
	}


	public void BtnQuit() //로그인 패널에서 종료를 누르면
	{
		//클로즈 소켓 이후 클라이언트 종료
		Application.Quit();
	}

}
