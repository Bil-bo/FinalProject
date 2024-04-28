using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Works in tandem with event manager
public interface IEvent { }

// For any event that alters the score
public class ScoreEvent: IEvent 
{
    public int Points { get; set; }
}

// Event called when the player loses
public class GameOverEvent: IEvent { }


// Event called when the scene fully changes
public class SceneChangeEvent: IEvent 
{
    public SceneIndex Stage { get; set; }  
}

// Event called at the beginning of a scene change, for transitioning purposes
public class SceneChangingEvent : IEvent { }


// Event for controlling when the transition layouts are added to the potential pool of layouts
public class DistanceMilestoneEvent: IEvent { }

// For when the game is paused
public class PauseEvent: IEvent 
{
    public bool Status { get; set; }
}


// Abstract for when the player activates a timed powerup
public abstract class PowerUpEvent: IEvent 
{
    public float ActiveTime { get; set; }

}


public class StrengthEvent : PowerUpEvent { }

public class SpeedEvent : PowerUpEvent
{
    public float SpeedIncrease { get; set; }
}


// For when the game is restarted
public class RestartEvent : IEvent { }



public class ReviveEvent: IEvent { }