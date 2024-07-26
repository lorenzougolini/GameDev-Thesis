using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBall : MonoBehaviour
{

    [SerializeField] private AgentController agent;

    private float airTime = 0f;
    private float groundLevel = 0.2f;
    private float floorLevel = -0.4f;

    private void Update()
    {
        TrackAirTime();
        CheckBelowFloor();
        CheckIfBelow();
    }

    private void TrackAirTime()
    {
        if (transform.localPosition.y > groundLevel)
        {
            airTime += Time.deltaTime;
            if (airTime > 5f)
            {
                agent.AddReward(1f);
                airTime = 0f;
            }
        }
        else
        {
            airTime = 0f;
        }
    }

    private void CheckBelowFloor()
    {
        if (transform.localPosition.y <= floorLevel)
        {
            agent.AddReward(-2f);
            agent.EndEpisode();
        }
    }

    private void CheckIfBelow()
    {
        if (transform.localPosition.y <= agent.transform.localPosition.y && MathF.Abs(transform.localPosition.x - agent.transform.localPosition.x) <= 1f)
            agent.AddReward(-0.01f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GoalRight"))

            agent.AgentScored(true);
        else if (collision.CompareTag("GoalLeft"))
            agent.AgentScored(false);
    }
}
