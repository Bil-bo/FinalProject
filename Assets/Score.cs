using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField]
    List<Sprite> ScoreSprites = new List<Sprite>();

    [SerializeField]
    public int Points;

    private SpriteRenderer Sprite;
    

    private void Start()
    { 
        Sprite = GetComponent<SpriteRenderer>();
        Sprite.sprite = ScoreSprites[Random.Range(0, ScoreSprites.Count)];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ScoreEvent evt = new ScoreEvent() {Points = Points };
            EventManager.Broadcast(evt);
            gameObject.SetActive(false);
        }
    }



}
