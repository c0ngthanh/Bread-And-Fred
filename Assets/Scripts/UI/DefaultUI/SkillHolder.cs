using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SkillHolder : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private DefaultUI defaultUI;
    [SerializeField] private Image lockIcon;
    [SerializeField] private int idPlayer;
    [SerializeField] private TMP_Text countDownText;
    private void Start()
    {
        countDownText.gameObject.SetActive(false);
        if (GameMode.Instance.GetGameMode().Value == GameMode.Mode.Multi)
        {
            defaultUI.GetPlayerController().SkillBurstChanged += SkillBurstChangedAction;
            defaultUI.GetPlayerController().SkillStateChanged += SkillStateChangedAction;
            defaultUI.GetPlayerController().CountDownTimeChanged += CountDownTimeChangedAction;
            foreach (var client in ServerManager.Instance.clientAndCharacterID)
            {
                if (client.x == NetworkManager.Singleton.LocalClientId)
                {
                    image.sprite = defaultUI.GetUIManager().characterDatabase.GetCharacterById((int)client.y).SkillIcon;
                }
            }
        }
        else if (GameMode.Instance.GetGameMode().Value == GameMode.Mode.Single)
        {
            PlayerHolder playerHolder = PlayerHolder.Instance;
            if (playerHolder.GetPlayerList()[idPlayer].GetSkillBurstHolder().GetComponent<SkillBurstHolder>().GetNameSkill().GetComponent<RexSkill>() != null)
            {
                image.sprite = playerHolder.GetPlayerList()[idPlayer].GetSkillBurstHolder().GetComponent<SkillBurstHolder>().GetNameSkill().GetComponent<RexSkill>().GetSkillIcon();
            }
            if (playerHolder.GetPlayerList()[idPlayer].GetSkillBurstHolder().GetComponent<SkillBurstHolder>().GetNameSkill().GetComponent<PinoSkill>() != null)
            {
                image.sprite = playerHolder.GetPlayerList()[idPlayer].GetSkillBurstHolder().GetComponent<SkillBurstHolder>().GetNameSkill().GetComponent<PinoSkill>().GetSkillIcon();
            }
            playerHolder.GetPlayerList()[idPlayer].SkillBurstChanged += SkillBurstChangedAction;
            playerHolder.GetPlayerList()[idPlayer].SkillStateChanged += SkillStateChangedAction;
            playerHolder.GetPlayerList()[idPlayer].CountDownTimeChanged += CountDownTimeChangedAction;
        }
    }

    private void CountDownTimeChangedAction(object sender, float e)
    {
        countDownText.text = e.ToString();
        if(e>0){
            countDownText.gameObject.SetActive(true);
        }else{
            countDownText.gameObject.SetActive(false);
        }
    }

    private void SkillStateChangedAction(object sender, bool e)
    {
        lockIcon.gameObject.SetActive(false);
    }

    private void SkillBurstChangedAction(object sender, bool e)
    {
        if (e)
        {
            image.color = new Color(1, 1, 1, 1);
        }
        else
        {
            image.color = new Color(1, 1, 1, 0.5f);
        }
    }
    
}
