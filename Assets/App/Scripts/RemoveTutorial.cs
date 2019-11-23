using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTutorial : MonoBehaviour
{
    [SerializeField] private GameObject TutorialText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(TutorialText)
            Destroy(TutorialText);
        Destroy(gameObject);
    }
}
