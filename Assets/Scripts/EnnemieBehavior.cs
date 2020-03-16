using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Chase();
        if (vie <= 0)
        {
            deathTimer -= 1 * Time.deltaTime;
        }

        if (deathTimer <= 0)
        {
                Destroy(gameObject);
        }
    }

    private void Chase()
    {
        transform.LookAt(player.transform);
        transform.Translate(transform.forward * speed * Time.deltaTime);
    }

    public void ApplyDammage(float dammage)
    {
        vie -= dammage;
    }
}
