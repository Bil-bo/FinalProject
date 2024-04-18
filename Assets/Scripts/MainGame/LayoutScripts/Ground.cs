using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ground : MovingObject
{
    private Material Floor;

    private Vector2 Offset;

    public float XVel = 0.1f;

    private void Awake()
    {
        Floor = GetComponent<Renderer>().material;
    }

    protected override void Update()
    {
        Offset = XVel * Speed * Time.deltaTime * Vector2.right;
        Floor.mainTextureOffset += Offset;
    }

}
