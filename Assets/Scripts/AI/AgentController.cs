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

    private bool isGrounded;

    private Rigidbody2D rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        float agentXpos = Random.Range(-7f, 7f);
        transform.localPosition = new Vector3(agentXpos, 0f);
        ball.localPosition = new Vector3(Random.Range(-7f, agentXpos), 2f);

        rb.velocity = Vector2.zero;

        isGrounded = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.localPosition);
        sensor.AddObservation((Vector2)ball.localPosition);
        sensor.AddObservation(isGrounded);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float jump = actions.ContinuousActions[1];
        
        float moveSpeed = 8f;
        float jumpForce = 8f;
        
        transform.localPosition += moveSpeed * Time.deltaTime * new Vector3(move, 0f);

        if (jump > 0 && isGrounded)
        {
            // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
            isGrounded = false;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal1");
        continuousActions[1] = Input.GetAxisRaw("Vertical1");
        // Debug.Log($"Horizontal axis: {continuousActions[0]}");

    }

    public void AgentScored(bool isOwnGoal)
    {
        if (isOwnGoal)
        {
            AddReward(-5f);
            floor.color = Color.red;
            EndEpisode();
        }
        else
        {
            AddReward(5f);
            floor.color = Color.green;
            EndEpisode();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            AddReward(1f);
            // EndEpisode();
        }
        
        if (other.gameObject.CompareTag("Wall"))
        {
            AddReward(-1f);
            // EndEpisode();
        }

        if (other.gameObject.CompareTag("GoalRight"))
        {
            AddReward(-1f);
            // EndEpisode();
        }

        if (other.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}
