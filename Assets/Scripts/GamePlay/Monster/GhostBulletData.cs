using UnityEngine;

[CreateAssetMenu(menuName = "GhostBulletData")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class GhostBulletData : ScriptableObject
{
	[Header("Type of Bullet")]
	public GhostBullet[] bulletList;
}
