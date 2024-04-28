using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Abstract class representing a power up on the screen
public abstract class PowerUp : MovingObject
{
    private float InitSpeed = 3f;

    [SerializeField]
    private Vector3 DirectionVec = Vector3.left;

    [SerializeField]
    private float Magnitude = 3f;

    [SerializeField]
    private float Frequency = 3f;


    [SerializeField]
    protected float activeTime;

    private bool pass = false;

    private Vector3 pos;

    protected void Start()
    {
        pos = transform.position;
    }

    // Checking for collision between it and the player
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log(activeTime);
            ActivatePowerUp();
        }

        else if (collision.CompareTag("Pass") && !pass)
        {
            pass = true;
        }

        // Deletes self on collision with second trigger
        else if (collision.CompareTag("Pass") && pass)
        {
            pass = false;
            Destroy(gameObject);
        }
    }

    // inherited method for what to do on a collision, varies between power-ups
    protected abstract void ActivatePowerUp();

    protected override void Update()
    {
        // Overrides typical movement to generate sinusoidal motion
        if (Speed != 0f)
        {
            pos += Speed * Time.deltaTime * DirectionVec;
            transform.position = pos + transform.up * Mathf.Sin(Time.time * Frequency) * Magnitude;
        }
    }

    // Sets up the variables for movement
    public void Send(Vector2 direction, float speed, float magnitude, float frequency)
    {
        DirectionVec = direction;
        transform.rotation = Quaternion.Euler(0, 0, 90 * direction.y);

        Speed = InitSpeed = speed;
        Magnitude = magnitude;
        Frequency = frequency;
    }

    // Direction doesn't update
    public override void UpdateDirection(Vector2 newDirection) { }

    // speed doesn't change unless the intent is to freeze its motion
    public override void UpdateSpeed(float newSpeed)
    {
        if (newSpeed == 0)
        {
            Speed = newSpeed;
        } 

        else { Speed = InitSpeed; }
    }
}
