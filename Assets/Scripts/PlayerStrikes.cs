using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class PlayerStrikes : MonoBehaviour
{

    public List<Collider> GetTrigger;
    [SerializeField] private VisualEffect _visualEffect;
    [SerializeField] private float uppercutMaxDistance;

    [SerializeField] private float forceAmount;

    [SerializeField] private float punchCd;

    [SerializeField] private float upperCd;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashCd;
    [SerializeField] private float grabDistance;
    [SerializeField] private GameObject grabHandler;
    [SerializeField] private float smashVelocityMin;
    [SerializeField] private float stompForce;
    [SerializeField] private bool isDashing;
    public bool isSlashing = false;

    private float uCoolDown;
    private bool grabbed = false;
    private float pCoolDown;

    private float dashCoolDown;

    private bool willStomp;
    private bool flip = true;
    // Start is called before the first frame update
    void Start()
    {
        _visualEffect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && pCoolDown <= 0)
        {
            Punch();
        }
        /*else if (Input.GetMouseButtonDown(1) && uCoolDown <= 0)
        {
            UpperCut();
        }*/
        else if (Input.GetKeyDown(KeyCode.LeftShift) && dashCoolDown <= 0 && GetComponent<PlayerBehavior>().GetJump())
        {
            Dash();
            isDashing = true;
            //FMODUnity.RuntimeManager.PlayOneShot("Nom De L'Event", transform.position);
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Grab();
            //FMODUnity.RuntimeManager.PlayOneShot("Nom De L'Event", transform.position);
        }

        if (Mathf.Abs(GetComponent<Rigidbody>().velocity.y) > smashVelocityMin)
        {
            willStomp = true;
        }

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Slash"))
        {
            isSlashing = true;
            print("je slash");
        }
        else
        {
            isSlashing = false;
        }
        decreaseCoolDown();
    }

   

    //            |||COUPS DU JOUEUR|||
    private void Punch()
    {
        if (grabbed == false)
        {
            if (flip)
            {
                GetComponent<Animator>().Play("CoupDroit");
                flip = false;
            }
            else
            {
                GetComponent<Animator>().Play("CoupGauche");
                flip = true;
            }
            
            foreach (Collider Co in GetTrigger)
            {
                if (Co != null)
                {
                    Co.gameObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * forceAmount + transform.up * forceAmount * 0.2f);
                    if(Co.transform.GetComponent<EnnemieBehavior>())
                        Co.transform.GetComponent<EnnemieBehavior>().ApplyDammage(10);
                    else if (Co.transform.CompareTag("LifeDistrib"))
                    {
                        Instantiate(GameObject.Find("LifeBox"),
                            new Vector3(Co.transform.position.x + 5f, transform.position.y, transform.position.z),
                            transform.rotation);
                        Destroy(Co.gameObject);
                    }
                }
                
            }

            pCoolDown = punchCd;
            StartCoroutine(StopParticles());
        }
        else
        {
            GetComponent<Animator>().Play("Slash");
        }
    }

    private void UpperCut()
    {
        if (grabbed == false)
        {
            GetComponent<Animator>().Play("Uppercut");
            foreach (Collider Co in GetTrigger)
            {
                if (Co != null && Vector3.Distance(transform.position, Co.gameObject.transform.position) <=
                    uppercutMaxDistance)
                {
                    Co.gameObject.GetComponent<Rigidbody>().AddForce(this.transform.up * forceAmount * 0.5f);
                }

            }

            uCoolDown = upperCd;
            StartCoroutine(StopParticles());
        }
    }

    private void Dash()
    {
        GetComponent<Animator>().Play("Jump");
        GetComponent<Rigidbody>().AddForce((new Vector3(0, 0.1f, 0) + transform.forward) * dashForce * 1000, ForceMode.Impulse);
        dashCoolDown = dashCd;
    }

    private void Grab()
    {
        if (grabbed == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance,
                    LayerMask.GetMask("Grabable")) && grabbed == false)
            {
                Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * grabDistance,
                    Color.blue);
                hit.transform.SetParent(grabHandler.transform);
                hit.transform.position = grabHandler.transform.position;
                hit.transform.rotation = grabHandler.transform.rotation;
                if (hit.transform.GetComponent<EnnemieBehavior>())
                {
                    hit.transform.GetComponent<EnnemieBehavior>().enabled = false;
                    hit.transform.GetComponentInChildren<Animator>().enabled = false;
                }
                hit.transform.GetComponent<NavMeshAgent>().enabled = false;
                hit.transform.GetComponent<Collider>().isTrigger = true;
                grabbed = true;
                GetComponent<Animator>().SetBool("IsGrabing", true);
                GetComponent<Animator>().Play("Grab");
            }
        }
        else
        {

            Transform hit = grabHandler.transform.GetChild(0);
            if (hit.GetComponent<EnnemieBehavior>())
            {
                hit.GetComponent<EnnemieBehavior>().enabled = true;
                hit.GetComponentInChildren<Animator>().enabled = true;
            }
            hit.transform.GetComponent<NavMeshAgent>().enabled = true;
            hit.parent = null;
            hit.transform.GetComponent<Collider>().isTrigger = false;
            grabbed = false;
            GetComponent<Animator>().SetBool("IsGrabing", false);
        }
        
    }

    //            |||AUTRES|||

    private void decreaseCoolDown()
    {
        if (uCoolDown > 0)
        {
            uCoolDown -= 1 * Time.deltaTime;
        }

        if (pCoolDown > 0)
        {
            pCoolDown -= 1 * Time.deltaTime;
        }

        if (dashCoolDown >= 0)
        {
            dashCoolDown -= 1 * Time.deltaTime;
        }
    }  //Gère les CDs

    IEnumerator StopParticles()
    {
        yield return new WaitForSeconds(0.25f);
        
        _visualEffect.Stop();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Punchable" || other.gameObject.tag == "LifeDistrib")
            GetTrigger.Add(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Punchable" || other.gameObject.tag == "LifeDistrib")
            GetTrigger.Remove(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isDashing && other.transform.CompareTag("Ground"))
        {
            //FMODUnity.RuntimeManager.PlayOneShot("Nom De L'Event", transform.position);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            GameObject.Find("StompParticles").GetComponent<ParticleSystem>().Play();
           /*foreach (var GO in GetComponentInChildren<GetCollider>().obj)
            {
                GO.GetComponent<Rigidbody>().AddForce(GO.transform.position.x, (GO.transform.position.y + 1) * stompForce,
                    GO.transform.position.z);
        
            }*/
            isDashing = false;
        }

        
    }

    public void LostGrab()
    {
        grabbed = false;
        GetComponent<Animator>().SetBool("IsGrabing", false);
    }

    private void PlayPunchSounds(string Target)
    {
        switch (Target)
        {
            case "Jaguar":
                //FMODUnity.RuntimeManager.PlayOneShot("Nom De L'Event", transform.position);
                break;
            case "Grenadier":
                //FMODUnity.RuntimeManager.PlayOneShot("Nom De L'Event", transform.position);
                break;
            case "Distrib":
                //FMODUnity.RuntimeManager.PlayOneShot("Nom De L'Event", transform.position);
                break;
        }
    }
}
