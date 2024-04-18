using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MonoBehaviour,
    IOnSceneChangingEvent, IOnSceneChangeEvent,
    IOnPauseEvent, IOnStrengthEvent

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

    private Camera cam;

    private Rigidbody2D rb;
    private Vector2 moveValue;
    private Vector3 minScreenBounds;
    private Vector3 maxScreenBounds;
    private Collider2D pBounds;
    private Animator Animator;
    private SceneIndex Scene;

    private bool Jumping = false;
    private bool Grounded = true;
    private bool Transitioning = false;

    public bool StrengthPowerUp { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        minScreenBounds = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        maxScreenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        rb = GetComponent<Rigidbody2D>();
        pBounds = GetComponent<Collider2D>();
        Animator = GetComponent<Animator>();

        EventManager.AddListener<SceneChangingEvent>(OnSceneChanging);
        EventManager.AddListener<SceneChangeEvent>(OnSceneChange);
        EventManager.AddListener<PauseEvent>(OnPauseEvent);
        EventManager.AddListener<StrengthEvent>(OnStrength);
    }

    private void OnJump()
    {
        if (IsGrounded()) {
            Jumping = true;
            Animator.SetBool("isJumping", Jumping);
            rb.AddForce(Vector2.up * JumpHeight, ForceMode2D.Impulse); 
        }

    }


    private void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    private void OnFallThrough()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer);
        if (hit.collider != null && hit.collider.CompareTag("Platform"))
        {
            StartCoroutine(hit.collider.gameObject.GetComponent<Platform>().FallThrough());
            Animator.SetTrigger("FallingThrough");
        }
    }


    private void FixedUpdate()
    {
        if (minScreenBounds != cam.ScreenToWorldPoint(Vector3.zero) || 
            maxScreenBounds != cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)))
        {
            minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
            maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        }

        if (Transitioning)
        {
            switch (Scene)
            {
                case SceneIndex.RUNNING:
                    rb.velocity = new Vector2(2 * speed, rb.velocity.y);
                    break;
            }

        }

        else
        {
            rb.velocity = new Vector2(
                ((minScreenBounds.x > pBounds.bounds.min.x && moveValue.x <= 0) ||
                (maxScreenBounds.x < pBounds.bounds.max.x && moveValue.x >= 0) ? 0 : moveValue.x * speed),
                rb.velocity.y);
        }

        if (IsGrounded())
        {
            if (Jumping && rb.velocity.y == 0) { 
                Jumping = false;
                Animator.SetBool("isJumping", Jumping);
            } 
        }

        else
        {
            Animator.SetFloat("pVelocity", rb.velocity.y);
        }

    }

    private bool IsGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
        {
            Grounded = true;
        }
        else { Grounded = false; }
        Animator.SetBool("isGrounded", Grounded);
        return Grounded;

    }

    public void OnSceneChanging(SceneChangingEvent eventData)
    {
        GetComponent<PlayerInput>().DeactivateInput();
        Transitioning = true;
    }

    public void OnPauseEvent(PauseEvent eventData)
    {
        if (eventData.Status)
        {
            GetComponent<PlayerInput>().DeactivateInput();
        } else
        {
            GetComponent<PlayerInput>().ActivateInput();
        }
    }


    public void OnSceneChange(SceneChangeEvent eventData)
    {
        GetComponent<PlayerInput>().ActivateInput();
        Transitioning = false;
        Scene = eventData.Stage;
    }

    public void OnStrength(StrengthEvent eventData) 
    {
        StartCoroutine(StrengthTimer(eventData.ActiveTime));

    }

    private IEnumerator StrengthTimer(float activeTime)
    {
        StrengthPowerUp = true;
        yield return new WaitForSeconds(activeTime);
        StrengthPowerUp = false; 
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);

    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<SceneChangingEvent>(OnSceneChanging);
        EventManager.RemoveListener<SceneChangeEvent>(OnSceneChange);
        EventManager.RemoveListener<PauseEvent>(OnPauseEvent);
        EventManager.AddListener<StrengthEvent>(OnStrength);
    }



}
