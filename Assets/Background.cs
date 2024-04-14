using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private Material background;

    private Vector2 Offset;

    public float xVel, yVel;

    private void Awake()
    {
        background = GetComponent<Renderer>().material;
        
    }


    // Update is called once per frame
    void Update()
    {
        Offset = new Vector2(xVel, yVel);
        background.mainTextureOffset += Offset * Time.deltaTime;
    }
}
