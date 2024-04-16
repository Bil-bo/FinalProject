using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GroundSpawner : MonoBehaviour, IOnSceneChangeEvent, IOnDistanceMilestoneEvent
{
    [SerializeField]
    public GameObject Layouts;

    [SerializeField]
    public SceneIndex Scene;


    [SerializeField]
    public List<GameObject> LayoutList;

    private GameObject CurrentLayout;
    private bool Activated = false;

    [SerializeField]
    private List<GameObject> TransitionList = new List<GameObject>();

    private List<GameObject> InstanceList = new List<GameObject>();
    private Queue<GameObject> HoldingList = new Queue<GameObject>();

    //private bool FirstSpawn = false;
    //private float timeUntilSpawn = 0;

    private void Start()
    {
        EventManager.AddListener<SceneChangeEvent>(OnSceneChange);
        EventManager.AddListener<DistanceMilestoneEvent>(OnDistanceMilestone);

        InstanceList = LayoutList;
        if (GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().Scene == Scene)
        {
            Activated = true;
        }

        SpawnGround(-1);
    }

    private void OnEnable()
    {
        InstanceList = LayoutList;
    }

    private void SpawnGround(int index)
    {
        if (Activated)
        {
            if (CurrentLayout != null)
            {
                Layout oldLayout = CurrentLayout.GetComponent<Layout>();
                oldLayout.InitPass -= (e) => SpawnGround(e);
            }

            if (index == -1)
            {
                if (InstanceList.Count == 0)
                {
                    InstanceList = HoldingList.ToList();
                    HoldingList.Clear();
                }

                System.Random pos = new System.Random();
                try
                {
                    CurrentLayout = InstanceList[pos.Next(InstanceList.Count)];
                    Send(false);
                } catch (Exception e) { Debug.LogError(e); }
            }

            else if (index == -2)
            {
                StartCoroutine(PrepareSwitch());
            }

            else
            {
                CurrentLayout = LayoutList[index];
                Send(true);
            }
        }


    }

    private void Send(bool queued)
    {
        if (!queued)
        {
            HoldingList.Enqueue(CurrentLayout);
            InstanceList.Remove(CurrentLayout);
            if (HoldingList.Count >= 5)
            {
                InstanceList.Add(HoldingList.Dequeue());
            }
        }

        CurrentLayout = Instantiate(CurrentLayout, this.transform.position, Quaternion.identity, Layouts.transform);
        Layout layoutCode = CurrentLayout.GetComponent<Layout>();

        layoutCode.InitPass += e => SpawnGround(e);

        CurrentLayout.SetActive(true);
    }

    private void RemoveObstacles()
    {
        foreach (Transform child in Layouts.transform)
        {
            if (!child.CompareTag("Switch"))
            {
                Destroy(child.gameObject);
            }

        }
    }

    private IEnumerator PrepareSwitch()
    {
        while (Layouts.transform.childCount > 1)
        {
            yield return null;  
        }
        EventManager.Broadcast(new SceneChangingEvent());

    }


    public void OnSceneChange(SceneChangeEvent eventData)
    {
        if (eventData.Stage == Scene)
        {
            Activated = true;
            SpawnGround(-1);
        }

        else
        {
            Activated = false;
        }
    }

    public void OnDistanceMilestone(DistanceMilestoneEvent eventData)
    {
        if (Activated)
        {
            InstanceList.AddRange(TransitionList);
        }
    }

    private void OnDisable()
    {
        InstanceList.Clear();
        HoldingList.Clear();
        
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<SceneChangeEvent>(OnSceneChange);
        EventManager.RemoveListener<DistanceMilestoneEvent>(OnDistanceMilestone);

        if (CurrentLayout != null)
        {
            Layout layoutCode = CurrentLayout.GetComponent<Layout>();
            layoutCode.InitPass -= (e) => SpawnGround(e);
        }
    }


}
