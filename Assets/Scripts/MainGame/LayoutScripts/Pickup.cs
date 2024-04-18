using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    private int Points;

    [SerializeField]
    private List<Sprite> PickupSprites = new List<Sprite>();

    private SpriteRenderer Sprite;

    private void Start()
    {
        Sprite = GetComponent<SpriteRenderer>();
        Sprite.sprite = PickupSprites[Random.Range(0, PickupSprites.Count)];
    }


    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ScoreEvent evt = new ScoreEvent() { Points = Points };
            EventManager.Broadcast(evt);
            gameObject.SetActive(false);
        }
    }

}
