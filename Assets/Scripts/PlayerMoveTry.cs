using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveTry : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpForce = 16f;

    private bool kickbool;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private Animator animator;

    private void Start() {
        animator = GetComponentInChildren<Animator> ();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal1");      
        if (Input.GetButtonDown("Vertical1") && isGrounded())
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        if (Input.GetButtonDown("Vertical1") && rb.velocity.y > 0f)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        kickbool = System.Convert.ToBoolean(Input.GetAxis("Jump1"));
    }

    private bool isGrounded(){
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        animator.SetBool ("kick", kickbool);
    }
}
