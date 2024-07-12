using System.Collections;
using UnityEngine;

public class Bot : MonoBehaviour
{

    public float speed = 8f;
    public float jumpForce = 16f;
    public float defenseRangeMin = 2f;
    public float defenseRangeMax = 5f;
    public bool isGrounded = false;
    
    private bool canDash = true;
    private bool isDashing;
    private float dashPower = 16f;
	private float dashTime = 0.2f;
	private float dashCooldown = 0.5f;

    public float kickRange = 2f;
    public bool kickPressed;
    private float kickCooldown = 0.5f;
    private bool canKick = true;

	private float knockbackDistance = 1.5f;

    public bool powerReady = false;
    public bool powerSetUp = false;
    public bool isUsingPower = false;
    
    public Transform defense;

    private GameObject ball;
    private GameObject opponent;

    private Rigidbody2D rb;
    private TrailRenderer tr;
    private Animator footAnimator;

    private ProgressBar progressBar;

    private System.Random random = new();

    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        footAnimator = transform.Find("Foot").GetComponent<Animator>();

        if (transform.CompareTag("Player")) {
			progressBar = Gui.S.progressBar1.GetComponent<ProgressBar>();
            opponent = GameObject.FindGameObjectWithTag("Enemy");
        } else {
			progressBar = Gui.S.progressBar2.GetComponent<ProgressBar>();   
            opponent = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update() {
        if (isDashing || !Gui.S.playing) return;

        Move();
        Jump();
        Kick();
        UsePowerUp();

        ReactToOpponentPower();
    }

    private void FixedUpdate() {
        if (kickPressed)
            footAnimator.SetBool("kick", true);
        else
            footAnimator.SetBool("kick", false);
    }

    private void Move() {

        float ballDistance = Vector3.Distance(ball.transform.position, transform.position);
        float opponentDistance = Vector3.Distance(opponent.transform.position, transform.position);
        float ballToGoalDistance = Vector3.Distance(ball.transform.position, defense.position);

        if (transform.CompareTag("Enemy")) {
            // Check if the ball is overhead
            if (Mathf.Abs(ball.transform.position.x - transform.position.x) < 0.5f && Mathf.Abs(ball.transform.position.y - transform.position.y) < 0.5f) {
                transform.Translate(-speed * Time.deltaTime, 0, 0); // Move back
                if (canKick && random.NextDouble() < 0.5) Kick();
            }
    
            // Check if the bot should attack
            if (ball.transform.position.x < transform.position.x || ballDistance >= opponentDistance) {
                Vector3 targetPosition = new Vector3(ball.transform.position.x, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            } 
            else if (ballToGoalDistance > defenseRangeMax || ballToGoalDistance < defenseRangeMin) {
                Vector3 targetPosition = new Vector3(defense.position.x, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            }
        } else {
            // Check if the ball is overhead
            if (Mathf.Abs(ball.transform.position.x - transform.position.x) < 0.5f && Mathf.Abs(ball.transform.position.y - transform.position.y) < 0.5f) {
                transform.Translate(speed * Time.deltaTime, 0, 0); // Move forward
                if (canKick && random.NextDouble() < 0.5) Kick();
            }
    
            // Check if the bot should attack
            if (ball.transform.position.x > transform.position.x || ballDistance >= opponentDistance) {
                Vector3 targetPosition = new Vector3(ball.transform.position.x, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            } 
            else if (ballToGoalDistance > defenseRangeMax || ballToGoalDistance < defenseRangeMin) {
                Vector3 targetPosition = new Vector3(defense.position.x, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            }
        }

        if (ballDistance > defenseRangeMax && opponentDistance > defenseRangeMax && canDash && random.NextDouble() < 0.5) {
            Vector3 dashDirection = (ball.transform.position - transform.position).normalized;
            int direction = dashDirection.x > 0 ? 1 : -1;
            
            StartCoroutine(Dash(direction));
        }
    }

    private void Jump() {
        float ballDistance = Vector3.Distance(ball.transform.position, transform.position);

        if (ballDistance <= 3 && ball.transform.position.y > transform.position.y && isGrounded && random.NextDouble() < 0.5) {
            Vector2 jumpDirection = Vector3.up;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void Kick() {

        float opponentDistance = Vector3.Distance(opponent.transform.position, transform.position);
        float ballDistance = Vector3.Distance(ball.transform.position, transform.position);
        if ((opponentDistance < kickRange || ballDistance < kickRange) && random.NextDouble() < 0.5) {
            kickPressed = true;
            StartCoroutine(KickCooldown());
        } else {
            kickPressed = false;
        }
    }

    private void UsePowerUp(float prob = 0.5f) {
        if (powerReady && random.NextDouble() < prob) {
            powerSetUp = true;
            
            Animator bodyAnimator = transform.Find("Body").GetComponent<Animator>();
			bodyAnimator.enabled = true;

			progressBar.current = 0f;
        }
    }

    public void PowerUsed() {
		powerReady = false;
		powerSetUp = false;
        
        Animator bodyAnimator = transform.Find("Body").GetComponent<Animator>();
		bodyAnimator.enabled = false;

		SpriteRenderer bodySprite = transform.Find("Body").GetComponent<SpriteRenderer>();
		bodySprite.color = Color.white;
	}

    private void ReactToOpponentPower() {
        // Debug.Log($"Opponent power: {opponent.GetComponent<Bot>().powerSetUp} - Power ready: {powerReady}");
        if (opponent.GetComponent<Bot>().powerSetUp && powerReady) {
            UsePowerUp(prob: 1f);
        }

        if (ball.GetComponent<Ball>().isShooting && opponent.GetComponent<Bot>().isUsingPower) {
            StartCoroutine(WaitAndJump());
        }
    }

    public void TakeDamage(int direction) {
        Vector3 knockbackPosition = transform.position + direction * knockbackDistance * Vector3.right;
        StartCoroutine(Knockback(knockbackPosition));
    }

    private void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D coll) {
        if (coll.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

	/* ----------- 	COROUTINES 	----------- */

    private IEnumerator KickCooldown() {
        canKick = false;
        yield return new WaitForSeconds(kickCooldown);
        canKick = true;
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

    private IEnumerator WaitAndJump() {
        float waitTime = random.Next(1, 3);
        yield return new WaitForSeconds(waitTime);

        if (isGrounded) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }

    }
}
