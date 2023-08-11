using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleMode : MonoBehaviour
{
    public void StartHost(){
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("SingleScene",LoadSceneMode.Single);
    }
}
