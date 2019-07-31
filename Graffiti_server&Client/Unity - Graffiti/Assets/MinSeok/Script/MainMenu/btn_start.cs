using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class btn_start : MonoBehaviour
{
	public GameObject obj_loadingBar;
	Animator am_loadingBar;
	public Text txt_startBtn;
	int flag = 1;

	void Start()
	{
		am_loadingBar = obj_loadingBar.GetComponent<Animator>();
	}

	public void BtnStart() //매칭버튼 눌렀을때.
	{
		flag = 1 - flag;

		if (flag == 0)
		{
			txt_startBtn.text = "취소";
			obj_loadingBar.SetActive(true);
			am_loadingBar.SetBool("isStart", true);

			// 매칭이 가능한지 서버로 전송한다.
			NetworkManager.instance.MayIMatch();

			// 코루틴을 돌려서 매칭이 잡힐때까지 반복한다.
			StartCoroutine(CheckMatch());
		}

		else if (flag == 1) //3초전에 취소눌렀을경우 원상복구
		{
			StopCoroutine(CheckMatch());    // 코루틴 정지
			txt_startBtn.text = "매칭";
			am_loadingBar.SetBool("isStart", false);
			obj_loadingBar.SetActive(false);
		}
	}

	private IEnumerator CheckMatch()
	{
		while (true)
		{
			// 아직 매칭이 안됐으면 그냥 나온다.
			if (NetworkManager.instance.CheckMatched() == false)
				yield return null;

			// 매칭에 성공했다면 씬을 로드하고
			//if (NetworkManager.instance.CheckMatchSuccess() == true)
			else
			{
				SceneManager.LoadScene("SelectWeapons");
				yield break;    // 코루틴 종료
			}

		}
	}
}
