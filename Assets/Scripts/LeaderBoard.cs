using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LeaderBoard
{
    public List<int> Scores = new();

    public void AddToScores(int score)
    {
        Scores.Add(score);
        Scores.Sort();
        Scores.Reverse();
        if (Scores.Count == 10 ) 
        {
            Scores.RemoveAt(9);
        }
        Scores.ForEach(x => Debug.Log(x));
    }

    public void SaveAsJSON()
    {

        PlayerPrefs.SetString("Highscore", JsonUtility.ToJson(this));
        Debug.Log(PlayerPrefs.GetString("Highscore"));
    }

    public void LoadFromJSON()
    {
        string JSONscores = PlayerPrefs.GetString("Highscore");
        Scores = JsonUtility.FromJson<LeaderBoard>(JSONscores).Scores;
        Scores.ForEach(x => Debug.Log(x));
        
    }

}

