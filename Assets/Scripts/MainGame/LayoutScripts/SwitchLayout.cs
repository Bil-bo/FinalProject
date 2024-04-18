using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLayout : Layout
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Pass") && !pass)
        {
            InitPass.Invoke(index);
            pass = true;

        }
    }
    public override void UpdateSpeed(float newSpeed)
    {
        if (!pass)
        {
            base.UpdateSpeed(newSpeed);
        }
        else
        {
            Speed = 0;
        }
        
    }
}
