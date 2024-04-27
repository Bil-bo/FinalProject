using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpScore : PowerUp
{
    [SerializeField]
    private int ExtraScore;

    protected override void ActivatePowerUp()
    {
        ScoreEvent evt = new ScoreEvent() { Points = ExtraScore};
        EventManager.Broadcast(evt);
        Destroy(gameObject);
    }
}
