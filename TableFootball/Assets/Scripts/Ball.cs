using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static List<Vector3> ballPositions = new List<Vector3>();

    void Start()
    {
        ballPositions.Add(transform.position);
    }
}
