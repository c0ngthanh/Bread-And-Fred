using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UIManager : NetworkBehaviour
{
    public CharacterDatabase characterDatabase;
    public NotificationUI notificationUI;
    public ShopUI shopUI;
    public ShopUILocal shopUILocal;
    public DefaultUI defaultUI;
    public DialoguePanel dialoguePanel;
    public MapUI mapUI;
    public TutorialUI tutorialUI;
}
