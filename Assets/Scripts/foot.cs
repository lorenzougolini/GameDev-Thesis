using UnityEngine;
using System.Collections;
using System;

public class Foot : MonoBehaviour {
	
	public float kickPower;

	private Vector2 previousPosition;
    private Vector2 currentVelocity;

    void Start() {
        previousPosition = transform.localPosition;
    }

	private void Update() {
		Vector2 currentPosition = transform.localPosition;
        currentVelocity = (currentPosition - previousPosition) / Time.deltaTime;
        previousPosition = currentPosition;
	}

	void OnCollisionEnter2D(Collision2D coll) 
	{
		HandleCollision(coll);
	}

	void HandleCollision(Collision2D coll) {
		if (coll.gameObject.tag == "Ball" && IsKickPressed()) {
			Vector2 kickForce = CalculateKickForce();
			coll.rigidbody.AddForce(kickForce, ForceMode2D.Impulse);

			// Take the parent object
			GameObject parentObject = transform.parent.gameObject;
            parentObject.TryGetComponent<AgentController>(out AgentController agent);
			if (agent)
				agent.ballHit = true;
		}

		if ((coll.gameObject.tag == "Enemy" || coll.gameObject.tag == "Player") && IsKickPressed()) {
			if (coll.gameObject != this.transform.parent.gameObject) {
				GameManager.KickOpponent(coll.gameObject.tag);
			}
		}

	}

	bool IsKickPressed() {
		try
		{
			if (MainMenu.mode == PlayingMode.MULTI) {
	            return GetComponentInParent<PlayerMovement>().kickPressed;
	        } else if (MainMenu.mode == PlayingMode.SINGLE) {
	            if (transform.parent.gameObject.CompareTag("Player")) {
	                return GetComponentInParent<PlayerMovement>().kickPressed;
	            } else if (transform.parent.gameObject.CompareTag("Enemy")) {
	                return GetComponentInParent<Bot>().kickPressed;
	            }
	        } else if (MainMenu.mode == PlayingMode.NONE) {
				return GetComponentInParent<Bot>().kickPressed;
			}
		} catch (Exception) 
		{
			return GetComponentInParent<AgentController>().kicking;
		}
        return false;
	}

	Vector2 CalculateKickForce() {
        float inclination = Mathf.Atan2(currentVelocity.y, currentVelocity.x);
        float forceMagnitude = currentVelocity.magnitude * kickPower;

        return new Vector2(Mathf.Cos(inclination) * forceMagnitude, Mathf.Sin(inclination) * forceMagnitude);
    }
}
