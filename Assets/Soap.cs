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

    private void OnTriggerExit(Collider other)
    {
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

}
