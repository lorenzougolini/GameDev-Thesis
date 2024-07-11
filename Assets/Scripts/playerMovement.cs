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

	public bool powerReady = false;
	public bool powerSetUp = false;

	[SerializeField] private Rigidbody2D rb;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private TrailRenderer tr;
	private ProgressBar progressBar;
	private Animator footAnimator;

	public bool kickPressed;

	void Start () {

		footAnimator = transform.Find("Foot").GetComponent<Animator>();

		if (playerNumber == 1)
			progressBar = Gui.S.progressBar1.GetComponent<ProgressBar>();
		else
			progressBar = Gui.S.progressBar2.GetComponent<ProgressBar>();
			
	}

	void Update() {

		// if (isDashing || !Gui.S.playing) return;
		if (!Gui.S.playing) return;

		// Move
		horizontal = Input.GetAxis("Horizontal" + playerNumber);
		
		// Jump
		if (Input.GetButtonDown("Vertical" + playerNumber) && isGrounded()) {
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
			GameLogger.Instance.LogEvent("Player " + playerNumber + " Jumped at Position: " + transform.position);
		}
		if (Input.GetButtonDown("Vertical" + playerNumber) && rb.velocity.y > 0f)
			rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
		
		// Kick
		kickPressed = Convert.ToBoolean(Input.GetAxis("Jump" + playerNumber));
		if (kickPressed)
			GameLogger.Instance.LogEvent("Player " + playerNumber + " Kicked at Position: " + transform.position);

		// Dash
		if (!ResetObjects.S.resetting)
			HandleDoubleClickDash();

		// Powerup
		if (Input.GetButtonDown("Fire" + playerNumber) && powerReady) {
			powerSetUp = true;
			
			Animator bodyAnimator = transform.Find("Body").GetComponent<Animator>();
			bodyAnimator.enabled = true;

			progressBar.current = 0f;
		}

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

	public void PowerUsed() {
		powerReady = false;
		powerSetUp = false;

		Animator bodyAnimator = transform.Find("Body").GetComponent<Animator>();
		bodyAnimator.enabled = false;

		SpriteRenderer bodySprite = transform.Find("Body").GetComponent<SpriteRenderer>();
		bodySprite.color = Color.white;
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
				GameLogger.Instance.LogEvent("Player " + playerNumber + " Dashed at Position: " + transform.position);
				StartCoroutine(Dash(-1));
			}
			lastLeftPressTime = currentTime;
		}

		// Check right direction
		if ((Input.GetKeyDown(KeyCode.D) && playerNumber == 1) || (Input.GetKeyDown(KeyCode.RightArrow) && playerNumber == 2)) {
			float currentTime = Time.time;
			if (currentTime - lastRightPressTime < doubleClickThreshold && canDash && rb.velocity.y == 0f) {
				GameLogger.Instance.LogEvent("Player " + playerNumber + " Dashed at Position: " + transform.position);
				StartCoroutine(Dash(1));
			}
			lastRightPressTime = currentTime;
		}
	}
	
	public void TakeDamage(int direction) {
        Vector3 knockbackPosition = transform.position + direction * knockbackDistance * Vector3.right;
		GameLogger.Instance.LogEvent("Player " + playerNumber + " Took Damage at Position: " + transform.position);
        StartCoroutine(Knockback(knockbackPosition));
    }

	/* ----------- 	COROUTINES 	----------- */

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
