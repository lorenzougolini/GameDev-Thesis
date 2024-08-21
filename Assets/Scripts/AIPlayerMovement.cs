using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;
using System;

public class AIPlayerMovement : Agent
{
    public static AIPlayerMovement Instance;

    public bool moveLeftToRight;

    public int playerNumber;
    private float horizontal;
    public float speed = 8f;
    public float jumpForce = 16f;

    private bool canDash = true;
    private bool isDashing;
    private float dashPower = 16f;
    private float dashTime = 0.2f;
    private float dashCooldown = 0.5f;
    private int dashDirection = 0;

    private float lastLeftPressTime = 0f;
    private float lastRightPressTime = 0f;
    private float doubleClickThreshold = 0.25f;

    private float knockbackDistance = 1.5f;

    public bool powerReady = false;
    public bool powerSetUp = false;
    public bool isUsingPower = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    private ProgressBar progressBar;
    private Animator footAnimator;

    public bool kickPressed;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        footAnimator = transform.Find("Foot").GetComponent<Animator>();

        if (playerNumber == 1)
            progressBar = Gui.S.progressBar1.GetComponent<ProgressBar>();
        else
            progressBar = Gui.S.progressBar2.GetComponent<ProgressBar>();

        moveLeftToRight = GameObject.FindGameObjectWithTag("Ball").transform.position.x > transform.position.x;
    }

    public override void OnEpisodeBegin()
    {
        // Reset player position and state at the beginning of each episode.
        rb.velocity = Vector2.zero;
        transform.position = new Vector3(-7, 1, 0);
        powerReady = false;
        powerSetUp = false;
        isUsingPower = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect observations from the environment
        sensor.AddObservation(transform.position.x); // Player's position
        sensor.AddObservation(rb.velocity); // Player's velocity
        sensor.AddObservation(isGrounded()); // Is the player grounded?
        sensor.AddObservation(GameObject.FindGameObjectWithTag("Ball").transform.position); // Ball's position
        sensor.AddObservation(moveLeftToRight); // Is the ball to the right or left?
        sensor.AddObservation(kickPressed); // Is the kick pressed?
        sensor.AddObservation(powerReady); // Is the power-up ready?
        sensor.AddObservation(isDashing); // Is the player dashing?
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // No action should be performed if the game is not playing
        if (!Gui.S.playing) return;

        var discreteActions = actionBuffers.DiscreteActions;

        // Actions: 0 = No-op, 1 = Move Left, 2 = Move Right, 3 = Jump, 4 = Kick, 5 = Power-up
        switch (discreteActions[0])
        {
            case 1:
                horizontal = -1f;
                break;
            case 2:
                horizontal = 1f;
                break;
            default:
                horizontal = 0f;
                break;
        }

        // Jump
        if (discreteActions[1] == 1 && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (discreteActions[1] == 1 && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // Dash
        if (discreteActions[2] == 1)
        {
            TriggerDash(dashDirection);
        }

        // Kick
        kickPressed = discreteActions[3] == 1;

        // Power-up
        if (discreteActions[4] == 1 && powerReady)
        {
            powerSetUp = true;

            Animator bodyAnimator = transform.Find("Body").GetComponent<Animator>();
			bodyAnimator.enabled = true;

			progressBar.current = 0f;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        // Manual controls mapped to discrete actions
        discreteActionsOut[0] = Input.GetAxis("Horizontal" + playerNumber) < 0 ? 1 :
                                Input.GetAxis("Horizontal" + playerNumber) > 0 ? 2 : 0;
        discreteActionsOut[1] = Input.GetButtonDown("Vertical" + playerNumber) ? 1 : 0; // Jump
        discreteActionsOut[2] = CheckDoubleClick(); // Dash using double-click check
        discreteActionsOut[3] = (int) Input.GetAxis("Jump" + playerNumber); // Kick
        discreteActionsOut[4] = Input.GetButtonDown("Fire" + playerNumber) ? 1 : 0; // Power-up
    }

    void Update()
    {
        // No action should be performed if the game is not playing
        if (!Gui.S.playing) return;

        // Position check
        if (transform.position.x < -10f)
        {
            transform.position = new Vector3(-10f, transform.position.y, transform.position.z);
        }
        if (transform.position.x > 10f)
        {
            transform.position = new Vector3(10f, transform.position.y, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (!Gui.S.playing || isDashing) return;

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        footAnimator.SetBool("kick", kickPressed);
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void PowerUsed()
    {
        powerReady = false;
        powerSetUp = false;

        Animator bodyAnimator = transform.Find("Body").GetComponent<Animator>();
        bodyAnimator.enabled = false;

        SpriteRenderer bodySprite = transform.Find("Body").GetComponent<SpriteRenderer>();
        bodySprite.color = Color.white;
    }

    public void TakeDamage(int direction) 
    {
        Vector3 knockbackPosition = transform.position + direction * knockbackDistance * Vector3.right;
        StartCoroutine(Knockback(knockbackPosition));
    }

    private int CheckDoubleClick()
    {
        // Check for double-click and return 1 for dash, 0 otherwise

        float currentTime = Time.time;

        // Check left direction (KeyCode.A or LeftArrow)
        if ((Input.GetKeyDown(KeyCode.A) && playerNumber == 1) || (Input.GetKeyDown(KeyCode.LeftArrow) && playerNumber == 2))
        {
            if (currentTime - lastLeftPressTime < doubleClickThreshold && canDash && isGrounded())
            {
                lastLeftPressTime = currentTime;
                dashDirection = -1;
                return 1; // Dash
            }
            lastLeftPressTime = currentTime;
        }

        // Check right direction (KeyCode.D or RightArrow)
        if ((Input.GetKeyDown(KeyCode.D) && playerNumber == 1) || (Input.GetKeyDown(KeyCode.RightArrow) && playerNumber == 2))
        {
            if (currentTime - lastRightPressTime < doubleClickThreshold && canDash && isGrounded())
            {
                lastRightPressTime = currentTime;
                dashDirection = 1;
                return 1; // Dash
            }
            lastRightPressTime = currentTime;
        }

        return 0; // No dash
    }

    public void TriggerDash(int direction)
    {
        if (canDash && isGrounded())
        {
            StartCoroutine(Dash(direction));
        }
    }

    private IEnumerator Dash(int direction)
    {
        canDash = false;
        isDashing = true;
        rb.velocity = new Vector2(direction * transform.localScale.x * dashPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator Knockback(Vector3 targetPosition) 
    {
		float elapsedTime = 0f;	
        float duration = 0.2f;
        Vector3 startingPosition = transform.position;
		transform.Find("Body").GetComponent<SpriteRenderer>().color = new Color(1f, 0.482f, 0.482f);

        while (elapsedTime < duration) {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
			
            yield return null;
        }

        transform.position = targetPosition;
		transform.Find("Body").GetComponent<SpriteRenderer>().color = Color.white;	
	}
}
