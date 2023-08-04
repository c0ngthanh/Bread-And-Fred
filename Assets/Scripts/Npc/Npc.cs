using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Npc : NetworkBehaviour
{
    [SerializeField] NpcData npcData;


    private UIManager uIManager;
    private DialoguePanel dialoguePanel;
    private TMP_Text dialogueText;
    private string npcName;
    private Sprite npcSprite;
    private string[] dialogue;
    private int index;
    private float wordSpeed = 0.06f;
    private bool canTalk = false;


    void Start()
    {
        uIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        dialoguePanel = uIManager.dialoguePanel;
        dialogue = npcData.dialogue;
        npcSprite = npcData.npcSprite;
        npcName = npcData.npcName;
        transform.localScale = new Vector3(npcData.scale, npcData.scale, npcData.scale);
        GetComponent<SpriteRenderer>().sprite = npcSprite;
        dialogueText = dialoguePanel.GetComponent<DialoguePanel>().dialogueText;
        dialoguePanel.GetComponent<DialoguePanel>().npcName.text = npcName;
        dialoguePanel.GetComponent<DialoguePanel>().npcImage.sprite = npcSprite;
        dialoguePanel.gameObject.SetActive(false);
        dialogueText.text = "";
    }
    private void OnEnable() {
        Debug.Log("Enable");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTalk == true)
        {
            if (!dialoguePanel.gameObject.activeInHierarchy)
            {
                dialoguePanel.SetActive(this);
                dialogueText.text = "";
                StartCoroutine(Typing());
            }
            else if (dialogueText.text.Length < dialogue[index].Length)
            {
                wordSpeed = 0.001f;
            }
            else if (dialogueText.text == dialogue[index])
            {
                wordSpeed = 0.06f;
                NextLine();
            }
        }
    }
    public void NextLine()
    {
        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            ZeroText();
        }
    }
    private void ZeroText()
    {
        dialogueText.text = "";
        wordSpeed = 0.06f;
        index = 0;
        dialoguePanel.gameObject.SetActive(false);
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            if(!canTalk){
                Debug.Log("StopCoroutine");
                yield break;
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer)
        {
        }
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerController>().IsLocalPlayer)
        {
            canTalk = true;
            uIManager.notificationUI.SetText("Press E to see");
            uIManager.notificationUI.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerController>().IsLocalPlayer)
        {
            uIManager.notificationUI.SetActive(false);
            canTalk = false;
            ZeroText();
        }
    }
    public string GetNPCName(){
        return npcName;
    }
    public Sprite getNPCSprite(){
        return npcSprite;
    }
}
