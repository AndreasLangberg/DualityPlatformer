using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardMode : MonoBehaviour
{
    [SerializeField] private GameObject HardModeText;
    [SerializeField] private GameObject Tutorial;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(Tutorial)
            Destroy(Tutorial);
        HardModeText.SetActive(true);
        Destroy(gameObject);
    }
}
