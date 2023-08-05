using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Laser : MonoBehaviour
{
    [SerializeField] bossAction boss;
    // private RaycastHit2D raycastHit2D;
    // [SerializeField] private LayerMask playerLayer;

    // Vector2 sample;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (boss.GetTypeAction().Value == 4)
            {
                other.gameObject.GetComponent<PlayerController>().TakeDamage(5);
            }
            // }
        }
    }

    // void Start()
    // {


    // }

    // void Update()
    // {
    //     sample = transform.position;
    //     sample.y -= 0.5f;

    //     raycastHit2D = Physics2D.Raycast(sample, new Vector2(7, -2), 40f, playerLayer);
    //     Debug.DrawRay(sample, new Vector2(7, -2), Color.red, 20f);

    //     if (raycastHit2D.collider != null)
    //     {
    //         Debug.Log("va cham laser +  " + raycastHit2D.collider);
    //     }
    // }

}
