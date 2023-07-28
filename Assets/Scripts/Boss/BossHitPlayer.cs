using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossHitPlayer : MonoBehaviour
{
    public static void LaserAttackPlayer(Laser laser, PlayerController player)
    {
        player.TakeDamage(20);
        // Destroy(bullet.gameObject);
    }
}
