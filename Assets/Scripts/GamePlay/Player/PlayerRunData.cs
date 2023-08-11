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
    [Header("Jump")]
    public float jumpForce;
    public float jumpInputBufferTime;
    public float coyoteTime;
    [Header("Rotate")]
    public float rotateForce;
    [Header("Properties")]
    public int health; //when attack with stone boss
}
