using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public static int playerChanger = 0; // 0 gracz; 1 AI
    public static List<List<Vector3>> positionsInOut = new List<List<Vector3>>();
    public static bool weHaveWiner = false;
}
