using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour
{
    [SerializeField] private float value;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;
        if (other.gameObject.tag == "Player")
        {
            foreach (var player in PlayerSpawner.playerList)
            {
                if (gameObject.tag == "Money")
                {
                    player.GetComponent<PlayerController>().SetMoney(player.GetComponent<PlayerController>().GetMoney().Value + this.value);
                }else{
                    player.GetComponent<PlayerController>().SetGem(player.GetComponent<PlayerController>().GetGems().Value + this.value);
                }
            Debug.Log("Gems: "+player.GetComponent<PlayerController>().GetGems());
            Debug.Log("Money: "+player.GetComponent<PlayerController>().GetMoney());
            }
            SetActiveCoinClientRpc();
        }
        if (other.gameObject.tag == "Ground")
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
    [ClientRpc]
    private void SetActiveCoinClientRpc()
    {
        gameObject.SetActive(false);
    }
}
