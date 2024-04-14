using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

public class Layout : MovingObject
{


    [SerializeField]
    public int index = -1;

    private bool pass = false;

    public Action<int> InitPass;

    public int test = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Pass") && !pass)
        {
            Debug.Log("invoking now");
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
