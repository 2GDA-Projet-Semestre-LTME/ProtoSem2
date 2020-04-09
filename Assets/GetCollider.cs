using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCollider : MonoBehaviour
{
    public List<GameObject> obj;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Punchable")
        {
            obj.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Punchable")
        {
            obj.Remove(other.gameObject);
        }
    }
}
