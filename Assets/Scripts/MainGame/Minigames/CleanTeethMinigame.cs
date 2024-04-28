using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


// Overarching minigame class for teeth brushing
public class CleanTeethMinigame : Minigame
{
    // Represents all teeth
    [SerializeField]
    private List<Tooth> TeethList = new List<Tooth>();

    // Represents teeth to be cleaned
    private List<Tooth> DirtyTeethList = new List<Tooth>();
    
    // Adjustable timer
    [SerializeField]
    private int Timer = 8;

    [SerializeField]
    private TextMeshProUGUI TimerText;

    

    // Minigame setup
    private void OnEnable()
    {
        // Activate a random number of teeth to be cleaned
        int DirtyTeeth = Random.Range(1, Mathf.Min(TeethList.Count, 5));

        // add to the dirty teeth list and remove from the regular teeth list
        while (DirtyTeeth != 0)
        {
            Tooth DirtyTooth = TeethList[Random.Range(0, TeethList.Count)];
            DirtyTooth.gameObject.SetActive(true);
            TeethList.Remove(DirtyTooth);
            DirtyTooth.IsDirty = true;

            // Subscribe to the cleaned event to be notified when a tooth is cleared
            DirtyTooth.Cleaned += CleanedTeeth;
            DirtyTeethList.Add(DirtyTooth); 
            DirtyTeeth--;
        }
        
        // Setting up the timer to appear like a 2 minute timer
        int timeLeft = 120 - Timer;
        
        // Deactivate the rest of the teeth
        TeethList.ForEach(x => x.gameObject.SetActive(false));
        TimerText.text = string.Format("{0:00}: {1:00}", timeLeft / 60, timeLeft % 60);
        StartCoroutine(CleanTimer()); // Start the timer
    }


    // Activated by 
    private void CleanedTeeth(Tooth cleanTooth)
    {
        // Readds the cleaned tooth back to the regular list
        TeethList.Add(cleanTooth);
        // removes it from the dirty list
        DirtyTeethList.Remove(cleanTooth);

        // If all teeth have been cleaned, the minigame is completed successfully
        if (DirtyTeethList.Count == 0)
        {
            StopCoroutine(CleanTimer());    
            completed.Invoke(true);
        }

    }

    // Timer associated with the minigame
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
        // If the timer runs out, the minigame is incomplete
        completed.Invoke(false);
    }


    // Remove unused listeners to avoid errors
    private void RemoveListeners()
    {
        foreach (var t in DirtyTeethList)
        {
            t.Cleaned -= CleanedTeeth;
        }

    }

    private void OnDisable()
    {
        // add back all of the teeth back to the teeth list
        DirtyTeethList.ForEach(x => TeethList.Add(x));
        DirtyTeethList.Clear();
        RemoveListeners();
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }

}
