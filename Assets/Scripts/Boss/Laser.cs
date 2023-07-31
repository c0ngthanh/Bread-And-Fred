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

    [SerializeField] bossAction boss;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // int type = GetComponent<bossAction>().GetTypeAction();
            // Debug.Log("type of boss " + type);
            // Debug.Log("type of boss ");

            // if (type == 2)
            // {

            Debug.Log("type of boss " + boss.GetTypeAction());
            if (boss.GetTypeAction().Value == 4)
            {
                other.gameObject.GetComponent<PlayerController>().TakeDamage(5);
            }
            // }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DoDelayAction(float delayTime)
    {
        StartCoroutine(DelayAction(delayTime));
    }

    IEnumerator DelayAction(float delayTime)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        //Do the action after the delay time has finished.

    }
}
