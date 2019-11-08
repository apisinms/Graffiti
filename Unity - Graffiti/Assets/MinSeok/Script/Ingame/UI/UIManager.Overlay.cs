using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//                  UIManager_Overlay
public partial class UIManager : MonoBehaviour
{

    #region KILL_DEATH
    public _IMG_KD_INFO killDeath;

    public struct _IMG_KD_INFO
    {
        public GameObject obj_parent;
        public TMP_Text txt_kd { get; set; }
    }
    #endregion

    #region GAME_TIMER
    public _IMG_TIMER_INFO timer;
    public double gameTime { get; set; }
    private int min, sec;

    public struct _IMG_TIMER_INFO
    {
        public GameObject obj_parent;
        public Image img_outline { get; set; }
        public Image img_back { get; set; }
        public Text txt_gameTime { get; set; }
    }
    #endregion

    #region GAME_SCORE
    public _IMG_SCORE_INFO[] score;

    public struct _IMG_SCORE_INFO
    {
        public GameObject obj_parent;
        public Image img_back;
        public TMP_Text txt_score;
    }
    #endregion

    #region RESPAWN_GAGE
    public _IMG_RESPAWN_INFO respawn;
    private bool isStartRespawnGageCor;

    public struct _IMG_RESPAWN_INFO
    {
        public GameObject obj_parent;
        public Image img_respawnGage { get; set; }
        public TMP_Text txt_deathFrom { get; set; }
    }
    #endregion

    #region KILL_LOG
    private GameObject[] obj_killLogs;

    //킬러, 무기, 빅팀을 각각 큐에넣은후에 _IMG_KILL_LOG[] killLog에 적용시킬것임 
    private Queue<_TMP_ATTRIBUTE> queueKillLog;

    public struct _IMG_KILL_LOG
    {
        public GameObject obj_parent;
        public TMP_Text txt_from;
        public Image img_weapon;
        public TMP_Text txt_to;
    }
    _IMG_KILL_LOG[] killLog;

    public struct _TMP_ATTRIBUTE //_IMG_KILL_LOG멤버와 대응되는 값들. tmp로써 값을넣는데 쓰임
    {
        public string from;
        public Sprite spr;
        public string to;
    }
    _TMP_ATTRIBUTE tmpAttribute;
    #endregion

    #region GRENADE_BTN
    public _IMG_GRENADE_INFO grenade;
    public Sprite[] spr_explosion;
    public bool isStartGrenadeCoolTime { get; set; }

    public struct _IMG_GRENADE_INFO
    {
        public GameObject obj_parent;
        public Image img_explosion { get; set; }
        public Image img_back { get; set; }
        public Button btn_grenade { get; set; }
    }
    #endregion

    private void Initialization_KillDeath()
    {
        killDeath = new _IMG_KD_INFO();

        killDeath.obj_parent = GameObject.FindGameObjectWithTag("Canvas_overlay").transform.GetChild(13).gameObject;
        killDeath.txt_kd = killDeath.obj_parent.transform.GetChild(0).GetComponent<TMP_Text>();
        killDeath.obj_parent.SetActive(true);
    }

    private void Initialization_GameTimer()
    {
        timer = new _IMG_TIMER_INFO();

        timer.obj_parent = GameObject.FindGameObjectWithTag("Canvas_overlay").transform.GetChild(10).gameObject;
        timer.img_outline = timer.obj_parent.transform.GetChild(0).GetComponent<Image>();
        timer.img_back = timer.obj_parent.transform.GetChild(1).GetComponent<Image>();
        timer.txt_gameTime = timer.obj_parent.transform.GetChild(2).GetComponent<Text>();
        timer.obj_parent.SetActive(true);

        gameTime = GameManager.instance.gameInfo.gameTime;
    }

    private void Initialization_Score()
    {
        score = new _IMG_SCORE_INFO[2];

        for (int i = 0; i < score.Length; i++)
        {
            score[i].obj_parent = GameObject.FindGameObjectWithTag("Canvas_overlay").transform.GetChild(i + 11).gameObject;
            score[i].img_back = score[i].obj_parent.transform.GetChild(0).GetComponent<Image>();
            score[i].txt_score = score[i].obj_parent.transform.GetChild(1).GetComponent<TMP_Text>();
            score[i].obj_parent.SetActive(true);
        }
    }

