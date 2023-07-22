using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GhostController : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackRange = 7;
    private RaycastHit2D raycastHit2D;
    private NetworkVariable<bool> isFacingRight = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private void Start() {
        animator = GetComponent<Animator>();
    }
    private void Update() {
        raycastHit2D = Physics2D.Raycast(transform.position- new Vector3(attackRange,0,0), Vector2.right, 2*attackRange, playerLayer);
        if(raycastHit2D.collider != null){
            Debug.Log(raycastHit2D.collider);
            if(raycastHit2D.collider.gameObject.transform.position.x < transform.position.x){
                if(isFacingRight.Value){
                    Debug.Log("Flip left");
                    FlipClientRpc();
                }
            }else{
                if(!isFacingRight.Value){
                    Debug.Log("Flip right");
                    FlipClientRpc();
                }
            }
            animator.SetTrigger("Appear");
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
        // if (GetComponentInChildren<HealthBarUI>() != null)
        // {
        //     GetComponentInChildren<HealthBarUI>().FlipHealthBar();
        // }
    }
}
