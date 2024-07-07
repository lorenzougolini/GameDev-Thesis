using System.Collections;
using UnityEngine;

public class Bot : MonoBehaviour
{

    public float speed, dashSpeed, jumpForce, defenseRange, kickRange;

    private GameObject ball;
    private GameObject opponent;

    private Transform defense;
    private Rigidbody2D rb;
    
    public bool isGrounded = false;

    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        opponent = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        
        // defence is a child named "Defense"
        defense = transform.Find("Defense");
    }

    // Update is called once per frame
    void Update() {
        if (!Gui.S.playing) return;

        Move();
        Jump();
        Kick();
    }

    private void Move() {

        float ballDistance = Vector3.Distance(ball.transform.position, transform.position);
        float opponentDistance = Vector3.Distance(opponent.transform.position, transform.position);

        if (ballDistance > defenseRange) {
            Vector3 targetPosition = new Vector3(ball.transform.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
        } else {
            if (transform.position.x < defense.position.x) {
                transform.Translate(Time.deltaTime * speed, 0, 0);
            }
        }

        if (ballDistance > defenseRange * 1.5 && opponentDistance > defenseRange * 1.5) {
            Vector3 dashDirection = (ball.transform.position - transform.position).normalized;
            // TODO: add Dash function from player
            // rb.AddForce(dashDirection * dashSpeed, ForceMode2D.Impulse);
        }
    }

    private void Jump() {
        float dist = Vector2.Distance(ball.transform.position, transform.position);

        if (dist < 1 && ball.transform.position.y > transform.position.y && isGrounded) {
            Vector2 jumpDirection = Vector2.up + Vector2.left;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void Kick() {

        // TODO: use GameManager.KickOpponent
        // float opponentDistance = Vector3.Distance(opponent.transform.position, transform.position);

        // if (opponentDistance < kickRange) 
        // {
        //     Vector2 kickDirection = (opponent.transform.position - transform.position).normalized;
        //     rb.AddForce(kickDirection * jumpForce, ForceMode2D.Impulse);  // Adjust the force as needed for kicking
        // }
    }
    
    private void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D coll) {
        if (coll.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
