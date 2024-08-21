using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System;

public class Ball : MonoBehaviour {
	
	public bool isShooting = false;

	private Animator animator;

	private void Start() {
		animator = GetComponent<Animator>();
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
			GameManager.Instance.matchTelemetry.playerScore++;
            Gui.S.ScoreGoalText(1);
            ResetObjects.S.Reset();
			Gui.S.progressBar2.UpdateCurrent(15f);

			GameLogger.Instance.LogEvent("Player 1 Scored a Goal");
		}
        if (coll.gameObject.tag == "GoalLeft") {

			if (isShooting) isShooting = false;

            Gui.S.player2Goals++;
			GameManager.Instance.matchTelemetry.opponentScore++;
            Gui.S.ScoreGoalText(2);
            ResetObjects.S.Reset();
			Gui.S.progressBar1.UpdateCurrent(15f);

			GameLogger.Instance.LogEvent("Player 2 Scored a Goal");
        }
    }

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

		//! NOT WORKING
		/* 
		TrailRenderer tr = gameObject.GetComponent<TrailRenderer>();

		// Create a new gradient
		Gradient gradient = new Gradient();

		// Set the color keys at the relative time 0 and 1 (start and end)
		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
			new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
		);

		// Assign the gradient to the TrailRenderer
		tr.colorGradient = gradient;
		*/
		//!

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

		//! NOT WORKING
		// tr.material.SetColor("_TintColor", Color.white);
		//!
		isShooting = false;

		if (playerMovement)
			playerMovement.isUsingPower = false;
		else if (bot)
			bot.isUsingPower = false;
		else if (agentController)
			agentController.isUsingPower = false;
	}
}
