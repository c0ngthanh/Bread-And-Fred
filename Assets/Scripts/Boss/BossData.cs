using UnityEngine;

[CreateAssetMenu(menuName = "BossData")]
public class BossData : ScriptableObject
{
    [Header("Property")]
    public float health = 90;
    public float speed = 2;
}
