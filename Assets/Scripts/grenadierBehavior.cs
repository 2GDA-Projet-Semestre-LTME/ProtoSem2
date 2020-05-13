using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEngine.AI;

public class grenadierBehavior : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private float range;

    private NavMeshAgent agent;
    [SerializeField] private float vie;
    [SerializeField] private float deathTimer;
    [SerializeField] private float projTimeToReachPlayer;
    [SerializeField] private GameObject Proj;
    private Vector3 fireDirection;
    public GameObject launcher;
    [SerializeField] private float angleOfLaunch;

    public float speed;

    public enum State
    {
        Idle,
        Charging,
        Firing
    }

    [SerializeField] private State state;

    [SerializeField] private float fireCharge;
    [SerializeField] private float fireChargeMax;
    [SerializeField] private float chargeValueIncrementation;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        fireDirection = new Vector3(0,0,0);
    }

    private void OnStateChanged()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Charging:
                break;
            case State.Firing:
                break;
        }
    }

    private void UpdateState()
    {
        switch (state)
        {
            case State.Idle:
                Debug.DrawRay(transform.position, (player.transform.position - transform.position).normalized * range,
                    Color.blue);
                if (Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized,
                    out RaycastHit hit, range))
                {

                    if (hit.transform.CompareTag("Player"))
                    {
                        SwitchState(State.Charging);
                    }
                }

                break;
            case State.Charging:
                if (Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized,
                    out RaycastHit hit2, range))
                {

                    if (hit2.transform.CompareTag("Player") && fireCharge < fireChargeMax)
                    {
                        fireCharge += chargeValueIncrementation * Time.deltaTime;
                    }
                    else if (fireCharge >= fireChargeMax && hit2.transform.CompareTag("Player"))
                    {
                        SwitchState(State.Firing);
                    }
                    else
                    {
                        fireCharge = 0;
                        SwitchState(State.Idle);
                    }
                }

                break;
            case State.Firing:
                Vector3 direction = calcBallisticVelocityVector(launcher.transform.position, 
                    player.transform.position - new Vector3(0,player.transform.localScale.y*0.5f,0), angleOfLaunch);
                GameObject GO = Instantiate(Proj, launcher.transform.position, launcher.transform.rotation);
                GO.GetComponent<Rigidbody>().velocity = direction;
                fireDirection = direction.normalized;
                SwitchState(State.Idle);
                fireCharge = 0;
                break;
        }
    }

    public void SwitchState(State newState) {
        this.state = newState;
        this.OnStateChanged();
    }
    public void ApplyDammage(float dammage)
    {
        vie -= dammage;
        GetComponentInChildren<ParticleSystem>().Play();
    }
    private void Death()
    {
        deathTimer -= 1 * Time.deltaTime;
        agent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        
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
        Debug.DrawRay(transform.position, fireDirection, Color.red);
    }

    Vector3 calcBallisticVelocityVector(Vector3 source, Vector3 target, float angle){
        Vector3 direction = target - source;                            
        float h = direction.y;                                           
        direction.y = 0;                                               
        float distance = direction.magnitude;                           
        float a = angle * Mathf.Deg2Rad;                                
        direction.y = distance * Mathf.Tan(a);                            
        distance += h/Mathf.Tan(a);                                      
 
        // calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2*a));
        return velocity * direction.normalized;    
    }
}
