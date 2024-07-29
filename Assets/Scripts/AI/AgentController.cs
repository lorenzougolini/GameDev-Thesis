using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentController : Agent
{

    [SerializeField] private Transform ball;
    [SerializeField] private SpriteRenderer floor;
	[SerializeField] private TrailRenderer tr;

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

    private Rigidbody2D ballRb;
    private Rigidbody2D rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        ballRb = ball.GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        float agentXpos = Random.Range(-7f, 7f);
        transform.localPosition = new Vector3(agentXpos, 0f);
        ball.localPosition = new Vector3(Random.Range(-7f, agentXpos), 2f);

        rb.velocity = Vector2.zero;
        ballRb.velocity = Vector2.zero;
        ballRb.velocity += new Vector2(Random.Range(0, 2f), Random.Range(0, 2f));

        isGrounded = true;
        elapsedTime = 0f;
        jumpCount = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.localPosition);
        
        sensor.AddObservation((Vector2)ball.localPosition);
        sensor.AddObservation(ballRb.velocity);
        
        sensor.AddObservation(Mathf.Abs(transform.localPosition.x - ball.localPosition.x)); // distance from ball

        sensor.AddObservation(isGrounded);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float jump = actions.ContinuousActions[1];
        float dash = actions.ContinuousActions[2];

        float moveSpeed = 8f;
        float jumpForce = 8f;
        
        if (!isDashing) 
            transform.localPosition += moveSpeed * Time.deltaTime * new Vector3(move, 0f);

        if (jump > 0 && isGrounded && !isDashing)
        {
            // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
            isGrounded = false;
            jumpCount += 1f;
        }

        if (dash > 0 && isGrounded && canDash && !isDashing)
        {
            StartCoroutine(Dash(-move));
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal1");
        continuousActions[1] = Input.GetAxisRaw("Vertical1");
        continuousActions[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        // Debug.Log($"Horizontal axis: {continuousActions[0]}");

    }

    public void AgentScored(bool isOwnGoal)
    {
        if (isOwnGoal)
        {
            AddReward(-10f - (jumpCount*0.01f) - (elapsedTime*0.01f));
            floor.color = Color.red;
            EndEpisode();
        }
        else
        {
            AddReward(15f - (jumpCount*0.01f) - (elapsedTime*0.01f));
            floor.color = Color.green;
            EndEpisode();
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

        if (other.gameObject.CompareTag("GoalRight"))
        {
            AddReward(-2f);
            // EndEpisode();
        }

        if (other.gameObject.CompareTag("Ground"))
            isGrounded = true;
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
            AddReward(-0.5f);
    }
}
