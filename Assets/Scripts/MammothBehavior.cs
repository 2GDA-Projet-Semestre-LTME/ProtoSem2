using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Comparers;

public class MammothBehavior : MonoBehaviour
{
    [SerializeField] private float attackSpeed;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float deathTimer;
    [SerializeField] private int vie;
    private GameObject player;
    [SerializeField] private float stunAmount;
    [SerializeField] private int stunToAdd;
    private bool isKnocked = false;
    private bool isStuned = false;
    [SerializeField] private float stunTimer;
    private float actualStunTimer;
    [SerializeField] private int stunReductionAmount;
    [SerializeField] private float stunTimeScale;
    public Transform ringPosition;
    [SerializeField] private float waitingDistance;
    [SerializeField] private int dammage;
    [SerializeField] private float knockDistance;
    [SerializeField] private float knockTime;
    [SerializeField] private float engagingDistance;

    private NavMeshAgent agent;

    public enum State
    {
        Knock,
        Stun,
        Chase,
        Guard,
        Fight,
        Wait
    }

    [SerializeField] private State BotState;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
        InvokeRepeating("Punch", 0, attackSpeed);
        SwitchState(State.Chase);
    }

    // Update is called once per frame
    void Update()
    {
        stunAmount = Mathf.Clamp(stunAmount, 0, 100);
        if (stunAmount >= 40 && BotState != State.Knock && stunAmount < 100)
        {
            SwitchState(State.Knock);
        }
        else if (stunAmount >= 100 && BotState != State.Stun)
        {
            SwitchState(State.Stun);
        }

        if (!agent.pathPending)
        {
            OnUpdateState();
        }
        if (ringPosition == null && player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count == 0 &&
            Vector3.Distance(transform.position, player.transform.position) < waitingDistance && stunAmount < 100)
        {
            SwitchState(State.Wait);
        }
        else if (ringPosition && Vector3.Distance(this.transform.position, ringPosition.position) > engagingDistance && stunAmount < 100)
        {
            print("Je chase");
            SwitchState(State.Chase);
        }
        else if(ringPosition && BotState == State.Chase && Vector3.Distance(this.transform.position, ringPosition.position) <= engagingDistance && stunAmount < 100)
        {
            SwitchState(State.Guard);
        }
        if (vie <= 0) {
            Death();
        }

        if (BotState == State.Stun)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.red;
        }

        if (deathTimer <= 0)
        {
            player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Add(ringPosition);
            player.GetComponent<Ennemies_Positionnement>().occupiedPosition.Remove(ringPosition);
            Destroy(gameObject);
        }
    }

    private void SwitchState(State state)
    {
        BotState = state;
        this.OnEnterState();
    }

    private void OnEnterState()
    {
        switch (BotState)
        {
            case State.Stun:
                actualStunTimer = stunTimer;
                isStuned = true;
                agent.SetDestination(transform.position);
                break;
            case State.Wait:
                if(agent.enabled)
                    GetComponent<NavMeshAgent>().SetDestination(transform.position);
                break;
            case State.Chase:
                if (ringPosition == null && player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count > 0)
                {
                    int I = UnityEngine.Random.Range(0, player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count);
                    ringPosition = player.GetComponent<Ennemies_Positionnement>().avalaiblePosition[I];
                    player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Remove(ringPosition);
                    player.GetComponent<Ennemies_Positionnement>().occupiedPosition.Add(ringPosition);
                    
                }

                break;
                
        }
    }
    private void OnUpdateState()
    {
        switch (BotState)
        {
            case State.Stun:
                if (actualStunTimer > 0)
                {
                    actualStunTimer -= 1 * Time.deltaTime;
                }
                else if(actualStunTimer <= 0)
                {
                    stunAmount = 0;
                    if(BotState != State.Chase)
                        SwitchState(State.Chase);
                    isStuned = false;
                }
                
                break;
            case State.Chase:
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
                        SwitchState(State.Wait);
                    }
                    //GetComponentInChildren<Animator>().Play("Running");
                }
                break;
            case State.Guard:
                transform.LookAt(player.transform);
                if(agent.enabled)
                    GetComponent<NavMeshAgent>().SetDestination(this.transform.position);
                //GetComponentInChildren<Animator>().Play("Fighting Idle");
                if (player.GetComponent<Ennemies_Positionnement>().avalaiblePosition.Count > 0)
                {
                    SwitchState(State.Chase);
                }
                break;
            case State.Fight:
                //GetComponentInChildren<Animator>().Play("Punching");
                break;
            case State.Knock:
                //transform.position = Vector3.Lerp(transform.position,
                    //transform.position + player.transform.forward * knockDistance, knockDistance * knockTime);
                break;
            
            
        }
    }

    private void Punch()
    {
        if (BotState == State.Guard)
        {
            SwitchState(State.Fight);
            if (player.GetComponent<PlayerStrikes>().isShielded)
            {
                if (player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0)
                    .GetComponent<EnnemieBehavior>())
                {
                    player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0)
                        .GetComponent<EnnemieBehavior>()
                        .lifeSetters(dammage);
                    if (player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0)
                            .GetComponent<EnnemieBehavior>()
                            .lifeGetters() <= 0)
                    {
                        Destroy(player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0).gameObject);
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
                        Destroy(player.GetComponent<PlayerStrikes>().grabHandler.transform.GetChild(0).gameObject);
                    }
                }
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
        yield return new WaitForSeconds(attackSpeed);
        SwitchState(State.Guard);
    }

    public void ApplyDammage(int dmg)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/NCP/Degat Ennemi/Crachat de sang + brisage d'os (ok)", transform.position);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Coup de poing/Coup de poing AVEC contact (ok)", transform.position);  
        if(isStuned)
            vie -= dmg;
        else
            stunAmount += stunToAdd;
    }
}
