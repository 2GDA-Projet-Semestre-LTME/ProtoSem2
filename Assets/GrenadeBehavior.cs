using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehavior : MonoBehaviour
{

    public GameObject explosionArea;
    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.CompareTag("Ground"))
        {
            Instantiate(explosionArea, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
