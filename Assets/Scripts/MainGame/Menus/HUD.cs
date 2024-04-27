using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    private void MiniGameCompleted(bool complete)
    {
        foreach (var item in Minigames)
        {
            item.SetActive(false);
        }

        if (complete)
        {
            SecondChance = true;
            CanPause = true;
            PauseButton.interactable = true;
            EventManager.Broadcast(new ReviveEvent());
        }
        else
        {
            int highscore = JsonUtility.FromJson<HighScoreSaver>(PlayerPrefs.GetString("Highscore")).Scores[0];
            GameOverHighScore.text = "Highscore\n" + highscore;
            GameOverMenu.SetActive(true);
            PauseButton.interactable = false;
            IsPaused = false;
            FactMenu.SetActive(true);
        }
    }

    public void ButtonPause()
    {

        if (!IsPaused && CanPause)
        {
            PauseButton.interactable = false;
            OnPause();
        }
    }

    public void OnPause()
    {
        if (CanPause)
        {
            PauseEvent data = new PauseEvent();

            if (!IsPaused)
            {
                IsPaused = true;
                PauseMenu.SetActive(true);
                FactMenu.SetActive(true);
                PauseButton.interactable = false;
                data.Status = true;
                EventManager.Broadcast(data);


            }
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

    private void OnApplicationPause(bool pause)
    {
        if (CanPause)
        {
            IsPaused = !pause;
            OnPause();
        }
    }


    public void OnGameOver(GameOverEvent eventData)
    {
        CanPause = false;
        PauseButton.interactable = false;
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
            miniGame.GetComponent<Minigame>().completed += MiniGameCompleted;
        }
        else
        {
            MiniGameCompleted(false);
        }
    }

    public void OnRestart(RestartEvent eventData)
    {
        CanPause = true;
        SecondChance = false;
        FactMenu.SetActive(false);
        _PauseButton.interactable = true;
        IsPaused = false;

    }


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
