using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soap : MonoBehaviour
{

    [SerializeField]
    private Vector3 Velocity;

    [SerializeField]
    private int Score;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collision");
        if (other.CompareTag("Negative"))
        {
            EventManager.Broadcast(new ScoreEvent() { Points = Score });
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

        else if (other.CompareTag("Pass")) 
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collisionds");
        if (collision.gameObject.CompareTag("Pass"))
        {
            Destroy(gameObject);
        }
    }

}
