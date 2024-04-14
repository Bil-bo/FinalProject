using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IOnScoreEvent
{
    [SerializeField]
    public float speed;

    public int Score = 0;

    private void Awake()
    {
        EventManager.AddListener<ScoreEvent>(OnScore);
    }

    // Update is called once per frame
    void Update()
    {
        MovingObject[] movingObjects = FindObjectsOfType<MovingObject>();

        foreach (MovingObject obj in movingObjects)
        {
            obj.UpdateSpeed(speed);
        }
        
    }

    public void OnScore(ScoreEvent eventData)
    {
        Score += eventData.Points;
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<ScoreEvent>(OnScore);
    }

}
