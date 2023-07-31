using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

public class DefaultUI : MonoBehaviour
{
    [SerializeField] private PlayerController localPlayer;
    [SerializeField] private TMP_Text moneyValue;
    [SerializeField] private TMP_Text gemValue;
    private void Start() {
        localPlayer = Camera.main.GetComponent<CameraFollowPlayer>().GetPlayer().GetComponent<PlayerController>();
        moneyValue.text = localPlayer.GetMoney().Value.ToString();
        gemValue.text = localPlayer.GetGems().Value.ToString();
        localPlayer.GetMoney().OnValueChanged += UpdateMoneyValue;
        localPlayer.GetGems().OnValueChanged += UpdateGemsValue;
    }

    private void UpdateGemsValue(float previousValue, float newValue)
    {
        gemValue.text = newValue.ToString();
    }

    private void UpdateMoneyValue(float previousValue, float newValue)
    {
        moneyValue.text = newValue.ToString();
    }
}
