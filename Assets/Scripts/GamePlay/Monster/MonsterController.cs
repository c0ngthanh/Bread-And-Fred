using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class MonsterController : NetworkBehaviour
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private Transform checkGroundPosition;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackRange = 7;
    // network properties
    private NetworkVariable<bool> isFacingRight = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> HP = new NetworkVariable<float>();
    public class Bullet : EventArgs
    {
        public float dmg;
    }

    //propertises
    private float speed;
    private float normalSpeed;
    private float angrySpeed;
    private float forceCollierX = 500, forceCollierY = 1000;
    private float checkGroundRadius = 0.1f;
    private bool isAngry = false;
    private bool isStuned = false;


    private RaycastHit2D raycastHit2D;
    private void Awake()
    {
        HP.Value = monsterData.HP;
        normalSpeed = monsterData.speed;
        angrySpeed = monsterData.speed * 2;
        speed = normalSpeed;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator.SetBool("Walk", true);
        HP.OnValueChanged += OnHPValueChanged;
    }

    private void OnHPValueChanged(float previousValue, float newValue)
    {
        if (newValue < 0 && IsHost && !this.isStuned)
        {
            StunEnemyClientRpc();
        }
    }
    [ClientRpc]
    private void StunEnemyClientRpc()
    {
        this.rb.bodyType = RigidbodyType2D.Kinematic;
        this.isStuned = true;
        rb.velocity = Vector2.zero;
        StartCoroutine(StartAgain());
        animator.SetTrigger("Stun");
    }
    IEnumerator StartAgain()
    {
        yield return new WaitForSeconds(10);
        this.SetHP(monsterData.HP);
        this.rb.bodyType = RigidbodyType2D.Kinematic;
        this.isStuned = false;
        animator.SetTrigger("NotStun");
    }

    private void Update()
    {
        if (isStuned) return;
        rb.velocity = new Vector2(speed, rb.velocity.y);

        if (!Physics2D.OverlapCircle(checkGroundPosition.position, checkGroundRadius, groundLayer))
        {
            FlipClientRpc();
        };
        raycastHit2D = Physics2D.Raycast(transform.position, isFacingRight.Value ? new Vector2(1, 0) : new Vector2(-1, 0), attackRange, playerLayer);
        Debug.DrawRay(transform.position, isFacingRight.Value ? new Vector2(attackRange, 0) : new Vector2(-attackRange, 0), Color.red, 0.1f);
        // if(raycastHit2Dnull){
        // }
        if (raycastHit2D.collider != null && !isAngry)
        {
            isAngry = true;
            animator.SetBool("Run", true);
            speed = angrySpeed * (isFacingRight.Value ? 1 : -1);
            StartCoroutine(NotSeePlayer());
        };
        // Debug.Log(raycastHit2D);
        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetBool("Run", true);

            speed *= 2;
        }

        IEnumerator NotSeePlayer()
        {
            yield return new WaitForSeconds(5);
            if (raycastHit2D.collider == null)
            {
                isAngry = false;
                animator.SetBool("Run", false);
                speed = normalSpeed * (isFacingRight.Value ? 1 : -1);
            }
            else
            {
                StartCoroutine(NotSeePlayer());
            }
        }
    }
    [ClientRpc]
    private void FlipClientRpc()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        speed *= -1;
        if (IsHost)
        {
            isFacingRight.Value = !isFacingRight.Value;
        }
        if (GetComponentInChildren<HealthBarUI>() != null)
        {
            GetComponentInChildren<HealthBarUI>().FlipHealthBar();
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isStuned) return;
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PlayerController>().IsOwner)
            {
                other.gameObject.GetComponent<PlayerController>().disUpdate.Value = true;
                StartCoroutine(RecallSetvelocityX(other.gameObject));
            }
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(isFacingRight.Value ? forceCollierX : -forceCollierX, forceCollierY));
        }
    }
    IEnumerator RecallSetvelocityX(GameObject other)
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(SetvelocityX(other));
    }
    IEnumerator SetvelocityX(GameObject other)
    {
        if (other.GetComponent<PlayerController>().IsGrounded())
        {
            other.gameObject.GetComponent<PlayerController>().disUpdate.Value = false;
            yield break;
        }
        yield return new WaitForSeconds(Time.deltaTime);
        StartCoroutine(SetvelocityX(other));
    }
    public void SetHP(float HP)
    {
        this.HP.Value = HP;
        Debug.Log(this.HP);
    }
    public NetworkVariable<float> GetHP()
    {
        return this.HP;
    }
    public MonsterData GetMonsterData()
    {
        return this.monsterData;
    }
}
