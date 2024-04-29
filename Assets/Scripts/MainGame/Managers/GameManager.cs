using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


// Central logic hub
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
    private AudioClip GameOverSound;

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
    private bool SceneChanging = false;


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

    // Controls the main game
    void Update()
    {
        // Constantly slightly increases the speed of the game so long as the bool is true
        if (UpdateSpeed) 
        { 
            Speed += SpeedIncrease * Time.deltaTime;
        }


        // Constantly updates the speed of active inheritors of moving objects
        MovingObject[] movingObjects = FindObjectsOfType<MovingObject>();

        foreach (MovingObject obj in movingObjects)
        {
            obj.UpdateSpeed(Speed);
        }


        // Calculate total distance travelled
        Distance += Speed * Time.deltaTime;

        // Calculate distance travelled to next score
        prevDistance += Speed * Time.deltaTime;


        // Check if the distance has hit the required mark to update score
        if (prevDistance >= TargetDistance)
        {
            Score += 1;
            prevDistance = 0f;
        }

        // Check if the distance travelled permits transitions to be added to the layout pool
        if (Mathf.RoundToInt(Distance - ScenePrevDistance) >= SceneTargetDistance && !MilestoneAchieved)
        {
            MilestoneAchieved = true;
            EventManager.Broadcast(new DistanceMilestoneEvent());
        }
        // Update the on screen labels
        canvas.UpdateLabels(Distance, Score);
    }

    // Event listener for score, updates score, checking for negative values
    public void OnScore(ScoreEvent eventData)
    {
        // Negative values are ignored during the superspeed powerUp
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

    // Event listener for a scene change
    // Depending on the scene, activates and deactivates appropriate gameObjects
    // And moves the camera and the player to their starting spots in the new scene
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

        // Reactivates the movement
        SceneChanging = false;
        GameStop(false);

        // Restart checking for distance milestones
        ScenePrevDistance = Distance;

        SceneTargetDistance = Random.Range(200, 800);
        MilestoneAchieved = false;
    }


    // Game over event listener
    // Saves the current score and stops everything from moving
    public void OnGameOver(GameOverEvent eventData)
    {
        SoundManager.instance.PlayClip(GameOverSound, transform, 1f);
        GameOver = true;
        GameStop(true);
        Highscores.AddToScores(Score);
        Highscores.SaveAsJSON();
    }

    // Pause event listener, stops and starts the game from moving 
    public void OnPauseEvent(PauseEvent eventData)
    {
        Paused = eventData.Status;
        GameStop(eventData.Status);

    }

    // Stops everything from moving
    // During the falling scene, temporarily deactivates the ground holding the player up
    public void OnSceneChanging(SceneChangingEvent eventData)
    {
        SceneChanging = true;
        GameStop(true);
        if (Scene == SceneIndex.FALLING) { FallingGround.SetActive(false); }
    }


    // Restart event listener
    // Resets variables
    public void OnRestart(RestartEvent eventData)
    {
        Score = 0;
        Distance = 0;
        Speed = SpeedSave = InitSpeed;
        if (GameOver) { GameOver = false; }

        EventManager.Broadcast(new SceneChangeEvent() { Stage = SceneIndex.RUNNING });

    }

    // Speed event powerUp listener
    // Temporarily increases the speed of the game
    public void OnSpeed(SpeedEvent eventData) 
    {
        StartCoroutine(SuperSpeed(eventData.SpeedIncrease, eventData.ActiveTime));
    }

    // Timer for superSpeed events
    // Stop during pauses and stops completely on a gameover or restart
    private IEnumerator SuperSpeed(float speed, float activeTime)
    {

        float superSave = Speed;
        Speed = speed;
        SpeedSuper = true;
        UpdateSpeed = false;
        while (activeTime > 0) 
        {
            if (!(Paused || SceneChanging))
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

    // event listener for if the player successfully completes the minigame
    // Resets the player to the starting position
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


    // For controlling whether or not objects in the scene are moving
    private void GameStop(bool stop) 
    {
        // Freezing the game
        if (stop) 
        {
            // Speed is not saved here during a super speed event or on a game over
            // To prevent permanently increasing or decreasing the speed to unintended values
            if (!(SpeedSuper && GameOver)) { SpeedSave = Speed; }
            Debug.Log(SpeedSave);
            Speed = 0;
            UpdateSpeed = false;
        }
        // Unfreezing the game
        else
        {
            Speed = SpeedSave;
            if (!SpeedSuper) { UpdateSpeed = true; }
        }
    }


    // Destroy event listeners to avoid accidently triggering an event multiple times
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
