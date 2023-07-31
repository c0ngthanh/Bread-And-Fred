using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Helper;
using Unity.Netcode;

public class HealthBar : NetworkBehaviour
{
    // Start is called before the first frame update
    private GameObject boss;
    public Slider slider;
    private NetworkVariable<float> hp = new NetworkVariable<float>();
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("StoneBoss");
        if (boss != null)
        {
            SetMaxHealth(100);
        }

        hp.Value = boss.GetComponent<bossAction>().GetHealth().Value;
        hp.OnValueChanged += UpdateHealthBar;
    }
    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }
    public void SetHealth(float health)
    {
        slider.value = health;
    }


    private void UpdateHealthBar(float previousValue, float newValue)
    {
        this.SetHealth(newValue);
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log("chay ham update");
        if (boss == null)
        {
            boss = GameObject.FindGameObjectWithTag("StoneBoss");
            if (boss != null)
            {
                SetMaxHealth(100);
            }
        }
        else
        {
            hp.Value = boss.GetComponent<bossAction>().GetHealth().Value;
        }

    }
}
