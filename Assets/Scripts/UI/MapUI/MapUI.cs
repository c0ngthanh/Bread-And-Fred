using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    [SerializeField] private WaypointList waypointList;
    private bool isActive = false;
    public void SetActiveMap(TeleportWaypoint sender)
    {
        if (isActive)
        {
            gameObject.SetActive(false);
            isActive=false;
        }
        else
        {
            gameObject.SetActive(true);
            isActive = true;
            waypointList.SetIsActiveNow(sender);
        }
    }
    public void SetActiveMap()
    {
        if (isActive)
        {
            gameObject.SetActive(false);
            isActive=false;
        }
        else
        {
            gameObject.SetActive(true);
            isActive = true;
        }
    }
    public WaypointList GetWaypointList(){
        return waypointList;
    }
}
