using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SkillHolder : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private DefaultUI defaultUI;
    [SerializeField] private Image lockIcon;
    private void Start()
    {
        defaultUI.GetPlayerController().SkillBurstChanged += SkillBurstChangedAction;
        defaultUI.GetPlayerController().SkillStateChanged += SkillStateChangedAction;
        foreach (var client in ServerManager.Instance.clientAndCharacterID)
        {
            if (client.x == NetworkManager.Singleton.LocalClientId)
            {
                image.sprite = defaultUI.GetUIManager().characterDatabase.GetCharacterById((int)client.y).SkillIcon;
            }
        }
    }

    private void SkillStateChangedAction(object sender, bool e)
    {
        lockIcon.gameObject.SetActive(false);
    }

    private void SkillBurstChangedAction(object sender, bool e)
    {
        if(e){
            image.color = new Color(1,1,1,1);
        }else{
            image.color = new Color(1,1,1,0.5f);
        }
    }
}
