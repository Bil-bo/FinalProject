using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserve { }

public interface IOnScoreEvent: IObserve 
{
    void OnScore(ScoreEvent eventData);
}

public interface IOnObstacleEvent : IObserve
{
    void OnObstacle(ObstacleEvent eventData);
}
public interface IOnGameOverEvent: IObserve
{
    void OnGameOver(GameOverEvent eventData);
}

public interface IOnSceneChangeEvent : IObserve
{
    void OnSceneChange(SceneChangeEvent eventData);
}

public interface IOnSceneChangingEvent : IObserve
{
    void OnSceneChanging(SceneChangingEvent eventData);
}


public interface IOnDistanceMilestoneEvent : IObserve
{
    void OnDistanceMilestone(DistanceMilestoneEvent eventData);
}