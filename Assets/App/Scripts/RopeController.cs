using System;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    [HideInInspector] public Transform Origin;
    [HideInInspector] public Transform Destination;

    private bool _lockedDestination;

    private MeshRenderer rend;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        rend.enabled = false;
    }

    void Update()
    {
        if (Origin && Destination)
        {
            var position = ((Destination.position + Origin.position) / 2);
            position.y += 0.05f;
            transform.position = position;
            transform.localScale = new Vector3(0.02f, Vector3.Distance(Destination.position, Origin.position), 0.01f);
            
            transform.rotation = Quaternion.Euler(0, 0, 90 + Mathf.Atan2(Destination.position.y-Origin.position.y, Destination.position.x-Origin.position.x) * Mathf.Rad2Deg);
        }

        rend.enabled = true;
    }
}
