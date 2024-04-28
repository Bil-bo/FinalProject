using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Formality class
// Forces a general setup to be taken when subscribing to events in classes
public interface IObserve { }

public interface IOnScoreEvent: IObserve 
{
    void OnScore(ScoreEvent eventData);
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

public interface IOnPauseEvent : IObserve
{
    void OnPauseEvent(PauseEvent eventData);
}

public interface IOnRestartEvent : IObserve
{
    void OnRestart(RestartEvent eventData);
}

public interface IOnStrengthEvent: IObserve 
{
    void OnStrength(StrengthEvent eventData);
}

public interface IOnSpeedEvent : IObserve
{
    void OnSpeed(SpeedEvent eventData);
}

public interface IOnReviveEvent : IObserve
{
    void OnRevive(ReviveEvent eventData);
}
