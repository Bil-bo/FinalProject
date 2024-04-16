using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundScroller : MovingObject
{
    [SerializeField]
    public Tilemap groundTilemap;

    private float TileSize = 1f;
    private float ScrollOffset = 0f;


    protected override void Update()
    {
        ScrollOffset = Time.deltaTime * Speed;


        if (ScrollOffset >= TileSize)
            UpdateGround();
        ScrollOffset -= TileSize;
    }

    private void UpdateGround()
    {
        // Shift ground tiles to the left
        Vector3Int startPos = groundTilemap.origin + new Vector3Int(1, 0, 0);
        groundTilemap.SetTilesBlock(groundTilemap.cellBounds, groundTilemap.GetTilesBlock(groundTilemap.cellBounds));

        // Randomly select a new tile for the rightmost position
        //Vector3Int newPos = startPos + new Vector3Int(groundTilemap.size.x - 1, 0, 0);
        //groundTilemap.SetTile(newPos, groundTiles[Random.Range(0, groundTiles.Length)]);
    }
}
