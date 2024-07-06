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
		if (coll.gameObject.tag == "Ball" && GetComponentInParent<PlayerMovement>().kickPressed) {
			coll.rigidbody.AddForce(Vector2.up * kickPower, ForceMode2D.Impulse);
		}
	}
}
