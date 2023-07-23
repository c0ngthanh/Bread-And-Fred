using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class bossAction : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth = 100;
    public HealthBar healthBar;
    private SpriteRenderer sprite;
    private Animator ani;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject laserAttack;
    [SerializeField] private SpriteRenderer laserAttack_sprite;



    private int direction = 1;
    private int follow = 0;
    private Vector3 movement;
    public int S = 500;
    private float distance;
    private int typeAction;



    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (follow == 0)
        {
            AutoMovement();
        }
        else
        {
            FollowPlayer();
        }

        distance = Vector3.Distance(transform.position, player.position);
        Debug.Log(distance + "//" + currentHealth);
        if (distance < 10.0)
        {
            follow = 1;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
        if (currentHealth == 0)
        {
            CallStatus(0);
        }

    }

    private void AutoMovement()
    {
        if (typeAction != 1)
        {
            movement = new Vector3(2 * direction, 0f);
            transform.position = transform.position + movement * Time.deltaTime;
            S--;
        }

        if (S == 0)
        {
            S = 500;
            direction = direction * -1;
        }
        if (direction > 0)
        {
            sprite.flipX = false;

        }
        else
        {
            sprite.flipX = true;
        }
    }

    private void FollowPlayer()
    {
        //lấy vector từ player --> boss
        Vector3 distVector = player.position - transform.position;
        Debug.Log(distVector);

        // boss follow player
        if (typeAction != 1)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, 1.5f * Time.deltaTime);

        }
        if (distVector.x < 0)
        {
            sprite.flipX = true;
            laserAttack_sprite.flipX = true;
        }
        else
        {
            sprite.flipX = false;
            laserAttack_sprite.flipX = false;

        }


        // if (HP > 0)
        // {
        // just follow and melee attack
        // if (distance < 7.0)
        // {
        //     CallStatus(4);
        // }
        // else
        // {
        //     CallStatus(2);
        // }
        // }
        // else if (HP > 20){
        //     // follow + melee attack + laser attach
        if (distance < 7.0)
        {
            CallStatus(3);
            typeAction = 3;
        }
        else if (17.0 < distance && distance < 22.0)
        {
            CallStatus(4);
            typeAction = 4;
        }
        else if (26.0 <= distance && distance < 32.0)
        {
            CallStatus(1);
            typeAction = 1;
        }
        else
        {
            CallStatus(2);
            typeAction = 2;
        }
        // }
        // else if (HP > 0)
        // {
        //     // follow + block + melee attack + laser attack 
        //     if (distance < 7.0)
        //     {
        //         CallStatus(3);
        //     }
        //     else if (10.0 < distance && distance < 15.0)
        //     {
        //         CallStatus(4);
        //     }
        //     else
        //     {
        //         CallStatus(1);
        //     }
        // }
        // else
        // {
        //     // death
        //     CallStatus(0);
        // }

        // if (distance < 7.0 && HP > 80)
        // {

        //     ani.SetBool("MeleeAttack", true);
        // }
        // else if (distance < 15.0 && HP > 0 && HP <= 80)
        // {

        //     ani.SetBool("LaserAttack", true);
        // }
        // else
        // {
        //     ani.SetBool("LaserAttack", false);

        // }
    }

    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         Debug.Log("trigger");
    //         // updateDirection(rb.velocity);
    //         // AppleTxt.text = "Apple: " + apples;
    //     }

    // }


    //  MeleeAttack: 3
    //  LaserAttack: 4
    //  normal:      2
    //  Block        1
    //  Death:       0
    private void CallStatus(int type)
    {
        switch (type)
        {
            case 0: //death
                ani.SetBool("Die", true);
                ani.SetBool("Block", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("LaserAttack", false);
                // Destroy(laserAttack);
                // Destroy(sprite);
                Destroy(this, 5);
                break;
            case 1: // block
                ani.SetBool("Block", true);
                ani.SetBool("Die", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("LaserAttack", false);

                break;
            case 3: // melee attack
                ani.SetBool("MeleeAttack", true);
                ani.SetBool("Die", false);
                ani.SetBool("Block", false);
                ani.SetBool("LaserAttack", false);
                break;
            case 4: // laser attack
                ani.SetBool("LaserAttack", true);
                ani.SetBool("Die", false);
                ani.SetBool("Block", false);
                ani.SetBool("MeleeAttack", false);

                break;
            default: // normal
                ani.SetBool("Die", false);
                ani.SetBool("Block", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("LaserAttack", false);
                break;
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        healthBar.SetHealth(currentHealth);
    }

}
