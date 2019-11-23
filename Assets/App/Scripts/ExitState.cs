using System;
using UnityEngine;

public class ExitState : MonoBehaviour
{
    [SerializeField] private GameObject Text;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Text.SetActive(true);
    }
}
