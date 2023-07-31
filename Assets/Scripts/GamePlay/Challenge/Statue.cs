using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Helper;

public class Statue : NetworkBehaviour
{
    [SerializeField] Element element;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject activeStatus;
    [SerializeField] private bool isActive=false;
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        switch (element)
        {
            case Element.Fire: spriteRenderer.color = Color.red; break;
            case Element.Water: spriteRenderer.color = Color.blue; break;
            case Element.Dendro: spriteRenderer.color = Color.green; break;
            case Element.Elec: spriteRenderer.color = new Color(0.509804f,0,1); break;
        }
        activeStatus.SetActive(false);
    }
    public void CheckBullet(Bullet bullet) {
        if(bullet.GetElement() == this.element){
            SetStatusClientRpc(true);
        }
    }
    [ClientRpc]
    private void SetStatusClientRpc(bool Value){
        activeStatus.SetActive(Value);
        isActive= true;
        if(GetComponentInParent<ShootStatueChallenge>() != null){
            GetComponentInParent<ShootStatueChallenge>().ActiveStatue();
        }
    }
    public bool GetIsActive(){
        return this.isActive;
    }
}
