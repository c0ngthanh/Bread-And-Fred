using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBullet : Bullet
{
    [SerializeField] GhostBulletDataInfo ghostBulletDataInfo;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(IsServer){
                Debug.Log(ghostBulletDataInfo.forceX*this.dir.Value.x);
                Destroy(gameObject);
                other.GetComponent<Rigidbody2D>().AddForce(new Vector2(ghostBulletDataInfo.forceX*this.dir.Value.x, ghostBulletDataInfo.forceY),ForceMode2D.Impulse);
            }
        }
    }
}
