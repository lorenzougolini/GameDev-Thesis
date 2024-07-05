using System.Collections;
using UnityEngine;

public class Bot : MonoBehaviour
{

    public float defenseRange;
    public float speed, jump;
    private Transform defense;
    public GameObject ball;
    Rigidbody2D rb;
    public bool isGrounded = false;

    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        rb = GetComponent<Rigidbody2D>();
        
        // defence is a child named "Defense"
        defense = transform.Find("Defense");
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
    }

    private void Move() {
        if (Mathf.Abs(ball.transform.position.x - transform.position.x) > defenseRange) {
            if (ball.transform.position.x > transform.position.x)
                transform.Translate(Time.deltaTime * speed, 0, 0);
            else if (ball.transform.position.x < transform.position.x)
                transform.Translate(-Time.deltaTime * speed, 0, 0);
            else if (ball.transform.position.x == transform.position.x)
                transform.position = new Vector2(transform.position.x + 1.5f, transform.position.y);
        } else {
            if (transform.position.x < defense.position.x)
                transform.Translate(Time.deltaTime * speed, 0, 0);
            else
                transform.Translate(0, 0, 0);
        }
    }

    private void Jump() {
        float dist = Vector2.Distance(ball.transform.position, transform.position);
        if (dist < 1 && isGrounded)
            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            isGrounded = false;
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
