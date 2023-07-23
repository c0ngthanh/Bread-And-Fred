using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    // public static PlayerController Instance { get; private set; }
    public enum PlayerState
    {
        Idle,
        Running,
        Sitting
    }
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2d;
    private Animator ani;
    private SpriteRenderer sprite;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Transform normalAttackPrefab;
    [SerializeField] private Transform checkGroundPosition;
    [SerializeField] private float checkGroundRadius;
    [SerializeField] private Transform spawnBulletPoint;
    private float rotationSpeed = 40;
    private float jumpSpeed = 18;
    public float dirX = 0;
    // network properties
    private NetworkVariable<bool> isFacingRight = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> disUpdate = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> damage = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<PlayerState> playerState = new NetworkVariable<PlayerState>(PlayerState.Idle);


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
        playerState.OnValueChanged += SetPlayerAnimation;
    }

    private void OnValueChanged(bool previousValue, bool newValue)
    {
        Debug.Log($"{previousValue}    {newValue}");
    }

    void Update()
    {
        if (!IsOwner) return;
        if (IsServer)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (this.playerState.Value != PlayerState.Sitting)
                {
                    this.playerState.Value = PlayerState.Sitting;
                    this.disUpdate.Value = true;
                }
                else
                {
                    this.playerState.Value = PlayerState.Idle;
                    this.disUpdate.Value = false;
                }
            }
        }
        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     OnAttackingSpawnServerRpc();
        // }
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        // dirX = Input.GetAxisRaw("Horizontal");
        dirX = 0;
        if (Input.GetKey(KeyCode.D))
        {
            dirX = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dirX = -1;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    // public void OnAttackingSpawnServerRpc()
    // {
    //     normalAttackPrefab.GetComponent<Bullet>().SetDir(isFacingRight.Value ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0));
    //     normalAttackPrefab.GetComponent<Bullet>().SetDamage(this.damage.Value);
    //     Transform transform = Instantiate(normalAttackPrefab, spawnBulletPoint.position, Quaternion.identity);
    //     transform.GetComponent<NetworkObject>().Spawn();
    // }
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
                SetPlayerStateServerRpc(PlayerState.Sitting);
            }
        }
        else
        {
            SetPlayerStateServerRpc(PlayerState.Idle);
            if (!disUpdate.Value)
            {
                // if (GameState.GetGameState() == GameState.State.Normal)
                // {
                // rb.velocity = new Vector2(dirX * 9f, rb.velocity.y);
                // }
                // else if (GameState.GetGameState() == GameState.State.Rotate)
                // {
                // }
                rb.AddForce(new Vector2(dirX, 0) * rotationSpeed);
            }
            updateAnimation(dirX);
        }
    }
    private void updateAnimation(float dirX)
    {
        if (dirX > 0f)
        {
            SetPlayerStateServerRpc(PlayerState.Running);

            if (!isFacingRight.Value)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
                isFacingRight.Value = true;
            }
        }
        else if (dirX < 0f)
        {
            SetPlayerStateServerRpc(PlayerState.Running);

            if (isFacingRight.Value)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);
                isFacingRight.Value = false;
            }
        }
        else
        {
            SetPlayerStateServerRpc(PlayerState.Idle);

        }

    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(checkGroundPosition.position, checkGroundRadius, GroundLayer);
    }
    public float GetSignFacingRight()
    {
        return this.isFacingRight.Value ? 1 : -1;
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerStateServerRpc(PlayerState state)
    {
        this.playerState.Value = state;
    }
    private void SetPlayerAnimation(PlayerState previousValue, PlayerState newValue)
    {
        if (this.playerState.Value == PlayerState.Idle)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ani.SetBool("sitting", false);
            ani.SetBool("running", false);
        }
        if (this.playerState.Value == PlayerState.Running)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ani.SetBool("running", true);
        }
        if (this.playerState.Value == PlayerState.Sitting)
        {
            ani.SetBool("sitting", true);
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
    public PlayerState GetPlayerState()
    {
        return this.playerState.Value;
    }
}