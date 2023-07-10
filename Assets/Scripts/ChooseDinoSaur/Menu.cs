using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void StartHost(){
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("ChooseDinoScene",LoadSceneMode.Single);
    }
    public void StartClient(){
        NetworkManager.Singleton.StartClient();
    }
}
