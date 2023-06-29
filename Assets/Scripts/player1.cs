using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player1 : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed = 500;
    private float jumpSpeed = 500;
    // Start is called before the first frame update
    Vector2 movement;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement = new Vector2(horizontal, vertical);
        // rb.velocity = new Vector2(getX, getY)*speed;
    }
    private void FixedUpdate() {
        rb.velocity = new Vector2(movement.x*speed*Time.deltaTime, rb.velocity.y);
        if(Input.GetKeyDown(KeyCode.W)){
            rb.AddForce(Vector2.up*jumpSpeed);
        }
    }
}
