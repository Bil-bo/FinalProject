using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderData;

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

        else if (collision.CompareTag("Pass") && pass)
        {
            pass = false;
            Destroy(gameObject);
        }
    }

    protected abstract void ActivatePowerUp();

    protected override void Update()
    {
        if (Speed != 0f)
        {
            pos += Speed * Time.deltaTime * DirectionVec;
            transform.position = pos + transform.up * Mathf.Sin(Time.time * Frequency) * Magnitude;
        }
    }

    public void Send(Vector2 direction, float speed, float magnitude, float frequency)
    {
        DirectionVec = direction;
        transform.rotation = Quaternion.Euler(0, 0, 90 * direction.y);

        Speed = InitSpeed = speed;
        Magnitude = magnitude;
        Frequency = frequency;
    }

    public override void UpdateDirection(Vector2 newDirection) { }
    public override void UpdateSpeed(float newSpeed)
    {
        if (newSpeed == 0)
        {
            Speed = newSpeed;
        } 

        else { Speed = InitSpeed; }
    }
}
