using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chasing : MonoBehaviour
{
    public GameObject[] targetPos;
    public GameObject[] map;

    private Camera camera;
    private int target = 0;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 TargetPos = new Vector3(targetPos[target].transform.position.x,
            targetPos[target].transform.position.y, targetPos[target].transform.position.z);
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * 10f);
    }

    public void setTarget(int _target)
    {
        target = _target;
        switch(_target)
        {
            case 0:
                camera.clearFlags = CameraClearFlags.Skybox;
                map[_target].SetActive(true);
                map[1].SetActive(false);
                break;
            case 1:
                camera.clearFlags = CameraClearFlags.SolidColor;
                map[_target].SetActive(true);
                map[0].SetActive(false);
                break;
        }
    }
}
