using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour, 
    IOnScoreEvent, IOnSceneChangeEvent, 
    IOnSceneChangingEvent, IOnGameOverEvent,
    IOnPauseEvent, IOnRestartEvent,
    IOnSpeedEvent, IOnReviveEvent
{

    [SerializeField]
    private CinemachineVirtualCamera VirtualCamera;

    [SerializeField]
    private Player _Player;

    [SerializeField] 
    private HUD canvas;

    [SerializeField]
    public float Speed;

    private float InitSpeed;

    private float SpeedSave;
    private bool SpeedSuper = false;

    private HighScoreSaver Highscores = new HighScoreSaver();

    [SerializeField]
    private float SpeedIncrease = 0.1f;

    [SerializeField]
    private GameObject RunningObjects;

    [SerializeField]
    private GameObject FallingObjects;

    [SerializeField]
    private GameObject FallingGround;

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
    private bool Paused = false;


    private void Awake()
    {
        EventManager.AddListener<ScoreEvent>(OnScore);
        EventManager.AddListener<SceneChangeEvent>(OnSceneChange);
        EventManager.AddListener<SceneChangingEvent>(OnSceneChanging);
        EventManager.AddListener<GameOverEvent>(OnGameOver);
        EventManager.AddListener<PauseEvent>(OnPauseEvent);
        EventManager.AddListener<RestartEvent>(OnRestart);
        EventManager.AddListener<SpeedEvent>(OnSpeed);
        EventManager.AddListener<ReviveEvent>(OnRevive);



        VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().enabled = false;

        SpeedSave = InitSpeed = Speed;
        Highscores.LoadFromJSON();

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
        CinemachineFramingTransposer camPart = VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        switch (Scene)
        {
            case SceneIndex.FALLING:
                RunningObjects.SetActive(false);
                FallingObjects.SetActive(true);
                FallingGround.SetActive(true);
                FlyingObjects.SetActive(false);
                _Player.transform.position = new Vector2(100, 10);
                VirtualCamera.transform.position = new Vector3(100, 0, -1);
                camPart.m_TrackedObjectOffset = new Vector2(0, camPart.m_TrackedObjectOffset.y);
                break;
            case SceneIndex.RUNNING:

                RunningObjects.SetActive(true);
                FallingObjects.SetActive(false);
                FlyingObjects.SetActive(false);

                _Player.transform.position = new Vector2(-14.5f, -8.5f);
                VirtualCamera.transform.position = new Vector3(0, 0, -1);
                camPart.m_TrackedObjectOffset = new Vector2(14.5f, camPart.m_TrackedObjectOffset.y);

                break;
            case SceneIndex.FLYING:
                RunningObjects.SetActive(false);
                FallingObjects.SetActive(false);
                FlyingObjects.SetActive(true);

                _Player.transform.position = new Vector2(200, -8);
                VirtualCamera.transform.position = new Vector3(200, 0, -1);
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
        Highscores.AddToScores(Score);
        Highscores.SaveAsJSON();
    }

    public void OnPauseEvent(PauseEvent eventData)
    {
        Paused = eventData.Status;
        GameStop(eventData.Status);

    }


    public void OnSceneChanging(SceneChangingEvent eventData)
    {
        GameStop(true);
        if (Scene == SceneIndex.FALLING) { FallingGround.SetActive(false); }

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

        float superSave = Speed;
        Speed = speed;
        SpeedSuper = true;
        UpdateSpeed = false;
        while (activeTime > 0) 
        {
            if (!Paused)
            {
                activeTime -= 0.1f;
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (GameOver)
        {
            SpeedSuper = false;
            SpeedSave = superSave;
        }

        while (Paused) { yield return new WaitForSeconds(0.1f); }

        if (!GameOver)
        {
            SpeedSuper = false;
            Speed = superSave;
            UpdateSpeed = true;
        }


    }

    public void OnRevive(ReviveEvent eventData) 
    {
        switch (Scene)
        {
            case SceneIndex.FALLING:
                _Player.transform.position = new Vector2(100, 10);
                break;
            case SceneIndex.RUNNING:
                _Player.transform.position = new Vector2(-14.5f, -8.5f);

                break;
            case SceneIndex.FLYING:
                _Player.transform.position = new Vector2(200, -8);
                break;
        }
        _Player.Animator.SetTrigger("Transition");
        GameOver = false;
        GameStop(false);

    }

    private void GameStop(bool stop) 
    {
        if (stop) 
        {
            if (!(SpeedSuper && GameOver)) { SpeedSave = Speed; }
            Debug.Log(SpeedSave);
            Speed = 0;
            UpdateSpeed = false;
        } else
        {
            Speed = SpeedSave;
            if (!SpeedSuper) { UpdateSpeed = true; }
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
        EventManager.RemoveListener<ReviveEvent>(OnRevive);
    }

}
