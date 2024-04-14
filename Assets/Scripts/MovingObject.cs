using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{

    protected float speed = 0f;

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.left);

    }

    public virtual void UpdateSpeed(float newSpeed)
    {
        speed = newSpeed;
    } 


}
