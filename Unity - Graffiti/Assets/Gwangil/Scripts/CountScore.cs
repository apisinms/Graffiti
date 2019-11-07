using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountScore : MonoBehaviour
{

    public float duration = 0.5f; // 카운팅에 걸리는 시간 설정. 
    public TMPro.TMP_Text score;

    public int target;

    void Start()
    {
        StartCoroutine(Count(target, 0));
    }

    IEnumerator Count(float target, float current)
    {

        float offset = (target - current) / duration;

        while (current < target)
        {
            current += offset * Time.deltaTime;
            score.text = ((int)current).ToString();

            yield return null;
        }
        current = target;
        score.text = ((int)current).ToString();
    }
}
