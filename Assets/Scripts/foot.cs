using UnityEngine;
using System.Collections;

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

	void OnCollisionEnter2D(Collision2D coll) {

		if (MainMenu.mode == PlayingMode.MULTI) {

			HandleCollision(coll);
			
		} else {
			
			HandleCollision(coll, isSinglePlayer: true);
		}
	}

	void HandleCollision(Collision2D coll, bool isSinglePlayer = false) {
		if (coll.gameObject.tag == "Ball" && IsKickPressed()) {
			Vector2 kickForce = CalculateKickForce();
            coll.rigidbody.AddForce(kickForce, ForceMode2D.Impulse);
		}

		if ((coll.gameObject.tag == "Enemy" || coll.gameObject.tag == "Player") && IsKickPressed()) {
			if (coll.gameObject != this.transform.parent.gameObject) {
				GameManager.KickOpponent(coll.gameObject.tag);
			}
		}

	}

	bool IsKickPressed() {
		if (MainMenu.mode == PlayingMode.MULTI) {
            return GetComponentInParent<PlayerMovement>().kickPressed;
        } else {
            if (transform.parent.gameObject.CompareTag("Player")) {
                return GetComponentInParent<PlayerMovement>().kickPressed;
            } else if (transform.parent.gameObject.CompareTag("Enemy")) {
                return GetComponentInParent<Bot>().kickPressed;
            }
        }
        return false;
	}

	Vector2 CalculateKickForce() {
        float inclination = Mathf.Atan2(currentVelocity.y, currentVelocity.x);
        float forceMagnitude = currentVelocity.magnitude * kickPower;

        return new Vector2(Mathf.Cos(inclination) * forceMagnitude, Mathf.Sin(inclination) * forceMagnitude);
    }
}
