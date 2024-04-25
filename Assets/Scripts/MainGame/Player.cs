using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Diagnostics;

public class Player : MonoBehaviour,
    IOnSceneChangingEvent, IOnSceneChangeEvent,
    IOnPauseEvent, IOnStrengthEvent, IOnGameOverEvent,
    IOnReviveEvent

{
    [SerializeField]
    public int JumpHeight;

    [SerializeField]
    private GameObject SoapShot;

    [SerializeField]
    public int speed;

    private int _speed;

    [SerializeField]
    public Vector2 boxSize;

    [SerializeField]
    public float castDistance;

    [SerializeField]
    public LayerMask groundLayer;

    private Camera cam;

    private Rigidbody2D rb;

    [SerializeField]
    private Vector2 moveValue;
    private Vector3 minScreenBounds;
    private Vector3 maxScreenBounds;
    private Collider2D pBounds;
    private Animator Animator;
    private SceneIndex Scene;
    private Vector2 VelSave;

    private bool Jumping = false;
    private bool Grounded = true;
    private bool Transitioning = false;
    private bool Paused = false;

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
        _speed = speed;

        EventManager.AddListener<SceneChangingEvent>(OnSceneChanging);
        EventManager.AddListener<SceneChangeEvent>(OnSceneChange);
        EventManager.AddListener<PauseEvent>(OnPauseEvent);
        EventManager.AddListener<StrengthEvent>(OnStrength);
        EventManager.AddListener<GameOverEvent>(OnGameOver);
        EventManager.AddListener<ReviveEvent>(OnRevive);
    }

    private void OnJump()
    {
        if (Scene == SceneIndex.FLYING)
        {
            Instantiate(SoapShot, transform.position, Quaternion.identity);
        }

        else if (IsGrounded()) {
            Jumping = true;
            Animator.SetBool("isJumping", Jumping);
            rb.AddForce(Vector2.up * JumpHeight, ForceMode2D.Impulse); 
        }

    }


    private void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
        if (Scene != SceneIndex.FLYING && (moveValue.x > 0 || moveValue.x < 0)) { moveValue.x = Mathf.RoundToInt(moveValue.x); }
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
                case SceneIndex.FLYING:
                    rb.velocity = new Vector2(0, 2 * speed); 
                    break;
            }

        }

        else
        {
            rb.velocity = new Vector2(
                ((minScreenBounds.x > pBounds.bounds.min.x && moveValue.x <= 0) ||
                (maxScreenBounds.x < pBounds.bounds.max.x && moveValue.x >= 0) ? 0 : moveValue.x * _speed),
                (Scene == SceneIndex.FLYING) ? CheckFlyingBounds() : rb.velocity.y);
            Animator.SetFloat("pVelocityX", moveValue.x);

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
            Animator.SetFloat("pVelocityY", rb.velocity.y);
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

    private float CheckFlyingBounds()
    {
       return ((minScreenBounds.y > pBounds.bounds.min.y && moveValue.y <= 0) ||
                (maxScreenBounds.y < pBounds.bounds.max.y && moveValue.y >= 0)) ? 0 : moveValue.y * _speed;

    }

    public void OnSceneChanging(SceneChangingEvent eventData)
    {
        GetComponent<PlayerInput>().DeactivateInput();
        Transitioning = true;
    }

    public void OnPauseEvent(PauseEvent eventData)
    {
        Paused = eventData.Status;
        if (eventData.Status)
        {
            VelSave = rb.velocity;
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            GetComponent<PlayerInput>().DeactivateInput();
        } else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.WakeUp();
            rb.velocity = VelSave;
            GetComponent<PlayerInput>().ActivateInput();
        }
    }


    public void OnSceneChange(SceneChangeEvent eventData)
    {
        if (Scene == SceneIndex.FLYING) { rb.gravityScale = 5; }
        GetComponent<PlayerInput>().ActivateInput();
        Transitioning = false;
        Scene = eventData.Stage;
        Animator.SetInteger("SceneIndex", (int)Scene);
        Animator.SetTrigger("Transition");

        switch (Scene) {
            case SceneIndex.FLYING:
                rb.gravityScale = 0;
                _speed = speed * 2;
                break;
            case SceneIndex.FALLING:
                _speed = speed * 3;
                break;
            default:
                _speed = speed; 
                break;
        }

        pBounds.enabled = true;
        rb.velocity = Vector2.zero; 
    }

    public void OnStrength(StrengthEvent eventData) 
    {
        StartCoroutine(StrengthTimer(eventData.ActiveTime));
    }

    private IEnumerator StrengthTimer(float activeTime)
    {
        StrengthPowerUp = true;

        while (activeTime > 0)
        {
            if (!Paused)
            {
                activeTime -= 0.1f;
            }

            yield return new WaitForSeconds(0.1f);
        }
        StrengthPowerUp = false; 

    }

    public void OnGameOver(GameOverEvent eventData)
    {
        GetComponent<PlayerInput>().DeactivateInput();
        pBounds.enabled = false;
        if (StrengthPowerUp) { StopAllCoroutines(); StrengthPowerUp = false; }
        rb.AddForce(Vector2.up * JumpHeight, ForceMode2D.Impulse);
    }

    public void OnRevive(ReviveEvent eventData) 
    {
        GetComponent<PlayerInput>().ActivateInput();
        pBounds.enabled = true;
        rb.velocity = Vector2.zero;
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
        EventManager.RemoveListener<GameOverEvent>(OnGameOver);
        EventManager.RemoveListener<ReviveEvent>(OnRevive);
    }



}
