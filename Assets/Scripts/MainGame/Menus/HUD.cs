using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


// Main class for the game UI
// Encapusulates most of the other menus in the game
public class HUD : MonoBehaviour, 
    IOnGameOverEvent, IOnRestartEvent
{
    [SerializeField]
    private TextMeshProUGUI DistanceText;

    [SerializeField]
    private TextMeshProUGUI ScoreText;

    [SerializeField]
    private GameObject FactMenu;

    [SerializeField]
    private GameObject GameOverMenu;

    [SerializeField]
    private TextMeshProUGUI GameOverScore;

    [SerializeField]
    private TextMeshProUGUI GameOverHighScore;


    [SerializeField]
    private GameObject PauseMenu;


    [SerializeField]
    private Button PauseButton;

    [SerializeField]
    private AudioClip PauseSound;
    public Button _PauseButton 
    {
        get { return PauseButton; } 
        set { PauseButton = value; } 
    }

    public bool IsPaused = false;
    private bool CanPause = true;
    private bool SecondChance = false;
    private GameObject[] Minigames;


    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener<GameOverEvent>(OnGameOver);
        EventManager.AddListener<RestartEvent>(OnRestart);


        Minigames = GameObject.FindGameObjectsWithTag("MiniGame");
        foreach (var item in Minigames)
        {
            item.SetActive(false);
        }
    }

    // Class attached to the action of the currently active minigame
    private void MiniGameCompleted(bool complete)
    {
        // Deactivates all minigames
        foreach (var item in Minigames)
        {
            item.SetActive(false);
        }

        // Revives the player on success
        if (complete)
        {
            SecondChance = true;
            CanPause = true;
            PauseButton.interactable = true;
            EventManager.Broadcast(new ReviveEvent());
        }
        // Full game over on failure
        else
        {
            int highscore = JsonUtility.FromJson<HighScoreSaver>
                (PlayerPrefs.GetString("Highscore")).Scores[0];
            GameOverHighScore.text = "Highscore\n" + highscore;
            GameOverMenu.SetActive(true);
            PauseButton.interactable = false;
            IsPaused = false;
            FactMenu.SetActive(true);
        }
    }

    // Behaviour specific to the pause button
    public void ButtonPause()
    {

        if (!IsPaused && CanPause)
        {
            PauseButton.interactable = false;
            OnPause();
        }
    }

    // For any pause event that occurs
    public void OnPause()
    {
        // If the game is still currently running as normal
        if (CanPause)
        {
            PauseEvent data = new PauseEvent();

            // If the game is not currently paused, pause the game
            if (!IsPaused)
            {
                SoundManager.instance.PlayClip(PauseSound, transform, 1f);
                IsPaused = true;
                PauseMenu.SetActive(true);
                FactMenu.SetActive(true);
                PauseButton.interactable = false;
                data.Status = true;
                EventManager.Broadcast(data);


            }

            // If the game is currently paused, unpause the game
            else
            {

                IsPaused = false;
                PauseMenu.SetActive(false);
                FactMenu.SetActive(false);
                PauseButton.interactable = true;
                data.Status = false;

                EventManager.Broadcast(data);
            }
        }
    }

    // For unitys own pause event, add it into the current pause ecosystem
    private void OnApplicationPause(bool pause)
    {
        if (CanPause)
        {
            IsPaused = !pause;
            OnPause();
        }
    }

    // Deactivate pausing on game over, and also activate a minigame to play
    public void OnGameOver(GameOverEvent eventData)
    {
        CanPause = false;
        PauseButton.interactable = false;
        // If the player has not already had a second chance, give the minigame
        if (!SecondChance)
        {
            GameObject miniGame;
            try
            {
                miniGame = Minigames[Random.Range(0, Minigames.Length)];
            }
            catch
            {
                miniGame = Minigames[0];
            }

            miniGame.SetActive(true);
            // Subscribe to the completed event for when the mini game finishes
            miniGame.GetComponent<Minigame>().completed += MiniGameCompleted;
        }
        // otherwise finish the game immediatly
        else
        {
            MiniGameCompleted(false);
        }
    }

    // Reactivate pausing and second chances on restart
    public void OnRestart(RestartEvent eventData)
    {
        CanPause = true;
        SecondChance = false;
        FactMenu.SetActive(false);
        _PauseButton.interactable = true;
        IsPaused = false;

    }

    // Update the onScreen labels
    public void UpdateLabels(float distance = -1, int score = -1) 
    {

        if (distance != -1) { DistanceText.text = "Distance = " + Mathf.RoundToInt(distance); }
        if (score != -1) 
        {
            ScoreText.text = "Score = " + score;
            GameOverScore.text = "Final Score\n" + score;
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<GameOverEvent>(OnGameOver);  
        EventManager.RemoveListener<RestartEvent>(OnRestart);  
    }


}
