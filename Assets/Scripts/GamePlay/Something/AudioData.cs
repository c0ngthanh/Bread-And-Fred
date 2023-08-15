using UnityEngine;

[CreateAssetMenu(menuName = "Audio Data")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class AudioData : ScriptableObject
{
    public AudioClip defaultAudio;
    public AudioClip bossAudio;
}
