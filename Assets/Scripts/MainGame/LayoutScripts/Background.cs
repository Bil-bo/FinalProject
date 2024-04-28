using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For moving textures in accordance with the changing speed
// Has a velocity offset to account for the faster scroll speed and to create parallax effect
public class Background : MovingObject
{
    private Material background;

    private Vector2 Offset;

    public float XVel = 0.1f;



    private void Awake()
    {
        background = GetComponent<Renderer>().material;

}


    // Update is called once per frame
    protected override void Update()
    {
        Offset = XVel * Speed * Time.deltaTime * Direction;
        background.mainTextureOffset += Offset;
    }

    // Direction in opposite to norm
    public override void UpdateDirection(Vector2 newDirection)
    {
        Direction = -newDirection;

    }
}
