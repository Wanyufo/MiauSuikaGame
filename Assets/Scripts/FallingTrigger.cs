using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTrigger : MonoBehaviour
{
    private Collider2D _collider2D;


    private void Start()
    {
        if (_collider2D == null)
        {
            bool success = TryGetComponent(out _collider2D);
            if (!success)
            {
                Debug.LogError("FallingTrigger: Could not find Collider2D component");
                Destroy(this);
                return;
            }
        }
        //
        // if (!_collider2D.isTrigger)
        // {
        //     Debug.LogWarning("FallingTrigger: Collider2D is not a trigger. Changing it to trigger.");
        //     _collider2D.isTrigger = true;
        // }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("FallingTrigger: OnTriggerEnter2D");
        if (other.attachedRigidbody.gameObject.TryGetComponent(out Cat cat))
        {
            cat.fallingOutOfGlass = true;
        }
    }
}