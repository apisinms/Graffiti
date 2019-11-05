using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCollision : MonoBehaviour
{
    public int myIndex { get; set; }

    private void Start()
    {
        myIndex = GameManager.instance.myIndex;
    }

    private void OnTriggerEnter(Collider other)
    {
#if NETWORK
        for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
        {
            if (other.CompareTag(GameManager.instance.playersTag[i]))
            {
                // 본인이 먹은 경우만 패킷 보냄
                if (other.CompareTag(GameManager.instance.myTag))
                {
                    if (PlayersManager.instance.actionState[myIndex] == _ACTION_STATE.DEATH)
                        break;

                    // 아이템 코드 서버로 전송
                    NetworkManager.instance.SendItemCode(ItemCode.HP_NORMAL);
                }
                else
                {
                    _ACTION_STATE tmpState = (_ACTION_STATE)NetworkManager.instance.GetActionState(i);
                    if (tmpState == _ACTION_STATE.DEATH)
                        break;
                }

                // 플레이어랑 충돌하기만 했으면 아이템 꺼줌
                this.gameObject.SetActive(false);
                ItemManager.instance.StartCoroutine(ItemManager.instance.Cor_PosionSpawnCoolTime(this.gameObject, 3.0f));

                break;
            }
        }

#else
        for (int i = 0; i < C_Global.MAX_CHARACTER; i++)
        {
            if (other.CompareTag(GameManager.instance.playersTag[i]))
            {
                Debug.Log("먹은놈은 " + i + "번째 플레이어");

                /* 여기서 체력회복 */
                UIManager.instance.hp[i].img_front.fillAmount += 0.3f; // UI체력만 회복

                this.gameObject.SetActive(false);
                ItemManager.instance.StartCoroutine(ItemManager.instance.Cor_PosionSpawnCoolTime(this.gameObject, 3.0f));
                break;
            }
        }
#endif

    }
}