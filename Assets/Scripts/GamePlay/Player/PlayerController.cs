using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    // public static PlayerController Instance { get; private set; }
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2d;
    private Animator ani;
    private SpriteRenderer sprite;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Transform normalAttackPrefab;
    [SerializeField] private Transform checkGroundPosition;
    [SerializeField] private float checkGroundRadius;
    [SerializeField] private Transform spawnBulletPoint;
    private float jumpSpeed = 18;
    public float dirX = 0;
    // network properties
    private NetworkVariable<bool> isFacingRight = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> disUpdate = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> damage = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    // Vector2 movement;
    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        // Instance = this;
        disUpdate.Value = false;
        disUpdate.OnValueChanged += OnValueChanged;
    }

    private void OnValueChanged(bool previousValue, bool newValue)
    {
        Debug.Log($"{previousValue}    {newValue}");
    }

    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.J))
        {
            OnAttackingSpawnServerRpc();
        }
        if (IsGrounded() && Input.GetKeyDown(KeyCode.W))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        dirX = Input.GetAxisRaw("Horizontal");
    }
    [ServerRpc(RequireOwnership = false)]
    public void OnAttackingSpawnServerRpc()
    {
        normalAttackPrefab.GetComponent<NormalAttack>().SetDir(isFacingRight.Value ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0));
        normalAttackPrefab.GetComponent<NormalAttack>().SetDamage(this.damage.Value);
        Transform transform = Instantiate(normalAttackPrefab, spawnBulletPoint.position, Quaternion.identity);
        transform.GetComponent<NetworkObject>().Spawn();
    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;


        playerMove(dirX);
    }
    private void playerMove(float dirX)
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (IsGrounded())
            {
                ani.SetBool("sitting", true);
            }
        }
        else
        {
            ani.SetBool("sitting", false);
            if (!disUpdate.Value)
            {
                rb.velocity = new Vector2(dirX * 9f, rb.velocity.y);
            }
            updateAnimation(dirX);
        }
    }

    private void updateAnimation(float dirX)
    {
        if (dirX > 0f)
        {
            ani.SetBool("running", true);
            if (!isFacingRight.Value)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
                isFacingRight.Value = true;
            }
        }
        else if (dirX < 0f)
        {
            ani.SetBool("running", true);
            if (isFacingRight.Value)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);
                isFacingRight.Value = false;
            }
        }
        else
        {
            ani.SetBool("running", false);
        }

    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(checkGroundPosition.position, checkGroundRadius, GroundLayer);
    }
}