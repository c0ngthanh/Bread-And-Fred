using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointList : MonoBehaviour
{
    [SerializeField] private List<TeleportWaypoint> teleportWaypoints = new List<TeleportWaypoint>();
    [SerializeField] private TeleportWaypoint isActiveNow;
    [SerializeField] private TeleportWaypoint oldTeleport = null;
    [SerializeField] private MapUI mapUI;
    [SerializeField] private Camera minimapCamera;
    private bool mapMode = false;
    private int index = 0;
    public List<TeleportWaypoint> GetTeleportWaypoints()
    {
        return teleportWaypoints;
    }
    private void Update()
    {
        if (!mapMode) return;
        if (teleportWaypoints.Count <= 1) { return; }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ChoosingNow(index);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            oldTeleport.TeleportServerRpc();
            mapUI.SetActiveMap();
        }
    }
    private void ChoosingNow(int i)
    {
        if(oldTeleport != null){
            oldTeleport.GetIsActiveIcon().gameObject.SetActive(false);
        }
        if (i == teleportWaypoints.Count)
        {
            index = 0;
        }
        if (teleportWaypoints[index] == isActiveNow)
        {
            index++;
            ChoosingNow(index);
            return;
        }
        teleportWaypoints[index].GetIsActiveIcon().gameObject.SetActive(true);
        oldTeleport = teleportWaypoints[index];
        minimapCamera.transform.position = new Vector3(minimapCamera.transform.position.x, teleportWaypoints[index].transform.position.y,minimapCamera.transform.position.z);
        index++;
    }

    private void OnEnable()
    {
        mapMode = true;
    }
    public void OnDisable()
    {
        mapMode = false;
        if(oldTeleport != null){
            oldTeleport.GetIsActiveIcon().gameObject.SetActive(false);
            oldTeleport = null;
        }
        isActiveNow.GetIsActiveIcon().gameObject.SetActive(false);
        isActiveNow.GetIsActiveIcon().color = Color.red;
    }
    public void SetIsActiveNow(TeleportWaypoint teleportWaypoint){
        isActiveNow = teleportWaypoint;
        isActiveNow.GetIsActiveIcon().gameObject.SetActive(true);
        isActiveNow.GetIsActiveIcon().color = Color.white;
    }
}
