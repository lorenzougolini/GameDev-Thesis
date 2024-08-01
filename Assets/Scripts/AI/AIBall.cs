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
    private bool isShooting;


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
                agent.AddReward(0.5f);
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
            agent.floor.color = Color.yellow;
            agent.floorText.text = "Ball under floor";
            agent.EndEpisode();

        }
    }

    private void CheckIfBelow()
    {
        if (transform.localPosition.y <= agent.transform.localPosition.y && MathF.Abs(transform.localPosition.x - agent.transform.localPosition.x) <= 1f)
            agent.AddReward(-0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GoalRight"))

            agent.AgentScored(true);
        else if (collision.CompareTag("GoalLeft"))
            agent.AgentScored(false);
    }

    private void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player") {

            coll.gameObject.TryGetComponent(out AgentController agentController);

			if (agentController && agentController.powerSetUp) {
			
				StartCoroutine(UsePowerUp(coll.gameObject, -coll.gameObject.transform.right));
				agentController.PowerUsed();
			
			}

		} else if (isShooting) {
			
			StopCoroutine("UsePowerUp");
			isShooting = false;
			Rigidbody2D rb = GetComponent<Rigidbody2D>();
			rb.velocity = Vector2.zero;
		}
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

		float t = 0;
		while (t < rotationDuration) {
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
