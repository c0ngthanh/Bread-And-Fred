using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Character",menuName ="Characters/Character")]
public class Character : ScriptableObject
{
    [SerializeField] private int id = -1;
    [SerializeField] private string displayName = "New Display Name";
    [SerializeField] private Sprite  icon;
    [SerializeField] private GameObject introPrefab;
    [SerializeField] private NetworkObject gameplayPrefab;
    [SerializeField] private Sprite skillIcon;
    // [SerializeField] private string nameDisplay;
    [SerializeField] private string description;
    [SerializeField] private string element;


    public int Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public GameObject IntroPrefab => introPrefab;
    public NetworkObject GameplayPrefab => gameplayPrefab;
    public Sprite SkillIcon => skillIcon;
    public string Description => description;
    public string Element => element;
}
