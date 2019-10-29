using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;

    [SerializeField]
    Image progressBar;
    [SerializeField]
    Button startButton;

    private AsyncOperation op;
    static bool skipstartButton;
    private void Start()
    {
        StartCoroutine(LoadScene(skipstartButton));
    }

    public static void LoadScene(string sceneName, bool _skipstartButton)
    {
        skipstartButton = _skipstartButton;
        nextScene = sceneName;

        SceneManager.LoadScene("LoadingScene");
    }



    IEnumerator LoadScene(bool _skipstartButton)
    {
        yield return null;

        op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);

                if (progressBar.fillAmount == 1.0f && _skipstartButton == true)
                {
                    op.allowSceneActivation = true;
                    yield break;

                }
                else if (progressBar.fillAmount == 1.0f && _skipstartButton == false)
                {
                    progressBar.transform.parent.gameObject.SetActive(false);
                    startButton.gameObject.SetActive(true);
                    yield break;
                }

            }

        }

    }

    public void StartButtonEnter()
    {
        op.allowSceneActivation = true;
    }

}