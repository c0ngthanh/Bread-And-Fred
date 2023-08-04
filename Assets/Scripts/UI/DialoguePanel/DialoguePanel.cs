using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public Image npcImage;
    public TMP_Text npcName;
    public TMP_Text dialogueText;
    
    public void SetActive(Npc npc){
        gameObject.SetActive(true);
        npcName.text = npc.GetNPCName();
        npcImage.sprite = npc.getNPCSprite();
    } 
}
