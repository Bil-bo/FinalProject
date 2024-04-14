using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField]
    public BoxCollider2D SolidGround;

    [SerializeField]
    public BoxCollider2D TriggerGround;

    private void Start()
    {
        SolidGround.size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, SolidGround.size.y);
        TriggerGround.size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, TriggerGround.size.y);
    }


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

    public IEnumerator FallThrough()
    {
        SolidGround.enabled = false;
        TriggerGround.enabled = false;   
        yield return new WaitForSeconds(1f);
        SolidGround.enabled = true;
        TriggerGround.enabled = true;
    }

}
