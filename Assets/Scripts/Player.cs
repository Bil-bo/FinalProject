using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MonoBehaviour

{
    [SerializeField]
    public int JumpHeight;

    [SerializeField]
    public int speed;

    [SerializeField]
    public Vector2 boxSize;

    [SerializeField]
    public float castDistance;

    [SerializeField]
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveValue;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnJump()
    {
        if (isGrounded()) { rb.AddForce(Vector2.up * JumpHeight, ForceMode2D.Impulse); }

    }

    private void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveValue.x * speed, rb.velocity.y);
    }

    private bool isGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
        {
            return true;
        }
        else { return false; }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);

    }





    // Update is called once per frame
    void Update()
    {
        
    }
}
