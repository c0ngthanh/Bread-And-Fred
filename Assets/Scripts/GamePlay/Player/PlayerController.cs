
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
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Transform normalAttackPrefab;
    [SerializeField] private Transform checkGroundPosition;
    [SerializeField] private float checkGroundRadius;
    [SerializeField] private Transform spawnBulletPoint;
    // public float maxHealth = 100;
    [SerializeField] private GameObject skillBurstHolder;
    private float jumpSpeed = 18;
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
    private NetworkVariable<float> speed = new NetworkVariable<float>(40);
    private NetworkVariable<float> maxHealth = new NetworkVariable<float>(100);
    private NetworkVariable<float> health = new NetworkVariable<float>(100);
    private NetworkVariable<SkillState> skillState = new NetworkVariable<SkillState>(SkillState.Locked);
    //EventHandler 
    public event EventHandler<bool> SkillBurstChanged;
    public event EventHandler<bool> SkillStateChanged;
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(0);


    //
    private float LastOnGroundTime;
    private float countDown;
    private bool canSkill = true;
    private bool canShoot = true;
    public override void OnNetworkSpawn()
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



        currentHealth.Value = maxHealth.Value;
        // healthBar.SetMaxHealth(maxHealth);
    }

    // private void OnValueChanged(bool previousValue, bool newValue)
    // {
    //     Debug.Log($"{previousValue}    {newValue}");
    // }

    void Update()
    {
        if (!IsOwner) return;
        LastOnGroundTime -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q) && canSkill)
        {
            if (skillState.Value == SkillState.Locked)
            {

            }
            else
            {
                SpawnSkillServerRpc(NetworkManager.Singleton.LocalClientId);
                SetCanSkill(false);
                StartCoroutine(CountDownTime());
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
            OnAttackingSpawnServerRpc();
            StartCoroutine(SetCanShoot());
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
        if(IsGrounded()){
            LastOnGroundTime = 0.1f;
        }
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
            if (GameState.gameState == GameState.State.Normal)
            {
                #region Run 
                float targetSpeed = dirX.Value * speed.Value;

                float speedDif = targetSpeed - rb.velocity.x;

                float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? 9.5f : 9.5f;

                float movement = Mathf.Pow(Mathf.Abs(speedDif), 1.2f) * Mathf.Sign(speedDif);

                rb.AddForce(movement * Vector2.right);
                #endregion
                #region Friction 
                if(LastOnGroundTime > 0 && Mathf.Abs(dirX.Value) < 0.01f){
                    float amount = Mathf.Min(Mathf.Abs(rb.velocity.x),Mathf.Abs(0.2f));

                    amount *= Mathf.Sign(rb.velocity.x);

                    rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
                }
                #endregion
            }
            else if (GameState.gameState == GameState.State.Rotate)
            {
                rb.AddForce(new Vector2(this.dirX.Value, 0) * speed.Value);
            }
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

