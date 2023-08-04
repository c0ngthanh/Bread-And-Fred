using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PinoSkill : NetworkBehaviour
{
    [SerializeField] PlayerController localPlayer;
    [SerializeField] Collider2D[] collider2Ds;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float damage;
    [SerializeField] float radius;
    public override void OnNetworkSpawn()
    {
        localPlayer = Camera.main.GetComponent<CameraFollowPlayer>().GetPlayer().GetComponent<PlayerController>();
        collider2Ds = Physics2D.OverlapCircleAll(this.transform.position, radius,layerMask);
        foreach(var monster in collider2Ds){
            monster.GetComponent<MonsterController>().SetHPServerRpc(monster.GetComponent<MonsterController>().GetHP().Value - damage);
        }
        StartCoroutine(DestroyHolder());
    }
    IEnumerator DestroyHolder(){
        yield return new WaitForSeconds(1);
        DestroyObjectServerRpc();
    }
    [ServerRpc(RequireOwnership =false)]
    private void DestroyObjectServerRpc(){
        Destroy(gameObject);
    }
}
