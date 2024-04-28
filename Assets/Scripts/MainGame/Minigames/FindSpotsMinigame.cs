using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


// Overarching class for the spot finding minigame
public class FindSpotsMinigame : Minigame
{
    // Start is called before the first frame update
    private List<Spot> SpotList;

    [SerializeField]
    private int Timer = 8;

    [SerializeField]
    private TextMeshProUGUI TimerText;


    // initial set up
    private void OnEnable()
    {
        TimerText.text = Timer.ToString();
        SpotList = GetComponentsInChildren<Spot>().ToList();
        foreach (var spot in SpotList)
        {
            spot.Send += FoundSpot;
        }
        StartCoroutine(MinigameTimer());
    }

    // remove unecessary subscribers
    private void OnDisable()
    {
        StopCoroutine(MinigameTimer());
        foreach (var spot in SpotList)
        {
            spot.Send -= FoundSpot;
        }
    }

    // checking when spot is found, if minigame is complete
    private void FoundSpot(Spot spot)
    {
        SpotList.Remove(spot);
        if (SpotList.Count <= 0 ) 
        {
            completed.Invoke(true);
        }
    }

    // Timer associated with the minigame
    private IEnumerator MinigameTimer()
    {
        int timer = Timer;
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer--;
            TimerText.text = timer.ToString();
        }
        // If the timer ends, the minigame is failed
        completed.Invoke(false);
    }


    private void OnDestroy()
    {
        foreach (var spot in SpotList)
        {
            spot.Send -= FoundSpot;
        }

    }

}
