using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents a layout that moves its parts
public class Layout : MovingObject
{
    [SerializeField]
    protected int index = -1;

    protected bool pass = false;

    public Action<int> InitPass;


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Initial pass invokes callback to SpawnerManager
        if (collision.CompareTag("Pass") && !pass)
        {
            InitPass.Invoke(index);
            pass = true;

        }

        // Secondary pass destroys layout
        else if (collision.CompareTag("Pass") && pass)
        {
            pass = false;
            Destroy(gameObject);
        }
    }
}
