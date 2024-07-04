using UnityEngine;
using System.Collections;

public class playerMovement : MonoBehaviour {
	public static playerMovement S;

    public int playerNumber;

	Rigidbody2D playerRigid;
	Animator animator;
	BoxCollider2D foot;

	[SerializeField]
	public float speed = 0f;
	[SerializeField]
	public float jump = 0f;
	[SerializeField]
	public float kick = 0f;
	public float moveX = 0f;
	public float moveY = 0f;
	public bool kickbool;
    public bool katana;
    public bool katanabool;


	void Start () {
		S = this;
		playerRigid = GetComponent<Rigidbody2D> ();
		animator = GetComponentInChildren<Animator> ();
	}
	
	void FixedUpdate () {
		moveX = Input.GetAxis ("Horizontal" + playerNumber);
		moveY = Input.GetAxis ("Vertical" + playerNumber);
		kick = Input.GetAxis ("Jump" + playerNumber);// spacebar
        katana = Input.GetKeyDown(KeyCode.Comma);
        
		playerRigid.velocity = new Vector2 (moveX * speed, playerRigid.velocity.y);
        
		kickbool = System.Convert.ToBoolean (kick);
        katanabool = System.Convert.ToBoolean(katana);
        katanabool = katana;

		Debug.Log("Katana Input: " + katana);

        animator.SetBool ("kick", kickbool);
        animator.SetBool("katana", katanabool);
    }

	void OnCollisionStay2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Ground" && (moveY > 0)) { // if grounded
			playerRigid.AddForce (Vector2.up * jump, ForceMode2D.Impulse);
		}
	}
}
