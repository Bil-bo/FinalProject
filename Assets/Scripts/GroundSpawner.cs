using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{

    [SerializeField] 
    public float TimeToFirstSpawn;

    [SerializeField]
    public GameObject Layouts;



    [SerializeField]
    public List<GameObject> LayoutList;

    private GameObject CurrentLayout;

    private List<GameObject> InstanceList = new List<GameObject>();
    private Queue<GameObject> HoldingList = new Queue<GameObject>();

    //private bool FirstSpawn = false;
    //private float timeUntilSpawn = 0;

    private void Start()
    {
        InstanceList = LayoutList;
        SpawnGround(-1);
    }

    private void SpawnGround(int index)
    {
        if (CurrentLayout != null) {
            Layout oldLayout = CurrentLayout.GetComponent<Layout>();
            oldLayout.InitPass -= (e) => SpawnGround(e); 
        }

        if (index == -1) {
            if (InstanceList.Count == 0)
            {
                InstanceList = HoldingList.ToList();
                HoldingList.Clear();
            }

            System.Random pos = new System.Random();
            Debug.Log(InstanceList.Count);
            CurrentLayout = InstanceList[pos.Next(InstanceList.Count)];
            Send(false);

        }

        else
        {
            CurrentLayout = LayoutList[index];
            Send(true);
        }


    }

    private void Send(bool queued)
    {
        if (!queued)
        {
            HoldingList.Enqueue(CurrentLayout);
            InstanceList.Remove(CurrentLayout);
            if (HoldingList.Count > 5)
            {
                InstanceList.Add(HoldingList.Dequeue());
            }
        }

        CurrentLayout = Instantiate(CurrentLayout, this.transform.position, Quaternion.identity, Layouts.transform);
        Layout layoutCode = CurrentLayout.GetComponent<Layout>();
        Debug.Log(layoutCode.test);

        layoutCode.InitPass += e => SpawnGround(e);

        CurrentLayout.SetActive(true);
    }


    private void OnDestroy()
    {
        if (CurrentLayout != null)
        {
            Layout layoutCode = CurrentLayout.GetComponent<Layout>();
            layoutCode.InitPass -= (e) => SpawnGround(e);
        }
    }


}
