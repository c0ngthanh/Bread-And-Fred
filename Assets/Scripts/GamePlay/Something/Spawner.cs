using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] GameObject Object;
    override public void OnNetworkSpawn()
    {
        Instantiate(Object).GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.LocalClientId);
        // Object.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.LocalClientId);
    }
}
