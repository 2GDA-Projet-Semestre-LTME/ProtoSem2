using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemies_Positionnement : MonoBehaviour
{
    public Transform PositionContainer;

    public List<Transform> avalaiblePosition;

    public List<Transform> allPosition;

    public List<Transform> occupiedPosition;

    private void Start()
    {
        foreach (var pos in allPosition)
        {
            Vector3 posDir = pos.position - transform.position;
            if (Vector3.Dot(transform.forward, posDir) >= 0)
            {
                avalaiblePosition.Add(pos);
            }
        }
        RefreshDotProduct();
    }
    private void Update()
    {
        PositionContainer.position = this.transform.position;
    }

    public void RefreshDotProduct()
    {
        occupiedPosition.Clear();
        avalaiblePosition.Clear();
        foreach (var pos in allPosition)
        {
            Vector3 posDir = pos.position - transform.position;
            if (Vector3.Dot(transform.forward, posDir) >= 0)
            {
                avalaiblePosition.Add(pos);
            }
        }

    }
}
