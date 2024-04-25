using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Lunchbox : MonoBehaviour
{
    public void Dropping(BaseEventData eventData)
    {
        PointerEventData ped = eventData as PointerEventData;
        print(ped.selectedObject);

        if (ped.selectedObject != null)
        {
            ped.selectedObject.transform.SetParent(transform, false);
            AdjustChildren();
        }

    }

    private void AdjustChildren()
    {
        int pos = 0;

        foreach (Transform child in transform)
        {
            child.localPosition = new Vector3 (pos, 0, 0);
            pos += 10;
        } 
    }
}
