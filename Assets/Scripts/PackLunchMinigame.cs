using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class PackLunchMinigame : Minigame
{

    [SerializeField]
    private GameObject LunchBox;

    private List<LunchItem> AllLunch = new List<LunchItem>();
    private List<LunchItem> ShownLunch = new List<LunchItem>();

    [SerializeField]    
    private Transform[] Positions = new Transform[5];

    private int HealthyNum = 0;

    public void OnEnable()
    {

        int pos = 0;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }


        AllLunch = GetComponentsInChildren<LunchItem>().ToList();

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

    void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
    }

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

    public void ValidateLunch()
    {
        bool unhealthy = false;
        LunchItem[] lunch = LunchBox.GetComponentsInChildren<LunchItem>();
        foreach (LunchItem item in lunch)
        {
            if (!item.IsHealthy)
            {
                Debug.Log("unhealthy");
                unhealthy = true;
                break;
            }

            else
            {
                HealthyNum--;
            }
        }

        completed.Invoke(!unhealthy && HealthyNum == 0);
    }

    private void OnDisable()
    {
        AllLunch.Clear();
        ShownLunch.Clear();
        HealthyNum = 0;
    }
}
