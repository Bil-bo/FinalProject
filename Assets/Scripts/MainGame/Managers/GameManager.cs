using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour, 
    IOnScoreEvent, IOnSceneChangeEvent, 
    IOnSceneChangingEvent, IOnGameOverEvent,
    IOnPauseEvent, IOnRestartEvent,
    IOnSpeedEvent
{

    [SerializeField]
    private GameObject VirtualCamera;

    [SerializeField]
    private GameObject Player;

    [SerializeField] 
    private HUD canvas;

    [SerializeField]
    public float Speed;

    private float InitSpeed;

    private float SpeedSave;
    private bool SpeedSuper = false;

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
    private bool GameOver = false;


    private void Awake()
    {
        EventManager.AddListener<ScoreEvent>(OnScore);
        EventManager.AddListener<SceneChangeEvent>(OnSceneChange);
        EventManager.AddListener<SceneChangingEvent>(OnSceneChanging);
        EventManager.AddListener<GameOverEvent>(OnGameOver);
        EventManager.AddListener<PauseEvent>(OnPauseEvent);
        EventManager.AddListener<RestartEvent>(OnRestart);
        EventManager.AddListener<SpeedEvent>(OnSpeed);



        VirtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().enabled = false;

        InitSpeed = Speed;
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
        canvas.UpdateLabels(Distance, Score);


        
    }

    public void OnScore(ScoreEvent eventData)
    {
        if (!(eventData.Points < 0 && SpeedSuper))
        {

            Score += eventData.Points;
            if (Score < 0)
            {
                Score = 0;
                EventManager.Broadcast(new GameOverEvent());
            }
        }
    }

    public void OnSceneChange(SceneChangeEvent eventData)
    {
        Scene = eventData.Stage;
        CinemachineFramingTransposer camPart = VirtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();

        switch (Scene)
        {
            case SceneIndex.FALLING:
                RunningObjects.SetActive(false);
                FallingObjects.SetActive(true);
                FlyingObjects.SetActive(false);
                ChangeDirection(Vector2.up);

                Player.transform.position = new Vector2(100, 10);
                VirtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = GameObject.FindGameObjectWithTag("Falling").GetComponent<PolygonCollider2D>();
                camPart.enabled = true;
                camPart.m_TrackedObjectOffset = new Vector2(0, camPart.m_TrackedObjectOffset.y);
                break;
            case SceneIndex.RUNNING:

                RunningObjects.SetActive(true);
                FallingObjects.SetActive(false);
                FlyingObjects.SetActive(false);
                ChangeDirection(Vector2.left);

                Player.transform.position = new Vector2(-14.5f, -8.5f);
                VirtualCamera.transform.position = new Vector3(0, 0, -1);
                VirtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = null;
                camPart.enabled = false;
                camPart.m_TrackedObjectOffset = new Vector2(14.5f, camPart.m_TrackedObjectOffset.y);

                break;
            case SceneIndex.FLYING:
                RunningObjects.SetActive(false);
                FallingObjects.SetActive(false);
                FlyingObjects.SetActive(true);
                ChangeDirection(Vector2.down);

                Player.transform.position = new Vector2(200, -8);
                VirtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = GameObject.FindGameObjectWithTag("Flying").GetComponent<PolygonCollider2D>();
                camPart.enabled = true;
                camPart.m_TrackedObjectOffset = new Vector2(0, camPart.m_TrackedObjectOffset.y);


                break;
        }

        GameStop(false);

        ScenePrevDistance = Distance;

        SceneTargetDistance = Random.Range(200, 1000);
        MilestoneAchieved = false;
    }

    public void OnGameOver(GameOverEvent eventData)
    {
        GameOver = true;
        GameStop(true);

    }

    public void OnPauseEvent(PauseEvent eventData)
    {
        GameStop(eventData.Status);

    }


    public void OnSceneChanging(SceneChangingEvent eventData)
    {
        GameStop(true);

    }

    public void OnRestart(RestartEvent eventData)
    {
        Score = 0;
        Distance = 0;
        Speed = SpeedSave = InitSpeed;
        if (GameOver) { GameOver = false; }

        EventManager.Broadcast(new SceneChangeEvent() { Stage = SceneIndex.RUNNING });

    }

    public void OnSpeed(SpeedEvent eventData) 
    {
        StartCoroutine(SuperSpeed(eventData.SpeedIncrease, eventData.ActiveTime));
    }

    private IEnumerator SuperSpeed(float speed, float activeTime)
    {
        GameStop(true);
        Speed = speed;
        SpeedSuper = true;
        yield return new WaitForSeconds(activeTime);
        if (!GameOver)
        {
            GameStop(false);
            SpeedSuper = false;
        }
    }

    private void ChangeDirection(Vector2 newDirection)
    {
        MovingObject[] movingObjects = FindObjectsOfType<MovingObject>();

        foreach (MovingObject obj in movingObjects)
        {
            obj.UpdateDirection(newDirection);
        }
    }

    private void GameStop(bool stop) 
    {
        if (stop) 
        {
            SpeedSave = Speed;
            Speed = 0;
            UpdateSpeed = false;
        } else
        {
            Speed = SpeedSave;
            UpdateSpeed = true;
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<ScoreEvent>(OnScore);
        EventManager.RemoveListener<SceneChangeEvent>(OnSceneChange);
        EventManager.RemoveListener<SceneChangingEvent>(OnSceneChanging);
        EventManager.RemoveListener<GameOverEvent>(OnGameOver);
        EventManager.RemoveListener<PauseEvent>(OnPauseEvent);
        EventManager.RemoveListener<RestartEvent>(OnRestart);
        EventManager.RemoveListener<SpeedEvent>(OnSpeed);
    }

}
