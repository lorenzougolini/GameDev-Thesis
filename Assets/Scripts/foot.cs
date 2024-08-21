using UnityEngine;
using System.Collections;
using System;

public class Foot : MonoBehaviour {
	
	public float kickPower;

	private Vector2 previousPosition;
    private Vector2 currentVelocity;

	[SerializeField] private AiEnvManager aiEnvManager;

    void Start() {
        previousPosition = transform.localPosition;
		// aiEnvManager = FindObjectOfType<AiEnvManager>();
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
				if (aiEnvManager)
					aiEnvManager.KickOpponent(coll.gameObject.tag);
			}
		}

	}

	bool IsKickPressed() {
		transform.parent.TryGetComponent(out PlayerMovement playerMovement);
		transform.parent.TryGetComponent(out Bot bot);
		transform.parent.TryGetComponent(out AgentController agentController);
		transform.parent.TryGetComponent(out AIPlayerMovement aIPlayerMovement);
		
		if (playerMovement)
			return playerMovement.kickPressed;
		else if (bot)
			return bot.kickPressed;
		else if (agentController)
			return agentController.kicking;
		else if (aIPlayerMovement)
			return aIPlayerMovement.kickPressed;
		
        return false;
	}

	Vector2 CalculateKickForce() {
        float inclination = Mathf.Atan2(currentVelocity.y, currentVelocity.x);
        float forceMagnitude = currentVelocity.magnitude * kickPower;

        return new Vector2(Mathf.Cos(inclination) * forceMagnitude, Mathf.Sin(inclination) * forceMagnitude);
    }
}
