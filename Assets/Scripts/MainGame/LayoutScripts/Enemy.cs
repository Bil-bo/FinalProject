using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            Player player = collision.GetComponent<Player>();
            if (player != null && player.StrengthPowerUp)
            {
                Destroy(gameObject);
            }
            else
            {
                EventManager.Broadcast(new GameOverEvent());
            }
        }
    }
}
