using UnityEngine;

[CreateAssetMenu(menuName = "MonsterData")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class MonsterData : ScriptableObject
{
    [Header("Property")]
    public float HP;
    public float speed;
}
