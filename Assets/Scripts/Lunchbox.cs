using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Lunchbox : MonoBehaviour
{
    public void Dropping(BaseEventData eventData)
    {
        PointerEventData ped = eventData as PointerEventData;
        print(ped.selectedObject);

        if (ped.selectedObject != null && !(transform.childCount >= 5))
        {
            ped.selectedObject.transform.SetParent(transform, true);
            AdjustChildren();
        }
        else
        {
            ped.selectedObject.GetComponent<LunchItem>().ResetPos();
        }

    }

    private void AdjustChildren()
    {
        float pos = GetComponent<RectTransform>().rect.width / 5f;
        Vector2 startPos = GetComponent<RectTransform>().rect.min;
        startPos.x += 80;

        foreach (Transform child in transform)
        {
            child.localPosition = new Vector3 (startPos.x, 0, 0);
            startPos.x += pos;
        } 
    }
}
