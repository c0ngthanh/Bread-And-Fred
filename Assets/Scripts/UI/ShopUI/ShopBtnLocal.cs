using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Helper;
using System;
using Unity.Netcode;
using UnityEngine.EventSystems;

public class ShopBtnLocal : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Item Info")]
    [SerializeField] private ItemShopData item;

    [Header("Dont edit please")]
    [SerializeField] private ShopUILocal shopUI;
    [SerializeField] private TMP_Text priceString;
    [SerializeField] private TMP_Text maxedText;
    [SerializeField] private Image iconCoin;
    [SerializeField] private TMP_Text nameItem;
    [SerializeField] private Image iconItem;
    [SerializeField] private Image levelBar;
    [SerializeField] private Transform InfoUIPos;
    private int level = 0;
    private float price;
    private PlayerHolder localPlayer;
    private void Start()
    {
        localPlayer = PlayerHolder.Instance;
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
        iconItem.sprite = item.iconItemSprite;
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
            localPlayer.SetMoney(localPlayer.GetMoney().Value - soldPrice);
        }
        else if (item.sellBy == SellBy.Gems)
        {
            localPlayer.SetGems(localPlayer.GetGems().Value - soldPrice);
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
        foreach (PlayerController playerController in localPlayer.GetPlayerList())
        {
            if (playerController.GetSkillBurstHolder().GetComponent<SkillBurstHolder>().GetNameSkill().GetComponent<PinoSkill>() != null)
            {
                if (item.itemName == "PINO Q")
                {
                    playerController.SetSkillStateServerRpc(SkillState.Unlocked);
                    playerController.UnlockSkill();
                    playerController.SetCanSkill(true);
                    Debug.Log(playerController + " Pino");
                }
            }
            if (playerController.GetSkillBurstHolder().GetComponent<SkillBurstHolder>().GetNameSkill().GetComponent<RexSkill>() != null)
            {
                if (item.itemName == "REX Q")
                {
                    playerController.SetSkillStateServerRpc(SkillState.Unlocked);
                    playerController.UnlockSkill();
                    playerController.SetCanSkill(true);
                    Debug.Log(playerController + " Rex");
                }
            }
        }
        // if (localPlayer.GetSkillState().Value == SkillState.Locked)
        // {
        //     localPlayer.SetSkillStateServerRpc(SkillState.Unlocked);
        //     localPlayer.UnlockSkill();
        //     localPlayer.SetCanSkill(true);
        // }
        return;
    }

    private void BuyHEALTH()
    {
        foreach (PlayerController playerController in localPlayer.GetPlayerList())
        {
            playerController.SetMaxHealthServerRpc(playerController.GetMaxHealth().Value * (1 + item.percentIncrease / 100));
            Debug.Log(playerController + " " + playerController.GetMaxHealth().Value);
        }
    }

    private void BuySPEED()
    {
        // localPlayer.SetSpeedServerRpc(shopUI.GetPlayerController().GetSpeed().Value * (1 + item.percentIncrease / 100));
        // Debug.Log(shopUI.GetPlayerController().GetSpeed().Value);
        foreach (PlayerController playerController in localPlayer.GetPlayerList())
        {
            playerController.SetSpeedServerRpc(playerController.GetSpeed().Value * (1 + item.percentIncrease / 100));
            Debug.Log(playerController + " " + playerController.GetSpeed().Value);
        }
    }

    private void BuyATKSPD()
    {
        // localPlayer.SetCountDownNormalAttackServerRpc(shopUI.GetPlayerController().GetCountDownTime().Value * (1 - item.percentIncrease / 100));
        // Debug.Log(shopUI.GetPlayerController().GetCountDownTime().Value);
        foreach (PlayerController playerController in localPlayer.GetPlayerList())
        {
            playerController.SetCountDownNormalAttackServerRpc(playerController.GetCountDownTime().Value * (1 - item.percentIncrease / 100));
            Debug.Log(playerController + " " + playerController.GetCountDownTime().Value);
        }
    }

    private void BuyATK()
    {
        // // localPlayer.SetDamageServerRpc(shopUI.GetPlayerController().GetDamge().Value * (1 + item.percentIncrease / 100));
        // Debug.Log(shopUI.GetPlayerController().GetDamge().Value);
        foreach (PlayerController playerController in localPlayer.GetPlayerList())
        {
            playerController.SetDamageServerRpc(playerController.GetDamge().Value * (1 - item.percentIncrease / 100));
            Debug.Log(playerController + " " + playerController.GetDamge().Value);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        shopUI.GetInfoUI().SetActiveTrue(item, InfoUIPos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopUI.GetInfoUI().gameObject.SetActive(false);
    }
}
