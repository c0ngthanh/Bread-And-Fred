using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private MonsterController monsterController;
    private void Start() {
        monsterController.GetHP().OnValueChanged += SetNewHealth;
        barImage.fillAmount = 1;
        Hide();
    }

    private void SetNewHealth(float previousValue, float newValue)
    {
        if(barImage.fillAmount == 1){
            Show();
        }
        barImage.fillAmount = newValue/monsterController.GetMonsterData().HP;
        if(barImage.fillAmount ==0){
            Hide();
        }
    }
    private void Hide(){
        gameObject.SetActive(false);
    }
    private void Show(){
        gameObject.SetActive(true);
    }
    public void FlipHealthBar(){
        transform.localScale = new Vector3(-1*transform.localScale.x,transform.localScale.y,transform.localScale.z);
    }
}
