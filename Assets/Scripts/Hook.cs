using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform player;

    // Update is called once per frame

    private void Update()
    {
        float dirX = Input.GetAxisRaw("Horizontal");
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);


    }
}
