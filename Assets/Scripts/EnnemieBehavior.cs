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
    [SerializeField] private float fightingDistance;
    [SerializeField] private NavMeshAgent agent;

    public enum BotState
    {
        Chase,
        Fight
    }

    [SerializeField] private BotState state;

    private void Start()
    {
        player = GameObject.Find("Player");
        GetComponent<NavMeshAgent>().speed = speed;
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnStateChanged()
    {
        switch (state)
        {
            case BotState.Chase:
                break;
            case BotState.Fight:
                break;
        }
    }

    private void UpdateState()
    {
        switch (state)
        {
            case BotState.Chase:
                GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
                GetComponentInChildren<Animator>().Play("Running");
                break;
            case BotState.Fight:
                transform.LookAt(player.transform);
                GetComponent<NavMeshAgent>().SetDestination(this.transform.position);
                GetComponentInChildren<Animator>().Play("Fighting Idle");
                break;
        }
    }
    
    public void SwitchState(BotState newState) {
        this.state = newState;
        this.OnStateChanged();
    }

    void Update()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) >= fightingDistance)
        {
            SwitchState(BotState.Chase);
        }
        else
        {
            SwitchState(BotState.Fight);
        }
        if(!agent.pathPending)
            UpdateState();
            
        if (vie <= 0) {
            deathTimer -= 1 * Time.deltaTime;
            agent.enabled = false;
        }

        if (deathTimer <= 0)
        {
            Destroy(gameObject);
        }
        
        
    }
    public void ApplyDammage(float dammage)
    {
        vie -= dammage;
    }
    


}
