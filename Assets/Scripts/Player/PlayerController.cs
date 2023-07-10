using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController Instance { get; private set; }
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2d;
    private Animator ani;
    private SpriteRenderer sprite;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Transform normalAttackPrefab;
    [SerializeField] private Transform spawnBulletPoint;
    private float jumpSpeed = 18;
    private float dirX;
    private NetworkVariable<bool> isFacingRight = new NetworkVariable<bool>(true,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    public event EventHandler<onAttackArgs> onAttacking;
    public class onAttackArgs : EventArgs
    {
        public Vector3 dir;
    }


    // Vector2 movement;
    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        Instance = this;
        // isFacingRight.OnValueChanged += OnFacingRight;
    }

    // private void OnFacingRight(bool previousValue, bool newValue)
    // {
    //     isFacingRight.Value = newValue;
    // }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Shoot");
            OnAttackingSpawnServerRpc();
        }
        dirX = Input.GetAxisRaw("Horizontal");
    }
    [ServerRpc(RequireOwnership = false)]
    public void OnAttackingSpawnServerRpc()
    {
        Debug.Log(isFacingRight.Value);
        normalAttackPrefab.GetComponent<NormalAttack>().SetDir(isFacingRight.Value ? new Vector3(1,0,0) :new Vector3(-1,0,0));
        Transform transform = Instantiate(normalAttackPrefab, spawnBulletPoint.position, Quaternion.identity);
        transform.GetComponent<NetworkObject>().Spawn();
    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        playerMove(dirX);
        if (isGrounded() && Input.GetKeyDown(KeyCode.W))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

    private void playerMove(float dirX)
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (isGrounded())
            {
                ani.SetBool("sitting", true);
            }
        }
        else
        {
            ani.SetBool("sitting", false);

            rb.velocity = new Vector2(dirX * 9f, rb.velocity.y);
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
                // transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
                isFacingRight.Value = true;
            }
        }
        else if (dirX < 0f)
        {
            ani.SetBool("running", true);
            if (isFacingRight.Value)
            {
                // transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);
                isFacingRight.Value = false;
            }
        }
        else
        {
            ani.SetBool("running", false);
        }

    }

    private bool isGrounded()
    {
        // return transform.Find("GroundCheck").GetComponent<GroundCheck>().isGrounded;
        float extraHeightText = 1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, extraHeightText, GroundLayer);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2d.bounds.center + new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, 0), Vector2.down * (boxCollider2d.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider2d.bounds.center - new Vector3(boxCollider2d.bounds.extents.x, boxCollider2d.bounds.extents.y), Vector2.right * (boxCollider2d.bounds.extents.x), rayColor);
        // Debug.Log(raycastHit.collider);
        return raycastHit.collider != null;

    }
}