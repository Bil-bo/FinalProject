using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Superclass representing all moving objects
public abstract class MovingObject : MonoBehaviour
{

    protected float Speed = 0f;

    [SerializeField]
    protected Vector2 Direction = Vector2.left;

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.Translate(Speed * Time.deltaTime * Direction);

    }

    // For speeding up or stopping all moving objects
    public virtual void UpdateSpeed(float newSpeed)
    {
        Speed = newSpeed;
    } 

    public virtual void UpdateDirection(Vector2 newDirection)
    {
        Direction = newDirection;   
    }


}
