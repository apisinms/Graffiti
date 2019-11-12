using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//                   UIManager_WorldSpace_1
public partial class UIManager : MonoBehaviour
{
    #region MARKER
    public GameObject obj_prefebsMarker;
    public _IMG_MARKER_INFO marker;
    public Vector3 markerAddPos { get; set; }

    public struct _IMG_MARKER_INFO //체력바는 맨뒤 중간 맨앞 이미지 3개가 겹쳐진 구성임.
    {
        public GameObject obj_parent;
        public Image img_marker { get; set; }
    }
    #endregion

    #region HP
    public GameObject[] obj_prefebsHP;   //0나, 1팀, 2적
    public _IMG_HP_INFO[] hp;
    public Vector3 hpAddPos { get; set; }
    public Coroutine curHpCor { get; set; }

    public struct _IMG_HP_INFO //체력바는 맨뒤 중간 맨앞 이미지 3개가 겹쳐진 구성임.
    {
        public GameObject obj_parent;
        public Image img_back { get; set; }
        public Image img_middle { get; set; }
        public Image img_front { get; set; }
    }
    #endregion

    #region NICKNAME
    public GameObject obj_prefebNickname;
    public _TXT_NICKNAME_INFO[] nickname;
    public Vector3 nickAddPos { get; set; }

    public struct _TXT_NICKNAME_INFO
    {
        public GameObject obj_parent;
        public TMP_Text txt_nickname { get; set; }
    }
    #endregion

    #region RELOAD_GAGE
    public GameObject obj_prefebReloadGage;
    public _IMG_RELOAD_INFO[] reloadGage;
    public Vector3 reloadAddPos { get; set; }

    public struct _IMG_RELOAD_INFO
    {
        public GameObject obj_parent;
        public Image img_reload { get; set; }
        public Image img_back { get; set; }
        public Text txt_reloadTime { get; set; }
    }
    #endregion

    #region CAPTURE_GAGE
    public GameObject obj_prefebCaptureGage;
    public GameObject obj_prefebCaptureGageSub;

    public _IMG_CAPTURE_INFO[] captureGage;
    public _IMG_CAPTURE_SUB_INFO[] captureGageSub;
    public Vector3 captureAddPos { get; set; }
    public Coroutine[] curCaptureCor { get; set; }
    public Coroutine[] curCaptureSubCor { get; set; }
    public bool[] isStartCaptureCor { get; set; }
    public bool[] isStartCaptureSubCor { get; set; }

    public TMP_Text txt_captureNotice { get; set; } //점령지 탈취, 도난시의 알림텍스트

    public struct _IMG_CAPTURE_INFO
    {
        public GameObject obj_parent;
        public Image img_capture { get; set; }
        public Image img_back { get; set; }
        public Text txt_captureTime { get; set; }
    }

    public struct _IMG_CAPTURE_SUB_INFO
    {
        public GameObject obj_parent;
        public Image img_back { get; set; }
        public Image img_captureSub { get; set; }
    }
    #endregion

    #region WEAPON_INFO
    public GameObject obj_prefebWeaponInfo;
    public Sprite[] spr_mainW;
    public _IMG_WEAPON_INFO weaponInfo;
    public Vector3 weaponAddPos { get; set; }

    public struct _IMG_WEAPON_INFO
    {
        public GameObject obj_parent;
        public Image img_mainW { get; set; }
        public TMP_Text txt_ammoState { get; set; }
    }
    #endregion

