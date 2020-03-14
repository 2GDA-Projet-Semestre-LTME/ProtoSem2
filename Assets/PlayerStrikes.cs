using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerStrikes : MonoBehaviour
{

    public List<Collider> GetTrigger;
    public GameObject strikeArea;
    public GameObject upperArea;
    public float[] Zpos;
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
            Co.gameObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * forceAmount + transform.up * forceAmount * 0.2f);
        }

        StartCoroutine(StopParticles());
    }

    private void UpperCut()
    {
        foreach (Collider Co in GetTrigger)
        {
            if(Vector3.Distance(transform.position, Co.gameObject.transform.position) <= uppercutMaxDistance)
                Co.gameObject.GetComponent<Rigidbody>().AddForce(this.transform.up * forceAmount * 0.25f);
        }
        StartCoroutine(StopParticles());
    }

    IEnumerator StopParticles()
    {
        yield return new WaitForSeconds(0.25f);
        
        _visualEffect.Stop();
    }
}
