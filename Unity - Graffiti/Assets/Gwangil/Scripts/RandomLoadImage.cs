using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLoadImage : MonoBehaviour
{
    public GameObject[] loadImage;
    void Awake()
    {
        loadImage[Random.Range(0, loadImage.Length)].SetActive(true);
    }
}
