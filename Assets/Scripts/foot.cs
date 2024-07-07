using UnityEngine;
using System.Collections;

public class Foot : MonoBehaviour {
	public float kickPower;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D coll) {
		Debug.Log("Kick to: " + coll.gameObject.tag);

		if (coll.gameObject.tag == "Ball" && GetComponentInParent<PlayerMovement>().kickPressed)
			coll.rigidbody.AddForce(Vector2.up * kickPower, ForceMode2D.Impulse);

		if ((coll.gameObject.tag == "Enemy" || coll.gameObject.tag == "Player") && GetComponentInParent<PlayerMovement>().kickPressed) {
            if (coll.gameObject != this.transform.parent.gameObject) {
				GameManager.KickOpponent(coll.gameObject.tag);
            }
        }
	}
}
