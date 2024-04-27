using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FindSpotsMinigame : Minigame
{
    // Start is called before the first frame update
    private List<Spot> SpotList;

    [SerializeField]
    private int Timer = 8;

    [SerializeField]
    private TextMeshProUGUI TimerText;

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

    private void OnDisable()
    {
        StopCoroutine(MinigameTimer());
        foreach (var spot in SpotList)
        {
            spot.Send -= FoundSpot;
        }
    }

    private void FoundSpot(Spot spot)
    {
        SpotList.Remove(spot);
        if (SpotList.Count <= 0 ) 
        {
            Debug.Log("Activating");
            completed.Invoke(true);
        }
    }

    private IEnumerator MinigameTimer()
    {
        int timer = Timer;
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer--;
            TimerText.text = timer.ToString();
        }
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
