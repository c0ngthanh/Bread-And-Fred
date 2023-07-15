using UnityEngine;

[CreateAssetMenu(menuName = "Player Run Data")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class PlayerRunData : ScriptableObject
{
	[Header("Run")]
	public float moveSpeed; //Target speed we want the player to reach.
	public float acceleration; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
	[HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
	public float decceleration; //Time (approx.) we want it to take for the player to accelerate from runMaxSpeed to 0.
	public float velPower;
	public float frictionAmount;
	[HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
	[Space(10)]
	[Range(0.01f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
	[Range(0.01f, 1)] public float deccelInAir;
	public bool doConserveMomentum;


    // private void OnValidate()
    // {
	// 	//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
	// 	runAccelAmount = (50 * runAcceleration) / moveSpeed;
	// 	runDeccelAmount = (50 * runDecceleration) / moveSpeed;

	// 	#region Variable Ranges
	// 	runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, moveSpeed);
	// 	runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, moveSpeed);
	// 	#endregion
	// }
}
