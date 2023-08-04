using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] public static GameObject[] playerList;
    [SerializeField] Rope ropePrefab;
    [SerializeField] CharacterDatabase characterDatabase;
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }
        Debug.Log(ServerManager.Instance.clientAndCharacterID);
        foreach (var client in ServerManager.Instance.ClientData)
        {
            var character = characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null)
            {
                var characterInstance = Instantiate(character.GameplayPrefab);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
        Rope rope = Instantiate(ropePrefab);
        rope.NetworkObject.Spawn();
        AddPlayerClientRpc();
    }
    [ClientRpc]
    private void AddPlayerClientRpc()
    {
        playerList = GameObject.FindGameObjectsWithTag("Player");
    }
}
