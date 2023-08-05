
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

    public float maxHealth = 100;
    private float rotationSpeed = 40;
    private float jumpSpeed = 18;
    private bool die = false;
    // network properties
    private NetworkVariable<bool> isFacingRight = new NetworkVariable<bool>(true);
    public NetworkVariable<bool> disUpdate = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> damage = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> dirX = new NetworkVariable<float>(0);
    public NetworkVariable<PlayerState> playerState = new NetworkVariable<PlayerState>(PlayerState.Idle);
    public NetworkVariable<float> money = new NetworkVariable<float>(0);
    public NetworkVariable<float> gems = new NetworkVariable<float>(0);
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(0);


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



        currentHealth.Value = maxHealth;
        // healthBar.SetMaxHealth(maxHealth);
    }

    private void OnValueChanged(bool previousValue, bool newValue)
    {
        // Debug.Log($"{previousValue}    {newValue}");
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
            if (this.currentHealth.Value <= 0)
            {
                die = true;
                DoDelayAction(1);
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            OnAttackingSpawnServerRpc();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            JumpServerRpc();
        }
        SetDirXServerRpc(0);
        if (Input.GetKey(KeyCode.D))
        {
            SetDirXServerRpc(1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            SetDirXServerRpc(-1);
        }
    }
    // [ServerRpc(RequireOwnership = false)]
    // private void UpdateServerRpc()
    // {
    //     if (Input.GetKeyDown(KeyCode.J))
    //     {
    //         OnAttackingSpawnServerRpc();
    //     }
    //     if (Input.GetKeyDown(KeyCode.W))
    //     {
    //         rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
    //     }
    //     SetDirXServerRpc(0);
    //     if (Input.GetKey(KeyCode.D))
    //     {
    //         SetDirXServerRpc(1);
    //     }
    //     if (Input.GetKey(KeyCode.A))
    //     {
    //         SetDirXServerRpc(-1);
    //     }
    // }
    // [ClientRpc]
    // private void UpdateClientRpc()
    // {
    //     if (Input.GetKeyDown(KeyCode.J))
    //     {
    //         OnAttackingSpawnServerRpc();
    //     }
    //     if (Input.GetKeyDown(KeyCode.W))
    //     {
    //         rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
    //     }
    //     SetDirXServerRpc(0);
    //     if (Input.GetKey(KeyCode.D))
    //     {
    //         SetDirXServerRpc(1);
    //     }
    //     if (Input.GetKey(KeyCode.A))
    //     {
    //         SetDirXServerRpc(-1);
    //     }
    // }
    [ServerRpc(RequireOwnership = false)]
    private void SetDirXServerRpc(float value)
    {
        if (IsServer)
        {
            this.dirX.Value = value;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetIsFacingRightServerRpc(bool value)
    {
        if (IsServer)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, value ? 0 : 180, transform.rotation.z);
            this.isFacingRight.Value = value;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void MoveServerRpc()
    {
        if (IsServer)
        {
            // rb.AddForce(new Vector2(this.dirX.Value, 0) * rotationSpeed);
            // if (GameState.GetGameState() == GameState.State.Normal)
            // {
            //     rb.velocity = new Vector2(dirX.Value * 9f, rb.velocity.y);
            // }
            // else if (GameState.GetGameState() == GameState.State.Rotate)
            // {
            // }
            rb.AddForce(new Vector2(this.dirX.Value, 0) * rotationSpeed);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void JumpServerRpc()
    {
        if (IsServer)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void OnAttackingSpawnServerRpc()
    {
        normalAttackPrefab.GetComponent<Bullet>().SetDir(isFacingRight.Value ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0));
        normalAttackPrefab.GetComponent<Bullet>().SetDamage(this.damage.Value);
        Transform transform = Instantiate(normalAttackPrefab, spawnBulletPoint.position, Quaternion.identity);
        transform.GetComponent<NetworkObject>().Spawn();
    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (disUpdate.Value) return;
        playerMove(dirX.Value);
    }
    [ServerRpc(RequireOwnership = false)]
    private void FixedUpdateServerRpc()
    {
        // FixedUpdateClientRpc();
        playerMove(dirX.Value);
    }
    [ClientRpc]
    private void FixedUpdateClientRpc()
    {
        playerMove(dirX.Value);
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
                MoveServerRpc();
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
                // transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
                SetIsFacingRightServerRpc(true);
            }
        }
        else if (dirX < 0f)
        {
            SetPlayerStateServerRpc(PlayerState.Running);

            if (isFacingRight.Value)
            {
                // transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);
                SetIsFacingRightServerRpc(false);
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

    public void TakeDamage(float dmg)
    {
        if (IsServer)
        {
            this.currentHealth.Value -= dmg;
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
            // Destroy(gameObject);
            Debug.Log("Player died");
            die = false;
            DoDelayAction(7);
        }
        if (!die)
        {
            Debug.Log("Song lai");
            die = true;
            // OnNetworkSpawn();
        }

    }

    public void SetMoney(float value)
    {
        this.money.Value = value;
    }
    public void SetGem(float value)
    {
        this.gems.Value = value;
    }
    public NetworkVariable<float> GetMoney()
    {
        return this.money;
    }
    public NetworkVariable<float> GetGems()
    {
        return this.gems;
    }

    public Bullet GetBullet()
    {
        return this.normalAttackPrefab.GetComponent<Bullet>();
    }

    public NetworkVariable<float> GetHealth()
    {
        return this.currentHealth;
    }
}

