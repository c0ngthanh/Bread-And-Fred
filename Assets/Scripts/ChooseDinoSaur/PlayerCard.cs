using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private GameObject visuals;
    [SerializeField] private Image characterIconImage;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text characterNameText;
    // [SerializeField] private CharacterInfo characterInfo;

    public void UpdateDisplay(CharacterSelectState state)
    {
        if (state.CharacterId != -1)
        {
            var character = characterDatabase.GetCharacterById(state.CharacterId);
            characterIconImage.sprite = character.Icon;
            characterIconImage.enabled = true;
            characterNameText.text = character.DisplayName;
            // characterInfo.name.text = character.DisplayName;
            // characterInfo.skillIcon.sprite = character.SkillIcon;
            // characterInfo.des.text = character.Description + "\n" + character.Element;
            // if (IsOwner)
            // {
            //     characterInfo.gameObject.SetActive(true);
            // }
        }
        else
        {
            // if (IsOwner)
            // {
            //     characterInfo.gameObject.SetActive(false);
            // }
            characterIconImage.enabled = false;
        }

        playerNameText.text = state.IsLockedIn ? $"Player {state.ClientId}" : $"Player {state.ClientId} (Picking...)";

        visuals.SetActive(true);
    }
    public void DisableDisplay()
    {
        visuals.SetActive(false);
    }
}
