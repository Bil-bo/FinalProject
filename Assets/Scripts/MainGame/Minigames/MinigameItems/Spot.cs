using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Represents a clickable spot in the find spots minigame
public class Spot : MonoBehaviour
{
    public bool found = false;
    public Action<Spot> Send;


    // Hides the object
    private void OnEnable()
    {
        found = false;
        Color C = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(C.r, C.g, C.b, 0);
    }


    // Once clicked send an event to the main minigame to confirm
    public void GotClicked()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Debug.Log("got clicked");
        if (!found)
        {
            Color C = GetComponent<Image>().color;
            GetComponent<Image>().color = new Color(C.r, C.g, C.b, 1);
            Debug.Log("First Find");
            found = true;
            Send.Invoke(this);

        }
    }
}
