using System.Collections;
using System.Collections.Generic;
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
