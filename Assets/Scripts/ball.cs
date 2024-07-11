using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class Ball : MonoBehaviour {
	
	private bool isShooting = false;

	private void OnCollisionEnter2D(Collision2D coll) {
		if (ResetObjects.S.resetting) return;
		if (coll.gameObject.tag == "Player" && coll.gameObject.GetComponent<PlayerMovement>().powerSetUp) {
			
			coll.gameObject.GetComponent<PlayerMovement>().PowerUsed();
			StartCoroutine(UsePowerUp(coll.gameObject, coll.gameObject.transform.right));
			GameLogger.Instance.LogEvent("Player 1 Used Power");

		} else if (coll.gameObject.tag == "Enemy") {
			
			if (coll.gameObject.GetComponent<PlayerMovement>() && coll.gameObject.GetComponent<PlayerMovement>().powerSetUp) {
			
				coll.gameObject.GetComponent<PlayerMovement>().PowerUsed();
				StartCoroutine(UsePowerUp(coll.gameObject, -coll.gameObject.transform.right));
				GameLogger.Instance.LogEvent("Player 2 Used Power");
			
			} else if (coll.gameObject.GetComponent<Bot>() && coll.gameObject.GetComponent<Bot>().powerSetUp) {
			
				coll.gameObject.GetComponent<Bot>().PowerUsed();
				StartCoroutine(UsePowerUp(coll.gameObject, -coll.gameObject.transform.right));
				GameLogger.Instance.LogEvent("Player 2 Used Power");
			}

		} else if (isShooting) {
			
			StopCoroutine("UsePowerUp");
			isShooting = false;
			Rigidbody2D rb = GetComponent<Rigidbody2D>();
			rb.velocity = Vector2.zero;
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "GoalRight") {
			
			if (isShooting) isShooting = false;

			Gui.S.player1Goals++;
            Gui.S.ScoreGoalText(1);
            ResetObjects.S.Reset();
			Gui.S.progressBar2.UpdateCurrent(15f);

			GameLogger.Instance.LogEvent("Player 1 Scored a Goal");
		}
        if (coll.gameObject.tag == "GoalLeft") {

			if (isShooting) isShooting = false;

            Gui.S.player2Goals++;
            Gui.S.ScoreGoalText(2);
            ResetObjects.S.Reset();
			Gui.S.progressBar1.UpdateCurrent(15f);

			GameLogger.Instance.LogEvent("Player 2 Scored a Goal");
        }
    }

	IEnumerator UsePowerUp(GameObject player, Vector2 direction) {
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
		// TrailRenderer tr = GetComponent<TrailRenderer>();
		// tr.material.SetColor("_TintColor", Color.blue);

		Vector3 newPos = player.transform.position + new Vector3(direction.x > 0 ? 1 : -1, 1, 0);
		transform.position = newPos;

		float t = 0;
		while (t < rotationDuration) {
			transform.Rotate(Vector3.forward * 360 * Time.deltaTime / rotationDuration);
			t += Time.deltaTime;
			yield return null;
		}

		ballRb.constraints = RigidbodyConstraints2D.None;
		ballRb.velocity = direction * moveSpeed;

		while (isShooting) {
            yield return new WaitForSeconds(1f);
        }

		playerRb.gravityScale = 3;
		playerRb.constraints = RigidbodyConstraints2D.None;
		playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;

		ballRb.gravityScale = 1;

		//! NOT WORKING
		// tr.material.SetColor("_TintColor", Color.white);
		isShooting = false;
	}
}
