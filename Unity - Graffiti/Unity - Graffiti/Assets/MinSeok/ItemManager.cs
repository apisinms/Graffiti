using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum _POTION_MOVE_STATE
{
    UP = 0,
    DOWN,
}

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    #region POTION
    public GameObject[] obj_potion { get; set; }
    public Transform[] tf_posion { get; set; }
    private Vector3[] tmpRot;
    private float[] tmpPos_min;
    private float[] tmpPos_max;
    public _POTION_MOVE_STATE[] potionMoveState { get; set; }
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;

        Initialization_Potion();
    }

    void Update()
    {
        PotionMovement();
    }

    public void Initialization_Potion()
    {
        obj_potion = new GameObject[5];
        tf_posion = new Transform[5];
        tmpRot = new Vector3[5];
        tmpPos_min = new float[5];
        tmpPos_max = new float[5];
        potionMoveState = new _POTION_MOVE_STATE[5];

        for (int i = 0; i < obj_potion.Length; i++)
        {
            obj_potion[i] = GameObject.FindGameObjectWithTag("Potion").transform.GetChild(i).gameObject;
            tf_posion[i] = obj_potion[i].GetComponent<Transform>().transform;
            tmpRot[i] = new Vector3(0, tf_posion[i].eulerAngles.y + 1.5f, 0);
            tmpPos_min[i] = tf_posion[i].localPosition.y;
            tmpPos_max[i] = tf_posion[i].localPosition.y + 0.5f;
            potionMoveState[i] = _POTION_MOVE_STATE.UP;
        }
    }

    public void PotionMovement()
    {
        for (int i = 0; i < obj_potion.Length; i++)
        {
            tf_posion[i].eulerAngles += tmpRot[i];

            if (potionMoveState[i] == _POTION_MOVE_STATE.UP)
            {
                if (tf_posion[i].localPosition.y >= tmpPos_max[i])
                    potionMoveState[i] = _POTION_MOVE_STATE.DOWN;

                tf_posion[i].localPosition = new Vector3(tf_posion[i].localPosition.x, tf_posion[i].localPosition.y + Time.smoothDeltaTime, tf_posion[i].localPosition.z);
            }
            else
            {
                if (tf_posion[i].localPosition.y <= tmpPos_min[i])
                    potionMoveState[i] = _POTION_MOVE_STATE.UP;

                tf_posion[i].localPosition = new Vector3(tf_posion[i].localPosition.x, tf_posion[i].localPosition.y - Time.smoothDeltaTime, tf_posion[i].localPosition.z);
            }
        }
    }

    public IEnumerator Cor_PosionSpawnCoolTime(GameObject _obj_potion, float _time)
    {
        yield return YieldInstructionCache.WaitForSeconds(_time);
        _obj_potion.SetActive(true);
    }
}
