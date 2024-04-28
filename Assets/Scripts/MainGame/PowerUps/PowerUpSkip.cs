using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Power up to make the screen temporarily faster
public class PowerUpSkip : PowerUp 
{
    [SerializeField]
    private float speedIncrease = 20f;


    private void Awake()
    {
        Send(Vector3.left, 3f, 3f, 3f);
    }

    protected override void ActivatePowerUp()
    {
        SpeedEvent evt = new SpeedEvent()
        {
            SpeedIncrease = speedIncrease,
            ActiveTime = activeTime
        };
        EventManager.Broadcast(evt);
        Destroy(gameObject);
    }

}
