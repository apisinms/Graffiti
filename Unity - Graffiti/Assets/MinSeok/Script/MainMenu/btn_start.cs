using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class btn_start : UnityEngine.MonoBehaviour
{
	public GameObject obj_loadingBar;
	public TMP_Text txt_startBtn;
    public Button prevSelectButton;
    public Button nextSelectButton;
    int flag = 1;
    private int selectMatch = 0;

	public void BtnStart() //매칭버튼 눌렀을때.
	{
		flag = 1 - flag;

		if (flag == 0)
		{
			txt_startBtn.text = "CANCLE";
            prevSelectButton.enabled = false;
            nextSelectButton.enabled = false;

            obj_loadingBar.SetActive(true);

            // 매칭이 가능한지 서버로 전송한다.
            NetworkManager.instance.MayIMatch(selectMatch);

            // 코루틴을 돌려서 매칭이 잡힐때까지 반복한다.
            StartCoroutine(CheckMatch());
        }

		else if (flag == 1) //3초전에 취소눌렀을경우 원상복구
		{
			NetworkManager.instance.MayICancelMatch();  // 매칭 취소가 가능한지 서버로 전송
			StartCoroutine(CheckMatchCancel());			// 매칭 취소 결과 받을 때까지 코루틴 돌림
		}
	}

	private IEnumerator CheckMatch()
	{
		while (true)
		{
			// 만약 매칭이 캔슬 됐으면 이 코루틴을 빠져나온다.
			if (NetworkManager.instance.CheckMatchCancel() == true)
				yield break;

			// 아직 매칭이 안됐으면 그냥 나온다.
			if (NetworkManager.instance.CheckMatched() == false)
				yield return null;

			// 매칭에 성공했다면 씬을 로드하고
			else
			{
                //LoadingSceneManager.LoadScene("SelectWeapons", true);
				SceneManager.LoadScene("SelectWeapons");
				yield break;    // 코루틴 종료
			}
		}
	}

	private IEnumerator CheckMatchCancel()
	{
		while (true)
		{
			// 아직 매칭취소 프로토콜이 안왔으면 그냥 나온다.
			if (NetworkManager.instance.CheckMatchCancel() == false)
				yield return null;

			// 매칭 취소 성공했다면
			else
			{
				// UI바꾸고
				txt_startBtn.text = "PLAY";
                prevSelectButton.enabled = true;
                nextSelectButton.enabled = true;
                obj_loadingBar.SetActive(false);

				yield break;    // 코루틴 종료
			}

		}
	}

    public void SelectMatch(int _select = 2)
    {
        switch(_select)
        {
            case 2:
                selectMatch = (int)C_Global.GameType._2vs2;
                break;
            case 1:
                selectMatch = (int)C_Global.GameType._1vs1;
                break;
        }
    }
}
