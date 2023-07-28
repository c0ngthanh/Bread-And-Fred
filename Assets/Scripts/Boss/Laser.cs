using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Laser : MonoBehaviour
{
    public NetworkVariable<float> damage = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public void SetDamage(float damage)
    {
        this.damage.Value = damage;
    }
    public float GetDamage()
    {
        return this.damage.Value;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            BossHitPlayer.LaserAttackPlayer(this, other.gameObject.GetComponent<PlayerController>());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
