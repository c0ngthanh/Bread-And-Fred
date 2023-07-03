using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class player1multiplayer : NetworkBehaviour
{
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
    void Update()
    {
        if(!IsOwner) return;
        // float dirX = Input.GetAxisRaw("Horizontal");
        // playerMove(dirX);
        // if (isGrounded() && Input.GetKeyDown(KeyCode.W))
        // {
        //     GetComponent<Rigidbody2D>().velocity = new Vector3(0, 18, 0);
        // }
        // float horizontal = Input.GetAxisRaw("Horizontal");
        // float vertical = Input.GetAxisRaw("Vertical");
        // movement = new Vector2(horizontal, vertical);
        // // rb.velocity = new Vector2(getX, getY)*speed;
        dirX = Input.GetAxisRaw("Horizontal");
    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        playerMove(dirX);
        if (isGrounded() && Input.GetKeyDown(KeyCode.W))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(rb.velocity.x,jumpSpeed);
        }
        // rb.velocity = new Vector2(movement.x * speed * Time.deltaTime, rb.velocity.y);
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     rb.AddForce(Vector2.up * jumpSpeed);
        // }
    }

    private void playerMove(float dirX)
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (isGrounded())
            {
                ani.SetBool("sitting", true);
            }
        }
        else
        {
            ani.SetBool("sitting", false);

            rb.velocity = new Vector2(dirX * 9f, rb.velocity.y);
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

    private bool isGrounded()
    {
        // return transform.Find("GroundCheck").GetComponent<GroundCheck>().isGrounded;
        float extraHeightText = 1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, extraHeightText, GroundLayer);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, boxCollider2d.bounds.extents.y), Vector2.right * (boxCollider2d.bounds.extents.x), rayColor);
        // Debug.Log(raycastHit.collider);
        return raycastHit.collider != null;

    }
}