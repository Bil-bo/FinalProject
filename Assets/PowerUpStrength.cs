using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpStrength : PowerUp
{

    protected override void ActivatePowerUp()
    {
        StrengthEvent evt = new StrengthEvent() { ActiveTime = activeTime};
        EventManager.Broadcast(evt);
        Destroy(gameObject);
    }


}
