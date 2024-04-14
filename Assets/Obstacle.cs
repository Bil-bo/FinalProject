using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    List<Sprite> ObstacleSprites = new List<Sprite>();

    private SpriteRenderer Sprite;

    private float OriginalY;
    private float OriginalX;

    private void Awake()
    {
        OriginalY = transform.localPosition.y;
        OriginalX = transform.localPosition.x;
    }

    private void Start()
    {
        transform.position = new Vector2(OriginalX, OriginalY);
        Sprite = GetComponent<SpriteRenderer>();
        Sprite.sprite = ObstacleSprites[Random.Range(0, ObstacleSprites.Count)]; 
    }

}
