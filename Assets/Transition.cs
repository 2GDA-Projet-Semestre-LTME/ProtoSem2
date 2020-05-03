using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    [SerializeField] private GameObject Door;
    public bool canLoadLvl;
    [SerializeField] private bool isStart;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && isStart)
        {
            Door.GetComponent<Animator>().Play("CloseDoor");
        }
        else if (other.transform.CompareTag("Player"))
        {
            Door.GetComponent<Animator>().Play("OpenDoor");
        }
    }

    private void Update()
    {
        if (canLoadLvl && isStart)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
        }
    }
}
