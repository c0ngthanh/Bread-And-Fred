using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class ShopUI : NetworkBehaviour
{
    [SerializeField] private Sprite MoneySprite;
    [SerializeField] private Sprite GemsSprite;
    [SerializeField] private PlayerController localPlayer;
    [SerializeField] private TMP_Text moneyValue;
    [SerializeField] private TMP_Text gemValue;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private InfoUI infoUI;
    private bool activeStatus = false;
    public Sprite GetMoneySprite()
    {
        return MoneySprite;
    }
    public Sprite GetGemsSprite()
    {
        return GemsSprite;
    }
    private void Start()
    {
        localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>();;
        moneyValue.text = localPlayer.GetMoney().Value.ToString();
        gemValue.text = localPlayer.GetGems().Value.ToString();
        localPlayer.GetMoney().OnValueChanged += UpdateMoneyValue;
        localPlayer.GetGems().OnValueChanged += UpdateGemsValue;
        gameObject.SetActive(false);
    }
    public void SetActiveStatus()
    {
        if (activeStatus)
        {
            gameObject.SetActive(false);
            activeStatus = false;
            localPlayer.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }else{
            gameObject.SetActive(true);
            activeStatus = true;
            localPlayer.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
    public bool GetActiveStatus()
    {
        return activeStatus;
    }
    private void UpdateGemsValue(float previousValue, float newValue)
    {
        gemValue.text = newValue.ToString();
    }

    private void UpdateMoneyValue(float previousValue, float newValue)
    {
        moneyValue.text = newValue.ToString();
    }
    public PlayerController GetPlayerController()
    {
        return this.localPlayer;
    }
    public UIManager GetUIManager(){
        return this.uIManager;
    }
    public InfoUI GetInfoUI(){
        return this.infoUI;
    }
}
