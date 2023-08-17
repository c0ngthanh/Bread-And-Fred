
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraFollowPlayer : NetworkBehaviour
{
    [SerializeField] GameObject player;
    private float OldDirY;
    public bool flag;
    private void Start()
    {
        // if (IsServer)
        // SetCameraClientRpc();
        player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
        if (GameMode.Instance.GetGameMode().Value == GameMode.Mode.Single)
        {
            player = player.GetComponent<PlayerHolder>().GetPlayerList()[0].gameObject;
        }
        OldDirY = player.transform.position.y + 5;
        flag = true;
    }
    [ClientRpc]
    private void SetCameraClientRpc()
    {
        // GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
        // if (gameObjectArray.Length == 2)
        // {
        //     if (gameObjectArray[0].GetComponent<PlayerController>().IsLocalPlayer)
        //     {
        //         player = gameObjectArray[0];
        //     }
        //     if (gameObjectArray[1].GetComponent<PlayerController>().IsLocalPlayer)
        //     {
        //         player = gameObjectArray[1];
        //     }
        // }
        // else
        // {
        //     player = gameObjectArray[0];

        // }
        // player = NetworkManager.LocalClient.PlayerObject.gameObject;

        // OldDirY = player.transform.position.y + 5;
    }
    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (flag)
            {
                if (player.transform.position.y - this.transform.position.y > 15 || this.transform.position.y - player.transform.position.y > 15)
                {
                    OldDirY = player.transform.position.y;
                }
            }
            // this.transform.position = new Vector3(player.transform.position.x, OldDirY, -10); 
            if (player.transform.position.y > 145 && flag)
            {
                OldDirY = 155;
                flag = false;
            }

            // transform.position = Vector2.MoveTowards(transform.position, new Vector3(Mathf.Clamp(player.transform.position.x, -2.0f, 70f), Mathf.Clamp(OldDirY, 10f, 200f), this.transform.position.z), 2 * Time.deltaTime);

            transform.position = new Vector3(Mathf.Clamp(player.transform.position.x, -2.0f, 70f), Mathf.Clamp(OldDirY, 10f, 200f), this.transform.position.z);
        }
    }
    public GameObject GetPlayer()
    {
        return this.player;
    }
    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }


    // void DoDelayAction(float delayTime)
    // {
    //     StartCoroutine(DelayAction(delayTime));
    // }

    // IEnumerator DelayAction(float delayTime)
    // {
    //     //Wait for the specified delay time before continuing.
    //     yield return new WaitForSeconds(delayTime);

    //     //Do the action after the delay time has finished.
    //     this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);


    // }
}



