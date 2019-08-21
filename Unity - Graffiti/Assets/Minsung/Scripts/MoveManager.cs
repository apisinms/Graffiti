using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public  Transform[] curPlayerPos { get; set; }
    private float speed = 2.0f;

    NetworkManager networkManager;
    Vector3 pos;

    void Awake()
    {
        Application.targetFrameRate = 60;
        networkManager = NetworkManager.instance;

        curPlayerPos = new Transform[4];

        for(int i=0; i<curPlayerPos.Length; i++)
            curPlayerPos[i] = GameManager.instance.obj_players[i].transform;

        pos = new Vector3();

        // 초기값 설정 //일단 내꺼까지다 넣어논것?
        for (int i = 0; i < 4; i++)
        {
            networkManager.SetPosX(i, curPlayerPos[i].position.x);
            networkManager.SetPosZ(i, curPlayerPos[i].position.z);
            networkManager.SetPosPlayerNum(i, i + 1);
        }

        StartCoroutine(this.CheckQuit());
    }


    // 플레이어를 뒤져봐서 위치가 다르면 업데이트
    void Update()
    {
        int j = 0;
        for (int i = 0; i < curPlayerPos.Length; i++)
        {
            // 자기 제외하고
            if (GameManager.instance.myIndex == i)
                continue;

            //위치가 바뀐애가 있으면
            if (networkManager.GetPosX(i) != curPlayerPos[i].position.x ||
               networkManager.GetPosZ(i) != curPlayerPos[i].position.z)
            {
                pos.x = networkManager.GetPosX(i);
                pos.y = curPlayerPos[i].position.y;
                pos.z = networkManager.GetPosZ(i);

              //  curPlayerPos[i].transform.localEulerAngles = new Vector3(0, networkManager.GetRotY(i), 0);

                curPlayerPos[i].position = Vector3.Lerp(curPlayerPos[i].position, pos,
                    Time.smoothDeltaTime * (OtherPlayerManager.instance.speed[j] * 3));

                OtherPlayerManager.instance.Anime_Circuit(j);
            }
            else // 위치가 바뀌지 않았다면 
            {
                OtherPlayerManager.instance.Anime_Idle(j);
            }
            j++;
        }
    }

    IEnumerator CheckQuit()
    {
        while (true)
        {
            switch (networkManager.CheckQuit())
            {
                case 1:
                    Destroy(GameObject.Find("red_1"));
                    break;

                case 2:
                    Destroy(GameObject.Find("red_2"));
                    break;

                case 3:
                    Destroy(GameObject.Find("blue_1"));
                    break;

                case 4:
                    Destroy(GameObject.Find("blue_2"));
                    break;

                default:
                    break;
            }

            yield return YieldInstructionCache.WaitForSeconds(0.25f);
        }
    }
}