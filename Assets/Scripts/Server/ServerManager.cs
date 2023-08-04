using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerManager : NetworkBehaviour
{
    private string ChooseDinoScene = "ChooseDinoScene";
    private string GameScene = "SampleScene";
    private int maxPlayer = 2;
    public static ServerManager Instance { get; private set; }


    public NetworkList<Vector2> clientAndCharacterID;
    public List<Vector2> clientAndCharacterIDLocal = new List<Vector2>();
    public Dictionary<ulong, ClientData> ClientData { get; private set;  }


    private ulong tempID;
    private bool gameHasStarted;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        clientAndCharacterID = new NetworkList<Vector2>();
    }
    public void StartHost()
    {

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartHost();
        if (IsServer)
        {
            foreach (var item in ClientData)
            {
                clientAndCharacterID.Add(new Vector2(item.Key, item.Value.characterId));
                clientAndCharacterIDLocal.Add(new Vector2(item.Key, item.Value.characterId));
            }
        }
    }


    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (ClientData.Count >= maxPlayer || gameHasStarted)
        {
            response.Approved = false;
            return;
        }
        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;
        // tempID = request.ClientNetworkId;
        ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);
        // AddClientClientRpc();
        if (IsServer)
        {
            clientAndCharacterID.Add(new Vector2(request.ClientNetworkId, 0));
            clientAndCharacterIDLocal.Add(new Vector2(request.ClientNetworkId, 0));
        }
        Debug.Log($"Add client {request.ClientNetworkId}");
    }

    private void OnNetworkReady()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        NetworkManager.Singleton.SceneManager.LoadScene(ChooseDinoScene, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (ClientData.ContainsKey(clientId))
        {
            if (ClientData.Remove(clientId))
            {
                Debug.Log($"Remove client {clientId}");
            }
        }
    }
    public void SetCharacter(ulong clientId, int characterId)
    {
        if (ClientData.TryGetValue(clientId, out ClientData data))
        {
            data.characterId = characterId;
        }
        if (!IsHost)
        {
            return;
        }
        for (int i = 0; i < clientAndCharacterID.Count; i++)
        {
            if (clientAndCharacterID[i].x == clientId)
            {
                clientAndCharacterID[i] = new Vector2(clientId, characterId);
                clientAndCharacterIDLocal[i] = (new Vector2(clientId, characterId));
                Debug.Log("Add " + new Vector2(clientId, characterId));
            }
        }
    }
    public void StartGame()
    {
        gameHasStarted = true;

        NetworkManager.Singleton.SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
    }
}