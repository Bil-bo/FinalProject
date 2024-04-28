using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


// Represents something the player can collide with to change their score
public class Pickup : MonoBehaviour
{
    [SerializeField]
    private int Points;

    [SerializeField]
    private AudioClip GoodSound;

    [SerializeField] 
    private AudioClip BadSound;

    [SerializeField]
    private List<Sprite> PickupSprites = new List<Sprite>();

    private SpriteRenderer Sprite;

    // Pick a random sprite from the list of sprites on instantiation
    private void Start()
    {
        Sprite = GetComponent<SpriteRenderer>();
        Sprite.sprite = PickupSprites[Random.Range(0, PickupSprites.Count)];
    }

    // Call score event on collision with player
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            ScoreEvent evt = new ScoreEvent() { Points = Points };
            EventManager.Broadcast(evt);
            SoundManager.instance.PlayClip((Points < 0) ? BadSound : GoodSound, transform, 1f);

            gameObject.SetActive(false);
        }
    }

}
