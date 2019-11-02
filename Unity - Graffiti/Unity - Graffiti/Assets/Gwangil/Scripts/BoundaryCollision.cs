using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryCollision : MonoBehaviour
{
    Rigidbody rigid;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Player") == true)
        {
            rigid = other.GetComponent<Rigidbody>();
            rigid.velocity = Vector3.zero;
        }
    }
}