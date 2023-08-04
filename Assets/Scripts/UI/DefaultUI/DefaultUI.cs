using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

public class DefaultUI : MonoBehaviour
{
    [SerializeField] private PlayerController localPlayer;
    [SerializeField] private UIManager uIManager;
    private void Start() {
        localPlayer = Camera.main.GetComponent<CameraFollowPlayer>().GetPlayer().GetComponent<PlayerController>();
    }
    public UIManager GetUIManager(){
        return this.uIManager;
    }
    public PlayerController GetPlayerController()
    {
        return this.localPlayer;
    }
}
