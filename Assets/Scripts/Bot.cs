using System;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class Bot : MonoBehaviour
{

    public bool moveLeftToRight;

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
    public Vector3 opponentGoalPosition;

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

        moveLeftToRight = opponentGoalPosition.x > transform.position.x;
    }

    // Update is called once per frame
    void Update() {
        if (isDashing || !Gui.S.playing) return;

        Move();
        Jump();
        Kick();
        UsePowerUp();

        HandleBotOverlap();
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

        if (BallIsOverhead()) {
            MoveFromBall();
            return;
        }

        if (HasToDefend()) {
            Defend();
            // return;
        }

        if (IsFieldClear()) {
            MoveTowardsGoal();
            // return;
        }

        if (transform.CompareTag("Enemy")) {
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

        if (ballDistance > (ballDistance + Vector3.Distance(opponent.transform.position, ball.transform.position)) && canDash && random.NextDouble() < 0.5) {
            Vector3 dashDirection = (ball.transform.position - transform.position).normalized;
            int direction = dashDirection.x > 0 ? 1 : -1;
            
            GameLogger.Instance.LogEvent("Bot Dashed at Position: " + transform.position);
            StartCoroutine(Dash(direction));
        }
    }

    private bool BallIsOverhead() {
        return Mathf.Abs(ball.transform.position.x - transform.position.x) < 0.5f 
            && (ball.transform.position.y >= transform.position.y)
            && (ball.transform.position.y - transform.position.y) < 0.5f;
    }

    // private IEnumerator MoveFromBall() {
    private void MoveFromBall() {

        if (moveLeftToRight) {
            if (ball.transform.position.x > transform.position.x)
                // Move left
                rb.velocity = new Vector2(-speed, rb.velocity.y);
            else
                // Move right
                rb.velocity = new Vector2(speed, rb.velocity.y);
            // transform.Translate(speed * Time.deltaTime, 0, 0); // Move back
        } else {
            if (ball.transform.position.x > transform.position.x)
                // Move right
                rb.velocity = new Vector2(speed, rb.velocity.y);
            else
                // Move left
                rb.velocity = new Vector2(speed, rb.velocity.y);
            // transform.Translate(-speed * Time.deltaTime, 0, 0); // Move forward
        }
    }

    private bool IsFieldClear() {
        // Check if the opponent is between the bot and the opponent's goal
        float botToGoalDistance = Vector3.Distance(transform.position, opponentGoalPosition);
        float opponentToGoalDistance = Vector3.Distance(opponent.transform.position, opponentGoalPosition);

        return botToGoalDistance < opponentToGoalDistance;
    }

    private void MoveTowardsGoal() {
        Vector3 targetPosition = new Vector3(ball.transform.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

        // Stay behind the ball and push it towards the goal
        if (moveLeftToRight) { // Bot moving left to right
            if (transform.position.x < ball.transform.position.x) {
                transform.Translate(speed * Time.deltaTime, 0, 0); // Move forward
            } else {
                transform.Translate(-speed * Time.deltaTime, 0, 0); // Move back
            }
        } else { // Bot moving right to left
            if (transform.position.x > ball.transform.position.x) {
                transform.Translate(-speed * Time.deltaTime, 0, 0); // Move back
            } else {
                transform.Translate(speed * Time.deltaTime, 0, 0); // Move forward
            }
        }
    }

    private bool HasToDefend() {
        float ballToGoalDistance = Vector3.Distance(ball.transform.position, defense.position);
        float botToGoalDistance = Vector3.Distance(transform.position, defense.position);

        if (moveLeftToRight) {
            return ball.transform.position.x < transform.position.x && ballToGoalDistance < defenseRangeMax;
        } else {
            return ball.transform.position.x > transform.position.x && ballToGoalDistance < defenseRangeMax;
        }
    }

    private void Defend() {
        Vector3 targetPosition = new Vector3(ball.transform.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

        // Optionally add more defensive actions such as kicking the ball away
        if (Vector3.Distance(transform.position, ball.transform.position) < kickRange) {
            Kick();
        }
    }


    private void Jump() {
        float ballDistance = Vector3.Distance(ball.transform.position, transform.position);

        if (ballDistance <= 3 && ball.transform.position.y > transform.position.y && isGrounded && random.NextDouble() < 0.5) {
            // Vector2 jumpDirection = Vector3.up;
            // rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            GameLogger.Instance.LogEvent("Bot Jumped at Position: " + transform.position);
        }
    }

    private void Kick() {

        float opponentDistance = Vector3.Distance(opponent.transform.position, transform.position);
        float ballDistance = Vector3.Distance(ball.transform.position, transform.position);
        if ((opponentDistance < kickRange) ^ (ballDistance < kickRange) && random.NextDouble() < 0.5 && canKick) {
            kickPressed = true;
            GameLogger.Instance.LogEvent("Bot Kicked at Position: " + transform.position);
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
         
        bool opponent_powerSetUp;
        try {
            opponent_powerSetUp = opponent.GetComponent<Bot>().powerSetUp;
        } catch (Exception) {
            opponent_powerSetUp = opponent.GetComponent<PlayerMovement>().powerSetUp;
        }
        if (opponent_powerSetUp && powerReady) {
            UsePowerUp(prob: 1f);
        }

        bool opponent_isUsingPower;
        try {
            opponent_isUsingPower = opponent.GetComponent<Bot>().isUsingPower;
        } catch (Exception) {
            opponent_isUsingPower = opponent.GetComponent<PlayerMovement>().isUsingPower;
        }
        if (ball.GetComponent<Ball>().isShooting && opponent_isUsingPower) {
            StartCoroutine(WaitAndJump());
        }
    }

    private void HandleBotOverlap() {
        float overlapThreshold = 0.1f; // Adjust as needed

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, overlapThreshold);
        foreach (var collider in colliders) {
            if (collider.gameObject != gameObject && (collider.CompareTag("Player") || collider.CompareTag("Enemy"))) {
                if (collider.transform.position.y > transform.position.y) {
                    // The other bot is above this bot
                    if (random.NextDouble() < 0.5)
                        rb.velocity = new Vector2(speed, rb.velocity.y); // Move forward
                    else
                        rb.velocity = new Vector2(-speed, rb.velocity.y); // Move back
                }
            }
        }
    }

    public void TakeDamage(int direction) {
        Vector3 knockbackPosition = transform.position + direction * knockbackDistance * Vector3.right;
        GameLogger.Instance.LogEvent("Bot Took Damage at Position: " + transform.position);
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

        if (canDash && !isDashing)
            if (moveLeftToRight)
                StartCoroutine(Dash(-1));
            else
                StartCoroutine(Dash(1));

        yield return new WaitForSeconds(waitTime);

        if (isGrounded) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }
}