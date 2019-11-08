using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using KetosGames.SceneTransition;
using System.Text;

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

    private const string GuestRandomNumString = "1234567890";
    private const int GuestMaxLength = 7;

    private string guestID = "";
    private string guestPW = "";
    private string guestNick = "";

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.instance;
    }

    public void EnablePanel(int _type) //로그인창이냐 회원가입창이냐.
    {

        switch (_type)
        {
            case 1: //가입창 클릭
                if (panel_join.gameObject.activeSelf == true) // 가입창이 이미 켜져있다면
                {
                    panel_join.SetActive(false);

                    // 게스트 로그인 버튼 켜주기 
                    btn_GuestLogin.gameObject.SetActive(true);

                }
                else if (panel_login.gameObject.activeSelf == true) // 로그인 창이 켜져있다면
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

        networkManager.SysMsg = string.Empty;

        // 우선 로그인이 가능한지 서버로 정보를 보낸다.
        networkManager.MayILogin(
           inputField_login_id.text,
           inputField_login_pw.text);

        btn_login_enter.SetActive(false);   // 버튼 비활성
        StartCoroutine(LoginCheck());   // 로그인 검사 코루틴 시작
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
                    SceneLoader.LoadScene("Lobby");

                btn_login_enter.SetActive(true);   // 버튼 다시 원상 복구
                yield break;
            }
        }
    }

    public void BtnEnterJoin() //회원가입 정보 입력후 확인버튼
    {
        //공백이 없을시에만
        if (inputField_join_id.text.Length == 0 || inputField_join_pw.text.Length == 0 || inputField_join_nick.text.Length == 0)
        {
            txt_join_result.text = "공백이 있습니다.";
            return;
        }

        // Guest 들어간거는 게스트로그인을 위해서 걸러줌
        if (inputField_join_id.text.Contains("Guest") || inputField_join_nick.text.Contains("Guest"))
        {
            txt_join_result.text = "아이디 또는 닉네임에 부적절한 문자가 포함되어 있습니다.";
            return;
        }

        networkManager.SysMsg = string.Empty;

        // 회원가입이 가능한지 서버로 보낸다.
        networkManager.MayIJoin(
           inputField_join_id.text,
           inputField_join_pw.text,
           inputField_join_nick.text);


        btn_join_enter.SetActive(false);
        StartCoroutine(JoinCheck());   // 회원가입 확인 코루틴 실행
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

    public void GuestJoin()   // 게스트 회원가입
    {
        // 게스트용 랜덤 ID, PW, NickName을 얻어온다.
        guestID = MakeGuestInfo("GuestID_");
        guestPW = MakeGuestInfo();
        guestNick = MakeGuestInfo("Guest_");

        Debug.Log("ID = " + guestID + "PW = " + guestPW + "Nick = " + guestNick);

        networkManager.SysMsg = string.Empty;

        // 게스트 회원가입이 가능한지 서버로 보낸다.
        networkManager.MayIJoin(
           guestID,
           guestPW,
           guestNick);

        StartCoroutine(GuestJoinCheck());    // 게스트 회원가입 확인 코루틴 실행
    }

    public void GuestLogin()   // 게스트 로그인
    {
        // (일로 내려오면 회원가입 성공)이제 로그인이 가능한지 서버로 정보를 보낸다.
        networkManager.SysMsg = string.Empty;

        networkManager.MayILogin(guestID, guestPW);
        StartCoroutine(GuestLoginCheck());    // 게스트 회원가입 확인 코루틴 실행
    }

    private IEnumerator GuestJoinCheck()
    {
        while (true)
        {
            // 아직 서버에서 응답안왔으면 넘어감
            if (networkManager.SysMsg == "")
                yield return null;

            else
            {
                // 이미 아이디가 존재한다면
                if (networkManager.CheckJoin_IDExist() == true)
                    GuestJoin();    // 다시 회원가입

                //회원가입에 성공하면
                else if (networkManager.CheckJoin_Success() == true)
                    GuestLogin();   // 게스트 로그인 고

                yield break;
            }
        }
    }

    private IEnumerator GuestLoginCheck()
    {
        while (true)
        {
            if (networkManager.SysMsg == "")
                yield return null;

            else
            {
                //로그인이 성공하면 아래로(어차피 게스트회원가입시에 다 걸러놔서 무조건 로그인 성공함)
                if (networkManager.CheckLoginSuccess() == true)
                    SceneLoader.LoadScene("Lobby");

                yield break;
            }
        }
    }

    private string MakeGuestInfo(string _str = "")
    {
        StringBuilder str = new StringBuilder(_str);

        for (int i = 0; i < GuestMaxLength; i++)
        {
            str.Append(
               GuestRandomNumString[Random.Range(0, GuestRandomNumString.Length)]);
        }

        return str.ToString();
    }
}