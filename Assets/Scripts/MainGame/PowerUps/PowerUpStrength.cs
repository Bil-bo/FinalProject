using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Power up to make the player temporarily invincible
public class PowerUpStrength : PowerUp
{
    protected override void ActivatePowerUp()
    {
        StrengthEvent evt = new StrengthEvent() { ActiveTime = activeTime};
        EventManager.Broadcast(evt);
        Destroy(gameObject);
    }


}
