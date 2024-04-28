using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Manager class used for dealing with sound
// Taken in part from video here
public class SoundManager : MonoBehaviour, 
    IOnGameOverEvent, IOnReviveEvent, IOnPauseEvent, IOnRestartEvent
{
    public static SoundManager instance;

    [SerializeField]
    private AudioSource MainMusicPlayer;

    [SerializeField]
    private AudioClip MainLoop;

    [SerializeField]
    private AudioClip StartLoop;

    [SerializeField]
    private AudioSource SFXPlayer;

    private bool Paused = false;
    private bool GameOver = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    // Set up listeners and initial main music
    private void Start()
    {
        EventManager.AddListener<GameOverEvent>(OnGameOver);
        EventManager.AddListener<PauseEvent>(OnPauseEvent);
        EventManager.AddListener<ReviveEvent>(OnRevive);
        EventManager.AddListener<RestartEvent>(OnRestart);
        MainMusicPlayer.clip = StartLoop;
        MainMusicPlayer.Play();

    }

    private void Update()
    {
        // Once the start of the music is finished, play the main loop
        if (!MainMusicPlayer.isPlaying && !(Paused || GameOver))
        {
            MainMusicPlayer.clip = MainLoop;
            MainMusicPlayer.loop = true;
            MainMusicPlayer.Play();
        }
    }

    // Creates a temporary gameObject that plays a sound
    public void PlayClip(AudioClip clip, Transform spawn, float volume)
    {
        AudioSource audioSource = Instantiate(SFXPlayer, spawn.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    // Stops music on a gameover
    public void OnGameOver(GameOverEvent eventData) 
    {
        GameOver = true;   
        MainMusicPlayer.Pause();
    }


    // Begins the music from the beginning on a restart
    public void OnRestart(RestartEvent eventData)
    {
        MainMusicPlayer.clip = StartLoop;
        MainMusicPlayer.loop = false;
        MainMusicPlayer.Stop();
        MainMusicPlayer.Play();
    }

    // changes music state on a pause
    public void OnPauseEvent(PauseEvent eventData)
    {
        Paused = eventData.Status;
        if (eventData.Status)
        {
            MainMusicPlayer.Pause();
        }
        else
        {
            MainMusicPlayer.Play();
        }
    }

    // Starts the music back up from when it was stopped on revival
    public void OnRevive(ReviveEvent eventData)
    {
        GameOver = false;
        MainMusicPlayer.Play();

    }

    // Remove listeners
    private void OnDestroy()
    {
        EventManager.RemoveListener<GameOverEvent>(OnGameOver);
        EventManager.RemoveListener<PauseEvent>(OnPauseEvent);
        EventManager.RemoveListener<RestartEvent>(OnRestart);
        EventManager.RemoveListener<ReviveEvent>(OnRevive);

    }
}


