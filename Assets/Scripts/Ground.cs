using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private Rigidbody2D rb;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  
        
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(-100, transform.position.y), 2 * Time.deltaTime); 
    }



}
