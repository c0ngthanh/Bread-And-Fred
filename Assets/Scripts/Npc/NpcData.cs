using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "NpcData")]
public class NpcData : ScriptableObject
{
    public Sprite npcSprite;
    public string npcName;
    public float scale;
    public string[] dialogue;
}
