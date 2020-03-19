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
    // Start is called before the first frame update
    void Start()
    {
        _visualEffect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Punch();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            UpperCut();
        }
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

    private void Punch()
    {
        _visualEffect.SetVector3("CameraRotation", Camera.main.transform.eulerAngles);
        _visualEffect.Play();
        foreach (Collider Co in GetTrigger)
        {
            if (Co != null)
            {
                Co.gameObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * forceAmount + transform.up * forceAmount * 0.2f);
                if(Co.transform.GetComponent<EnnemieBehavior>())
                    Co.transform.GetComponent<EnnemieBehavior>().ApplyDammage(10);
            }
                
        }
        
        StartCoroutine(StopParticles());
    }

    private void UpperCut()
    {
        foreach (Collider Co in GetTrigger)
        {
            if (Co != null && Vector3.Distance(transform.position, Co.gameObject.transform.position) <= uppercutMaxDistance )
            {
                Co.gameObject.GetComponent<Rigidbody>().AddForce(this.transform.up * forceAmount * 0.25f);
            }
                
        }
        StartCoroutine(StopParticles());
    }

    IEnumerator StopParticles()
    {
        yield return new WaitForSeconds(0.25f);
        
        _visualEffect.Stop();
    }
}
