using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class bossAction : NetworkBehaviour
{
    // public HealthBar healthBar;
    private SpriteRenderer sprite;
    private Animator ani;
    private GameObject player1;
    private GameObject player2;
    private GameObject bossStand;

    // public new Animation animation;


    public float maxHealth = 100;
    private NetworkVariable<bool> follow = new NetworkVariable<bool>();
    private float distance1 = 1000, distance2 = 1000, distance = 0;
    private float speed = 2f;


    private float CurrentTimer = 0;
    private float TimeBetweenTicks = 1f;

    private NetworkVariable<float> angrySpeed = new NetworkVariable<float>();
    private NetworkVariable<bool> die = new NetworkVariable<bool>();
    private NetworkVariable<bool> isAttacked = new NetworkVariable<bool>();

    public NetworkVariable<float> currentHealth = new NetworkVariable<float>();

    // cập nhật animation cho boss
    public NetworkVariable<int> typeAction = new NetworkVariable<int>();

    public NetworkVariable<bool> grow = new NetworkVariable<bool>();
    public NetworkVariable<int> minLengthMeleeAttack = new NetworkVariable<int>();
    public NetworkVariable<int> minLengthLaserAttack = new NetworkVariable<int>();
    public NetworkVariable<int> maxLengthLaserAttack = new NetworkVariable<int>();
    public NetworkVariable<int> minLengthBlock = new NetworkVariable<int>();
    public NetworkVariable<int> maxLengthBlock = new NetworkVariable<int>();



    public NetworkVariable<bool> flag = new NetworkVariable<bool>();
    public NetworkVariable<bool> angry = new NetworkVariable<bool>();

    private float checkTime_before;
    private float checkTime_after;
    private float checkHealth_before;
    private float checkHealth_after;


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
        currentHealth.Value = maxHealth;
        typeAction.Value = 2;
        isAttacked.Value = false;
        die.Value = false;
        angrySpeed.Value = speed;
        angry.Value = false;
        follow.Value = false;
        grow.Value = false;
        minLengthMeleeAttack.Value = 9;
        minLengthLaserAttack.Value = 10;
        maxLengthLaserAttack.Value = 19;
        sprite.color = new Color(255, 255, 255);

        bossStand = GameObject.FindGameObjectWithTag("BossStand");
        currentHealth.OnValueChanged += CheckAlphaHealth;
        flag.Value = true;
        checkTime_before = Time.time;
        checkHealth_before = currentHealth.Value;
    }

    private void CheckAlphaHealth(float oldVal, float newVal)
    {

        checkTime_after = Time.time;
        checkHealth_after = currentHealth.Value;
        float num = (checkHealth_before - checkHealth_after) / (checkTime_after - checkTime_before);
        if (num > 5)
        {
            CallStatus(1);
            SetTypeActionServerRpc(1);
        }
        else if (num < 3)
        {
            if (!angry.Value)
            {
                CallStatus(2);
                SetTypeActionServerRpc(2);
            }
            else
            {
                CallStatus(7);
                SetTypeActionServerRpc(7);
            }
        }
        checkHealth_before = checkHealth_after;
        checkTime_before = checkTime_after;
    }





    [ServerRpc(RequireOwnership = false)]
    public void SetHealthServerRpc(float HP)
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
    public void SetAngryStatusServerRpc(bool val)
    {
        if (IsServer)
        {
            this.angry.Value = val;
        }
    }

    public NetworkVariable<bool> GetAngryStatus()
    {
        return this.angry;
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
    public void SetMinLengthLaserAttackServerRpc(int num)
    {
        if (IsServer)
        {
            this.minLengthLaserAttack.Value = num;
        }
    }





    [ServerRpc(RequireOwnership = false)]
    public void SetMaxLengthLaserAttackServerRpc(int num)
    {
        if (IsServer)
        {
            this.maxLengthLaserAttack.Value = num;
        }
    }






    [ServerRpc(RequireOwnership = false)]
    public void SetFlagServerRpc(bool num)
    {
        if (IsServer)
        {
            this.flag.Value = num;
        }
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

        if (currentHealth.Value > 60 && currentHealth.Value < 65)
        {
            SetAngrySpeedServerRpc(4f);
        }
        else if (currentHealth.Value > 50 && currentHealth.Value < 55)
        {
            SetAngrySpeedServerRpc(5f);
            SetAngryStatusServerRpc(true);
        }
        else if (currentHealth.Value > 40 && currentHealth.Value < 45)
        {
            SetAngrySpeedServerRpc(6f);
        }
        else if (currentHealth.Value < 30)
        {
            SetAngrySpeedServerRpc(7f);
        }


        if (!follow.Value)
        {
            if (distance < 30.0)
            {
                FollowPlayer();
                SetFollowServerRpc(true);
            }
        }
        else if (follow.Value == true)
        {
            FollowPlayer();
        }

        if (currentHealth.Value <= 0)
        {
            CallStatus(0);
            SetTypeActionServerRpc(0);
        }

        if (!bossStand.activeSelf && flag.Value)
        {
            GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            SetFlagServerRpc(false);
        }
    }

    private void FollowPlayer()
    {
        //lấy vector từ player --> boss
        Vector3 distVector = player1.transform.position - transform.position;
        float num;
        if (typeAction.Value != 1)
        {
            if (distVector.x < 0)
            {
                num = player1.transform.position.x + 4f;
                sprite.flipX = true;
            }
            else
            {
                sprite.flipX = false;
                num = player1.transform.position.x - 4f;
            }
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(num, transform.position.y), angrySpeed.Value * Time.deltaTime);
        }
        if (angry.Value && grow.Value == false)
        {
            CallStatus(6);
            SetTypeActionServerRpc(6);
            SetGrowServerRpc(true);
            transform.localScale = new Vector2(transform.localScale.x * (float)1.5, transform.localScale.y * (float)1.5);
            sprite.color = new Color(0.72f, 0f, 0f);
            SetMinLengthMeleeAttackServerRpc(15);
            SetMinLengthLaserAttackServerRpc(15);
            SetMaxLengthLaserAttackServerRpc(26);
        }
        else
        {
            if (typeAction.Value == 1)
            {
                DoDelayAction(8);
            }
            if (typeAction.Value != 1)
            {
                if (distance < minLengthMeleeAttack.Value)
                {
                    CallStatus(3);
                    SetTypeActionServerRpc(3);
                }
                else if (minLengthLaserAttack.Value < distance && distance < maxLengthLaserAttack.Value)
                {
                    if (distVector.x > 0)
                    {
                        CallStatus(4);
                        SetTypeActionServerRpc(4);
                    }
                    else
                    {

                        CallStatus(5);
                        SetTypeActionServerRpc(5);
                    }
                }
                else
                {
                    if (!angry.Value)
                    {
                        CallStatus(2);
                        SetTypeActionServerRpc(2);
                    }
                    else
                    {
                        CallStatus(7);
                        SetTypeActionServerRpc(7);
                    }

                }
            }

        }
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground" || other.gameObject.tag == "BossStand")
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {

        if (other.gameObject.tag == "Player" && this.typeAction.Value == 3)
        {
            CurrentTimer += Time.deltaTime;
            if (CurrentTimer >= TimeBetweenTicks)
            {
                if (!angry.Value)
                {
                    other.gameObject.GetComponent<PlayerController>().TakeDamage(1);
                }
                else
                {
                    other.gameObject.GetComponent<PlayerController>().TakeDamage(2);
                }
                CurrentTimer = 0;
            }
        }
    }

    void DoDelayAction(float delayTime)
    {
        if (die.Value)
        {
            StartCoroutine(DelayAction(delayTime));
        }
        else
        {
            if (typeAction.Value == 1)
            {
                StartCoroutine(WaitTimeNormalAction(delayTime));

            }
        }
    }

    IEnumerator DelayAction(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (die.Value)
        {
            Destroy(gameObject);
        }
    }
    IEnumerator WaitTimeNormalAction(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (!angry.Value)
        {
            CallStatus(2);
            SetTypeActionServerRpc(2);
        }
        else
        {
            CallStatus(7);
            SetTypeActionServerRpc(7);
        }
    }


    // 0 : Die
    // 1 : Block
    // 2 : Idle
    // 3 : MeleeAttack
    // 4 : LaserAttack
    // 5 : LaserAttack2
    // 6 : Grow 
    // 7 : AmorBuff = Idle - angry

    private void CallStatus(int type)
    {

        switch (type)
        {
            case 0: //death
                ani.SetBool("Die", true);

                ani.SetBool("Block", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("LaserAttack", false);
                ani.SetBool("LaserAttack2", false);
                ani.SetBool("Grow", false);
                ani.SetBool("LaserAttack_angry2", false);
                ani.SetBool("LaserAttack_angry", false);
                ani.SetBool("AmorBuff", false);
                SetDieServerRpc(true);
                DoDelayAction(1);
                break;
            case 1: // block
                ani.SetBool("Block", true);

                ani.SetBool("Die", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("LaserAttack", false);
                ani.SetBool("Grow", false);
                ani.SetBool("LaserAttack_angry", false);
                ani.SetBool("LaserAttack_angry2", false);
                ani.SetBool("LaserAttack2", false);
                ani.SetBool("AmorBuff", false);
                break;
            case 3: // melee attack
                ani.SetBool("MeleeAttack", true);

                ani.SetBool("Die", false);
                ani.SetBool("Block", false);
                ani.SetBool("LaserAttack", false);
                ani.SetBool("Grow", false);
                ani.SetBool("LaserAttack2", false);
                ani.SetBool("LaserAttack_angry", false);
                ani.SetBool("LaserAttack_angry2", false);
                ani.SetBool("AmorBuff", false);
                break;
            case 4: // laser attack
                if (!angry.Value)
                {
                    ani.SetBool("LaserAttack", true);

                    ani.SetBool("Die", false);
                    ani.SetBool("Block", false);
                    ani.SetBool("MeleeAttack", false);
                    ani.SetBool("Grow", false);
                    ani.SetBool("LaserAttack2", false);
                    ani.SetBool("LaserAttack_angry", false);
                    ani.SetBool("LaserAttack_angry2", false);
                    ani.SetBool("AmorBuff", false);
                }
                else
                {
                    ani.SetBool("LaserAttack_angry", true);

                    ani.SetBool("Die", false);
                    ani.SetBool("Block", false);
                    ani.SetBool("MeleeAttack", false);
                    ani.SetBool("Grow", false);
                    ani.SetBool("LaserAttack2", false);
                    ani.SetBool("LaserAttack", false);
                    ani.SetBool("LaserAttack_angry2", false);
                    ani.SetBool("AmorBuff", false);
                }

                break;
            case 5: // laser attack 2
                if (!angry.Value)
                {
                    ani.SetBool("LaserAttack2", true);

                    ani.SetBool("LaserAttack", false);
                    ani.SetBool("Die", false);
                    ani.SetBool("Block", false);
                    ani.SetBool("MeleeAttack", false);
                    ani.SetBool("Grow", false);
                    ani.SetBool("LaserAttack_angry", false);
                    ani.SetBool("LaserAttack_angry2", false);
                    ani.SetBool("AmorBuff", false);
                }
                else
                {
                    ani.SetBool("LaserAttack_angry2", true);

                    ani.SetBool("LaserAttack2", false);
                    ani.SetBool("LaserAttack", false);
                    ani.SetBool("Die", false);
                    ani.SetBool("Block", false);
                    ani.SetBool("MeleeAttack", false);
                    ani.SetBool("Grow", false);
                    ani.SetBool("LaserAttack_angry", false);
                    ani.SetBool("AmorBuff", false);
                }

                break;
            case 6: // grow
                // DoDelayAction(10);

                ani.SetBool("Grow", true);

                ani.SetBool("LaserAttack", false);
                ani.SetBool("LaserAttack2", false);
                ani.SetBool("Die", false);
                ani.SetBool("Block", false);
                ani.SetBool("MeleeAttack", false);
                ani.SetBool("LaserAttack_angry", false);
                ani.SetBool("LaserAttack_angry2", false);
                ani.SetBool("AmorBuff", false);
                break;
            default:
                // idle
                if (!angry.Value)
                {
                    ani.SetBool("Die", false);
                    ani.SetBool("Block", false);
                    ani.SetBool("MeleeAttack", false);
                    ani.SetBool("LaserAttack", false);
                    ani.SetBool("LaserAttack2", false);
                    ani.SetBool("Grow", false);
                    ani.SetBool("LaserAttack_angry", false);
                    ani.SetBool("LaserAttack_angry2", false);
                    ani.SetBool("AmorBuff", false);
                }
                else // idle_angry
                {
                    ani.SetBool("AmorBuff", true);

                    ani.SetBool("Die", false);
                    ani.SetBool("Block", false);
                    ani.SetBool("MeleeAttack", false);
                    ani.SetBool("LaserAttack", false);
                    ani.SetBool("LaserAttack2", false);
                    ani.SetBool("LaserAttack_angry", false);
                    ani.SetBool("LaserAttack_angry2", false);
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




