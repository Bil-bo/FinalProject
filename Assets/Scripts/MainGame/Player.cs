using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine;


// Main controller class of the game
public class Player : MonoBehaviour,
    IOnSceneChangingEvent, IOnSceneChangeEvent,
    IOnPauseEvent, IOnStrengthEvent, IOnGameOverEvent,
    IOnReviveEvent

{
    [SerializeField]
    public int JumpHeight;

    // Set speed
    [SerializeField]
    private int speed;

    // Changeable speed
    private int Cspeed;

    [SerializeField]
    public Vector2 boxSize;

    [SerializeField]
    public float castDistance;

    [SerializeField]
    public LayerMask groundLayer;

    [SerializeField]
    private GameObject SoapShot;

    private Camera cam;

    private Rigidbody2D rb;

    [SerializeField]
    private Vector2 moveValue;
    private Vector3 minScreenBounds;
    private Vector3 maxScreenBounds;
    private Collider2D pBounds;
    public Animator Animator;
    private SceneIndex Scene;
    private Vector2 VelSave;
   

    private bool Jumping = false;
    private bool Grounded = true;
    private bool Transitioning = false;
    private bool Paused = false;

    public bool StrengthPowerUp { get; private set; } = false;


    [SerializeField]
    private AudioClip JumpSound;

    [SerializeField]
    private AudioClip ShootSound;
    

    // Set up of important variables
    void Start()
    {

        // Camera bounds to stop player moving too far on screen
        // Extra bounds for larger screen size set up in scene
        cam = Camera.main;
        minScreenBounds = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        maxScreenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        rb = GetComponent<Rigidbody2D>();
        pBounds = GetComponent<Collider2D>();
        Animator = GetComponent<Animator>();
        Cspeed = speed;

        EventManager.AddListener<SceneChangingEvent>(OnSceneChanging);
        EventManager.AddListener<SceneChangeEvent>(OnSceneChange);
        EventManager.AddListener<PauseEvent>(OnPauseEvent);
        EventManager.AddListener<StrengthEvent>(OnStrength);
        EventManager.AddListener<GameOverEvent>(OnGameOver);
        EventManager.AddListener<ReviveEvent>(OnRevive);
    }


    // Input Action for SpaceBar action
    private void OnJump()
    {
        // Changes to fire a projectile in the flying scene
        if (Scene == SceneIndex.FLYING)
        {
            Instantiate(SoapShot, transform.position, Quaternion.identity);
            SoundManager.instance.PlayClip(ShootSound, transform, 1f);
        }

        // Otherwise if the player is grounded, jump in a quadratic arc
        else if (IsGrounded()) {
            Jumping = true;
            SoundManager.instance.PlayClip(JumpSound, transform, 1f);
            Animator.SetBool("isJumping", Jumping);
            rb.AddForce(Vector2.up * JumpHeight, ForceMode2D.Impulse); 
        }

    }

    // Input Action for movement
    private void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();

        // Onflying, up and down are active, which changes the x value slightly. So when not in flying, the diagonal direction is effectively redundant
        if (Scene != SceneIndex.FLYING && (moveValue.x > 0 || moveValue.x < 0)) { moveValue.x = Mathf.RoundToInt(moveValue.x); }
    }


    // Input action bound to the down direction
    // Only does anything if the player is standing on something they can fall through,
    // avoiding errors in other scenes which make different use of the direction
    private void OnFallThrough()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer);
        if (hit.collider != null && hit.collider.CompareTag("Platform"))
        {
            StartCoroutine(hit.collider.gameObject.GetComponent<Platform>().FallThrough());
            Animator.SetTrigger("FallingThrough");
        }
    }

    // Fixed update used for rigidbody physics interactions
    private void FixedUpdate()
    {
        // Alters bounds if the screen size changes
        if (minScreenBounds != cam.ScreenToWorldPoint(Vector3.zero) || 
            maxScreenBounds != cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)))
        {
            minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
            maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        }

        // During a transition event, forces a specific movement on the player
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


        // Normal movement
        else
        {
            // Stops movement if the player has ended up out of bounds
            // Adds vertical movement and bound checks during the flying scene
            rb.velocity = new Vector2(
                ((minScreenBounds.x > pBounds.bounds.min.x && moveValue.x <= 0) ||
                (maxScreenBounds.x < pBounds.bounds.max.x && moveValue.x >= 0) ? 0 : moveValue.x * Cspeed),
                (Scene == SceneIndex.FLYING) ? CheckFlyingBounds() : rb.velocity.y);
            Animator.SetFloat("pVelocityX", moveValue.x); // Animator values to determine which way the player is facing

        }

        // check if the player is currently jumping
        if (IsGrounded())
        {
            // if they aren't deactivate the jumping animation and return to base animation
            if (Jumping && rb.velocity.y == 0) { 
                Jumping = false;
                Animator.SetBool("isJumping", Jumping);
            } 
        }

        // Otherwise, keep track of which part of the jumping animation the player is in
        else
        {
            Animator.SetFloat("pVelocityY", rb.velocity.y);
        }

    }

    // Check to see if the player is currently touching solid ground or not
    private bool IsGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
        {
            Grounded = true;
        }
        else { Grounded = false; }
        // Set animation to correctly represent state
        // For example, if the player has just fallen off something, this trigger will cause the player to enter a falling animation
        Animator.SetBool("isGrounded", Grounded);
        return Grounded;

    }

    // Check vertical bounds during flying section, to stop player from flying off screen
    private float CheckFlyingBounds()
    {
       return ((minScreenBounds.y > pBounds.bounds.min.y && moveValue.y <= 0) ||
                (maxScreenBounds.y < pBounds.bounds.max.y && moveValue.y >= 0)) ? 0 : moveValue.y * Cspeed;
    }


    // Move to transition when the scene changing event is triggered
    public void OnSceneChanging(SceneChangingEvent eventData)
    {
        GetComponent<PlayerInput>().DeactivateInput();
        Transitioning = true;
    }

    // stop or start movement during a pause event
    public void OnPauseEvent(PauseEvent eventData)
    {
        Paused = eventData.Status;
        if (eventData.Status)
        {
            // Save velocity so the player continues jumping normally on unpause
            VelSave = rb.velocity;
            // Stop the player from moving entirely
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            GetComponent<PlayerInput>().DeactivateInput();
        } else
        {
            // Reactive normal movement
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.WakeUp();
            rb.velocity = VelSave;
            GetComponent<PlayerInput>().ActivateInput();
        }
    }


    // Scene change 
    public void OnSceneChange(SceneChangeEvent eventData)
    {
        // Reactivate gravity for the rigidbody when moving out of the flying scene
        if (Scene == SceneIndex.FLYING) { rb.gravityScale = 5; }

        // make sure to reactivate appropriate constraints and movement
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.WakeUp();
        rb.velocity = VelSave;
        GetComponent<PlayerInput>().ActivateInput();
        Transitioning = false;
        Scene = eventData.Stage;
        // Sets the appropriate animation
        Animator.SetInteger("SceneIndex", (int)Scene);
        Animator.SetTrigger("Transition");

        // Changes speed to be appropriate for scene
        switch (Scene) {
            case SceneIndex.FLYING:
                rb.gravityScale = 0;
                Cspeed = speed * 2;
                break;
            case SceneIndex.FALLING:
                Cspeed = speed * 3;
                break;
            default:
                Cspeed = speed; 
                break;
        }
        pBounds.enabled = true;
        rb.velocity = Vector2.zero; 
    }


    // Start timer for strength event
    public void OnStrength(StrengthEvent eventData) 
    {
        StartCoroutine(StrengthTimer(eventData.ActiveTime));
    }


    // Timer for strength event
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
        Animator.SetTrigger("GameOver");
        GetComponent<PlayerInput>().DeactivateInput();
        pBounds.enabled = false;
        // Deactivate any powerup affecting the player
        if (StrengthPowerUp) { StopAllCoroutines(); StrengthPowerUp = false; }
        // Throw player comically up in the air 
        rb.AddForce(Vector2.up * JumpHeight, ForceMode2D.Impulse);
    }


    // Reactivate the player
    public void OnRevive(ReviveEvent eventData) 
    {
        GetComponent<PlayerInput>().ActivateInput();
        pBounds.enabled = true;
        rb.velocity = Vector2.zero;
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
