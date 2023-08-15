using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ChooseGameMethod : MonoBehaviour
{

    // public static ChooseGameMethod Instance { get; private set; }



    // [SerializeField] private Transform chooseGameMethod;
    // [SerializeField] private Button localCoop;
    // [SerializeField] private Button OnlineMultiple;

    // Start is called before the first frame update
    // void Awake()
    // {
    //     chooseGameMethod.gameObject.SetActive(false);

    //     localCoop.onClick.AddListener(LocalCoopClick);
    //     OnlineMultiple.onClick.AddListener(OnlineMultipleClick);
    // }


    // private void LocalCoopClick()
    // {

    // }

    // private void OnlineMultipleClick()
    // {
    //     LobbyListUI.Instance.Show();
    // }

    // // Update is called once per frame
    public void Hide()
    {
        Debug.Log("Hide");
        gameObject.SetActive(false);
    }

    // private void Show()
    // {
    //     gameObject.SetActive(true);
    // }
}
