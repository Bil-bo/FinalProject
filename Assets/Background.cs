using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MovingObject
{
    private Material background;

    private Vector2 Offset;

    public float XVel = 0.1f;



    private void Awake()
    {
        background = GetComponent<Renderer>().material;
        Direction = Vector2.right;

}


    // Update is called once per frame
    protected override void Update()
    {
        Offset = XVel * Speed * Time.deltaTime * Direction;
        background.mainTextureOffset += Offset;
    }

    public override void UpdateDirection(Vector2 newDirection)
    {
        Direction = -newDirection;

    }
}
