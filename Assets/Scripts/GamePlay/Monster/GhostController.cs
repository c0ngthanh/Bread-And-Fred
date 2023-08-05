using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GhostController : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackRange = 7;
    [SerializeField] private GhostBulletData ghostBulletData;
    [SerializeField] private Transform monsterBulletSpawnPoint;
    private GhostBullet monsterBullet;
    private RaycastHit2D raycastHit2D;
    private NetworkVariable<bool> isFacingRight = new NetworkVariable<bool>(true);
    private NetworkVariable<bool> canFlip = new NetworkVariable<bool>(true);
    private float randomShooting;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        raycastHit2D = Physics2D.Raycast(transform.position - new Vector3(attackRange, 0, 0), Vector2.right, 2 * attackRange, playerLayer);
        if (raycastHit2D.collider != null)
        {
            if (raycastHit2D.collider.gameObject.transform.position.x < transform.position.x)
            {
                if (isFacingRight.Value && canFlip.Value)
                {
                    FlipClientRpc();
                }
            }
            else
            {
                if (!isFacingRight.Value && canFlip.Value)
                {
                    FlipClientRpc();
                }
            }
            animator.SetTrigger("Appear");

        }
    }
    public void ChooseBullet()
    {
        randomShooting = Random.Range(0, 2);
        if (randomShooting == 0)
        {
            animator.SetTrigger("Danger");
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }
    public void IsAppear()
    {
        if (IsHost)
        {
            canFlip.Value = true;
        }
    }
    public void DoneAppear()
    {
        if (IsHost)
        {
            canFlip.Value = false;
        }
    }
    public void Attack()
    {
        monsterBullet = ghostBulletData.bulletList[0];
        if (IsServer)
        {
            monsterBullet.SetDir(isFacingRight.Value ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0));
            monsterBullet.SetDamage(0);
            GhostBullet transform = Instantiate(monsterBullet, monsterBulletSpawnPoint.position, Quaternion.identity);
            transform.GetComponent<NetworkObject>().Spawn();
        }
    }
    public void AttackDanger()
    {
        monsterBullet = ghostBulletData.bulletList[1];
        if (IsServer)
        {
            monsterBullet.SetDir(isFacingRight.Value ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0));
            monsterBullet.SetDamage(0);
            GhostBullet transform = Instantiate(monsterBullet, monsterBulletSpawnPoint.position, Quaternion.identity);
            transform.GetComponent<NetworkObject>().Spawn();
        }
    }
    [ClientRpc]
    private void FlipClientRpc()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        if (IsHost)
        {
            isFacingRight.Value = !isFacingRight.Value;
        }
    }
}
