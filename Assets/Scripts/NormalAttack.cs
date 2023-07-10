using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NormalAttack : NetworkBehaviour
{
    public NetworkVariable<Vector3> dir = new NetworkVariable<Vector3>(new Vector3(0,1,0),NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    private float speed=0;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        speed = 5;
        dir.OnValueChanged += ChangValue;
        // PlayerController.Instance.onAttacking += PlayerController_OnAttacking;
    }

    private void ChangValue(Vector3 previousValue, Vector3 newValue)
    {
        Debug.Log("ChangValue: " + previousValue + " " + newValue);
    }

    public void SetDir(Vector3 dir){
        this.dir.Value = dir;
    }
    // private void PlayerController_OnAttacking(object sender, PlayerController.onAttackArgs e)
    // {
    //     dir = e.dir;
    // }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(transform.position + " " + this.dir + " " + speed);
        transform.position += speed * Time.deltaTime * this.dir.Value.normalized;
    }
}
