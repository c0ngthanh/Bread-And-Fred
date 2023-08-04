using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoUI : MonoBehaviour
{
    [SerializeField] TMP_Text btnDesCription;
    private void Start() {
        gameObject.SetActive(false);
    }
    public void SetActiveTrue(ItemShopData item,Transform InfoUIPos){
        btnDesCription.text = item.itemDescription;
        transform.position = InfoUIPos.position;
        gameObject.SetActive(true);
    }
}
