using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// Represents a food item within the lunch minigame
public class LunchItem : MonoBehaviour
{

    [SerializeField]
    public bool IsHealthy;

    [SerializeField]
    private RectTransform MiniGameBounds;

    private Vector2 BSize;
    private Vector3 BPos;

    private float BoundLeft;
    private float BoundRight;
    private float BoundTop;
    private float BoundBottom;

    public Transform initPos;

    private Image img;
    public Vector3 StartPos { get; private set; }

    private void Start()
    {
        StartPos = transform.localPosition;
        img = GetComponent<Image>();
        BSize = MiniGameBounds.rect.size;
        BPos = MiniGameBounds.position;

        BoundLeft = (BPos.x - BSize.x / 2f)  + 20;
        BoundRight = (BPos.x + BSize.x) / 2f;
        BoundTop = (BPos.y + BSize.y) / 2f;
        BoundBottom = (BPos.y - BSize.y / 2f) + 20;

    }

    // Turns of the raycast to allow for events to trigger on objects underneath
    public void DragStart()
    {
        img.raycastTarget = false;
        transform.SetParent(initPos, true);
    }

    // Moves the object around the screen
    public void Dragging(BaseEventData eventData)
    {
        eventData.selectedObject = gameObject;
        PointerEventData ped = eventData as PointerEventData;
        transform.position = ped.position;
    }

    // Reactivates the raycast to allow for future picking up
    public void DragEnd()
    {
        img.raycastTarget = true;
        Debug.Log("Dropping");

        Vector3 pos = transform.position;
        if (pos.x < BoundLeft || pos.x >= BoundRight ||
            pos.y < BoundBottom || pos.y >= BoundTop)
        {
            // Resets position back to marker if outside of bounds
            ResetPos();
        }
    }

    public void ResetPos()
    {
        transform.localPosition = StartPos;
    }

}
