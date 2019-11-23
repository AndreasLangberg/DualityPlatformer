using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToLive : MonoBehaviour
{
    public float DestructionTime = 10f;
    
    void Start()
    {
        Destroy (gameObject, DestructionTime);
    }
}
