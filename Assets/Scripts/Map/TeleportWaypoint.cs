using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Helper;

public class TeleportWaypoint : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<WaypointState> waypointState = new NetworkVariable<WaypointState>(WaypointState.Locked);
    [SerializeField] private WaypointBtn waypointBtn;
    [SerializeField] private SpriteRenderer isActiveIcon;
    private UIManager uIManager;
    private bool canInteract = false;
    private SpriteRenderer renderer;
    private void Start()
    {
        isActiveIcon.gameObject.SetActive(false);
        uIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        renderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            if (waypointState.Value == WaypointState.Locked)
            {
                SetWaypointStatusServerRpc(WaypointState.Unlocked);
                uIManager.notificationUI.SetText("Press E to open Map");
            }
            else if (waypointState.Value == WaypointState.Unlocked)
            {
                uIManager.mapUI.SetActiveMap(this);
            }
        }
    }
    public void Teleport()
    {
        Debug.Log("hihi");
        TeleportServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void TeleportServerRpc()
    {
        GameObject[] listPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in listPlayers)
        {
            Debug.Log(player);
            player.gameObject.transform.position = transform.position;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer)
        {
        }
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerController>().IsLocalPlayer)
        {
            canInteract = true;
            if (waypointState.Value == WaypointState.Locked)
            {
                uIManager.notificationUI.SetText("Press E to active");
                uIManager.notificationUI.SetActive(true);
            }
            else if (waypointState.Value == WaypointState.Unlocked)
            {
                uIManager.notificationUI.SetText("Press E to open Map");
                uIManager.notificationUI.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        canInteract = false;
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerController>().IsLocalPlayer)
        {
            uIManager.notificationUI.SetActive(false);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetWaypointStatusServerRpc(WaypointState state)
    {
        waypointState.Value = state;
        AddActiveWaypointClientRpc();
    }
    [ClientRpc]
    private void AddActiveWaypointClientRpc()
    {
        uIManager.mapUI.GetWaypointList().GetTeleportWaypoints().Add(this);
        renderer.color = Color.white;
        waypointBtn.GetComponent<SpriteRenderer>().color = Color.white;
    }
    public WaypointBtn GetWaypointBtn(){
        return waypointBtn;
    }
    public SpriteRenderer GetIsActiveIcon(){
        return isActiveIcon;
    }
}
