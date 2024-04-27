using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutTooth : MonoBehaviour
{
    private bool Cleaned = false;

    [SerializeField]
    private Sprite HealthyTooth;

    private SpriteRenderer SR;

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !Cleaned)
        {
            Cleaned = true;
            SR.sprite = HealthyTooth;
            EventManager.Broadcast(new ScoreEvent() { Points = 10 });
        }
    }

    private void OnDestroy()
    {
        if (!Cleaned)
        {
            EventManager.Broadcast(new ScoreEvent() { Points = -10});
        }
    }
}
