using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class player1 : NetworkBehaviour
{
    
    [SerializeField] private Transform checkGroundPosition;
    [SerializeField] private float checkGroundRadius;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2d;
    private Animator ani;
    private SpriteRenderer sprite;
    [SerializeField] private LayerMask GroundLayer;
    private float jumpSpeed = 18;
    private float dirX;
    // Vector2 movement;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        boxCollider2d = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     dirX = Input.GetAxisRaw("Horizontal");
    //     CheckKeyboardInputs(dirX);
    // }

    private void CheckKeyboardInputs(float dirX)
    {
        if (isGrounded() && Input.GetKeyDown(KeyCode.W))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

    private void playerMove(float dirX)
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (isGrounded())
            {
                ani.SetBool("sitting", true);
                GetComponent<Rigidbody2D>().velocity = new Vector2(dirX * 0f, 0);
            }
        }
        else
        {
            ani.SetBool("sitting", false);
            // rb.velocity = new Vector2(dirX * 9f, rb.velocity.y);
            // rb.AddForce(new Vector2(dirX * 9, 0));
            updateAnimation(dirX);
        }
    }

    private void updateAnimation(float dirX)
    {
        if (dirX > 0f)
        {
            ani.SetBool("running", true);
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            ani.SetBool("running", true);
            sprite.flipX = true;
        }
        else
        {
            ani.SetBool("running", false);
        }
    }

    public bool isGrounded()
    {
        return Physics2D.OverlapCircle(checkGroundPosition.position, checkGroundRadius, GroundLayer);
    }
}