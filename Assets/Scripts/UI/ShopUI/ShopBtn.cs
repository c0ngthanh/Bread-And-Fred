using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Helper;
using System;
using Unity.Netcode;
using UnityEngine.EventSystems;

public class ShopBtn : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [Header("Item Info")]
    [SerializeField] private ItemShopData item;

    [Header("Dont edit please")]
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private TMP_Text priceString;
    [SerializeField] private TMP_Text maxedText;
    [SerializeField] private Image iconCoin;
    [SerializeField] private TMP_Text nameItem;
    [SerializeField] private Image iconItem;
    [SerializeField] private Image levelBar;
    [SerializeField] private Transform InfoUIPos;
    private int level = 0;
    private float price;
    private PlayerController localPlayer;
    private void Start()
    {
        localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>();;
        if (item.sellBy == SellBy.Money)
        {
            iconCoin.sprite = shopUI.GetMoneySprite();
        }
        else if (item.sellBy == SellBy.Gems)
        {
            iconCoin.sprite = shopUI.GetGemsSprite();
            priceString.color = new Color(1, 0.937f, 0.18f);
        }
        maxedText.gameObject.SetActive(false);
        price = item.price[0];
        priceString.text = item.price[0].ToString();
        nameItem.text = item.itemName;
        if (item.typeSell == TypeSell.SKILL)
        {
            foreach (var client in ServerManager.Instance.clientAndCharacterID)
            {
                if(client.x == NetworkManager.Singleton.LocalClientId){
                    iconItem.sprite = shopUI.GetUIManager().characterDatabase.GetCharacterById((int)client.y).SkillIcon;
                }
            }
        }
        else
        {
            iconItem.sprite = item.iconItemSprite;
        }
        levelBar.sprite = item.levelBarListSprite[level];
        level++;
        if (item.sellBy == SellBy.Money)
        {
            localPlayer.GetMoney().OnValueChanged += UpdateMoneyValue;
            if (localPlayer.GetMoney().Value < price)
            {
                priceString.color = Color.red;
            }
            else
            {
                priceString.color = Color.white;
            }
        }
        else if (item.sellBy == SellBy.Gems)
        {
            localPlayer.GetGems().OnValueChanged += UpdateGemsValue;
            if (localPlayer.GetGems().Value < price)
            {
                priceString.color = Color.red;
            }
            else
            {
                priceString.color = new Color(1, 0.937f, 0.18f);
            }
        }
    }

    private void UpdateGemsValue(float previousValue, float newValue)
    {
        if (newValue < price)
        {
            priceString.color = Color.red;
        }
        else
        {
            priceString.color = new Color(1, 0.937f, 0.18f);
        }
    }

    private void UpdateMoneyValue(float previousValue, float newValue)
    {
        if (newValue < price)
        {
            priceString.color = Color.red;
        }
        else
        {
            priceString.color = Color.white;
        }
    }

    public void Buy()
    {
        if (item.sellBy == SellBy.Money)
        {
            if (localPlayer.GetMoney().Value < price)
            {
                return;
            }
        }
        else if (item.sellBy == SellBy.Gems)
        {
            if (localPlayer.GetGems().Value < price)
            {
                return;
            }
        }
        if (level == item.levelBarListSprite.Length)
        {
            return;
        }
        levelBar.sprite = item.levelBarListSprite[level];
        level++;
        priceString.text = item.price[level - 1].ToString();
        float soldPrice = price;
        price = item.price[level - 1];
        if (item.sellBy == SellBy.Money)
        {
            localPlayer.SetMoneyServerRpc(localPlayer.GetMoney().Value - soldPrice);
        }
        else if (item.sellBy == SellBy.Gems)
        {
            localPlayer.SetGemServerRpc(localPlayer.GetGems().Value - soldPrice);
        }
        switch (item.typeSell)
        {
            case TypeSell.ATK: BuyATK(); break;
            case TypeSell.ATKSPD: BuyATKSPD(); break;
            case TypeSell.SPEED: BuySPEED(); break;
            case TypeSell.HEALTH: BuyHEALTH(); break;
            case TypeSell.SKILL: BuySKILL(); break;
        }
        if (level == item.levelBarListSprite.Length)
        {
            maxedText.gameObject.SetActive(true);
            priceString.gameObject.SetActive(false);
        }
    }

    private void BuySKILL()
    {
        if (localPlayer.GetSkillState().Value == SkillState.Locked)
        {
            localPlayer.SetSkillStateServerRpc(SkillState.Unlocked);
            localPlayer.UnlockSkill();
            localPlayer.SetCanSkill(true);
        }
        return;
    }

    private void BuyHEALTH()
    {
        localPlayer.SetMaxHealthServerRpc(shopUI.GetPlayerController().GetMaxHealth().Value * (1 + item.percentIncrease / 100));
        Debug.Log(shopUI.GetPlayerController().GetMaxHealth().Value);
        Debug.Log(shopUI.GetPlayerController().GetCurrentHealth().Value);
    }

    private void BuySPEED()
    {
        localPlayer.SetSpeedServerRpc(shopUI.GetPlayerController().GetSpeed().Value * (1 + item.percentIncrease / 100));
        Debug.Log(shopUI.GetPlayerController().GetSpeed().Value);
    }

    private void BuyATKSPD()
    {
        localPlayer.SetCountDownNormalAttackServerRpc(shopUI.GetPlayerController().GetCountDownTime().Value * (1 - item.percentIncrease / 100));
        Debug.Log(shopUI.GetPlayerController().GetCountDownTime().Value);
    }

    private void BuyATK()
    {
        localPlayer.SetDamageServerRpc(shopUI.GetPlayerController().GetDamge().Value * (1 + item.percentIncrease / 100));
        Debug.Log(shopUI.GetPlayerController().GetDamge().Value);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        shopUI.GetInfoUI().SetActiveTrue(item,InfoUIPos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopUI.GetInfoUI().gameObject.SetActive(false);
    }
}
