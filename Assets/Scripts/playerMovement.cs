using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public static PlayerMovement S;

    public int playerNumber;

	Rigidbody2D playerRigid;
	Animator animator;
	private BoxCollider2D foot;

	[SerializeField]
	public float speed;
	[SerializeField]
	public float jump;
	[SerializeField]
	public float moveX;

	public bool kickPressed;
	public bool jumpPressed;
    public bool katana;
    public bool katanabool;


	void Start () {
		S = this;
		playerRigid = GetComponent<Rigidbody2D> ();
		animator = GetComponentInChildren<Animator> ();
	}
	
	void FixedUpdate () {
		moveX = Input.GetAxis ("Horizontal" + playerNumber);
		jumpPressed = System.Convert.ToBoolean(Input.GetAxis("Vertical" + playerNumber));
		kickPressed = System.Convert.ToBoolean(Input.GetAxis("Jump" + playerNumber));// spacebar
        // katana = Input.GetKeyDown(KeyCode.Comma);
        
		playerRigid.velocity = new Vector2 (moveX * speed, playerRigid.velocity.y);
        
        // katanabool = System.Convert.ToBoolean(katana);
        // katanabool = katana;

        animator.SetBool ("kick", kickPressed);
        animator.SetBool("katana", katanabool);
    }

	void OnCollisionStay2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Ground" && jumpPressed) { // if grounded
			playerRigid.AddForce (Vector2.up * jump, ForceMode2D.Impulse);
		}
	}
}