    private void Initialization_RespawnGage()
    {
        respawn.obj_parent = GameObject.FindGameObjectWithTag("Canvas_overlay").transform.GetChild(6).gameObject;
        respawn.img_respawnGage = respawn.obj_parent.transform.GetChild(2).GetComponent<Image>();
        respawn.txt_deathFrom = respawn.obj_parent.transform.GetChild(3).GetComponent<TMP_Text>();
        line.obj_parent.SetActive(false);
    }

    private void Initialization_KillLog()
    {
        killLog = new _IMG_KILL_LOG[4];
        queueKillLog = new Queue<_TMP_ATTRIBUTE>();

        for (int i = 0; i < killLog.Length; i++)
        {
            killLog[i].obj_parent = GameObject.FindGameObjectWithTag("Canvas_overlay").transform.GetChild(7).GetChild(i).gameObject;
            killLog[i].txt_from = killLog[i].obj_parent.transform.GetChild(0).GetComponent<TMP_Text>();
            killLog[i].img_weapon = killLog[i].obj_parent.transform.GetChild(1).GetComponent<Image>();
            killLog[i].txt_to = killLog[i].obj_parent.transform.GetChild(2).GetComponent<TMP_Text>();
            killLog[i].obj_parent.SetActive(false);
        }

        tmpAttribute = new _TMP_ATTRIBUTE();
        tmpAttribute.from = null;
        tmpAttribute.spr = null;
        tmpAttribute.to = null;
    }


    private void Initialization_Grenade()
    {
        grenade = new _IMG_GRENADE_INFO();

        grenade.obj_parent = GameObject.FindGameObjectWithTag("Canvas_overlay").transform.GetChild(9).gameObject;
        grenade.img_back = grenade.obj_parent.transform.GetChild(0).GetComponent<Image>();
        grenade.btn_grenade = grenade.obj_parent.transform.GetChild(1).GetComponent<Button>();
        grenade.img_explosion = grenade.obj_parent.transform.GetChild(2).GetComponent<Image>();

        grenade.btn_grenade.interactable = true;
        grenade.img_back.fillAmount = 0;
        grenade.img_explosion.gameObject.SetActive(false);
    }

    public IEnumerator StartGameTimer()
    {
        while (true)
        {
            if (gameTime <= 0)
                yield break;

            gameTime -= Time.smoothDeltaTime;
            min = (int)gameTime / 60;
            sec = (int)gameTime % 60;

            if (sec < 0 && min > 1)
                min--;

            if ((int)min <= 0) //초단위만 남았을경우 빨간색
            {
                timer.txt_gameTime.color = Color.red;
                timer.img_outline.color = Color.red;
            }

            timer.txt_gameTime.text = ((int)min).ToString() + " : " + sec.ToString("00");

            yield return null;
        }
    }

    public void SetScore(int _playerIndex, int _point)
    {
        int idx = _playerIndex - 1;

        switch ((C_Global.GameType)GameManager.instance.gameInfo.gameType)
        {
            case C_Global.GameType._2vs2:
                {
                    if (idx == 0 || idx == 1) //레드
                    {
                        int tmp = (int.Parse(score[0].txt_score.text) + _point);
                        score[0].txt_score.text = tmp.ToString();
                    }
                    else if (idx == 2 || idx == 3)
                    {
                        int tmp = (int.Parse(score[1].txt_score.text) + _point);
                        score[1].txt_score.text = tmp.ToString();
                    }
                }
                break;

            case C_Global.GameType._1vs1:
                {
                    if (idx == 0) //레드
                    {
                        int tmp = (int.Parse(score[0].txt_score.text) + _point);
                        score[0].txt_score.text = tmp.ToString();
                    }
                    else
                    {
                        int tmp = (int.Parse(score[1].txt_score.text) + _point);
                        score[1].txt_score.text = tmp.ToString();
                    }
                }
                break;
        }
    }

    public void SetMyKillDeath(string _type)
    {
        if (_type.Equals("kill"))
            PlayersManager.instance.myKill++;
        else if (_type.Equals("death"))
            PlayersManager.instance.myDeath++;

        killDeath.txt_kd.text = "K/D:  " + PlayersManager.instance.myKill.ToString() + " / " + PlayersManager.instance.myDeath.ToString();
    }

    public void Grenade_Btn()
    {
        if (isStartGrenadeCoolTime == true) //아래 쿨타임 코루틴으 끝나면 false가 되어 다시 던질수 있게됨.
            return;

        isStartGrenadeCoolTime = true;

        StartCoroutine(Cor_StartGrenadeCoolTime(3.0f));
    }

