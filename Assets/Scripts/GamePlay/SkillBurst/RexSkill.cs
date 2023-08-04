using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RexSkill : NetworkBehaviour
{
    [SerializeField] private Transform normalAttack;
    [SerializeField] private Transform skillNormalAttack;
    [SerializeField] private Transform player;
    public void SetPlayer(Transform value){
        player = value;
    }
    public Transform GetPlayer(){
        return player;
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log(player);
        player.GetComponent<PlayerController>().SetBulletPrefab(skillNormalAttack);
        StartCoroutine(ResetBullet());
    }
    IEnumerator ResetBullet(){
        yield return new WaitForSeconds(10);
        player.GetComponent<PlayerController>().SetBulletPrefab(normalAttack);
        DestroyObjectServerRpc();
    }
    [ServerRpc(RequireOwnership =false)]
    private void DestroyObjectServerRpc(){
        Destroy(gameObject);
    }
}
