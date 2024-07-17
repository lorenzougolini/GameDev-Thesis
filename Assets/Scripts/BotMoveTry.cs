using UnityEngine;
using System.Collections;

public class BotMovement : MonoBehaviour {

    public int playerNumber;
    public float speed = 8f;
    public float jumpForce = 16f;

    private bool canDash = true;
    private bool isDashing;
    private float dashPower = 16f;
    private float dashTime = 0.2f;
    private float dashCooldown = 0.5f;

    private float knockbackDistance = 1.5f;

    public bool powerReady = false;
    public bool powerSetUp = false;
    public bool isUsingPower = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    private Animator footAnimator;

    private ProgressBar progressBar;

    private GameObject ball;
    private Vector3 targetPosition;
    private bool kickPressed;

    void Start() {
        footAnimator = transform.Find("Foot").GetComponent<Animator>();
        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    void Update() {
        if (!Gui.S.playing) return;

        // Move towards the ball
        targetPosition = ball.transform.position;
        float horizontal = targetPosition.x > transform.position.x ? 1 : -1;

        // Jump if the ball is in the air and close enough
        if (isGrounded() && Mathf.Abs(targetPosition.x - transform.position.x) < 1f && targetPosition.y > transform.position.y) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            GameLogger.Instance.LogEvent("Bot " + playerNumber + " Jumped at Position: " + transform.position);
        }

        // Kick if close to the ball
        kickPressed = Vector3.Distance(transform.position, targetPosition) < 1.5f;
        if (kickPressed)
            GameLogger.Instance.LogEvent("Bot " + playerNumber + " Kicked at Position: " + transform.position);

        // Dash if needed (example logic, can be refined)
        if (!ResetObjects.S.resetting && Mathf.Abs(targetPosition.x - transform.position.x) > 5f) {
            HandleDoubleClickDash(horizontal);
        }
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        footAnimator.SetBool("kick", kickPressed);

        // Powerup logic (example, refine as needed)
        if (powerReady) {
            powerSetUp = true;
            Animator bodyAnimator = transform.Find("Body").GetComponent<Animator>();
            bodyAnimator.enabled = true;
            // Assuming progressBar is assigned correctly
            progressBar.current = 0f;
        }
    }

    bool isGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void HandleDoubleClickDash(float horizontal) {
        if (canDash && isGrounded()) {
            GameLogger.Instance.LogEvent("Bot " + playerNumber + " Dashed at Position: " + transform.position);
            StartCoroutine(Dash(horizontal));
        }
    }

    void TakeDamage(int direction) {
        Vector3 knockbackPosition = transform.position + direction * knockbackDistance * Vector3.right;
        GameLogger.Instance.LogEvent("Bot " + playerNumber + " Took Damage at Position: " + transform.position);
        StartCoroutine(Knockback(knockbackPosition));
    }

    IEnumerator Dash(float direction) {
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

    IEnumerator Knockback(Vector3 targetPosition) {
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
