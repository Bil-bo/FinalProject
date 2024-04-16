using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour, IOnScoreEvent, IOnSceneChangeEvent, IOnSceneChangingEvent
{

    [SerializeField]
    private GameObject VirtualCamera;

    [SerializeField]
    private GameObject Player;

    [SerializeField]
    public float Speed;

    private float SpeedSave;

    [SerializeField]
    private float SpeedIncrease = 0.1f;

    [SerializeField]
    private GameObject RunningObjects;

    [SerializeField]
    private GameObject FallingObjects;

    [SerializeField]
    private GameObject FlyingObjects;

    [SerializeField]
    private int TargetDistance = 20;

    [SerializeField]
    private int SceneTargetDistance = 200;

    public SceneIndex Scene;

    private int Score = 0;
    private float Distance = 0f;

    private float prevDistance = 0f;

    private float ScenePrevDistance = 0f;
    private bool MilestoneAchieved = false;

    private bool UpdateSpeed = true;


    [SerializeField]
    private TextMeshProUGUI DistanceText;

    [SerializeField]
    private TextMeshProUGUI ScoreText;



    private void Awake()
    {
        EventManager.AddListener<ScoreEvent>(OnScore);
        EventManager.AddListener<SceneChangeEvent>(OnSceneChange);
        EventManager.AddListener<SceneChangingEvent>(OnSceneChanging);
        VirtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (UpdateSpeed) 
        { 
            Speed += SpeedIncrease * Time.deltaTime;
        }

        MovingObject[] movingObjects = FindObjectsOfType<MovingObject>();

        foreach (MovingObject obj in movingObjects)
        {
            obj.UpdateSpeed(Speed);
        }

        Distance += Speed * Time.deltaTime;



        prevDistance += Speed * Time.deltaTime;

        DistanceText.text = "Distance = " + Mathf.RoundToInt(Distance);

        if (prevDistance >= TargetDistance)
        {
            Score += 1;
            prevDistance = 0f;
        }

        if (Mathf.RoundToInt(Distance - ScenePrevDistance) >= SceneTargetDistance && !MilestoneAchieved)
        {
            MilestoneAchieved = true;
            EventManager.Broadcast(new DistanceMilestoneEvent());
        }
        ScoreText.text = "Score = " + Score;

        
    }

    public void OnScore(ScoreEvent eventData)
    {
        Score += eventData.Points;
        if (Score < 0)
        {
            Score = 0;
            Debug.Log("Game Over");
        }
    }

    public void OnSceneChange(SceneChangeEvent eventData)
    {
        Scene = eventData.Stage;

        switch (Scene)
        {
            case SceneIndex.FALLING:
                RunningObjects.SetActive(false);
                FallingObjects.SetActive(true);
                FlyingObjects.SetActive(false);
                ChangeDirection(Vector2.up);

                Player.transform.position = new Vector2(100, 10);
                VirtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = GameObject.FindGameObjectWithTag("Falling").GetComponent<PolygonCollider2D>();
                VirtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().enabled = true;
                break;
            case SceneIndex.RUNNING:

                RunningObjects.SetActive(true);
                FallingObjects.SetActive(false);
                FlyingObjects.SetActive(false);
                ChangeDirection(Vector2.left);

                Player.transform.position = new Vector2(-14.5f, -8.5f);
                VirtualCamera.transform.position = new Vector3(0, 0, -1);
                VirtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = null;
                VirtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().enabled = false;

                break;
            case SceneIndex.FLYING:
                RunningObjects.SetActive(false);
                FallingObjects.SetActive(false);
                FlyingObjects.SetActive(true);
                ChangeDirection(Vector2.down);

                Player.transform.position = new Vector2(200, -8);
                VirtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = GameObject.FindGameObjectWithTag("Flying").GetComponent<PolygonCollider2D>();
                VirtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().enabled = true;

                break;
        }

        Speed = SpeedSave;
        UpdateSpeed = true;

        ScenePrevDistance = Distance;

        System.Random target = new System.Random();
        SceneTargetDistance = target.Next(200, 1000);
        MilestoneAchieved = false;


    }

    private void ChangeDirection(Vector2 newDirection)
    {
        MovingObject[] movingObjects = FindObjectsOfType<MovingObject>();

        foreach (MovingObject obj in movingObjects)
        {
            obj.UpdateDirection(newDirection);
        }
    }

    public void OnSceneChanging(SceneChangingEvent eventData)
    {

        SpeedSave = Speed;
        Speed = 0;
        UpdateSpeed = false;

    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<ScoreEvent>(OnScore);
        EventManager.RemoveListener<SceneChangeEvent>(OnSceneChange);
        EventManager.RemoveListener<SceneChangingEvent>(OnSceneChanging);
    }

}
