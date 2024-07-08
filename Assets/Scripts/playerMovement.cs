using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class PlayerMovement : MonoBehaviour {
	
    public int playerNumber;
	private float horizontal;
	public float speed = 8f;
	public float jumpForce = 16f;

	private bool canDash = true;
	private bool isDashing;
	private float dashPower = 16f;
	private float dashTime = 0.2f;
	private float dashCooldown = 0.5f;

	private float lastLeftPressTime = 0f;
	private float lastRightPressTime = 0f;
	private float doubleClickThreshold = 0.25f;

	private float knockbackDistance = 1.5f;

	[SerializeField] private Rigidbody2D rb;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private TrailRenderer tr;
	private Animator footAnimator;

	public bool kickPressed;
    // public bool katana;
    // public bool katanabool;

	void Start () {
		footAnimator = GetComponentInChildren<Animator> ();
	}

	void Update() {

		if (isDashing || !Gui.S.playing) return;

		// Move
		horizontal = Input.GetAxis("Horizontal" + playerNumber);      
		
		// Jump
		if (Input.GetButtonDown("Vertical" + playerNumber) && isGrounded())
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
		if (Input.GetButtonDown("Vertical" + playerNumber) && rb.velocity.y > 0f)
			rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
		
		// Kick
		kickPressed = Convert.ToBoolean(Input.GetAxis("Jump" + playerNumber));

		// Dash
		HandleDoubleClickDash();

		// Position check
		if (transform.position.x < -10f) {
			Vector3 newPosition = new Vector3(-10f, transform.position.y, transform.position.z);
			transform.position = newPosition;
		}
		if (transform.position.x > 10f) {
			Vector3 newPosition = new Vector3(10f, transform.position.y, transform.position.z);
			transform.position = newPosition;
		}
	}

	private bool isGrounded(){
		return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
	}

	private void FixedUpdate() {

		if (isDashing) return;

		rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
		footAnimator.SetBool ("kick", kickPressed);
	}

	private void HandleDoubleClickDash() {
		// Check left direction
		if ((Input.GetKeyDown(KeyCode.A) && playerNumber == 1) || (Input.GetKeyDown(KeyCode.LeftArrow) && playerNumber == 2)) {
			float currentTime = Time.time;
			if (currentTime - lastLeftPressTime < doubleClickThreshold && canDash && rb.velocity.y == 0f) {
				StartCoroutine(Dash(-1));
			}
			lastLeftPressTime = currentTime;
		}

		// Check right direction
		if ((Input.GetKeyDown(KeyCode.D) && playerNumber == 1) || (Input.GetKeyDown(KeyCode.RightArrow) && playerNumber == 2)) {
			float currentTime = Time.time;
			if (currentTime - lastRightPressTime < doubleClickThreshold && canDash && rb.velocity.y == 0f) {
				StartCoroutine(Dash(1));
			}
			lastRightPressTime = currentTime;
		}
	}
	
	public void TakeDamage(int direction) {
        Vector3 knockbackPosition = transform.position + direction * knockbackDistance * Vector3.right;
        StartCoroutine(Knockback(knockbackPosition));
    }

	private IEnumerator Dash(int direction) {
		canDash = false;
		isDashing = true;
		rb.velocity = new Vector2(direction * transform.localScale.x * dashPower, 0f);
		tr.emitting = true;
		yield return new WaitForSeconds(dashTime);
		tr.emitting = false;
		isDashing = false;
		yield return new WaitForSeconds(dashCooldown);
		canDash = true;
	}

	/* ----------- 	COROUTINES 	----------- */

	private IEnumerator Knockback(Vector3 targetPosition) {
		float elapsedTime = 0f;	
        float duration = 0.2f;
        Vector3 startingPosition = transform.position;
		transform.Find("Body").GetComponent<SpriteRenderer>().color = new Color(1f, 0.482f, 0.482f);

        while (elapsedTime < duration) {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
			
            yield return null;
        }

        transform.position = targetPosition;
		transform.Find("Body").GetComponent<SpriteRenderer>().color = Color.white;	
	}

}



// ---------------------------------- OLD CODE ----------------------------------
	// void Start () {
	// 	rb = GetComponent<Rigidbody2D> ();
	// 	animator = GetComponentInChildren<Animator> ();
	// }
	
	// void FixedUpdate () {
	// 	horizontal = Input.GetAxis ("Horizontal" + playerNumber);
	// 	jumpPressed = System.Convert.ToBoolean(Input.GetAxis("Vertical" + playerNumber));
	// 	kickPressed = System.Convert.ToBoolean(Input.GetAxis("Jump" + playerNumber));// spacebar
    //     // katana = Input.GetKeyDown(KeyCode.Comma);
        
	// 	rb.velocity = new Vector2 (horizontal * speed, rb.velocity.y);
        
    //     // katanabool = System.Convert.ToBoolean(katana);
    //     // katanabool = katana;

    //     animator.SetBool ("kick", kickPressed);
    //     // animator.SetBool("katana", katanabool);
    // }

	// void OnCollisionStay2D(Collision2D coll)
	// {
	// 	if (coll.gameObject.tag == "Ground" && jumpPressed) { // if grounded
	// 		rb.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
	// 	}
	// }
