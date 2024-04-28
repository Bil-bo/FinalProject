using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Concretion focused on boosting player score
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
