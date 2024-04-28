using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


// Controls spaswning layouts in the currently active scene
public class LayoutManager : MonoBehaviour, 
    IOnSceneChangeEvent, IOnDistanceMilestoneEvent,
    IOnReviveEvent
{
    [SerializeField]
    public GameObject Layouts;

    [SerializeField]
    public SceneIndex Scene;

    // The trigger points for layouts in the current scene
    [SerializeField]
    private GameObject[] TriggerPoints = new GameObject[2];

    // Holds the uninstantiated prefab layouts
    [SerializeField]
    public List<GameObject> LayoutList;

    // Stores the layout currently moving towards the first trigger point
    private GameObject CurrentLayout;
    private bool Activated = false;

    // Holds the uninstantiated switch layouts
    [SerializeField]
    private List<GameObject> TransitionList = new List<GameObject>();

    // Holds a copy of layout list that will get changed 
    [SerializeField]
    private List<GameObject> InstanceList = new List<GameObject>();


    // Holds a list of x layouts that have been instantiated recently
    private Queue<GameObject> HoldingList = new Queue<GameObject>();

    // Holds the power-ups to be instantiated
    [SerializeField]
    private List<GameObject> PowerUpList = new List<GameObject>();

    // Holds the direction to send powerups and layouts in
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

    // Spawns the initial layout and reactivates the trigger points
    private void OnEnable()
    {
        foreach (GameObject t in TriggerPoints ) { t.SetActive(true); }
        RemoveObstacles();
        Activated = true;
        SpawnGround(-1);

    }
   

    // For spawning a new layout
    private void SpawnGround(int index)
    {
        // Check if the layout manager should actually spawn something
        if (Activated)
        {
            // Unsubscribe from the previous layouts trigger event, to prepare for the next layout
            if (CurrentLayout != null)
            {
                Layout oldLayout = CurrentLayout.GetComponent<Layout>();
                oldLayout.InitPass -= (e) => SpawnGround(e);
            }

            // Random Index
            if (index == -1)
            {
                // Check if there are any layouts to instantiate
                if (InstanceList.Count == 0)
                {
                    // Check if the holding list has any layouts
                    if (HoldingList.Count == 0)
                    {
                        // If not, add the layout list to the instance list
                        InstanceList.AddRange(LayoutList);
                    }
                    else
                    {
                        // Otherwise, re-add the contents of the holding list back to the instance list
                        InstanceList.AddRange(HoldingList.ToList());
                        HoldingList.Clear();
                    }
                }

                // Try catch for similar problem to above
                try
                {
                    // Randomly get a layout from the instance list
                    CurrentLayout = InstanceList[Random.Range(0, InstanceList.Count)];
                    Send(false);
                } catch (Exception) { InstanceList.AddRange(LayoutList); }
            }

            // Switch layout index
            // prepares to switch to new scene
            else if (index == -2)
            {
                StartCoroutine(PrepareSwitch());
            }

            // Selected index
            // For when layouts want a specific layout to appear behind them
            // Used for larger layouts, for example
            else
            {
                CurrentLayout = LayoutList[index];
                Send(true);
            }
        }
    }


    // For instantiating and moving layouts into the scene
    private void Send(bool queued)
    {
        // Queued layouts come from the Layout list, not the instance list, so should not be added into the holding list
        // or back into the instanceList
        if (!queued)
        {
            HoldingList.Enqueue(CurrentLayout);
            InstanceList.Remove(CurrentLayout);
            // re-adds a layout to the pool once enough layouts have passed
            if (HoldingList.Count >= 5)
            {
                InstanceList.Add(HoldingList.Dequeue());
            }
        }

        
        CurrentLayout = Instantiate(CurrentLayout, this.transform.position, Quaternion.identity, Layouts.transform);
        Layout layoutCode = CurrentLayout.GetComponent<Layout>();
        layoutCode.UpdateDirection(Direction); // Update direction to match spawner direction

        layoutCode.InitPass += e => SpawnGround(e); // Attach to event that checks for initial trigger point pass

        CurrentLayout.SetActive(true);

        // 1/10 chance to spawn a power up
        if (Random.value >= 0.9)
        {
            SpawnPowerUp();
        }

    }

    // Creates a power up with random sinusoidal movement
    private void SpawnPowerUp() 
    {
        GameObject powerUp = Instantiate(PowerUpList[Random.Range(0, PowerUpList.Count)], this.transform.position + (Vector3.up * 9), 
            Quaternion.identity, Layouts.transform);

        PowerUp powData = powerUp.GetComponent<PowerUp>();

        float magnitude = Random.Range(1f, 10f);

        float frequency = Random.Range(1f, 6f);

        float speed = Random.Range(1f, 6f);

        // Activates the movement
        powData.Send(Direction, speed, magnitude, frequency);

    }

    // Deletes all remaining layouts in the scene
    private void RemoveObstacles()
    {
        foreach (Transform child in Layouts.transform)
        {
            Destroy(child.gameObject);

        }
    }

    // Coroutine waiting for all layouts apart from the switchLayout to be deleted
    private IEnumerator PrepareSwitch()
    {
        while (Layouts.transform.childCount > 1)
        {
            yield return null;  
        }
        // Deactivates the trigger points to allow the player free movement
        // Broadcasts a scene changing event to begin transition
        foreach (GameObject t in TriggerPoints) { t.SetActive(false); }
        EventManager.Broadcast(new SceneChangingEvent());
    }

    // Scene change event listener
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


    // Adds the transition layouts to the pool
    public void OnDistanceMilestone(DistanceMilestoneEvent eventData)
    {
        if (Activated)
        {
            InstanceList.AddRange(TransitionList);
            if (HoldingList.Count == 0) { SpawnGround(-1); }
        }
    }

    // Removes all harmful objects from the layouts
    // To prevent an instant game over due to positioning
    public void OnRevive(ReviveEvent eventData) 
    {
        GameObject[] bad = GameObject.FindGameObjectsWithTag("Negative");
        foreach (GameObject b in bad) 
        {
            b.SetActive(false);
        }
    }

    // Clears out the holding list back into the instance list
    private void OnDisable()
    {
        RemoveObstacles();
        InstanceList.AddRange(HoldingList.ToList());
        HoldingList.Clear();
        
    }

    // Removes all listeners
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
