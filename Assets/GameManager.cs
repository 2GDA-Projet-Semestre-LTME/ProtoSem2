using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public EnnemieBehavior[] Jaguar;
    // Start is called before the first frame update
    void Start()
    {
        GetAllJaguar();
    }

    // Update is called once per frame
    public void GetAllJaguar()
    {
        Jaguar = GameObject.FindObjectsOfType<EnnemieBehavior>();
    }
}
