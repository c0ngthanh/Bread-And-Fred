using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraFollowPlayer : NetworkBehaviour
{
    [SerializeField] Transform player;
    void Start()
    {
        SetCameraClientRpc();
    }
    [ClientRpc]
    private void SetCameraClientRpc(){
        GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
        if(gameObjectArray[0].GetComponent<PlayerController>().IsLocalPlayer){
            player = gameObjectArray[0].transform;
        }
        if(gameObjectArray[1].GetComponent<PlayerController>().IsLocalPlayer){
            player = gameObjectArray[1].transform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(player.position.x, player.position.y, -10);
    }
}
