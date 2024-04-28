using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// Menu class for instantiating the leaderboard
public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI LeaderboardText;


    private HighScoreSaver ScoreSaver = new();
    private List<int> Scores = new List<int>();
    void Awake()
    {
        // Load the scores
        Scores = ScoreSaver.LoadFromJSON();

        // Write the scores to the text box
        for (int i = 0;  i < Scores.Count; i++) 
        {
            LeaderboardText.text = LeaderboardText.text + (i+1) + ". " + Scores[i] + "\n";
        }

    }
}
