using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI LeaderboardText;


    private HighScoreSaver ScoreSaver = new();
    private List<int> Scores = new List<int>();
    void Awake()
    {
        Scores = ScoreSaver.LoadFromJSON();

        for (int i = 0;  i < Scores.Count; i++) 
        {
            LeaderboardText.text = LeaderboardText.text + (i+1) + ". " + Scores[i] + "\n";
        }

    }
}
