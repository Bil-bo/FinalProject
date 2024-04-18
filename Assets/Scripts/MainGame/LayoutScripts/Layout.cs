using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layout : MovingObject
{


    [SerializeField]
    protected int index = -1;

    protected bool pass = false;

    public Action<int> InitPass;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Pass") && !pass)
        {
            InitPass.Invoke(index);
            pass = true;

        }

        else if (collision.CompareTag("Pass") && pass)
        {
            pass = false;
            Destroy(gameObject);
        }
    }
}
