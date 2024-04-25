using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class LayoutSpawner : MonoBehaviour, 
    IOnSceneChangeEvent, IOnDistanceMilestoneEvent,
    IOnReviveEvent
{
    [SerializeField]
    public GameObject Layouts;

    [SerializeField]
    public SceneIndex Scene;

    [SerializeField]
    private GameObject[] TriggerPoints = new GameObject[2];


    [SerializeField]
    public List<GameObject> LayoutList;

    private GameObject CurrentLayout;
    private bool Activated = false;

    [SerializeField]
    private List<GameObject> TransitionList = new List<GameObject>();

    [SerializeField]
    private List<GameObject> InstanceList = new List<GameObject>();

    private Queue<GameObject> HoldingList = new Queue<GameObject>();

    [SerializeField]
    private List<GameObject> PowerUpList = new List<GameObject>();

    [SerializeField]
    private Vector2 Direction;



    private void Start()
    {
        EventManager.AddListener<SceneChangeEvent>(OnSceneChange);
        EventManager.AddListener<DistanceMilestoneEvent>(OnDistanceMilestone);
        EventManager.AddListener<ReviveEvent>(OnRevive);

        if (GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().Scene == Scene)
        {
            OnEnable();
        }
    }

    private void OnEnable()
    {
        foreach (GameObject t in TriggerPoints ) { t.SetActive(true); }
        RemoveObstacles();
        Activated = true;
        SpawnGround(-1);

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
                    if (HoldingList.Count == 0)
                    {
                        InstanceList.AddRange(LayoutList);
                    }
                    else
                    {
                        InstanceList.AddRange(HoldingList.ToList());
                        HoldingList.Clear();
                    }
                }

                try
                {
                    CurrentLayout = InstanceList[Random.Range(0, InstanceList.Count)];
                    Send(false);
                } catch (Exception) { InstanceList.AddRange(LayoutList); }
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
        layoutCode.UpdateDirection(Direction);

        layoutCode.InitPass += e => SpawnGround(e);

        CurrentLayout.SetActive(true);

        if (Random.value >= 0.9)
        {
            SpawnPowerUp();
        }

    }

    private void SpawnPowerUp() 
    {
        GameObject powerUp = Instantiate(PowerUpList[Random.Range(0, PowerUpList.Count)], this.transform.position + (Vector3.up * 9), 
            Quaternion.identity, Layouts.transform);

        PowerUp powData = powerUp.GetComponent<PowerUp>();

        float magnitude = Random.Range(1f, 10f);

        float frequency = Random.Range(1f, 6f);

        float speed = Random.Range(1f, 6f);

        powData.Send(Direction, speed, magnitude, frequency);

    }

    private void RemoveObstacles()
    {
        foreach (Transform child in Layouts.transform)
        {
            Destroy(child.gameObject);

        }
    }

    private IEnumerator PrepareSwitch()
    {
        Debug.Log("Triggered");
        while (Layouts.transform.childCount > 1)
        {
            yield return null;  
        }
        foreach (GameObject t in TriggerPoints) { t.SetActive(false); }
        EventManager.Broadcast(new SceneChangingEvent());

    }


    public void OnSceneChange(SceneChangeEvent eventData)
    {
        OnDisable();
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
            if (HoldingList.Count == 0) { SpawnGround(-1); }
        }
    }

    public void OnRevive(ReviveEvent eventData) 
    {
        GameObject[] bad = GameObject.FindGameObjectsWithTag("Negative");
        foreach (GameObject b in bad) 
        {
            b.SetActive(false);
        }
    }

    private void OnDisable()
    {
        RemoveObstacles();
        InstanceList.AddRange(HoldingList.ToList());
        HoldingList.Clear();
        
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<SceneChangeEvent>(OnSceneChange);
        EventManager.RemoveListener<DistanceMilestoneEvent>(OnDistanceMilestone);
        EventManager.RemoveListener<ReviveEvent>(OnRevive);

        if (CurrentLayout != null)
        {
            Layout layoutCode = CurrentLayout.GetComponent<Layout>();
            layoutCode.InitPass -= (e) => SpawnGround(e);
        }
    }


}
