using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShopKeeper : NetworkBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private UIManager uIManager;
    private bool canBuy = false;
    private void Start() {
        uIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    }
    protected void Update()
    {
        Openchest();
    }
    public virtual void Openchest()
    {
        if (Input.GetKeyDown(KeyCode.E) && canBuy == true)
        {
            uIManager.shopUI.SetActiveStatus();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer)
        {
        }
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerController>().IsLocalPlayer)
        {
            canBuy = true;
            uIManager.notificationUI.SetText("Press E to buy");
            uIManager.notificationUI.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerController>().IsLocalPlayer)
        {
            uIManager.notificationUI.SetActive(false);
            canBuy = false;
        }
    }
}
