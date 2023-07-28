using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraFollowPlayer : NetworkBehaviour
{
    [SerializeField] GameObject player;
    private void Start()
    {
        SetCameraClientRpc();
    }
    [ClientRpc]
    private void SetCameraClientRpc(){
        GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
        if(gameObjectArray[0].GetComponent<PlayerController>().IsLocalPlayer){
            player = gameObjectArray[0];
        }
        if(gameObjectArray[1].GetComponent<PlayerController>().IsLocalPlayer){
            player = gameObjectArray[1];
        }
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
    }
    public GameObject GetPlayer(){
        return this.player;
    }
}
