using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Projectile class for the flying scene
public class Soap : MonoBehaviour
{

    [SerializeField]
    private Vector3 Velocity;

    [SerializeField]
    private int Score;


    void Update()
    {
        transform.Translate(Velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Hitting an enemy destroys the enemy, the projectile, and causes a score event
        if (other.CompareTag("Negative"))
        {
            EventManager.Broadcast(new ScoreEvent() { Points = Score });
            Destroy(other.gameObject);
            Destroy(transform.parent.gameObject);
        }

        // Hitting the top boundary causes the projectile to be destroyed
        else if (other.CompareTag("Pass")) 
        {
            Destroy(transform.parent.gameObject);
        }
    }

}
