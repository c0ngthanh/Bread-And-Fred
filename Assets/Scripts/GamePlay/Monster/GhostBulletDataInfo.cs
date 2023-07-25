using UnityEngine;

[CreateAssetMenu(menuName = "GhostBulletDataInfo")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class GhostBulletDataInfo : ScriptableObject
{
	[Header("Force of Bullet")]
	public float forceX;
	public float forceY;
}
