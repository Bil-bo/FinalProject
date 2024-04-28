using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Represents a moving object that the player can stand on or fall through
public class Platform : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D SolidGround;

    [SerializeField]
    private BoxCollider2D TriggerGround;

    private void Start()
    {
        SolidGround.size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, SolidGround.size.y);
        TriggerGround.size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, TriggerGround.size.y);
    }

    // Trigger ground positioned underneath Solid ground, to temporarily deactivate solid ground and allow player to pass through
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SolidGround.enabled = false;    

        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SolidGround.enabled = true;

        }
    }

    // Temporarily deactivates self to allow player to fall through
    public IEnumerator FallThrough()
    {
        SolidGround.enabled = false;
        TriggerGround.enabled = false;   
        yield return new WaitForSeconds(1f);
        try
        {
            SolidGround.enabled = true;
            TriggerGround.enabled = true;
        } catch (Exception e) { Debug.Log(e); }
    }

}
