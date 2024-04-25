using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CleanTeethMinigame : Minigame
{
    private List<Tooth> TeethList = new List<Tooth>();
    private List<Tooth> DirtyTeethList = new List<Tooth>();

    [SerializeField]
    private int Timer = 8;

    [SerializeField]
    private TextMeshProUGUI TimerText;


    private void OnEnable()
    {
        TeethList = GetComponentsInChildren<Tooth>().ToList();
        int DirtyTeeth = Random.Range(1, Mathf.Min(TeethList.Count, 5));
        while (DirtyTeeth != 0)
        {
            Tooth DirtyTooth = TeethList[Random.Range(0, TeethList.Count)];
            Debug.Log(DirtyTooth.ToString());
            TeethList.Remove(DirtyTooth);
            DirtyTooth.IsDirty = true;
            DirtyTooth.Cleaned += CleanedTeeth;
            DirtyTeethList.Add(DirtyTooth); 
            DirtyTeeth--;
        }
        int timeLeft = 120 - Timer;
        TimerText.text = string.Format("{0:00}: {1:00}", timeLeft / 60, timeLeft % 60);
        StartCoroutine(CleanTimer());   
    }

    private void CleanedTeeth(Tooth cleanTooth)
    {
        Debug.Log("Here Here");
        DirtyTeethList.Remove(cleanTooth);
        if (DirtyTeethList.Count == 0)
        {
            StopCoroutine(CleanTimer());    
            completed.Invoke(true);
        }

    }

    private IEnumerator CleanTimer()
    {
        int timer = Timer;
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer--;
            int timeLeft = 120 - timer;
            TimerText.text = string.Format("{0:00}: {1:00}", timeLeft / 60, timeLeft % 60);
        }
        completed.Invoke(false);
    }

    private void RemoveListeners()
    {
        foreach (var t in DirtyTeethList)
        {
            t.Cleaned -= CleanedTeeth;
        }

    }

    private void OnDisable()
    {
        DirtyTeethList.Clear();
        RemoveListeners();
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }

}
