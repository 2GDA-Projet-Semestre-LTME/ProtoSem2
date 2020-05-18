using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EagleBehavior : MonoBehaviour
{
    public float vie;

    private GameObject player;

    [SerializeField] private float speed;
    [SerializeField] private float deathTimer;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private int dammage;
    [SerializeField] private float attackInterval;
    [SerializeField] private float attackDistance;
    public enum BotState
    {
        Chase,
        Fight,
        Guard
    }
    
    [SerializeField] private BotState state;
    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.Find("Player");
        GetComponent<NavMeshAgent>().speed = speed;
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("Fire", 0, attackInterval);
        SwitchState(BotState.Chase);
    }

    public void SwitchState(BotState newState) {
        this.state = newState;
        this.OnStateChanged();
    }

    private void OnStateChanged()
    {
        switch (state)
        {
            case BotState.Fight:
                if(GetComponentInChildren<Animator>())
                    GetComponentInChildren<Animator>().Play("Fighting Idle");  //Set l'anim de tir
                break;
        }
    }

    private void UpdateState()
    {
        switch (state)
        {
            case BotState.Chase:
                if (Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized, out RaycastHit hit, attackDistance))
                {
                    print("touché");
                    if(hit.transform.CompareTag("Player"))
                        SwitchState(BotState.Guard);
                }
                else if(agent.enabled)
                {
                    agent.SetDestination(player.transform.position);
                    
                }

                if (agent.enabled && GetComponentInChildren<Animator>())
                {
                    GetComponentInChildren<Animator>().Play("Fighting Idle");  //Set l'anim de course
                }
                break;
            case BotState.Guard:
                transform.LookAt(player.transform);
                if(GetComponentInChildren<Animator>())
                    GetComponentInChildren<Animator>().Play("Fighting Idle");  //Set l'anim de tir
                if(agent.enabled)
                    GetComponent<NavMeshAgent>().SetDestination(this.transform.position);
                if (!Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized, out RaycastHit hit2, attackDistance))
                {
                    SwitchState(BotState.Chase);
                }
                break;
        }
    }

    private void Fire()
    {
        if (state == BotState.Guard)
        {
            SwitchState(BotState.Fight);
            player.GetComponent<PlayerBehavior>().ApplyDammage(dammage);
            StartCoroutine(GoToGuard());
           
        }
    }

    IEnumerator GoToGuard()
    {
        yield return new WaitForSeconds(1);
        SwitchState(BotState.Guard);
    }
    
    

    // Update is called once per frame
    void Update()
    {
        if(!agent.pathPending)
            UpdateState();
            
        if (vie <= 0) {
            Death();
        }

        if (deathTimer <= 0)
        {
            Destroy(gameObject);
        }
        
        Debug.DrawRay(transform.position, (player.transform.position - transform.position).normalized * attackDistance, Color.blue);
    }
    public void ApplyDammage(float dammage)
    {
        vie -= dammage;
        GetComponentInChildren<ParticleSystem>().Play();
        FMODUnity.RuntimeManager.PlayOneShot("event:/NCP/Degat Ennemi/Crachat de sang + brisage d'os (ok)", transform.position);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Coup de poing/Coup de poing AVEC contact (ok)", transform.position);  
    }

    private void Death()
    {
        deathTimer -= 1 * Time.deltaTime;
        agent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        
    }
}
