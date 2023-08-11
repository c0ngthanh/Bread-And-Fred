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
            if (GameMode.Instance.GetGameMode().Value == GameMode.Mode.Multi)
            {
                foreach (var player in PlayerSpawner.playerList)
                {
                    if (gameObject.tag == "Money")
                    {
                        player.GetComponent<PlayerController>().SetMoneyServerRpc(player.GetComponent<PlayerController>().GetMoney().Value + this.value);
                    }
                    else
                    {
                        player.GetComponent<PlayerController>().SetGemServerRpc(player.GetComponent<PlayerController>().GetGems().Value + this.value);
                    }
                }
            }
            else if (GameMode.Instance.GetGameMode().Value == GameMode.Mode.Single)
            {
                if (gameObject.tag == "Money")
                {
                    PlayerHolder.Instance.SetMoney(PlayerHolder.Instance.GetMoney().Value + this.value);
                }
                else
                {
                    PlayerHolder.Instance.SetGems(PlayerHolder.Instance.GetGems().Value + this.value);
                }
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
