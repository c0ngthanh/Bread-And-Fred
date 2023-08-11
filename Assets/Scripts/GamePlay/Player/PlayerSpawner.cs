using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerSpawner : NetworkBehaviour
{
    public static GameObject[] playerList;
    [SerializeField] Rope ropePrefab;
    [SerializeField] CharacterDatabase characterDatabase;
    private void Start()
    {
        if (IsServer)
        {
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
            AddPlayerClientRpc();
            if (GameMode.Instance.GetGameMode().Value != GameMode.Mode.Single)
            {
                Rope rope = Instantiate(ropePrefab);
                rope.NetworkObject.Spawn();
            }
        }
    }
    [ClientRpc]
    private void AddPlayerClientRpc()
    {
        // Debug.Log(NetworkManager.LocalClient.PlayerObject.gameObject);
        // Camera.main.gameObject.GetComponent<CameraFollowPlayer>().SetPlayer(NetworkManager.LocalClient.PlayerObject.gameObject);
        playerList = GameObject.FindGameObjectsWithTag("Player");
    }
}
