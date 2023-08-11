using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Laser : MonoBehaviour
{
    [SerializeField] bossAction boss;
    private float CurrentTimer = 0;
    private float TimeBetweenTicks = 2f;


    private void OnTriggerStay2D(Collider2D other)
    {
        // new WaitForSeconds(10);
        if ((boss.GetTypeAction().Value == 4 || boss.GetTypeAction().Value == 5) && other != null)
        {

            CurrentTimer += Time.deltaTime;
            if (CurrentTimer >= TimeBetweenTicks)
            {
                if (other.gameObject.GetComponent<PlayerController>() != null)
                {
                    if (!boss.GetComponent<bossAction>().GetAngryStatus().Value)
                    {
                        other.gameObject.GetComponent<PlayerController>().TakeDamage(2);
                    }
                    else
                    {
                        other?.gameObject.GetComponent<PlayerController>().TakeDamage(3);
                    }
                    CurrentTimer = 0;
                }
            }
        }
    }


    // IEnumerator IncreaseHealth(float time)
    // {
    //     if (isTrigger)
    //     {
    //         Debug.Log("laser" + player.GetHealth().Value);
    //         player.TakeDamage(5);
    //         // player.TakeDamage(2);
    //         isTrigger = !isTrigger;
    //         yield return new WaitForSeconds(time);
    //         Debug.Log("========");
    //         isTrigger = !isTrigger;
    //     }
    // }
}
