using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemieSpawner : MonoBehaviour
{

    [SerializeField] private List<GameObject> NMI;
    [SerializeField] private List<GameObject> SpawnedNMI;

    [SerializeField] private float spawnRate;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", 2, spawnRate);
    }

    // Update is called once per frame
    void Spawn()
    {
        SpawnedNMI.Add(Instantiate(NMI[0], this.transform.position, this.transform.rotation));
    }
}
