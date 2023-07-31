using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class bossAction : NetworkBehaviour
{

    [SerializeField] private BossData bossData;
    [SerializeField] private GameObject laserAttack;
    [SerializeField] private SpriteRenderer laserAttack_sprite;


    // public HealthBar healthBar;
    private SpriteRenderer sprite;
    private Animator ani;
    private Vector3 movement;
    private GameObject player1;
    private GameObject player2;


    public float maxHealth = 100;
    private int direction = 1;
    private NetworkVariable<bool> follow = new NetworkVariable<bool>();
    public int S = 500;
    private float distance1 = 1000, distance2 = 1000, distance = 0;
    // private int typeAction = 2;
    private float speed = 2;
    private NetworkVariable<float> angrySpeed = new NetworkVariable<float>();
    private NetworkVariable<bool> die = new NetworkVariable<bool>();
    private NetworkVariable<bool> isAttack = new NetworkVariable<bool>();

    private NetworkVariable<bool> attackPlayer = new NetworkVariable<bool>();

    public NetworkVariable<float> currentHealth = new NetworkVariable<float>();
    public NetworkVariable<int> typeAction = new NetworkVariable<int>();



    private void Awake()
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


        sprite = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        // healthBar.enabled = false;
        currentHealth.Value = maxHealth;
        typeAction.Value = 2;

        // healthBar.SetMaxHealth(maxHealth);
        // currentHealth.OnValueChanged += UpdateCurrentHealth;

        attackPlayer.Value = false;
        isAttack.Value = false;
        die.Value = false;
        angrySpeed.Value = speed;
        follow.Value = false;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetHealthServerRpc(int HP)
    {
        if (IsServer)
        {
            this.currentHealth.Value = HP;
        }
    }



    public NetworkVariable<bool> GetAttackPlayer()
    {
        return this.attackPlayer;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetAttackPlayerServerRpc(bool type)
    {
        if (IsServer)
        {
            attackPlayer.Value = type;
        }
    }



    public NetworkVariable<float> GetHealth()
    {
        return this.currentHealth;
    }
    public NetworkVariable<int> GetTypeAction()
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

        if (IsServer)
        {
            if (currentHealth.Value > 60)
            {

                angrySpeed.Value *= 3;
            }
            else if (currentHealth.Value > 40)
            {
                angrySpeed.Value = 4;
            }
            else
            {
                angrySpeed.Value = 5;
            }
        }

        // Debug.Log("min distance " + distance);

        if (!follow.Value)
        {
            if (distance > 15.0)
            {
                AutoMovement();
            }
            else
            {
                FollowPlayer();
                follow.Value = true;
            }
        }
        else if (follow.Value || isAttack.Value)
        {
            FollowPlayer();
        }

        if (currentHealth.Value <= 0)
        {
            CallStatus(0);
        }

    }

    private void AutoMovement()
    {
        if (typeAction.Value != 1)
        {
            movement = new Vector3(speed * direction, 0f);
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
        Vector3 distVector = player1.transform.position - transform.position;

        if (typeAction.Value != 1)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player1.transform.position, 3 * Time.deltaTime);
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
        if (distance < 7.0)
        {
            CallStatus(3);
            typeAction.Value = 3;
        }
        else if (17.0 < distance && distance < 22.0)
        {
            CallStatus(4);
            typeAction.Value = 4;
        }
        else if (26.0 <= distance && distance < 32.0)
        {
            CallStatus(1);
            typeAction.Value = 1;
        }
        else
        {
            CallStatus(2);
            typeAction.Value = 2;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && this.typeAction.Value == 3)
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(5);
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
        if (die.Value)
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
                if (IsServer)
                {
                    die.Value = true;
                }
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
                break;
            default: // normal
                ani.SetBool("Die", false);
                ani.SetBool("Block", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("LaserAttack", false);
                break;
        }
    }

    // public void UpdateCurrentHealth(float previousValue, float newValue)
    // {

    //     // healthBar.SetHealth(newValue);

    //     if (IsServer)
    //     {
    //         if (newValue > 60)
    //         {

    //             angrySpeed.Value *= 3;
    //         }
    //         else if (newValue > 40)
    //         {
    //             angrySpeed.Value = 4;
    //         }
    //         else
    //         {
    //             angrySpeed.Value = 5;
    //         }
    //     }
    // }

    public void TakeDamage(float dmg)
    {
        if (IsServer)
        {
            currentHealth.Value -= dmg;
            if (!isAttack.Value)
            {
                isAttack.Value = true;
            }

        }

    }

}
