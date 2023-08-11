using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Helper;

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
    [SerializeField] private PlayerRunData data;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Transform normalAttackPrefab;
    [SerializeField] private Transform checkGroundPosition;
    [SerializeField] private float checkGroundRadius;
    [SerializeField] private Transform spawnBulletPoint;
    // public float maxHealth = 100;
    [SerializeField] private GameObject skillBurstHolder;
    private bool die = false;
    // network properties
    private NetworkVariable<bool> isFacingRight = new NetworkVariable<bool>(true);
    public NetworkVariable<bool> disUpdate = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> dirX = new NetworkVariable<float>(0);
    private NetworkVariable<PlayerState> playerState = new NetworkVariable<PlayerState>(PlayerState.Idle);
    private NetworkVariable<float> money = new NetworkVariable<float>(0);
    private NetworkVariable<float> gems = new NetworkVariable<float>(0);
    private NetworkVariable<float> damage = new NetworkVariable<float>(1);
    private NetworkVariable<float> countDownNormalAttack = new NetworkVariable<float>(2);
    private NetworkVariable<float> speed = new NetworkVariable<float>(15);
    private NetworkVariable<float> maxHealth = new NetworkVariable<float>(100);
    private NetworkVariable<float> health = new NetworkVariable<float>(100);
    private NetworkVariable<SkillState> skillState = new NetworkVariable<SkillState>(SkillState.Locked);
    private NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false);
    //EventHandler 
    public event EventHandler<bool> SkillBurstChanged;
    public event EventHandler<bool> SkillStateChanged;
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(100);


    //
    private float LastOnGroundTime;
    private float LastJumpTime;
    private float countDown;
    private bool canSkill = true;
    public bool canShoot = true;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        // Instance = this;
        disUpdate.Value = false;
        // disUpdate.OnValueChanged += OnValueChanged;
        playerState.OnValueChanged += SetPlayerAnimation;
        // uIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        countDown = skillBurstHolder.GetComponent<SkillBurstHolder>().GetCountDown();
        // speed.Value = data.moveSpeed;
        // currentHealth.Value = maxHealth.Value;
        // healthBar.SetMaxHealth(maxHealth);
    }

    // private void OnValueChanged(bool previousValue, bool newValue)
    // {
    //     Debug.Log($"{previousValue}    {newValue}");
    // }

    void Update()
    {
        LastOnGroundTime -= Time.deltaTime;
        LastJumpTime -= Time.deltaTime;
        if (GameMode.Instance.GetGameMode().Value == GameMode.Mode.Multi)
        {
            if (!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.Q) && canSkill)
            {
                if (skillState.Value == SkillState.Locked)
                {

                }
                else
                {
                    UseSkill();
                }
                // if (this.currentHealth.Value <= 0)
                // {
                //     die = true;
                //     DoDelayAction(1);
                // }
                // if (this.currentHealth.Value <= 0)
                // {
                //     die = true;
                //     DoDelayAction(1);
                // }
            }
            if (Input.GetKeyDown(KeyCode.J) && canShoot)
            {
                Attack();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                OnJumpInput();
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
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (IsGrounded())
                {
                    SetPlayerStateServerRpc(PlayerState.Sitting);
                }
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                if (IsGrounded())
                {
                    SetPlayerStateServerRpc(PlayerState.Idle);
                }
            }
        }
        if (LastOnGroundTime > 0 && LastJumpTime > 0 && !isJumping.Value)
        {
            SetJumpServerRpc(true);
            JumpServerRpc();
        }
        if (isJumping.Value && rb.velocity.y < 0)
        {
            SetJumpServerRpc(false);
        }
        if (IsGrounded() && !isJumping.Value)
        {
            LastOnGroundTime = data.coyoteTime;
        }
    }
    public void UseSkill()
    {
        SpawnSkillServerRpc(NetworkManager.Singleton.LocalClientId);
        SetCanSkill(false);
        StartCoroutine(CountDownTime());
    }
    public void Attack()
    {
        OnAttackingSpawnServerRpc();
        StartCoroutine(SetCanShoot());
    }
    public void OnJumpInput()
    {
        LastJumpTime = data.jumpInputBufferTime;
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetJumpServerRpc(bool value)
    {
        isJumping.Value = value;
    }
    IEnumerator CountDownTime()
    {
        yield return new WaitForSeconds(countDown);
        SetCanSkill(true);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnSkillServerRpc(ulong clientID)
    {
        StartCoroutine(SpawnSkill());
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };
        SpawnSkillClientRpc(clientRpcParams);
    }
    [ClientRpc]
    private void SpawnSkillClientRpc(ClientRpcParams clientRpcParams = default)
    {
        GameObject temp = Instantiate(skillBurstHolder, transform.position, Quaternion.identity);
        // StartCoroutine(SpawnSkill());
    }
    IEnumerator SpawnSkill()
    {
        yield return new WaitForSeconds(1);
        if (skillBurstHolder.GetComponent<SkillBurstHolder>().GetNameSkill().GetComponent<RexSkill>())
        {
            skillBurstHolder.GetComponent<SkillBurstHolder>().GetNameSkill().GetComponent<RexSkill>().SetPlayer(transform);
        }
        GameObject temp = Instantiate(skillBurstHolder.GetComponent<SkillBurstHolder>().GetNameSkill(), transform.position, Quaternion.identity);
        temp.GetComponent<NetworkObject>().Spawn();
    }
    IEnumerator SetCanShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(countDownNormalAttack.Value);
        canShoot = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetDirXServerRpc(float value)
    {
        if (IsServer)
        {
            this.dirX.Value = value;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetIsFacingRightServerRpc(bool value)
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
            if (GameState.Instance.GetGameState().Value == GameState.State.Normal)
            {
                #region Run 
                float targetSpeed = dirX.Value * speed.Value;
                float speedDif = targetSpeed - rb.velocity.x;

                float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.acceleration : data.decceleration;

                float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, data.velPower) * Mathf.Sign(speedDif);

                rb.AddForce(movement * Vector2.right);
                #endregion
                #region Friction 
                if (LastOnGroundTime > 0 && Mathf.Abs(dirX.Value) < 0.01f)
                {
                    float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(data.frictionAmount));

                    amount *= Mathf.Sign(rb.velocity.x);

                    rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
                }
                #endregion
            }
            else if (GameState.Instance.GetGameState().Value == GameState.State.Rotate)
            {
                rb.AddForce(new Vector2(this.dirX.Value, 0) * data.rotateForce);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void JumpServerRpc()
    {
        //Ensures we can't call Jump multiple times from one press
        LastJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = data.jumpForce;
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
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
        // if (!IsOwner) return;
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
        if (playerState.Value != PlayerState.Sitting)
        {
            SetPlayerStateServerRpc(PlayerState.Idle);
            updateAnimation(dirX);
        }
        MoveServerRpc();
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
    public void SetPlayerStateServerRpc(PlayerState state)
    {
        this.playerState.Value = state;
    }
    private void SetPlayerAnimation(PlayerState previousValue, PlayerState newValue)
    {
        if (this.playerState.Value == PlayerState.Idle)
        {
            // GameState.Instance.SetGameState(GameState.State.Normal);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ani.SetBool("sitting", false);
            ani.SetBool("running", false);
        }
        if (this.playerState.Value == PlayerState.Running)
        {
            // GameState.Instance.SetGameState(GameState.State.Normal);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ani.SetBool("running", true);
        }
        if (this.playerState.Value == PlayerState.Sitting)
        {
            // GameState.Instance.SetGameState(GameState.State.Rotate);
            ani.SetBool("sitting", true);
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        if (newValue == PlayerState.Sitting)
        {
            SetGameStateServerRpc(GameState.State.Rotate);
        }
        else if (previousValue == PlayerState.Sitting && (newValue != PlayerState.Sitting))
        {
            SetGameStateServerRpc(GameState.State.Normal);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetGameStateServerRpc(GameState.State state){
        GameState.Instance.SetGameState(state);
    }
    public bool GetCanSkill()
    {
        return this.canSkill;
    }
    public PlayerState GetPlayerState()
    {
        return this.playerState.Value;
    }
    public NetworkVariable<float> GetDamge()
    {
        return this.damage;
    }
    public NetworkVariable<float> GetMoney()
    {
        return this.money;
    }
    public NetworkVariable<float> GetGems()
    {
        return this.gems;
    }
    public NetworkVariable<float> GetMaxHealth()
    {
        return this.maxHealth;
    }
    public NetworkVariable<float> GetSpeed()
    {
        return this.speed;
    }
    public NetworkVariable<float> GetCountDownTime()
    {
        return this.countDownNormalAttack;
    }
    public NetworkVariable<float> GetCurrentHealth()
    {
        return this.health;
    }
    public NetworkVariable<SkillState> GetSkillState()
    {
        return this.skillState;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetSkillStateServerRpc(SkillState skillState)
    {
        this.skillState.Value = skillState;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetMoneyServerRpc(float value)
    {
        this.money.Value = value;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetGemServerRpc(float value)
    {
        this.gems.Value = value;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetDamageServerRpc(float value)
    {
        if (IsServer)
        {
            this.damage.Value = value;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetCountDownNormalAttackServerRpc(float value)
    {
        if (IsServer)
        {
            this.countDownNormalAttack.Value = value;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetMaxHealthServerRpc(float value)
    {
        if (IsServer)
        {
            this.health.Value = this.health.Value * value / this.maxHealth.Value;
            this.maxHealth.Value = value;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetSpeedServerRpc(float value)
    {
        if (IsServer)
        {
            this.speed.Value = value;
        }
    }
    public void SetBulletPrefab(Transform value)
    {
        normalAttackPrefab = value;
    }
    public void UnlockSkill()
    {
        SkillStateChanged?.Invoke(this, true);
    }
    public void SetCanSkill(bool value)
    {
        canSkill = value;
        SkillBurstChanged?.Invoke(this, canSkill);
    }

    public Bullet GetBullet()
    {
        return this.normalAttackPrefab.GetComponent<Bullet>();
    }
    public bool GetCanShoot()
    {
        return this.canShoot;
    }

    public NetworkVariable<float> GetHealth()
    {
        return this.currentHealth;
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
    public GameObject GetSkillBurstHolder()
    {
        return skillBurstHolder;
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
}

