using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TMPro;

public class AgentController : Agent
{

    [SerializeField] private Transform ball;
    [SerializeField] public Transform ownGoal;
    [SerializeField] public Transform opponentGoal;
    [SerializeField] public SpriteRenderer floor;
    [SerializeField] public TextMeshProUGUI floorText;
	[SerializeField] private TrailRenderer tr;
	[SerializeField] public ProgressBar progressBar;
    [SerializeField] private Transform opponent;
    private Animator footAnimator;

    public Vector3 spawnPosition;

    private bool isGrounded;
    private float jumpCount = 0f;

    private float elapsedTime = 0f;

    private float lastHitTime = 0f;
    private float hitInterval = 1f;
    private int consecutiveHits;

    private bool canDash = true;
    private bool isDashing = false;
    private float dashPower = 16f;
    private float dashTime = 0.2f;
    private float dashCooldown = 1f;

    public bool kicking;
    public bool ballHit;

	private float knockbackDistance = 1.5f;

    public bool powerReady;
    public bool powerSetUp;
    public bool isUsingPower;

    private Rigidbody2D ballRb;
    private Rigidbody2D rb;

    public int Score;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        ballRb = ball.GetComponent<Rigidbody2D>();
    }

    // public override void OnEpisodeBegin()
    // {
    //     float agentXpos = Random.Range(-7f, 7f);
    //     transform.localPosition = new Vector3(agentXpos, 0f);
    //     ball.localPosition = new Vector3(Random.Range(-7f, agentXpos), 2f);

    //     rb.velocity = Vector2.zero;
    //     ballRb.velocity = Vector2.zero;
    //     ballRb.velocity += new Vector2(Random.Range(0, 5f), Random.Range(0, 5f));
    //     // remove rotation of the ball
    //     ballRb.angularVelocity = 0f;

    //     progressBar.SetCurrent(Random.Range(0, 100f));

    //     isGrounded = true;
    //     elapsedTime = 0f;
    //     jumpCount = 0f;
    //     PowerUsed();
    // }

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
        sensor.AddObservation((Vector2)transform.localPosition);
        
        sensor.AddObservation((Vector2)ball.localPosition);
        sensor.AddObservation(ballRb.velocity);
        
        sensor.AddObservation((Vector2)opponent.localPosition);
        // sensor.AddObservation(Mathf.Abs(transform.localPosition.x - ball.localPosition.x)); // distance from ball

        // sensor.AddObservation(isGrounded);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveAction = actions.DiscreteActions[0];
        float jumpAction = actions.DiscreteActions[1];
        float dashAction = actions.DiscreteActions[2];
        float kickAction = actions.DiscreteActions[3];
        float powerAction = actions.DiscreteActions[4];

        float moveSpeed = 8f;
        float jumpForce = 8f;
        
        if (!isDashing && !isUsingPower)
        {
            if (moveAction == 1)
            {
                transform.localPosition += moveSpeed * Time.deltaTime * Vector3.left;
            }
            else if (moveAction == 2)
            {
                transform.localPosition += moveSpeed * Time.deltaTime * Vector3.right;
            }
        }

        if (jumpAction == 1 && isGrounded && !isDashing && !isUsingPower)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            jumpCount += 1f;
        }

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

        discreteActions[0] = Input.GetAxisRaw("Horizontal1") < 0 ? 1 : (Input.GetAxisRaw("Horizontal1") > 0 ? 2 : 0);
        discreteActions[1] = Input.GetAxisRaw("Vertical1") > 0 ? 1 : 0;
        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        discreteActions[3] = Input.GetAxisRaw("Jump1") > 0 ? 1 : 0;
        discreteActions[4] = Input.GetButtonDown("Fire1") ? 1 : 0;
        // Debug.Log($"Horizontal axis: {continuousActions[0]}");
    }

    public void AgentScored(bool isOwnGoal)
    {
        if (isOwnGoal)
        {
            AddReward(-10f - (jumpCount*0.05f) - (elapsedTime*0.01f));
            floor.color = Color.red;
            floorText.text = "Own Goal";
            // EndEpisode();
            // ResetPlayer();
        }
        else
        {
            AddReward(15f - (jumpCount*0.05f) - (elapsedTime*0.01f));
            floor.color = Color.green;
            floorText.text = "Goal";
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
}
