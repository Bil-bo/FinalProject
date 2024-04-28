using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// Represents a dirty tooth in the scene
public class Tooth : MonoBehaviour
{
    public bool IsDirty = false;
    private float CleanPercent = 100f;

    [SerializeField]
    private Image img;
    public Action<Tooth> Cleaned;

    private Vector2 pos;


    // Restarts potentially changed variables
    private void OnEnable()
    {
        CleanPercent = 100f;
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
    }

    public void CleaningStart(BaseEventData eventData)
    {
        if (IsDirty) 
        {
            PointerEventData ped = eventData as PointerEventData;
            pos = ped.position;

        }
    }

    // Dragging the mouse across the screen slowly cleans the tooth
    public void Cleaning(BaseEventData eventData)
    {
        if (IsDirty)
        {
            PointerEventData ped = eventData as PointerEventData;
            if (ped.position != pos) 
            {
                // Clean percent slowly goes down from 100 using the following algorith
                CleanPercent -= (Mathf.Abs(pos.x - ped.position.x) + Mathf.Abs(pos.y - ped.position.y)) / 500f;
                // This is then applied to the image to make it more opaque over time, until its fully disappeared
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f * (CleanPercent/100f));
                Debug.Log(CleanPercent.ToString());
                if (CleanPercent <= 0f) 
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
                    Debug.Log("Cleaned");
                    IsDirty = false;
                    Cleaned.Invoke(this);
                }

            }


        }
    }

}
