using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;

public class GhostBullet : Bullet
{
    [SerializeField] GhostBulletDataInfo ghostBulletDataInfo;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (IsServer)
            {
                if (this.GetComponent<GhostBullet>().GetElement() == Element.Fire)
                {
                    if (other.gameObject.GetComponent<PlayerController>().IsOwner)
                    {
                        other.gameObject.GetComponent<PlayerController>().disUpdate.Value = true;
                        StartCoroutine(RecallSetvelocityX(other.gameObject));
                    }
                }else{
                    gameObject.SetActive(false);
                }
                other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(ghostBulletDataInfo.forceX * this.dir.Value.x, ghostBulletDataInfo.forceY), ForceMode2D.Impulse);
            }
        }
    }

    IEnumerator RecallSetvelocityX(GameObject other)
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(SetvelocityX(other));
        // gameObject.SetActive(false);
    }

    IEnumerator SetvelocityX(GameObject other)
    {
        if (other.GetComponent<PlayerController>().IsGrounded())
        {
            other.gameObject.GetComponent<PlayerController>().disUpdate.Value = false;
            Destroy(gameObject);
            yield break;
        }
        yield return new WaitForSeconds(Time.deltaTime);
        StartCoroutine(SetvelocityX(other));
    }
}
