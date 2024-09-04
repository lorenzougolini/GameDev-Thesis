using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TMPro;

public class AgentController : Agent
{
    public Transform ball;
    [SerializeField] public Transform ownGoal;
    [SerializeField] public Transform opponentGoal;
    // [SerializeField] public SpriteRenderer floor;
    // [SerializeField] public TextMeshProUGUI floorText;
	[SerializeField] private TrailRenderer tr;
	[SerializeField] public ProgressBar progressBar;
    [SerializeField] public Transform opponent;
    private Animator footAnimator;

    public Vector3 spawnPosition;

    public int playerNumber;

    private bool isGrounded;
    public float jumpCount = 0f;

    private float elapsedTime = 0f;

    public float speed = 6f;
    public float jumpForce = 16f;

    private float lastHitTime = 0f;
    private float hitInterval = 1f;
    private int consecutiveHits;

    private bool canDash = true;
    private bool isDashing = false;
    private float dashPower = 16f;
    private float dashTime = 0.2f;
    private float dashCooldown = 1f;
    public int dashDirection = 0;
    private float lastLeftPressTime = 0f;
    private float lastRightPressTime = 0f;
    private float doubleClickThreshold = 0.25f;

    public bool kicking;
    public bool ballHit;

	private float knockbackDistance = 1.5f;

    public bool powerReady;
    public bool powerSetUp;
    public bool isUsingPower;

    private Rigidbody2D ballRb;
    private Rigidbody2D rb;

    public int Score;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballRb = ball.GetComponent<Rigidbody2D>();
        // progressBar = Gui.S.progressBar2.GetComponent<ProgressBar>();
        if (transform.CompareTag("Player"))
            playerNumber = 1;
        else if (transform.CompareTag("Enemy"))
            playerNumber = 2;

