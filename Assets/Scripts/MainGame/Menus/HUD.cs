using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour, IOnGameOverEvent, IOnRestartEvent
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
    private GameObject PauseMenu;

    [SerializeField]
    private Button PauseButton;

    public Button _PauseButton 
    {
        get { return PauseButton; } 
        set { PauseButton = value; } 
    }

    public bool IsPaused = false;


    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener<GameOverEvent>(OnGameOver);
        EventManager.AddListener<RestartEvent>(OnRestart);
        
    }

    public void ButtonPause()
    {

        if (!IsPaused)
        {
            Debug.Log("Button Pressed");
            PauseButton.interactable = false;
            OnPause();
        }
    }

    public void OnPause()
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

    public void OnGameOver(GameOverEvent eventData)
    {
        GameOverMenu.SetActive(true);
        PauseButton.interactable = false;
        IsPaused = false;       
        FactMenu.SetActive(true);
    }

    public void OnRestart(RestartEvent eventData)
    {
        FactMenu.SetActive(false);
        _PauseButton.interactable = true;
        IsPaused = false;

    }

    public void UpdateLabels(float distance, int score) 
    {
        DistanceText.text = "Distance = " + Mathf.RoundToInt(distance);
        ScoreText.text = "Score = " + score;
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<GameOverEvent>(OnGameOver);  
        EventManager.RemoveListener<RestartEvent>(OnRestart);  
    }


}
