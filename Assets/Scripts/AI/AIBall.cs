using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBall : MonoBehaviour
{

    [SerializeField] private AgentController agent;

    private float floorLevel = -0.4f;

    private void Update()
    {
        CheckIfBelow();
    }

    private void CheckIfBelow()
    {
        if (transform.position.y <= agent.transform.position.y)
        {
            agent.AddReward(-3f);
            agent.EndEpisode(); 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GoalRight"))

            agent.AgentScored(true);
        else if (collision.CompareTag("GoalLeft"))
            agent.AgentScored(false);
    }
}
