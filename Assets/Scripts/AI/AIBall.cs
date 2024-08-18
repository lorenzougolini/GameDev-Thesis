using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBall : MonoBehaviour
{

    [SerializeField] private AgentController agent1;
    [SerializeField] private AgentController agent2;
    [SerializeField] private AiEnvManager AiEnv;

    private float airTime = 0f;
    private float groundLevel = 0.2f;
    private float floorLevel = -0.4f;
    private bool isShooting;

    private int lastTouch;


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
                agent1.AddReward(0.5f);
                agent2.AddReward(0.5f);
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
            agent1.AddReward(-2f);
            agent1.floor.color = Color.yellow;
            agent1.floorText.text = "Ball under floor";
            
            agent2.AddReward(-2f);
            agent2.floor.color = Color.yellow;
            agent2.floorText.text = "Ball under floor";
            
            agent1.EndEpisode();
            agent2.EndEpisode();

        }
    }

    private void CheckIfBelow()
    {
        if (transform.localPosition.y <= agent1.transform.localPosition.y && MathF.Abs(transform.localPosition.x - agent1.transform.localPosition.x) <= 1f)
            agent1.AddReward(-0.1f);
        if (transform.localPosition.y <= agent2.transform.localPosition.y && MathF.Abs(transform.localPosition.x - agent2.transform.localPosition.x) <= 1f)
            agent2.AddReward(-0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GoalRight"))
        {
            if (lastTouch == 1)
                AiEnv.player1.AgentScored(false);
            else
                AiEnv.player2.AgentScored(true);

            AiEnv.GoalScored(1);

            // AiEnv.player1.ResetPlayer();
            // AiEnv.player2.ResetPlayer();
            
        }
        else if (collision.CompareTag("GoalLeft"))
        {
            if (lastTouch == 2)
                AiEnv.player2.AgentScored(false);
            else
                AiEnv.player1.AgentScored(true);

            AiEnv.GoalScored(2);

            // AiEnv.player1.ResetPlayer();
            // AiEnv.player2.ResetPlayer();
        }

        if (isShooting)
        {
            StopCoroutine("UsePowerUp");
			isShooting = false;
			Rigidbody2D rb = GetComponent<Rigidbody2D>();
			rb.velocity = Vector2.zero;
        }

    }

    private void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player") 
        {
            lastTouch = 1;

            coll.gameObject.TryGetComponent(out AgentController agentController);

			if (agentController && agentController.powerSetUp) {
			
				StartCoroutine(UsePowerUp(coll.gameObject, coll.gameObject.transform.right));
				agentController.PowerUsed();
			
			}

		} 
        else if (coll.gameObject.tag == "Enemy") 
        {
            lastTouch = 2;

            coll.gameObject.TryGetComponent(out AgentController agentController);

			if (agentController && agentController.powerSetUp) {
			
				StartCoroutine(UsePowerUp(coll.gameObject, -coll.gameObject.transform.right));
				agentController.PowerUsed();
			
			}
            
        }
        else if (isShooting) 
        {
			StopCoroutine("UsePowerUp");
			isShooting = false;
			Rigidbody2D rb = GetComponent<Rigidbody2D>();
			rb.velocity = Vector2.zero;
		}
	}

    public void StopPower()
    {
        StopCoroutine("UsePowerUp");
        isShooting = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
    }

	IEnumerator UsePowerUp(GameObject player, Vector2 direction) {
        player.TryGetComponent(out AgentController agentController);
        agentController.isUsingPower = true;

		float rotationDuration = 1f;
		float moveSpeed = 80f;

		Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
		playerRb.velocity = Vector2.zero;
		playerRb.gravityScale = 0;
		playerRb.constraints = RigidbodyConstraints2D.FreezeAll;

		Rigidbody2D ballRb = GetComponent<Rigidbody2D>();
		ballRb.velocity = Vector2.zero;
		ballRb.gravityScale = 0;
		ballRb.constraints = RigidbodyConstraints2D.FreezeAll;

		isShooting = true;

		Vector3 newPos = player.transform.position + new Vector3(direction.x > 0 ? 1 : -1, 0.5f, 0);
		transform.position = newPos;

        direction = new Vector2(direction.x, 0).normalized;

		float t = 0;
		while (t < rotationDuration) 
        {
			transform.Rotate(Vector3.forward * 360 * Time.deltaTime / rotationDuration);
			t += Time.deltaTime;
			yield return null;
		}

		ballRb.constraints = RigidbodyConstraints2D.None;
		ballRb.velocity = direction * moveSpeed;

		playerRb.gravityScale = 3;
		playerRb.constraints = RigidbodyConstraints2D.None;
		playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;

		ballRb.gravityScale = 1;

		isShooting = false;
		agentController.isUsingPower = false;
	} 
}
