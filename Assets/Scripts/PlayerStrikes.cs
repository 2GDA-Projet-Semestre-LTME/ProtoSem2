using System;
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

    private float uCoolDown;
    private bool grabbed = false;
    private float pCoolDown;

    private float dashCoolDown;

    private bool willStomp;
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
            print("coup");
            Punch();
        }
        else if (Input.GetMouseButtonDown(1) && uCoolDown <= 0)
        {
            UpperCut();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && dashCoolDown <= 0 && GetComponent<PlayerBehavior>().GetJump())
        {
            Dash();
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Grab();
        }

        if (Mathf.Abs(GetComponent<Rigidbody>().velocity.y) > smashVelocityMin)
        {
            willStomp = true;
        }

        decreaseCoolDown();
    }

   

    //            |||COUPS DU JOUEUR|||
    private void Punch()
    {
        print(grabbed);
        if (grabbed == false)
        {
            _visualEffect.SetVector3("CameraRotation", Camera.main.transform.eulerAngles);
            _visualEffect.Play();
            GetComponent<Animator>().Play("CoupDroit");
            foreach (Collider Co in GetTrigger)
            {
                if (Co != null)
                {
                    Co.gameObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * forceAmount + transform.up * forceAmount * 0.2f);
                    if(Co.transform.GetComponent<EnnemieBehavior>())
                        Co.transform.GetComponent<EnnemieBehavior>().ApplyDammage(10);
                }
                
            }

            pCoolDown = punchCd;
            StartCoroutine(StopParticles());
        }
        else
        {
            print("Slash");
            GetComponent<Animator>().Play("Slash");
        }

    }

    private void UpperCut()
    {
        GetComponent<Animator>().Play("Uppercut");
        foreach (Collider Co in GetTrigger)
        {
            if (Co != null && Vector3.Distance(transform.position, Co.gameObject.transform.position) <= uppercutMaxDistance )
            {
                Co.gameObject.GetComponent<Rigidbody>().AddForce(this.transform.up * forceAmount * 0.5f);
            }
                
        }

        uCoolDown = upperCd;
        StartCoroutine(StopParticles());
    }

    private void Dash()
    {
        GetComponent<Rigidbody>().AddForce((new Vector3(0, 0.75f, 0) + transform.forward) * dashForce);
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
                Debug.Log(hit.transform.name);
                Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * grabDistance,
                    Color.blue);
                hit.transform.SetParent(grabHandler.transform);
                hit.transform.position = grabHandler.transform.position;
                hit.transform.GetComponent<Rigidbody>().isKinematic = true;
                if (hit.transform.GetComponent<EnnemieBehavior>())
                {
                    hit.transform.GetComponent<EnnemieBehavior>().enabled = false;
                    hit.transform.GetComponentInChildren<Animation>().enabled = false;
                }

                grabbed = true;
            }
        }
        else
        {

            Transform hit = grabHandler.transform.GetChild(0);
            print("UnGrab");
            if (hit.GetComponent<EnnemieBehavior>())
            {
                hit.GetComponent<EnnemieBehavior>().enabled = true;
                hit.GetComponentInChildren<Animation>().enabled = true;
            }
            hit.GetComponent<Rigidbody>().isKinematic = false;
            hit.parent = null;
            grabbed = false;
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
        if(other.gameObject.tag == "Punchable")
            GetTrigger.Add(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Punchable")
            GetTrigger.Remove(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (willStomp && other.transform.CompareTag("Ground"))
        {

            foreach (var GO in GetComponentInChildren<GetCollider>().obj)
            {
                GO.GetComponent<Rigidbody>().AddForce(GO.transform.position.x, (GO.transform.position.y + 1) * stompForce,
                    GO.transform.position.z);

            }

            
        }
        willStomp = false;
    }
}