        // check if gui object exists and access its progressBar2
        if (Gui.S)
        {
            progressBar = Gui.S.progressBar2.GetComponent<ProgressBar>();
        }

    }

    public void ResetPlayer()
    {
        transform.localPosition = spawnPosition;
        rb.velocity = Vector2.zero;

        isGrounded = true;
        elapsedTime = 0f;
        jumpCount = 0f;
        // PowerUsed();
        // Score = 0;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        // progressBar.UpdateCurrent(0.1f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 agentPosition = (Vector2)transform.localPosition;
        Vector2 ballPosition = (Vector2)ball.localPosition;
        Vector2 opponentPosition = (Vector2)opponent.localPosition;

        // agent
        sensor.AddObservation(agentPosition);

        // ball
        sensor.AddObservation(ballPosition);
        if (ballRb)
            sensor.AddObservation(ballRb.velocity);
        else
            findBall();

        // opponent
        sensor.AddObservation(opponentPosition);

        // relative positions
        // sensor.AddObservation(agentPosition - ballPosition);
        // sensor.AddObservation(agentPosition - opponentPosition);

        // // distances
        // sensor.AddObservation(Vector2.Distance(agentPosition, ballPosition));
        // sensor.AddObservation(Vector2.Distance(agentPosition, opponentPosition));

        // // power-up charge
        // sensor.AddObservation(progressBar.current);

        // time
        // sensor.AddObservation(AiGameManager.instance.timeRemaining);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // if (!Gui.S.playing) return;

        float moveAction = actions.DiscreteActions[0];
        float jumpAction = actions.DiscreteActions[1];
        float dashAction = actions.DiscreteActions[2];
        float kickAction = actions.DiscreteActions[3];
        float powerAction = actions.DiscreteActions[4];
        
        if (!isDashing && !isUsingPower)
        {
            if (moveAction == 1)
            {
                transform.localPosition += speed * Time.deltaTime * Vector3.left;
            }
            else if (moveAction == 2)
            {
                transform.localPosition += speed * Time.deltaTime * Vector3.right;
            }
        }

        if (jumpAction == 1 && isGrounded && !isUsingPower)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            jumpCount += 1f;
        }
        // if (jumpAction == 1 && rb.velocity.y > 0f)
        // {
        //     rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        //     isGrounded = false;
        // }

        if (dashAction == 1 && isGrounded && canDash && !isDashing && !isUsingPower)
        {
            StartCoroutine(Dash(moveAction == 1 ? 1 : -1));
        }

        if (kickAction == 1 && !isUsingPower)
        {
            kicking = true;
            footAnimator = transform.Find("Foot").GetComponent<Animator>();
            footAnimator.SetBool("kick", true);
            StartCoroutine(Kick());
        }

        if (powerAction == 1 && powerReady && !isUsingPower && !powerSetUp)
        {
            powerSetUp = true;
			
			Animator bodyAnimator = transform.Find("Body").GetComponent<Animator>();
			bodyAnimator.enabled = true;

			progressBar.current = 0f;
        }
            
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        // Reset actions first
        discreteActions[0] = 0; // No movement by default
        discreteActions[1] = 0; // No jump by default
        discreteActions[2] = 0; // No dash by default
        discreteActions[3] = 0; // No kick by default
        discreteActions[4] = 0; // No power by default

        discreteActions[0] = Input.GetAxis("Horizontal1") < 0 ? 1 :
                                Input.GetAxis("Horizontal1") > 0 ? 2 : 0;
        discreteActions[1] = Input.GetButtonDown("Vertical1") ? 1 : 0; // Jump
        discreteActions[2] = CheckDoubleClick(); // Dash using double-click check
        discreteActions[3] = (int) Input.GetAxis("Jump1"); // Kick
        discreteActions[4] = Input.GetButtonDown("Fire1") ? 1 : 0; // Power-up
        // Debug.Log($"Horizontal axis: {continuousActions[0]}");
    }

    public void AgentScored(bool isOwnGoal)
    {
        if (isOwnGoal)
        {
            AddReward(-10f - (elapsedTime*0.01f));
            // floor.color = Color.red;
            // floorText.text = "Own Goal";
            // EndEpisode();
            // ResetPlayer();
        }
        else
        {
            AddReward(15f - (elapsedTime*0.01f));
            // floor.color = Color.green;
            // floorText.text = "Goal";
            // EndEpisode();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            AddReward(2f);

            if (Time.time - lastHitTime <= hitInterval)
            {
                consecutiveHits++;
                AddReward(0.05f * consecutiveHits);
            }
            else
            {
                consecutiveHits = 1;
            }

            lastHitTime = Time.time;
        }
        
        if (other.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.5f);
            // EndEpisode();
        }

        if (other.gameObject.CompareTag("GoalRight") || other.gameObject.CompareTag("GoalLeft"))
        {
            AddReward(-2f);
            // EndEpisode();
        }

        if (other.gameObject.CompareTag("Ground"))
            isGrounded = true;
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
            if (currentTime - lastLeftPressTime < doubleClickThreshold && canDash && isGrounded)
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
            if (currentTime - lastRightPressTime < doubleClickThreshold && canDash && isGrounded)
            {
                lastRightPressTime = currentTime;
                dashDirection = 1;
                return 1; // Dash
            }
            lastRightPressTime = currentTime;
        }

        return 0; // No dash
    }

    private IEnumerator Dash(float direction)
    {
        float startBallDist = Mathf.Abs(transform.localPosition.x - ball.localPosition.x);

        canDash = false;
        isDashing = true;
        float originalVelocityX = rb.velocity.x;
        rb.velocity = new Vector2(direction * transform.localScale.x * dashPower, rb.velocity.y);
        tr.emitting = true;
        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        rb.velocity = new Vector2(originalVelocityX, rb.velocity.y);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

        float endBallDist = Mathf.Abs(transform.localPosition.x - ball.localPosition.x);
        if (endBallDist < startBallDist)
            AddReward(0.5f);
        else
            AddReward(-1.5f);
    }

    private IEnumerator Kick()
    {
        float startBallDistToGoal = Mathf.Abs(opponentGoal.transform.localPosition.x - ball.localPosition.x);
        
        if (ballHit)
            AddReward(0.5f);
        else
            AddReward(-1.5f);

        yield return new WaitForSeconds(0.1f);
        footAnimator.SetBool("kick", false);
        kicking = false;
        ballHit = false;

        float endBallDistToGoal = Mathf.Abs(opponentGoal.transform.localPosition.x - ball.localPosition.x);
        if (endBallDistToGoal <= startBallDistToGoal)
            AddReward(0.5f);
        else
            AddReward(-1.5f);
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

    private void findBall()
    {
        ball = GameObject.FindGameObjectWithTag("Ball").transform;
        ballRb = ball.GetComponent<Rigidbody2D>();
    }
}
