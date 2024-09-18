using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System;

public class Ball : MonoBehaviour {
	
	public bool isShooting = false;

	private Animator animator;

	public MatchTelemetry.BallTelemetry ballTelemetry;
	public MatchTelemetry.ScoreTelemetry scoreTelemetry;
	private float lastTelemetryTime = 0f;

	private void Start() {
		animator = GetComponent<Animator>();
	}

	private void FixedUpdate()
	{
		if (Time.time - lastTelemetryTime >= MatchTelemetry.telemetryTimeInterval)
		{
			ballTelemetry.time = Time.time;
			ballTelemetry.position = (Vector2) transform.position;
			GameManager.Instance.matchTelemetry.ballTelemetry.Add(ballTelemetry);
			ballTelemetry = new MatchTelemetry.BallTelemetry();
			scoreTelemetry = new MatchTelemetry.ScoreTelemetry();
			lastTelemetryTime = Time.time;
		}
	}

	private void OnCollisionEnter2D(Collision2D coll) 
	{
		if (ResetObjects.S.resetting) return;
		if (coll.gameObject.tag == "Player") 
		{
			coll.gameObject.TryGetComponent(out PlayerMovement playerMovement);
			coll.gameObject.TryGetComponent(out Bot bot);
			coll.gameObject.TryGetComponent(out AIPlayerMovement aIPlayerMovement);

			if (playerMovement && playerMovement.powerSetUp) 
			{
				StartCoroutine(UsePowerUp(coll.gameObject, coll.gameObject.transform.right));
				playerMovement.PowerUsed();
				GameLogger.Instance.LogEvent("Player 1 Used Power");
			
			}
			else if (bot && bot.powerSetUp) 
			{
				StartCoroutine(UsePowerUp(coll.gameObject, coll.gameObject.transform.right));
				bot.PowerUsed();
				GameLogger.Instance.LogEvent("Player 1 Used Power");
			}
			else if (aIPlayerMovement && aIPlayerMovement.powerSetUp)
			{
				StartCoroutine(UsePowerUp(coll.gameObject, coll.gameObject.transform.right));
				aIPlayerMovement.PowerUsed();
				GameLogger.Instance.LogEvent("Player 1 Used Power");
			}

		} 
		else if (coll.gameObject.tag == "Enemy") 
		{
			coll.gameObject.TryGetComponent(out PlayerMovement playerMovement);
			coll.gameObject.TryGetComponent(out Bot bot);
			coll.gameObject.TryGetComponent(out AgentController agentController);

			if (playerMovement && playerMovement.powerSetUp) 
			{
				StartCoroutine(UsePowerUp(coll.gameObject, -coll.gameObject.transform.right));
				playerMovement.PowerUsed();
				GameLogger.Instance.LogEvent("Player 2 Used Power");
			} 
			else if (bot && bot.powerSetUp) 
			{
				StartCoroutine(UsePowerUp(coll.gameObject, -coll.gameObject.transform.right));
				bot.PowerUsed();
				GameLogger.Instance.LogEvent("Player 2 Used Power");
			}
			else if (agentController && agentController.powerSetUp) 
			{
				StartCoroutine(UsePowerUp(coll.gameObject, -coll.gameObject.transform.right));
				agentController.PowerUsed();
				GameLogger.Instance.LogEvent("Player 2 Used Power");
			}

		} 
		else if (coll.gameObject.tag == "Agent") 
		{

			if (coll.gameObject.GetComponent<AgentController>().powerSetUp) 
			{
				StartCoroutine(UsePowerUp(coll.gameObject, coll.gameObject.transform.right));
				coll.gameObject.GetComponent<AgentController>().PowerUsed();
				GameLogger.Instance.LogEvent("Player 1 Used Power");
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

	void OnTriggerEnter2D(Collider2D coll) 
	{
		if (coll.gameObject.tag == "GoalRight") {
			
			if (isShooting) isShooting = false;

			Gui.S.player1Goals++;
			scoreTelemetry.scoringPlayer = "0";
			scoreTelemetry.time = Time.time;
			GameManager.Instance.matchTelemetry.scoreTelemetry.Add(scoreTelemetry);
            Gui.S.ScoreGoalText(1);
            ResetObjects.S.Reset();
			Gui.S.progressBar2.UpdateCurrent(15f);

			GameLogger.Instance.LogEvent("Player 1 Scored a Goal");
		}
        if (coll.gameObject.tag == "GoalLeft") {

			if (isShooting) isShooting = false;

            Gui.S.player2Goals++;
			scoreTelemetry.scoringPlayer = "1";
			scoreTelemetry.time = Time.time;
			GameManager.Instance.matchTelemetry.scoreTelemetry.Add(scoreTelemetry);
            Gui.S.ScoreGoalText(2);
            ResetObjects.S.Reset();
			Gui.S.progressBar1.UpdateCurrent(15f);

			GameLogger.Instance.LogEvent("Player 2 Scored a Goal");
        }
    }

	/* ----------- 	COROUTINES 	----------- */

	IEnumerator UsePowerUp(GameObject player, Vector2 direction) 
	{
		player.TryGetComponent(out PlayerMovement playerMovement);
		player.TryGetComponent(out Bot bot);
		player.TryGetComponent(out AgentController agentController);

		if (playerMovement)
			playerMovement.isUsingPower = true;
		else if (bot)
			bot.isUsingPower = true;
		else if (agentController)
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

		if (playerMovement)
			playerMovement.isUsingPower = false;
		else if (bot)
			bot.isUsingPower = false;
		else if (agentController)
			agentController.isUsingPower = false;
	}
}
