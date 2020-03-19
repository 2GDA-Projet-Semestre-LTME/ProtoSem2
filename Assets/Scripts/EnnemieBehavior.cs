using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemieBehavior : MonoBehaviour
{
    [SerializeField] private float vie;

    private GameObject player;

    [SerializeField] private float speed;
    [SerializeField] private float deathTimer;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        
        if (vie <= 0)
        {
            deathTimer -= 1 * Time.deltaTime;
            //GetComponent<NavMeshAgent>().enabled = false;
        }
        else
        {
            Chase();
        }

        if (deathTimer <= 0)
        {
                Destroy(gameObject);
        }
    }

    private void Chase()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) > 5f)
        {
            transform.LookAt(player.transform);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        
    }

    public void ApplyDammage(float dammage)
    {
        vie -= dammage;
    }
    


}
