using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public NetworkVariable<Vector3> dir = new NetworkVariable<Vector3>(new Vector3(0, 1, 0), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> damage = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private float speed = 0;
    private float scale = 1.2f;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        speed = 5;
        Debug.Log("OnnetworkSpawn bullet");
        Debug.Log(speed);
    }
    public void SetDamage(float damage)
    {
        this.damage.Value = damage * scale;
    }
    public float GetDamage()
    {
        return this.damage.Value;
    }

    public void SetDir(Vector3 dir)
    {
        this.dir.Value = dir;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Monster" && IsHost)
            MonsterAndBullet.BulletAttackMonster(this, other.gameObject.GetComponent<MonsterController>());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += speed * Time.deltaTime * this.dir.Value.normalized;
    }
}
