using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public interface IEvent { }

public class ScoreEvent: IEvent 
{
    public int Points { get; set; }
}

public class ObstacleEvent: IEvent 
{
    public int PointsRemoved { get; set; }
}

public class GameOverEvent: IEvent { }

public class SceneChangeEvent: IEvent 
{
    public SceneIndex Stage { get; set; }  
}

public class SceneChangingEvent : IEvent { }

public class DistanceMilestoneEvent: IEvent { }

public class PauseEvent: IEvent 
{
    public bool Status { get; set; }
}

public abstract class PowerUpEvent: IEvent 
{
    public float ActiveTime { get; set; }

}

public class RestartEvent : IEvent { }

public class StrengthEvent: PowerUpEvent
{ }

public class SpeedEvent: PowerUpEvent 
{
    public float SpeedIncrease {  get; set; }   
}

public class ReviveEvent: IEvent { }