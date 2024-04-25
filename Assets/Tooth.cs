using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooth : MonoBehaviour
{
    // Start is called before the first frame update
    public bool IsDirty = false;
    private float CleanPercent = 100f;
    public Action<Tooth> Cleaned;

    private Vector2 pos;


    private void OnEnable()
    {
        CleanPercent = 100f;
    }

    public void CleaningStart(BaseEventData eventData)
    {
        if (IsDirty) 
        {
            PointerEventData ped = eventData as PointerEventData;
            pos = ped.position;

        }
    }
    public void Cleaning(BaseEventData eventData)
    {
        if (IsDirty)
        {
            PointerEventData ped = eventData as PointerEventData;
            if (ped.position != pos) 
            {
                CleanPercent -= (Mathf.Abs(pos.x - ped.position.x) + Mathf.Abs(pos.y - ped.position.y)) / 1000f;
                Debug.Log(CleanPercent.ToString());
                if (CleanPercent <= 0f) 
                {
                    Debug.Log("Cleaned");
                    IsDirty = false;
                    Cleaned.Invoke(this);
                }

            }


        }
    }

}
