using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D Door;
    [SerializeField] private Sprite SwitchDown;
    [SerializeField] private Sprite OpenDoorSprite;
    [SerializeField] private SpriteRenderer ExitDoor;

    private SpriteRenderer _renderer;
    
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _renderer.sprite = SwitchDown;
        Door.bodyType = RigidbodyType2D.Dynamic;
        ExitDoor.sprite = OpenDoorSprite;
        ExitDoor.GetComponent<Collider2D>().isTrigger = true;
        GetComponent<Collider2D>().enabled = false;
    }
}