    public IEnumerator Cor_StartGrenadeCoolTime(float _time)
    {
        grenade.btn_grenade.interactable = false;

        while (true)
        {
            if (grenade.img_back.fillAmount >= 1.0f)
            {
                grenade.btn_grenade.interactable = true;
                grenade.img_back.fillAmount = 0;
                StartCoroutine(Cor_StartGrenadeExplosion(0.025f));
                yield break;
            }

            grenade.img_back.fillAmount += Time.smoothDeltaTime / _time;
            yield return null;
        }
    }

    public IEnumerator Cor_StartGrenadeExplosion(float _time)
    {
        grenade.img_explosion.gameObject.SetActive(true);

        while (true)
        {
            for (int i = 0; i < spr_explosion.Length; i++)
            {
                grenade.img_explosion.sprite = spr_explosion[i];
                grenade.img_explosion.SetNativeSize();
                yield return YieldInstructionCache.WaitForSeconds(_time);
            }

            grenade.img_explosion.gameObject.SetActive(false);
            yield break;
        }
    }

    public void ReloadAmmo_Btn() //재장전 이미지 버튼
    {
        WeaponManager.instance.ReloadAmmoProcess(myIndex); //총알개수 체크후 바로 다이렉트 재장전
                                                           /*
                                                           #if NETWORK
                                                                   NetworkManager.instance.SendIngamePacket();
                                                           #endif
                                                           */
    }

    public IEnumerator Cor_DecreaseRespawnGageImg(float _time)
    {
        if (isStartRespawnGageCor == true)
            yield break;

        isStartRespawnGageCor = true;
        respawn.obj_parent.SetActive(true);

        while (true)
        {
            if (respawn.img_respawnGage.fillAmount <= 0)
            {
                respawn.img_respawnGage.fillAmount = 1.0f;
                WeaponManager.instance.SupplyAmmo(myIndex);
                respawn.obj_parent.SetActive(false);
                isStartRespawnGageCor = false;
                yield break;
            }

            respawn.img_respawnGage.fillAmount -= Time.smoothDeltaTime / _time;
            yield return null;
        }
    }

    //킬로그 코루틴을 계속돌려서 유저가 죽을떄마다 큐에 킬로그가 들어오는지 계속확인(브릿지 킬프로세스에서 들어옴)
    //큐가 들어오면 준비된 로그ui슬롯 4개에 순서대로 출력 시간 1초단위
    public IEnumerator Cor_CheckKillLogQueue()
    {
        int i = 0;

        while (true)
        {
            if (queueKillLog.Count > 4)
                Debug.Log("큐가터졌다. 4개 이상이다.");

            if (queueKillLog.Count > 0 && queueKillLog.Count <= 4)
            {
                _TMP_ATTRIBUTE tmp = queueKillLog.Dequeue(); //큐에서 킬로그를 바로빼와서

                if (killLog[0].obj_parent.activeSelf == false) //맨윗줄이 1초뒤꺼졌으면 맨위로 다시 정렬해줘야됨
                    i = 0;

                //그후 킬러, 빅팀의 닉네임, 사용된무기 정보를 킬로그에 업데이트함
                killLog[i].txt_from.text = tmp.from;
                killLog[i].img_weapon.sprite = tmp.spr;
                killLog[i].txt_to.text = tmp.to;
                killLog[i].obj_parent.SetActive(true);

                StartCoroutine(Cor_CheckKillLogTime(killLog[i]));
                i++;
            }
            yield return null;
        }
    }

    //유저간의 전투
    public void EnqueueKillLog_Player(int _killer, int _victim)
    {
        tmpAttribute.from = PlayersManager.instance.nickname[_killer - 1];
        tmpAttribute.spr = WeaponManager.instance.GetWeaponSprite(_killer - 1);
        tmpAttribute.to = PlayersManager.instance.nickname[_victim - 1];

        queueKillLog.Enqueue(tmpAttribute);
    }

    //차에 치어 즉사
    public void EnqueueKillLog_CarCrash(int _victim)
    {
        AudioManager.Instance.Play(10);

        tmpAttribute.from = null;
        tmpAttribute.spr = spr_mainW[3];
        tmpAttribute.to = PlayersManager.instance.nickname[_victim - 1];

        queueKillLog.Enqueue(tmpAttribute);
    }

    public IEnumerator Cor_CheckKillLogTime(_IMG_KILL_LOG _tmp)
    {
        yield return YieldInstructionCache.WaitForSeconds(3.0f);
        _tmp.obj_parent.SetActive(false);
    }
}