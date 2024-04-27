using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PackHealthy : MonoBehaviour
{

    private Image img;
    public Vector3 StartPos { get; private set; }
    private void Start()
    {
        StartPos = transform.localPosition;
        img = GetComponent<Image>();

    }
    public void DragStart()
    {
        img.raycastTarget = false;

    }
    public void Dragging(BaseEventData eventData)
    {
        eventData.selectedObject = gameObject;
        PointerEventData ped = eventData as PointerEventData;
        transform.position = ped.position;   
    }

    public void DragEnd()
    {
        img.raycastTarget = true;
        Debug.Log("Dropping");

    }



}