    public void Initialization_Marker()
    {
        markerAddPos = new Vector3(0, 2.3f, 2.1f);
        marker.obj_parent = Instantiate(obj_prefebsMarker, GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
        marker.img_marker = marker.obj_parent.transform.GetChild(0).GetComponent<Image>();
        marker.obj_parent.SetActive(true);

        switch (GameManager.instance.gameInfo.gameType)
        {
            case (int)C_Global.GameType._2vs2:
                {
                    if (myIndex == 0 || myIndex == 1)
                        marker.img_marker.color = Color.red;
                    else if (myIndex == 2 || myIndex == 3)
                        marker.img_marker.color = Color.blue;
                }
                break;

            case (int)C_Global.GameType._1vs1:
                {
                    if (myIndex == 0)
                        marker.img_marker.color = Color.red;
                    else
                        marker.img_marker.color = Color.blue;
                }
                break;
        }
    }

    private void Initialization_HP()
    {
        hp = new _IMG_HP_INFO[GameManager.instance.gameInfo.maxPlayer];
        hpAddPos = new Vector3(0, 2.1f, 1.35f);
        curHpCor = null;

        for (int i = 0; i < hp.Length; i++)
        {
            if (i == playersIndex[0]) // == myIndex
            {
                hp[i].obj_parent = Instantiate(obj_prefebsHP[0], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
            }

            switch (GameManager.instance.gameInfo.gameType)
            {
                case (int)C_Global.GameType._2vs2:
                    {
                        switch (GameManager.instance.gameInfo.maxPlayer)
                        {
                            case 2:
                                {
                                    if (i == playersIndex[1]) // == teamIndex
                                    {
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
                                    }

                                }
                                break;

                            case 3:
                                {
                                    if (i == playersIndex[1]) // == teamIndex
                                    {
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
                                    }

                                    else if (i == playersIndex[2]) // == enemyIndex[0]
                                    {
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
                                    }
                                }
                                break;

                            case 4:
                                {
                                    if (i == playersIndex[1]) // == teamIndex
                                    {
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[1], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
                                    }

                                    else if (i == playersIndex[2] || i == playersIndex[3]) // == enemyIndex[0]
                                    {
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
                                    }
                                }
                                break;
                        }
                    }
                    break;

                case (int)C_Global.GameType._1vs1:
                    {
                        switch (GameManager.instance.gameInfo.maxPlayer)
                        {
                            case 2:
                                {
                                    if (i == playersIndex[1]) // == enemy
                                    {
                                        hp[i].obj_parent = Instantiate(obj_prefebsHP[2], GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }

            hp[i].img_back = hp[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            hp[i].img_middle = hp[i].obj_parent.transform.GetChild(1).GetComponent<Image>();
            hp[i].img_front = hp[i].obj_parent.transform.GetChild(2).GetComponent<Image>();
        }
    }

    private void Initialization_Nickname()
    {
        nickname = new _TXT_NICKNAME_INFO[GameManager.instance.gameInfo.maxPlayer];
        nickAddPos = new Vector3(0, 2.0f, 0.9f);

        for (int i = 0; i < nickname.Length; i++)
        {
            nickname[i].obj_parent = Instantiate(obj_prefebNickname, GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
            nickname[i].txt_nickname = nickname[i].obj_parent.transform.GetChild(0).GetComponent<TMP_Text>();
#if NETWORK
         if(i == myIndex)
         {
            nickname[i].txt_nickname.text = NetworkManager.instance.NickName;
            PlayersManager.instance.nickname[i] = NetworkManager.instance.NickName;
         }
#else
            if (i == myIndex)
            {
                nickname[i].txt_nickname.text = "김민석";
            }
#endif
        }
    }

    private void Initialization_ReloadGage()
    {
        reloadGage = new _IMG_RELOAD_INFO[GameManager.instance.gameInfo.maxPlayer];
        reloadAddPos = new Vector3(0, 2.5f, 2.2f);

        for (int i = 0; i < reloadGage.Length; i++)
        {
            reloadGage[i].obj_parent = Instantiate(obj_prefebReloadGage, GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
            reloadGage[i].img_reload = reloadGage[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            reloadGage[i].img_back = reloadGage[i].obj_parent.transform.GetChild(1).GetComponent<Image>();
            reloadGage[i].txt_reloadTime = reloadGage[i].obj_parent.transform.GetChild(2).GetComponent<Text>();
            reloadGage[i].obj_parent.SetActive(false);
        }
    }

    private void Initialization_CaptureGage()
    {
        txt_captureNotice = GameObject.FindGameObjectWithTag("Canvas_overlay").transform.GetChild(8).GetComponent<TMP_Text>();
        captureGage = new _IMG_CAPTURE_INFO[CaptureManager.instance.MAX_TERRITORY_NUM];
        captureGageSub = new _IMG_CAPTURE_SUB_INFO[CaptureManager.instance.MAX_TERRITORY_NUM];
        captureAddPos = new Vector3(0, 4.5f, 0);
        curCaptureCor = new Coroutine[CaptureManager.instance.MAX_TERRITORY_NUM];
        curCaptureSubCor = new Coroutine[CaptureManager.instance.MAX_TERRITORY_NUM];
        isStartCaptureCor = new bool[CaptureManager.instance.MAX_TERRITORY_NUM];
        isStartCaptureSubCor = new bool[CaptureManager.instance.MAX_TERRITORY_NUM];

        for (int i = 0; i < captureGage.Length; i++)
        {
            curCaptureCor[i] = null;
            captureGage[i].obj_parent = Instantiate(obj_prefebCaptureGage, GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
            captureGage[i].img_capture = captureGage[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            captureGage[i].img_back = captureGage[i].obj_parent.transform.GetChild(1).GetComponent<Image>();
            captureGage[i].txt_captureTime = captureGage[i].obj_parent.transform.GetChild(2).GetComponent<Text>();
            captureGage[i].obj_parent.SetActive(false);
        }

        for (int i = 0; i < captureGageSub.Length; i++)
        {
            curCaptureSubCor[i] = null;
            captureGageSub[i].obj_parent = Instantiate(obj_prefebCaptureGageSub, GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
            captureGageSub[i].img_back = captureGageSub[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            captureGageSub[i].img_captureSub = captureGageSub[i].obj_parent.transform.GetChild(1).GetComponent<Image>();
            captureGageSub[i].obj_parent.SetActive(false);
        }
        txt_captureNotice.gameObject.SetActive(false);
    }

    public void Initialization_WeaponInfo_1()
    {
        weaponAddPos = new Vector3(-0.15f, 1.8f, -1.85f);

        weaponInfo.obj_parent = Instantiate(obj_prefebWeaponInfo, GameObject.FindGameObjectWithTag("Canvas_worldSpace1").transform);
        weaponInfo.img_mainW = weaponInfo.obj_parent.transform.GetChild(1).GetComponent<Image>();
        weaponInfo.txt_ammoState = weaponInfo.obj_parent.transform.GetChild(2).GetComponent<TMP_Text>();
    }

    public void Initialization_WeaponInfo_2()
    {
#if NETWORK
        switch (WeaponManager.instance.mainWeapon[myIndex])
        {
            case _WEAPONS.AR:
                weaponInfo.img_mainW.sprite = spr_mainW[0];
                weaponInfo.txt_ammoState.text = WeaponManager.instance.weaponInfoAR.maxAmmo.ToString();//weaponInfoAR.maxAmmo.ToString();
                break;
            case _WEAPONS.SG:
                weaponInfo.img_mainW.sprite = spr_mainW[1];
                weaponInfo.txt_ammoState.text = WeaponManager.instance.weaponInfoSG.maxAmmo.ToString();//WeaponManager.instance.weaponInfoSG.maxAmmo.ToString();
                break;
            case _WEAPONS.SMG:
                weaponInfo.img_mainW.sprite = spr_mainW[2];
                weaponInfo.txt_ammoState.text = WeaponManager.instance.weaponInfoSMG.maxAmmo.ToString();// WeaponManager.instance.weaponInfoSMG.maxAmmo.ToString();
                break;
        }
#else
        switch (WeaponManager.instance.mainWeapon[myIndex])
        {
            case _WEAPONS.AR:
                weaponInfo.img_mainW.sprite = spr_mainW[0];
                weaponInfo.txt_ammoState.text = 21.ToString();//weaponInfoAR.maxAmmo.ToString();
                break;
            case _WEAPONS.SG:
                weaponInfo.img_mainW.sprite = spr_mainW[1];
                weaponInfo.txt_ammoState.text = 2.ToString();//WeaponManager.instance.weaponInfoSG.maxAmmo.ToString();
                break;
            case _WEAPONS.SMG:
                weaponInfo.img_mainW.sprite = spr_mainW[2];
                weaponInfo.txt_ammoState.text = 17.ToString();// WeaponManager.instance.weaponInfoSMG.maxAmmo.ToString();
                break;
        }
#endif
    }

    public void SetNickname(string _nickname, int _index)
    {
        nickname[_index].txt_nickname.text = _nickname;
    }

    public void SetAmmoStateTxt(int _num)
    {
        weaponInfo.txt_ammoState.text = _num.ToString();
    }

    public void HealthUIChanger(int _idx, float _health)
    {
        hp[_idx].img_front.fillAmount = _health * 0.01f;
        StartCoroutine(Cor_DecreaseMiddleHPImg(_idx, _health * 0.01f));
    }

    public IEnumerator Cor_DecreaseMiddleHPImg(int _index, float _curHP) //중간 체력 img는 효과를 적용할것이다.
    {
        //Debug.Log("들어옴");
        yield return YieldInstructionCache.WaitForSeconds(0.6f);

#if NETWORK
      hp[_index].img_middle.fillAmount = _curHP;
#else
        hp[_index].img_middle.fillAmount -= _curHP;
#endif
        yield break;
    }

    public IEnumerator Cor_DecreaseReloadGageImg(float _time, int _index)
    {
        reloadGage[_index].obj_parent.SetActive(true);
        reloadGage[_index].txt_reloadTime.text = (_time).ToString();

        while (true)
        {
            if (reloadGage[_index].img_reload.fillAmount <= 0)
            {
                reloadGage[_index].img_reload.fillAmount = 1.0f;
                reloadGage[_index].obj_parent.SetActive(false);
                BridgeClientToServer.instance.isStartReloadGageCor[_index] = false; //중복실행방지
                yield break;
            }

            reloadGage[_index].img_reload.fillAmount -= Time.smoothDeltaTime / _time;
            reloadGage[_index].txt_reloadTime.text = (reloadGage[_index].img_reload.fillAmount * _time).ToString();
            yield return null;
        }
    }

    public void DecreaseCaptureGageSubImg(int _triggerIdx, string _tag, bool _value)
    {
        if (_value == true)
        {
            if (isStartCaptureSubCor[_triggerIdx] == false)
            {
                curCaptureSubCor[_triggerIdx] = StartCoroutine(Cor_DecreaseCaptureGageSubImg(
               GameManager.instance.gameInfo.subSprayingTime,
               _triggerIdx,
               _tag)); //내 점령지 인덱스를 넘김
            }
        }
        else
        {
            if (isStartCaptureSubCor[_triggerIdx] == true)
            {
                isStartCaptureSubCor[_triggerIdx] = false;
                AudioManager.Instance.Stop(12);
                captureGageSub[_triggerIdx].img_captureSub.fillAmount = 1;
                captureGageSub[_triggerIdx].obj_parent.SetActive(false);
                StopCoroutine(curCaptureSubCor[_triggerIdx]);
            }
        }
    }

    public IEnumerator Cor_DecreaseCaptureGageSubImg(float _time, int _triggerIdx, string _playerTag)
    {
        if (isStartCaptureSubCor[_triggerIdx] == true)
            yield break;

        isStartCaptureSubCor[_triggerIdx] = true;

        captureGageSub[_triggerIdx].obj_parent.SetActive(true);

        if (_playerTag.Equals(GameManager.instance.myTag)) //나일때만 스프레이 뚜껑따는 사운드재생
            AudioManager.Instance.Play(12);

        while (true)
        {
            if (captureGageSub[_triggerIdx].img_captureSub.fillAmount <= 0)
            {
                captureGageSub[_triggerIdx].img_captureSub.fillAmount = 1.0f;
                captureGageSub[_triggerIdx].obj_parent.SetActive(false);

                //점령중애가 "나"면 서브게이지 끝난후 스프레이상태변환, 진짜 점령시작. 다른3명은 여기가 아니라 뷰어에서 해줌. 내스테이트를 서버로 보내니까 내꺼만,.
                if (_playerTag.Equals(GameManager.instance.myTag))
                {
                    StateManager.instance.Spray(true, _triggerIdx); //서브게이지가 끝나면 SPRAY상태로 변환. _triggerIdx를넣어 점령지를 바라보게함
#if NETWORK
                    NetworkManager.instance.SendIngamePacket();
#endif
                }
                DecreaseCaptureGageImg(_triggerIdx, _playerTag, true);
                StartGraffitySpraying(_triggerIdx, _playerTag, true);
                yield break;
            }

            captureGageSub[_triggerIdx].img_captureSub.fillAmount -= Time.smoothDeltaTime / _time;
            yield return null;
        }
    }

    public void DecreaseCaptureGageImg(int _triggerIdx, string _tag, bool _value)
    {
        if (_value == true)
        {
            if (isStartCaptureCor[_triggerIdx] == false)
                curCaptureCor[_triggerIdx] = StartCoroutine(Cor_DecreaseCaptureGageImg(
               GameManager.instance.gameInfo.mainSprayingTime,
               _triggerIdx,
               _tag)); //내 점령지 인덱스를 넘김
        }
        else
        {
            if (isStartCaptureCor[_triggerIdx] == true)
            {
                isStartCaptureCor[_triggerIdx] = false;
                captureGage[_triggerIdx].img_capture.fillAmount = 1;
                captureGage[_triggerIdx].obj_parent.SetActive(false);
                StopCoroutine(curCaptureCor[_triggerIdx]);
            }
        }
    }

    public IEnumerator Cor_DecreaseCaptureGageImg(float _time, int _triggerIdx, string _playerTag)
    {
        if (isStartCaptureCor[_triggerIdx] == true)
            yield break;

        isStartCaptureCor[_triggerIdx] = true;

        captureGage[_triggerIdx].obj_parent.SetActive(true);
        captureGage[_triggerIdx].txt_captureTime.text = (_time).ToString();

        while (true)
        {
            if (captureGage[_triggerIdx].img_capture.fillAmount <= 0)
            {
                captureGage[_triggerIdx].img_capture.fillAmount = 1.0f;
                captureGage[_triggerIdx].obj_parent.SetActive(false);

                /*
                int idx = 0;
                switch (_playerTag)
                {
                    case "Player1":
                        idx = 0;
                        break;
                    case "Player2":
                        idx = 1;
                        break;
                    case "Player3":
                        idx = 2;
                        break;
                    case "Player4":
                        idx = 3;
                        break;
                }
            
                switch ((C_Global.GameType)GameManager.instance.gameInfo.gameType)
                {
                    case C_Global.GameType._2vs2:
                        {
                            //진짜 점령및 탈취가 끝난후.
                            if (idx == GameManager.instance.playersIndex[0] || idx == GameManager.instance.playersIndex[1])
                            {
                                //Debug.Log("점령지를 탈취했습니다!");
                                UIManager.instance.txt_captureNotice.color = Color.green;
                                UIManager.instance.txt_captureNotice.text = (_triggerIdx + 1).ToString() + "번 점령지를 탈취 했습니다 !";
                                CaptureManager.instance.captureResult_team[_triggerIdx] = _CAPTURE_RESULT.GET;
                                CaptureManager.instance.captureResult_enemy[_triggerIdx] = _CAPTURE_RESULT.LOSE;
                                CaptureManager.instance.territoryOutline[_triggerIdx].OutlineColor = Color.green;
                                CaptureManager.instance.territoryOutline[_triggerIdx].HaloColor = Color.green;
                            }
                            else
                            {
                                //Debug.Log("점령지를 빼앗겼습니다!");
                                UIManager.instance.txt_captureNotice.color = Color.red;
                                UIManager.instance.txt_captureNotice.text = (_triggerIdx + 1).ToString() + "번 점령지를 빼앗겼습니다 !";
                                CaptureManager.instance.captureResult_team[_triggerIdx] = _CAPTURE_RESULT.LOSE;
                                CaptureManager.instance.captureResult_enemy[_triggerIdx] = _CAPTURE_RESULT.GET;
                                CaptureManager.instance.territoryOutline[_triggerIdx].OutlineColor = Color.red;
                                CaptureManager.instance.territoryOutline[_triggerIdx].HaloColor = Color.red;
                            }
                        }
                        break;

                    case C_Global.GameType._1vs1:
                        {
                            //진짜 점령및 탈취가 끝난후.
                            if (idx == GameManager.instance.playersIndex[0])
                            {
                                //Debug.Log("점령지를 탈취했습니다!");
                                UIManager.instance.txt_captureNotice.color = Color.green;
                                UIManager.instance.txt_captureNotice.text = (_triggerIdx + 1).ToString() + "번 점령지를 탈취 했습니다 !";
                                CaptureManager.instance.captureResult_team[_triggerIdx] = _CAPTURE_RESULT.GET;
                                CaptureManager.instance.captureResult_enemy[_triggerIdx] = _CAPTURE_RESULT.LOSE;
                                CaptureManager.instance.territoryOutline[_triggerIdx].OutlineColor = Color.green;
                                CaptureManager.instance.territoryOutline[_triggerIdx].HaloColor = Color.green;
                            }
                            else
                            {
                                //Debug.Log("점령지를 빼앗겼습니다!");
                                UIManager.instance.txt_captureNotice.color = Color.red;
                                UIManager.instance.txt_captureNotice.text = (_triggerIdx + 1).ToString() + "번 점령지를 빼앗겼습니다 !";
                                CaptureManager.instance.captureResult_team[_triggerIdx] = _CAPTURE_RESULT.LOSE;
                                CaptureManager.instance.captureResult_enemy[_triggerIdx] = _CAPTURE_RESULT.GET;
                                CaptureManager.instance.territoryOutline[_triggerIdx].OutlineColor = Color.red;
                                CaptureManager.instance.territoryOutline[_triggerIdx].HaloColor = Color.red;
                            }
                        }
                        break;
                }

                UIManager.instance.txt_captureNotice.gameObject.SetActive(true); //셋팅한거 켜주고
                StartCoroutine(Cor_CheckCaptureNoticeTime()); // x초뒤에 캡처 알림 텍스트 셋엑티브펄스.
                isStartCaptureSubCor[_triggerIdx] = false; //서브코루틴 종료는 여기서 초기화되어야함. 메인캡처코루틴이 끝난후
                isStartCaptureCor[_triggerIdx] = false;
                */

                //브릿지 네트워크의 CaptureResult(int _capturePlayerNum) 에서 결과작업이 이루어짐 정확한 동기화를 위함.

                if (_playerTag.Equals(GameManager.instance.myTag))
                {
                    StateManager.instance.Idle(true); //점령완료 후 다시 IDLE상태변환
#if NETWORK
                    NetworkManager.instance.SendIngamePacket();
                    NetworkManager.instance.SendCaptureSuccess(_triggerIdx);
#endif
                }
                yield break;
            }

            captureGage[_triggerIdx].img_capture.fillAmount -= Time.smoothDeltaTime / _time;
            captureGage[_triggerIdx].txt_captureTime.text = (captureGage[_triggerIdx].img_capture.fillAmount * _time).ToString();
            yield return null;
        }
    }

    public IEnumerator Cor_CheckCaptureNoticeTime()
    {
        yield return YieldInstructionCache.WaitForSeconds(2.5f);
        UIManager.instance.txt_captureNotice.gameObject.SetActive(false);
        yield break;
    }
}