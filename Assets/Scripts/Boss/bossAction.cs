using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class bossAction : NetworkBehaviour
{

    [SerializeField] private SpriteRenderer laserAttack_sprite;


    // public HealthBar healthBar;
    private SpriteRenderer sprite;
    private Animator ani;
    private Vector3 movement;
    private GameObject player1;
    private GameObject player2;

    // public new Animation animation;


    public float maxHealth = 100;
    private int direction = 1;
    private NetworkVariable<bool> follow = new NetworkVariable<bool>();
    public int S = 500;
    private float distance1 = 1000, distance2 = 1000, distance = 0;
    // private int typeAction = 2;
    private float speed = 2f;
    private NetworkVariable<float> angrySpeed = new NetworkVariable<float>();
    private NetworkVariable<bool> die = new NetworkVariable<bool>();
    private NetworkVariable<bool> isAttacked = new NetworkVariable<bool>();

    public NetworkVariable<float> currentHealth = new NetworkVariable<float>();

    // cập nhật animation cho boss
    public NetworkVariable<int> typeAction = new NetworkVariable<int>();

    // fight: true --> đang chiến đấu, bật health bar
    //        false --> không chiến đấu, tắt health bar
    public NetworkVariable<bool> fight = new NetworkVariable<bool>();

    public NetworkVariable<bool> grow = new NetworkVariable<bool>();
    public NetworkVariable<int> minLengthMeleeAttack = new NetworkVariable<int>();



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


        // animation = GetComponent<Animation>();

        sprite = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        currentHealth.Value = maxHealth;
        typeAction.Value = 2;
        isAttacked.Value = false;
        die.Value = false;
        angrySpeed.Value = speed;
        follow.Value = false;
        fight.Value = false;
        grow.Value = false;
        // increaseCount.Value = 2;
        minLengthMeleeAttack.Value = 7;
    }



    [ServerRpc(RequireOwnership = false)]
    public void SetHealthServerRpc(int HP)
    {
        if (IsServer)
        {
            this.currentHealth.Value = HP;
        }
    }

    public NetworkVariable<float> GetHealth()
    {
        return this.currentHealth;
    }



    [ServerRpc(RequireOwnership = false)]
    public void SetFightSatusServerRpc(bool status)
    {
        if (IsServer)
        {
            this.fight.Value = status;
        }
    }

    public NetworkVariable<bool> GetFight()
    {
        return this.fight;
    }



    [ServerRpc(RequireOwnership = false)]
    public void SetMinLengthMeleeAttackServerRpc(int num)
    {
        if (IsServer)
        {
            this.minLengthMeleeAttack.Value = num;
        }
    }

    public NetworkVariable<int> GetMinLengthMeleeAttack()
    {
        return this.minLengthMeleeAttack;
    }



    [ServerRpc(RequireOwnership = false)]
    public void SetDieServerRpc(bool num)
    {
        if (IsServer)
        {
            this.die.Value = num;
        }
    }

    public NetworkVariable<bool> GetDie()
    {
        return this.die;
    }



    [ServerRpc(RequireOwnership = false)]
    public void SetGrowServerRpc(bool num)
    {
        if (IsServer)
        {
            this.grow.Value = num;
        }
    }

    public NetworkVariable<bool> GetGrow()
    {
        return this.grow;
    }



    [ServerRpc(RequireOwnership = false)]
    public void SetFollowServerRpc(bool status)
    {
        if (IsServer)
        {
            this.follow.Value = status;
        }
    }

    public NetworkVariable<bool> GetFollow()
    {
        return this.follow;
    }



    [ServerRpc(RequireOwnership = false)]
    public void SetAngrySpeedServerRpc(float sp)
    {
        if (IsServer)
        {
            this.angrySpeed.Value = sp;
        }
    }
    public NetworkVariable<float> GetAngrySpeed()
    {
        return this.angrySpeed;
    }




    [ServerRpc(RequireOwnership = false)]
    public void SetTypeActionServerRpc(int type)
    {
        if (IsServer)
        {
            this.typeAction.Value = type;
        }
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

        if (player1.transform.position.y > 142.69f && player2.transform.position.y > 142.69f)
        {
            SetFightSatusServerRpc(true);
        }

        // Debug.Log("currenthealth - boss: " + currentHealth.Value);
        if (currentHealth.Value > 60 && currentHealth.Value < 80)
        {

            SetAngrySpeedServerRpc(4f);

        }
        else if (currentHealth.Value > 40 && currentHealth.Value < 60)
        {
            SetAngrySpeedServerRpc(5f);
        }
        else if (currentHealth.Value < 40)
        {
            SetAngrySpeedServerRpc(6f);
        }


        // Debug.Log("angry speed" + angrySpeed.Value); 


        if (!follow.Value)
        {
            if (distance < 30.0)
            {
                FollowPlayer();
                SetFollowServerRpc(true);
            }
        }
        else if (follow.Value == true || isAttacked.Value == true)
        {
            FollowPlayer();
        }

        if (currentHealth.Value <= 0)
        {
            CallStatus(0);
            SetTypeActionServerRpc(0);
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
            transform.position = Vector2.MoveTowards(this.transform.position, player1.transform.position, angrySpeed.Value * Time.deltaTime);
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
        Debug.Log("distance " + distance);

        if (angrySpeed.Value != speed && grow.Value == false)
        {
            CallStatus(5);
            SetTypeActionServerRpc(5);
            SetGrowServerRpc(true);
            transform.localScale = new Vector2(transform.localScale.x * (float)1.1, transform.localScale.y * (float)1.1);
            SetMinLengthMeleeAttackServerRpc(10);

        }
        else
        {
            // SetGrowServerRpc(true);
            if (distance < minLengthMeleeAttack.Value)
            {
                CallStatus(3);
                SetTypeActionServerRpc(3);
            }
            else if (17.0 < distance && distance < 22.0)
            {
                CallStatus(4);
                typeAction.Value = 4;
                SetTypeActionServerRpc(4);
            }
            else if (26.0 <= distance && distance < 32.0)
            {
                CallStatus(1);
                typeAction.Value = 1;
                SetTypeActionServerRpc(1);
            }
            else
            {
                if (angrySpeed.Value == speed)
                {
                    CallStatus(2);
                    SetTypeActionServerRpc(2);
                }
                else
                {
                    CallStatus(6);
                    SetTypeActionServerRpc(6);
                }

            }
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
        if (die.Value)
        {
            StartCoroutine(DelayAction(delayTime));
        }
        // else if (angrySpeed.Value != speed && grow.Value == true)
        // {
        //     Debug.Log("growth");
        //     StartCoroutine(DelayActionGrow(delayTime));

        // }

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
    // IEnumerator DelayActionGrow(float delayTime)
    // {
    //     //Wait for the specified delay time before continuing.
    //     yield return new WaitForSeconds(delayTime);
    //     SetTypeActionServerRpc(5);
    //     // CallStatus(5);
    // }



    // 0 : Die
    // 1 : Block
    // 2 : Idle
    // 3 : MeleeAttack
    // 4 : LaserAttack
    // 5 : Grow 
    // 6 : AmorBuff = Idle - angry

    private void CallStatus(int type)
    {

        switch (type)
        {
            case 0: //death
                ani.SetBool("Die", true);

                ani.SetBool("Block", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("LaserAttack", false);
                ani.SetBool("Grow", false);
                ani.SetBool("AmorBuff", false);
                SetDieServerRpc(true);
                SetFightSatusServerRpc(false);
                DoDelayAction(1);
                break;
            case 1: // block
                ani.SetBool("Block", true);

                ani.SetBool("Die", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("LaserAttack", false);
                ani.SetBool("Grow", false);
                ani.SetBool("AmorBuff", false);
                break;
            // case 2:
            //     ani.SetBool("Die", false);
            //     ani.SetBool("Block", false);
            //     ani.SetBool("MeleeAttack", false);
            //     ani.SetBool("LaserAttack", false);
            //     ani.SetBool("Grow", false);
            //     ani.SetBool("AmorBuff", false);
            //     break;
            case 3: // melee attack
                ani.SetBool("MeleeAttack", true);

                ani.SetBool("Die", false);
                ani.SetBool("Block", false);
                ani.SetBool("LaserAttack", false);
                ani.SetBool("Grow", false);
                ani.SetBool("AmorBuff", false);
                break;
            case 4: // laser attack
                ani.SetBool("LaserAttack", true);

                ani.SetBool("Die", false);
                ani.SetBool("Block", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("Grow", false);
                ani.SetBool("AmorBuff", false);
                break;
            case 5: // grow
                // DoDelayAction(10);

                ani.SetBool("Grow", true);

                ani.SetBool("LaserAttack", false);
                ani.SetBool("Die", false);
                ani.SetBool("Block", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("AmorBuff", false);
                break;

            // case 6:
            //     ani.SetBool("AmorBuff", true);

            //     ani.SetBool("Die", false);
            //     ani.SetBool("Block", false);
            //     ani.SetBool("MeleeAttack", false);
            //     ani.SetBool("LaserAttack", false);
            //     ani.SetBool("Grow", false);
            //     break;
            default:
                // idle
                bool check = angrySpeed.Value == speed;
                if (angrySpeed.Value == speed)
                {
                    ani.SetBool("Die", false);
                    ani.SetBool("Block", false);
                    ani.SetBool("MeleeAttack", false);
                    ani.SetBool("LaserAttack", false);
                    ani.SetBool("Grow", false);
                    ani.SetBool("AmorBuff", false);
                }
                else // idle_angry
                {
                    ani.SetBool("AmorBuff", true);

                    ani.SetBool("Die", false);
                    ani.SetBool("Block", false);
                    ani.SetBool("MeleeAttack", false);
                    ani.SetBool("LaserAttack", false);
                    ani.SetBool("Grow", false);
                }
                break;
        }
    }


    public void TakeDamage(float dmg)
    {
        if (IsServer && this.typeAction.Value != 1 && this.typeAction.Value != 5)
        {
            currentHealth.Value -= dmg;
            if (!isAttacked.Value)
            {
                isAttacked.Value = true;
            }

        }

    }

}




