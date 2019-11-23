using System;
using UnityEngine;

public class TeleportToPosition : MonoBehaviour
{
    [SerializeField] private Transform RespawnPosition;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        other.transform.position = RespawnPosition.position;
    }
}
