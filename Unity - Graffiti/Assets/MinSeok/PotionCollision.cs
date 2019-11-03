using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
#if NETWORK
        for (int i = 0; i < GameManager.instance.gameInfo.maxPlayer; i++)
        {
            if (other.CompareTag(GameManager.instance.playersTag[i]))
            {
                Debug.Log("먹은놈은 " + i + "번째 플레이어");
				Debug.Log("기존 피:" + NetworkManager.instance.GetHealth(i));

				// 아이템 코드 서버로 전송
				NetworkManager.instance.SendItemCode(ItemCode.HP_NORMAL);	

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