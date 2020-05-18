using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
    [SerializeField] private float destroyTime;

    private int dmg;
    // Start is called before the first frame update
    void Start()
    { 
        Invoke("Destroy", destroyTime);   
    }

    // Update is called once per frame
    void Destroy()
    {
        Destroy();       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerBehavior>().ApplyDammage(dmg);
        }
    }
}
