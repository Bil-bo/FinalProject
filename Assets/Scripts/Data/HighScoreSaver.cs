using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Class To Load and Store Highscore Data from playerPrefs
[Serializable]
public class HighScoreSaver
{
    public List<int> Scores = new();


    // Add a score to the list. Capped at 10 scores.
    public void AddToScores(int score)
    {
        Scores.Add(score);
        Scores.Sort();
        Scores.Reverse();
        if (Scores.Count == 10 ) 
        {
            Scores.RemoveAt(9);
        }
    }


    // Saves List to PlayerPrefs
    public void SaveAsJSON()
    {
        PlayerPrefs.SetString("Highscore", JsonUtility.ToJson(this));
        Debug.Log(PlayerPrefs.GetString("Highscore"));
    }

    // Loads and returns the list from PlayerPrefs
    public List<int> LoadFromJSON()
    {
        if (PlayerPrefs.HasKey("Highscore"))
        {
            Debug.Log("Found Key");
            string JSONscores = PlayerPrefs.GetString("Highscore");
            Scores = JsonUtility.FromJson<HighScoreSaver>(JSONscores).Scores;
        }

        else { Debug.Log("No Key Found"); }

        return Scores;
    }

}

