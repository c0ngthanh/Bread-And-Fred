using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RexSkillBullet : NetworkBehaviour
{
    [SerializeField] Bullet[] listBullets;
    public override void OnNetworkSpawn()
    {
        foreach (Bullet bullet in listBullets)
        {   
            bullet.SetDir(gameObject.GetComponent<Bullet>().dir.Value);
            bullet.SetDamage(gameObject.GetComponent<Bullet>().damage.Value);
        }   
    }
}
