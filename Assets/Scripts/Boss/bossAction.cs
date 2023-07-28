using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class bossAction : MonoBehaviour
{

    [SerializeField] private BossData bossData;
    [SerializeField] private GameObject laserAttack;
    [SerializeField] private SpriteRenderer laserAttack_sprite;


    public HealthBar healthBar;
    private SpriteRenderer sprite;
    private Animator ani;
    private Vector3 movement;
    private GameObject player1;
    private GameObject player2;


    public float maxHealth = 50;
    public float currentHealth;
    private int direction = 1;
    private int follow = 0;
    public int S = 500;
    private float distance1 = 1000, distance2 = 1000, distance = 0;
    private int typeAction = 2;
    private float speed = 2;
    private float angrySpeed;
    private bool die = false;
    private bool isAttack = false;

    private bool attackPlayer = false;


    private NetworkVariable<bool> isFacing = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> BossHP = new NetworkVariable<float>();



    private void Awake()
    {
        // đọc dữ liệu từ bảng data lên trên thuộc tính của nó được up lên cloud
        BossHP.Value = 100;
        if (bossData.speed != 0)
        {
            speed = bossData.speed;
        }

        angrySpeed = bossData.speed * 2;
        // speed = normalSpeed;
        // Debug.Log("speed_boss: " + speed);

        // player1 = GameObject.FindGameObjectWithTag("Player");
        // Debug.Log(player1);

        GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
        if (gameObjectArray.Length == 2)
        {
            player1 = gameObjectArray[0];
            player2 = gameObjectArray[1];
        }
        else if (gameObjectArray.Length == 1)
        {
            player1 = gameObjectArray[0];
            player2 = gameObjectArray[0];
        }




        // player2 = GameObject.FindGameObjectWithTag("Player2");
        // player2 = GetComponent<Transform>();
        sprite = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        // HP.OnValueChanged += OnHPValueChanged;
    }

    public void SetHP(float HP)
    {
        int dmg = (int)this.BossHP.Value - (int)HP;
        this.BossHP.Value = HP;
        // Debug.Log(this.BossHP);
        TakeDamage(dmg);
    }
    public NetworkVariable<float> GetHP()
    {
        return this.BossHP;
    }
    public BossData GetBossData()
    {
        return this.bossData;
    }

    public bool GetAttackPlayer()
    {
        return attackPlayer;
    }

    public void SetAttackPlayer(bool type)
    {
        attackPlayer = type;
    }

    public int GetTypeAction()
    {
        return this.typeAction;
    }




    private void Update()
    {
        if (player1 == null || player2 == null)
        {
            GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
            if (gameObjectArray.Length == 2)
            {
                player1 = gameObjectArray[0];
                player2 = gameObjectArray[1];
            }
            else if (gameObjectArray.Length == 1)
            {
                player1 = gameObjectArray[0];
                player2 = gameObjectArray[0];
            }

        }

        Debug.Log(player1.transform.position + " " + player2.transform.position);



        distance1 = Vector3.Distance(transform.position, player1.transform.position);
        distance2 = Vector3.Distance(transform.position, player2.transform.position);
        if (distance1 < distance2)
        {
            distance = distance1;
        }
        else
        {
            distance = distance2;
        }
        Debug.Log("min distance " + distance);

        if (follow == 0)
        {
            if (distance > 15.0)
            {
                AutoMovement();
            }
            else
            {
                FollowPlayer();
                follow = 1;
            }
        }
        else if (follow == 1 || isAttack)
        {
            FollowPlayer();
        }




        if (currentHealth <= 0)
        {
            CallStatus(0);
        }

    }

    private void AutoMovement()
    {
        Debug.Log("typeaction: " + typeAction);
        if (typeAction != 1)
        {
            movement = new Vector3(speed * direction, 0f);
            transform.position = transform.position + movement * Time.deltaTime;
            // Debug.Log(speed + " " + transform.position + " " + "type  " + typeAction);
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
        Vector3 distVector = player1.transform.position - transform.position;

        // Debug.Log(distVector);

        // boss follow player
        if (typeAction != 1)
        {
            if (currentHealth > 80)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, player1.transform.position, 1.5f * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(this.transform.position, player1.transform.position, 2 * 1.5f * Time.deltaTime);
            }

        }
        if (distVector.x < 0)
        {
            sprite.flipX = true;
            laserAttack_sprite.flipX = true;
            // SpriteRenderer sample = laserAttack_sprite;
            // laserAttack_sprite.transform.position = new Vector3(2 * sprite.transform.position.x - sample.transform.position.x, laserAttack_sprite.transform.position.y, 0);

        }
        else
        {
            sprite.flipX = false;
            laserAttack_sprite.flipX = false;

        }
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(20);
            Debug.Log("trigger wuth player");
        }
    }

    void DoDelayAction(float delayTime)
    {
        StartCoroutine(DelayAction(delayTime));
    }

    IEnumerator DelayAction(float delayTime)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        //Do the action after the delay time has finished.
        if (die)
        {
            Destroy(gameObject);
        }

    }


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
                die = true;
                DoDelayAction(1);
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
                // raycastHit2D = Physics2D.Raycast(transform.position, direction ? new Vector2(1, 0) : new Vector2(-1, 0), attackRange, playerLayer);
                // Debug.DrawRay(transform.position, isFacingRight.Value ? new Vector2(attackRange, 0) : new Vector2(-attackRange, 0), Color.red, 0.1f);

                break;
            default: // normal
                ani.SetBool("Die", false);
                ani.SetBool("Block", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("LaserAttack", false);
                break;
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        healthBar.SetHealth(currentHealth);
        if (!isAttack)
        {
            isAttack = true;
        }

    }

}
