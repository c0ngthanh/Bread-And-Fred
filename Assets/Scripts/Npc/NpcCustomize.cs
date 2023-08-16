using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NpcCustomize : Npc
{
    public override void NextLine(){
        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            NetworkManager.Singleton.SceneManager.LoadScene("CreditScene", LoadSceneMode.Single);
        }
    }
}
