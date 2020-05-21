using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class EnnemieBehavior : MonoBehaviour
{
    [SerializeField] private int vie;

    private GameObject player;

    [SerializeField] private float speed;
    [SerializeField] private float deathTimer;
    [SerializeField] private float waitingDistance;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private int dammage;
    [SerializeField] private float attackInterval;
    [SerializeField] private float grabHitForce;
    public Transform ringPosition;
    [SerializeField] private float engagingDistance;
    [SerializeField] private int lifePointSustenance;
    public Vector3 destination;

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
                    bool positionTaken = false;
                    EnnemieBehavior[] NMIList = GameObject.Find("GameManager").GetComponent<GameManager>().Jaguar;
                    foreach (var nmi in NMIList)
                    {
                        if (nmi.ringPosition == player.GetComponent<Ennemies_Positionnement>().avalaiblePosition[I])
                        {
                            positionTaken = true;
                        }
                    }

                    if (positionTaken == false)
                    {
                        ringPosition = player.GetComponent<Ennemies_Positionnement>().avalaiblePosition[I];
                        player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Remove(ringPosition);
                        player.GetComponent<Ennemies_Positionnement>().occupiedPosition.Add(ringPosition);
                    }
                    else
                    {
                        SwitchState(BotState.Wait);
                    }
                    
                    
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
        if (vie > 0)
        {
            switch (state)
            {
            case BotState.Chase:
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
                if (ringPosition == null && player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count == 0 &&
                        Vector3.Distance(transform.position, player.transform.position) < waitingDistance)
                {
                        SwitchState(BotState.Wait);
                }
                if(ringPosition && Vector3.Distance(this.transform.position, ringPosition.position) <= engagingDistance)
                {
                        print("Je garde");
                        SwitchState(BotState.Guard);
                }
                GetComponentInChildren<Animator>().Play("Running");
                
                break;
            case BotState.Guard:
                    transform.LookAt(player.transform);
                    if(agent.enabled)
                        GetComponent<NavMeshAgent>().SetDestination(this.transform.position);
                    GetComponentInChildren<Animator>().Play("Fighting Idle");
                    if (player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count > 0 && !ringPosition)
                    {
                        SwitchState(BotState.Chase);
                    }
                    if (ringPosition && Vector3.Distance(this.transform.position, ringPosition.position) > engagingDistance)
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
                else if (Vector3.Distance(transform.position, player.transform.position) < waitingDistance - 0.5f)
                {
                    print("Je recule");
                    agent.SetDestination(-transform.forward * waitingDistance);
                    Debug.DrawRay(transform.position,-transform.forward * waitingDistance , Color.green);
                    GetComponentInChildren<Animator>().Play("Running");
                }
                else
                {
                    GetComponentInChildren<Animator>().Play("Fighting Idle");
                    transform.LookAt(player.transform);
                }
               
                break;
        }
        }
        
    }
    
    public void SwitchState(BotState newState) {
        this.state = newState;
        this.OnStateChanged();
    }

    void Update()
    {
        destination = agent.destination;
        if (ringPosition && Vector3.Dot(player.transform.forward, ringPosition.position - player.transform.position) < 0 && player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count > 0)
        {
            ringPosition = null;
            int I = UnityEngine.Random.Range(0, player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count);
            bool positionTaken = false;
            EnnemieBehavior[] NMIList = GameObject.Find("GameManager").GetComponent<GameManager>().Jaguar;
            foreach (var nmi in NMIList)
            {
                if (nmi.ringPosition == player.GetComponent<Ennemies_Positionnement>().avalaiblePosition[I])
                {
                    positionTaken = true;
                }
            }

            if (positionTaken == false)
            {
                ringPosition = player.GetComponent<Ennemies_Positionnement>().avalaiblePosition[I];
                player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Remove(ringPosition);
                player.GetComponent<Ennemies_Positionnement>().occupiedPosition.Add(ringPosition);
            }
            else
            {
                SwitchState(BotState.Wait);
            }

        }
        
        
        
        if(!agent.pathPending)
            UpdateState();
            
        if (vie <= 0) {
            GetComponentInChildren<Animator>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;
            Death();
        }

        if (deathTimer <= 0)
        {
            player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Add(ringPosition);
            player.GetComponent<Ennemies_Positionnement>().occupiedPosition.Remove(ringPosition);
            player.GetComponent<PlayerBehavior>().AddLifePoints(lifePointSustenance);
            GameObject.Find("GameManager").GetComponent<GameManager>().GetAllJaguar();
            Destroy(gameObject);
        }
        
        
    }
    public void ApplyDammage(int dammage)
    {
        vie -= dammage;
        GetComponentInChildren<ParticleSystem>().Play();
        FMODUnity.RuntimeManager.PlayOneShot("event:/NCP/Degat Ennemi/Crachat de sang + brisage d'os (ok)", transform.position);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Coup de poing/Coup de poing AVEC contact (ok)", transform.position);   
    }

    public void Punch()
    {
        if (state == BotState.Guard)
        {
            SwitchState(BotState.Fight);
            if (player.GetComponent<PlayerStrikes>().isShielded)
            {
                if (player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0)
                    .GetComponent<EnnemieBehavior>())
                {
                    player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0).GetComponent<EnnemieBehavior>()
                        .vie -= dammage;
                    if (player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0)
                            .GetComponent<EnnemieBehavior>()
                            .vie <= 0)
                    {
                        Destroy( player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0).gameObject);
                    }
                }
                    
                else if (player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0)
                    .GetComponent<EagleBehavior>())
                {
                    player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0).GetComponent<EagleBehavior>()
                        .vie -= dammage;
                    if (player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0)
                            .GetComponent<EagleBehavior>()
                            .vie <= 0)
                    {
                        Destroy( player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0).gameObject);
                    }
                }
                /*else if (player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0)
                    .GetComponent<grenadierBehavior>())
                {
                    player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0).GetComponent<grenadierBehavior>()
                        .vie -= dammage;
                    if (player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0)
                            .GetComponent<grenadierBehavior>()
                            .vie <= 0)
                    {
                        Destroy( player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0).gameObject);
                    }
                    
                }*/
            }
            else
            {
                player.GetComponent<PlayerBehavior>().ApplyDammage(dammage);
            }
                
                
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<EnnemieBehavior>() && GetComponent<Rigidbody>().isKinematic == false && GetComponent<Rigidbody>().velocity.x > 10f)
        {
            other.transform.GetComponent<Rigidbody>().isKinematic = false;
            other.transform.GetComponent<EnnemieBehavior>().ApplyDammage(1000);
            other.transform.GetComponent<Rigidbody>().AddForce(-other.transform.forward * grabHitForce);
        }
    }

    public void lifeSetters(int dammage)
    {
        vie -= dammage;
    }

    public int lifeGetters()
    {
        return vie;
    }
}
