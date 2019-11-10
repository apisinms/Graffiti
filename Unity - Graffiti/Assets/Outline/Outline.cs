using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    private Color outlineColor;
    private Light lightHalo; //아우라 라이트
    public Material[] material; //0은 바디에붙은 모든 머터리얼, 1은 간판머터리얼

    private void Awake()
    {
        lightHalo = this.gameObject.transform.GetChild(1).GetComponent<Light>();
    }

    private void Start()
    {
        SetColor(Color.yellow); //맨처음엔 중립이므로 노랑색
    }

    public void SetColor(Color _outlineColor)
    {
        material[0].SetColor("_OutlineColor", _outlineColor); //점령지 바디의 아웃라인 색지정. 같은머터리얼쓰는 여러개의 파츠일 수도있음
        material[1].SetColor("_OutlineColor", _outlineColor); //간판머터리얼 색지정
        lightHalo.color = _outlineColor;
    }
}
