using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class EnnemieBehavior : MonoBehaviour
{
    [SerializeField] private float vie;

    private GameObject player;

    [SerializeField] private float speed;
    [SerializeField] private float deathTimer;
    [SerializeField] private float waitingDistance;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private int dammage;
    [SerializeField] private float attackInterval;
    [SerializeField] private float grabHitForce;
    [SerializeField] private int grabHitSelfDammage;
    public Transform ringPosition;

    public enum BotState
    {
        Chase,
        Fight,
        Guard,
        Wait
    }

    [SerializeField] private BotState state;

    private void Start()
    {
        player = GameObject.Find("Player");
        GetComponent<NavMeshAgent>().speed = speed;
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("Punch", 0, attackInterval);
        SwitchState(BotState.Chase);
    }

    private void OnStateChanged()
    {
        switch (state)
        {
            case BotState.Chase:
                
                if (ringPosition == null && player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count > 0)
                {
                    int I = UnityEngine.Random.Range(0, player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count);
                    ringPosition = player.GetComponent<Ennemies_Positionnement>().avalaiblePosition[I];
                    player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Remove(ringPosition);
                    player.GetComponent<Ennemies_Positionnement>().occupiedPosition.Add(ringPosition);
                    
                }
                break;
            case BotState.Wait:
                if(agent.enabled)
                    GetComponent<NavMeshAgent>().SetDestination(transform.position);
                break;
        }
    }

    private void UpdateState()
    {
        switch (state)
        {
            case BotState.Chase:
                if (vie > 0)
                {
                    if(ringPosition && agent.enabled)
                        GetComponent<NavMeshAgent>().SetDestination(ringPosition.position);
                    else if(Vector3.Distance(transform.position, player.transform.position) > waitingDistance)
                    {
                        agent.SetDestination(player.transform.position);
                    }
                    else
                    {
                        agent.SetDestination(transform.position);
                        SwitchState(BotState.Wait);
                    }
                    GetComponentInChildren<Animator>().Play("Running");
                }
                break;
            case BotState.Guard:
                    transform.LookAt(player.transform);
                    if(agent.enabled)
                        GetComponent<NavMeshAgent>().SetDestination(this.transform.position);
                    GetComponentInChildren<Animator>().Play("Fighting Idle");
                    if (player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count > 0)
                    {
                        SwitchState(BotState.Chase);
                    }
                    break;
            case BotState.Fight:
                GetComponentInChildren<Animator>().Play("Punching");
                break;
            case BotState.Wait:
                if (Vector3.Distance(transform.position, player.transform.position) > waitingDistance)
                {
                    SwitchState(BotState.Chase);
                }
                GetComponentInChildren<Animator>().Play("Fighting Idle");
                transform.LookAt(player.transform);
                break;
        }
    }
    
    public void SwitchState(BotState newState) {
        this.state = newState;
        this.OnStateChanged();
    }

    void Update()
    {
        
        if (ringPosition == null && player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count == 0 &&
            Vector3.Distance(transform.position, player.transform.position) < waitingDistance)
        {
            SwitchState(BotState.Wait);
        }
        else if (ringPosition && Vector3.Distance(this.transform.position, ringPosition.position) > 3)
        {
            SwitchState(BotState.Chase);
        }
        else if(ringPosition && state == BotState.Chase && Vector3.Distance(this.transform.position, ringPosition.position) <= 3)
        {
            SwitchState(BotState.Guard);
        }
        if(!agent.pathPending)
            UpdateState();
            
        if (vie <= 0) {
            Death();
        }

        if (deathTimer <= 0)
        {
            player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Add(ringPosition);
            player.GetComponent<Ennemies_Positionnement>().occupiedPosition.Remove(ringPosition);
            Destroy(gameObject);
        }
        
        
    }
    public void ApplyDammage(float dammage)
    {
        vie -= dammage;
        GetComponentInChildren<ParticleSystem>().Play();
    }

    public void Punch()
    {
        if (state == BotState.Guard)
        {
            SwitchState(BotState.Fight);
            player.GetComponent<PlayerBehavior>().ApplyDammage(dammage);
           StartCoroutine(GoToGuard());
           
        }
            
    }

    private void Death()
    {
        deathTimer -= 1 * Time.deltaTime;
        agent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        
    }

    IEnumerator GoToGuard()
    {
        yield return new WaitForSeconds(1);
        SwitchState(BotState.Guard);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<EnnemieBehavior>() && GetComponent<Collider>().isTrigger && player.GetComponent<PlayerStrikes>().isSlashing)
        {
            other.transform.GetComponent<Rigidbody>().isKinematic = false;
            other.transform.GetComponent<EnnemieBehavior>().ApplyDammage(100);
            other.GetComponent<Rigidbody>().AddForce(other.transform.right * grabHitForce);
            if (vie - grabHitSelfDammage <= 0)
            {
                player.GetComponent<PlayerStrikes>().LostGrab();
                Destroy(gameObject);
            }
            vie -= grabHitSelfDammage;
            
            
        }
    }
}
