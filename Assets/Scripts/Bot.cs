using System;
using System.Collections;
// using System.Diagnostics;
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

    private Vector3 lastBallPosition;
    private float ballStuckTime = 0f;
    private float ballStuckThreshold = 1.5f;

    void Start()
    {
        InitializeBot();
    }

    void Update()
    {
        if (isDashing || !Gui.S.playing) return;

        PerformActions();
    }

    private void FixedUpdate()
    {
        AnimateKick();
    }

    private void InitializeBot()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        footAnimator = transform.Find("Foot").GetComponent<Animator>();

        if (transform.CompareTag("Player"))
        {
            progressBar = Gui.S.progressBar1.GetComponent<ProgressBar>();
            opponent = GameObject.FindGameObjectWithTag("Enemy");
        }
        else
        {
            progressBar = Gui.S.progressBar2.GetComponent<ProgressBar>();
            opponent = GameObject.FindGameObjectWithTag("Player");
        }

        moveLeftToRight = opponentGoalPosition.x > transform.position.x;
    }

    private void PerformActions()
    {

        // CheckBallStuck();

        Move();
        Jump();
        Kick();
        UsePowerUp();
        HandleBotOverlap();
        ReactToOpponentPower();
    }

    private void CheckBallStuck()
    {
        float ballXMovement = Mathf.Abs(ball.transform.position.x - lastBallPosition.x);
        float ballYMovement = Mathf.Abs(ball.transform.position.y - lastBallPosition.y);

        Debug.Log($"X movement: {ballXMovement}\nY movement: {ballYMovement}");

        if (ballYMovement < 0.8 && ballXMovement < 0.8)
        {
            ballStuckTime += Time.deltaTime;
            Debug.Log($"Stuck Time: {ballStuckTime}");
        }
        else
        {
            ballStuckTime = 0f;
        }

        if (ballStuckTime > ballStuckThreshold)
        {
            Debug.Log($"Ball is stuck at position: {ball.transform.position}");
            HandleStuckBall();
            ballStuckTime = 0f;
        }
        
        lastBallPosition = ball.transform.position;
    }

    private void HandleStuckBall()
    {
        // Debug.Break();
        if (ball.transform.position.y >= transform.position.y)
        {
            // Ball is stuck on the head, move to the side and jump
            float direction = ball.transform.position.x > transform.position.x ? -1 : 1;
            rb.velocity = new Vector2(direction * speed, jumpForce);
        }
        else
        {
            // Ball is under the bot, move to the side
            float direction = ball.transform.position.x > transform.position.x ? -1 : 1;
            rb.velocity = new Vector2(direction * speed, rb.velocity.y);
        }
    }

    private void Move()
    {
        // if (BallIsOverhead())
        // {
        //     MoveFromBall();
        //     return;
        // }

        if (HasToDefend())
        {
            Defend();
            return;
        }

        if (IsFieldClear())
        {
            MoveTowardsGoal();
            return;
        }

        AttackOrDefend();
    }

    private void AttackOrDefend()
    {
        float ballDistance = Vector3.Distance(ball.transform.position, transform.position);
        float opponentDistance = Vector3.Distance(opponent.transform.position, transform.position);
        float ballToGoalDistance = Vector3.Distance(ball.transform.position, defense.position);

        if (transform.CompareTag("Enemy"))
        {
            MoveEnemyBot(ballDistance, opponentDistance, ballToGoalDistance);
        }
        else
        {
            MovePlayerBot(ballDistance, opponentDistance, ballToGoalDistance);
        }
    }

    private void MoveEnemyBot(float ballDistance, float opponentDistance, float ballToGoalDistance)
    {
        if (ball.transform.position.x < transform.position.x || ballDistance >= opponentDistance)
        {
            MoveTowardsPosition(ball.transform.position.x);
        }
        else if (ballToGoalDistance > defenseRangeMax || ballToGoalDistance < defenseRangeMin)
        {
            MoveTowardsPosition(defense.position.x);
        }
    }

    private void MovePlayerBot(float ballDistance, float opponentDistance, float ballToGoalDistance)
    {
        if (ball.transform.position.x > transform.position.x || ballDistance >= opponentDistance)
        {
            MoveTowardsPosition(ball.transform.position.x);
        }
        else if (ballToGoalDistance > defenseRangeMax || ballToGoalDistance < defenseRangeMin)
        {
            MoveTowardsPosition(defense.position.x);
        }
    }

    private void MoveTowardsPosition(float xPosition)
    {

        //! not right but better result
        // Vector3 targetPosition = new Vector3(xPosition, transform.position.y, transform.position.z);
        // transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
        
        //* right but bot is stupid
        if (MathF.Abs(ball.transform.position.x - transform.position.x) > 5)
        {
            if (canDash && !isDashing)
            {
                int dashDirection = xPosition > transform.position.x ? 1 : -1;
                StartCoroutine(Dash(dashDirection));
            }
        }
        else 
        {
            Vector3 targetPosition = new Vector3(xPosition, transform.position.y, transform.position.z);
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * speed * Time.deltaTime;
        }

        // Vector3 targetPosition = new Vector3(xPosition, transform.position.y, transform.position.z);
        // Vector3 moveDirection = (targetPosition - transform.position).normalized;
        // transform.position += moveDirection * speed * Time.deltaTime;
    }

    // private bool BallIsOverhead()
    // {
    //     return Mathf.Abs(ball.transform.position.x - transform.position.x) < 0.5f && ball.transform.position.y >= transform.position.y && ball.transform.position.y - transform.position.y < 0.5f;
    // }

    // private void MoveFromBall()
    // {
    //     float direction = ball.transform.position.x > transform.position.x ? -1 : 1;
    //     rb.velocity = new Vector2(direction * speed, rb.velocity.y);
    // }

    private bool IsFieldClear()
    {
        float botToGoalDistance = Vector3.Distance(transform.position, opponentGoalPosition);
        float opponentToGoalDistance = Vector3.Distance(opponent.transform.position, opponentGoalPosition);
        return botToGoalDistance < opponentToGoalDistance;
    }

    private void MoveTowardsGoal()
    {
        MoveTowardsPosition(ball.transform.position.x);

        if (moveLeftToRight)
        {
            MoveBotWithBallDirection();
        }
        else
        {
            MoveBotAgainstBallDirection();
        }
    }

    private void MoveBotWithBallDirection()
    {
        if (transform.position.x < ball.transform.position.x)
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        else
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
    }

    private void MoveBotAgainstBallDirection()
    {
        if (transform.position.x > ball.transform.position.x)
        {
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        else
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);
        }
    }

    private bool HasToDefend()
    {
        float ballToGoalDistance = Vector3.Distance(ball.transform.position, defense.position);
        float botToGoalDistance = Vector3.Distance(transform.position, defense.position);

        if (moveLeftToRight)
        {
            return ball.transform.position.x < transform.position.x && ballToGoalDistance < defenseRangeMax;
        }
        else
        {
            return ball.transform.position.x > transform.position.x && ballToGoalDistance < defenseRangeMax;
        }
    }

    private void Defend()
    {
        MoveTowardsPosition(ball.transform.position.x);
        if (Vector3.Distance(transform.position, ball.transform.position) < kickRange)
        {
            Kick();
        }
    }

    private void Jump()
    {
        float ballDistance = Vector3.Distance(ball.transform.position, transform.position);

        if (ballDistance <= 3 && ball.transform.position.y > transform.position.y && isGrounded && random.NextDouble() < 0.5)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            GameManager.Instance.matchTelemetryStruct.opponentAction = "Jump";
            // GameLogger.Instance.LogEvent("Bot Jumped at Position: " + transform.position);
        }
    }

    private void Kick()
    {
        float opponentDistance = Vector3.Distance(opponent.transform.position, transform.position);
        float ballDistance = Vector3.Distance(ball.transform.position, transform.position);
        if ((opponentDistance < kickRange) ^ (ballDistance < kickRange) && random.NextDouble() < 0.5 && canKick)
        {
            kickPressed = true;
            GameManager.Instance.matchTelemetryStruct.opponentAction = "Kick";
            // GameLogger.Instance.LogEvent("Bot Kicked at Position: " + transform.position);
            StartCoroutine(KickCooldown());
        }
        else
        {
            kickPressed = false;
        }
    }

    private void UsePowerUp(float prob = 0.5f)
    {
        if (powerReady && random.NextDouble() < prob)
        {
            powerSetUp = true;
			GameManager.Instance.matchTelemetryStruct.opponentAction = "Power Set Up";

            Animator bodyAnimator = transform.Find("Body").GetComponent<Animator>();
            bodyAnimator.enabled = true;

            progressBar.current = 0f;
        }
    }

    public void PowerUsed()
    {
        GameManager.Instance.matchTelemetryStruct.opponentAction = "Power Used";
        
        powerReady = false;
        powerSetUp = false;

        Animator bodyAnimator = transform.Find("Body").GetComponent<Animator>();
        bodyAnimator.enabled = false;

        SpriteRenderer bodySprite = transform.Find("Body").GetComponent<SpriteRenderer>();
        bodySprite.color = Color.white;
    }

    private void ReactToOpponentPower()
    {
        bool opponent_powerSetUp = GetOpponentPowerState("powerSetUp");
        if (opponent_powerSetUp && powerReady)
        {
            UsePowerUp(prob: 1f);
        }

        bool opponent_isUsingPower = GetOpponentPowerState("isUsingPower");
        if (ball.GetComponent<Ball>().isShooting && opponent_isUsingPower)
        {
            StartCoroutine(WaitAndJump());
        }
    }

    private bool GetOpponentPowerState(string property)
    {
        var opponentBot = opponent.GetComponent<Bot>();
        if (opponentBot != null)
        {
            var prop = opponentBot.GetType().GetProperty(property);
            if (prop != null)
            {
                return (bool)prop.GetValue(opponentBot);
            }
        }
        else
        {
            var opponentPlayer = opponent.GetComponent<PlayerMovement>();
            if (opponentPlayer != null)
            {
                var prop = opponentPlayer.GetType().GetProperty(property);
                if (prop != null)
                {
                    return (bool)prop.GetValue(opponentPlayer);
                }
            }
        }
        return false;
    }

    private void HandleBotOverlap()
    {
        float overlapThreshold = 0.1f;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, overlapThreshold);
        foreach (var collider in colliders)
        {
            if (collider.gameObject != gameObject && (collider.CompareTag("Player") || collider.CompareTag("Enemy")))
            {
                HandleOverlapMovement(collider);
            }
        }
    }

    private void HandleOverlapMovement(Collider2D collider)
    {
        if (collider.transform.position.y > transform.position.y)
        {
            float direction = random.NextDouble() < 0.5 ? 1 : -1;
            rb.velocity = new Vector2(direction * speed, rb.velocity.y);
        }
    }

    public void TakeDamage(int direction)
    {
        Vector3 knockbackPosition = transform.position + direction * knockbackDistance * Vector3.right;
        GameManager.Instance.matchTelemetryStruct.opponentAction = "Damage Taken";
        // GameLogger.Instance.LogEvent("Bot Took Damage at Position: " + transform.position);
        StartCoroutine(Knockback(knockbackPosition));
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    private void AnimateKick()
    {
        footAnimator.SetBool("kick", kickPressed);
    }

    private IEnumerator KickCooldown()
    {
        canKick = false;
        yield return new WaitForSeconds(kickCooldown);
        canKick = true;
    }

    private IEnumerator Knockback(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 0.2f;
        Vector3 startingPosition = transform.position;
        transform.Find("Body").GetComponent<SpriteRenderer>().color = new Color(1f, 0.482f, 0.482f);

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.Find("Body").GetComponent<SpriteRenderer>().color = Color.white;
    }

    private IEnumerator Dash(int direction)
    {
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

    private IEnumerator WaitAndJump()
    {
        float waitTime = random.Next(1, 3);

        if (canDash && !isDashing)
        {
            int dashDirection = moveLeftToRight ? -1 : 1;
            StartCoroutine(Dash(dashDirection));
        }

        yield return new WaitForSeconds(waitTime);

        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }
}
