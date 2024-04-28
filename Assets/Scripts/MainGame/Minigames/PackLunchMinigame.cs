using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

// Overarching class for the packed lunch minigame
public class PackLunchMinigame : Minigame
{

    [SerializeField]
    private GameObject LunchBox;

    private List<LunchItem> AllLunch = new List<LunchItem>();
    private List<LunchItem> ShownLunch = new List<LunchItem>();

    [SerializeField]    
    private Transform[] Positions = new Transform[5];

    private int HealthyNum = 0;


    // Initial set up
    public void OnEnable()
    {

        int pos = 0;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        // Get all lunch items
        AllLunch = GetComponentsInChildren<LunchItem>().ToList();

        // Get 5 random items
        while (AllLunch.Count > 0 && ShownLunch.Count < 5) 
        {
            int rand = Random.Range(0, AllLunch.Count);
            if (AllLunch[rand].IsHealthy) { HealthyNum++; }
            ShownLunch.Add(AllLunch[rand]);
            AllLunch.RemoveAt(rand);
            ShownLunch[ShownLunch.Count-1].transform.SetParent(Positions[pos], true);
            ShownLunch[ShownLunch.Count - 1].initPos = Positions[pos];
            ShownLunch[ShownLunch.Count - 1].transform.localPosition = Vector3.zero;
            pos++;
        }

        foreach (LunchItem lunch in AllLunch)
        {
            lunch.transform.SetParent(transform);
            lunch.gameObject.SetActive(false);
        }

    }

    // For if the currently selected lunch item is dropped outside of the lunchbox
    public void Dropping(BaseEventData eventData)
    {
        PointerEventData ped = eventData as PointerEventData;
        print(ped.selectedObject);

        if (ped.selectedObject != null)
        {
            LunchItem selected = ped.selectedObject.GetComponent<LunchItem>();
            selected.transform.SetParent(selected.initPos, true);
            ped.selectedObject.transform.localPosition = Vector3.zero;
        }
    }


    // Checking the contents of the lunchbox
    public void ValidateLunch()
    {
        bool unhealthy = false;
        // get the contents of the lunchbox
        LunchItem[] lunch = LunchBox.GetComponentsInChildren<LunchItem>();
        foreach (LunchItem item in lunch)
        {
            // Check if there's an unhealthy item in the lunchbox
            if (!item.IsHealthy)
            {
                // instant loss
                Debug.Log("unhealthy");
                unhealthy = true;
                break;
            }

            // Checking if all healthy items are in the lunchbox
            else
            {
                HealthyNum--;
            }
        }
        // minigame is completed successfully if all healthy items are present and no unhealthy items are present
        completed.Invoke(!unhealthy && HealthyNum == 0);
    }

    // reset the minigame
    private void OnDisable()
    {
        AllLunch.Clear();
        ShownLunch.Clear();
        HealthyNum = 0;
    }
}
