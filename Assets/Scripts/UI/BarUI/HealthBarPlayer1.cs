using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Helper;
using Unity.Netcode;

public class HealthBarPlayer1 : NetworkBehaviour
{
    // Start is called before the first frame update
    private GameObject player1;
    public Slider slider;
    // private Element element;
    private NetworkVariable<float> hp = new NetworkVariable<float>();
    private GameObject[] gameObjectArray;
    void Start()
    {
        gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
        if (gameObjectArray[0].GetComponent<PlayerController>().GetBullet().GetElement() == Element.Water)
        {
            player1 = gameObjectArray[0];
        }
        else
        {
            player1 = gameObjectArray[1];
        }
        if (player1 != null)
        {
            SetMaxHealth(100);
        }
        hp.Value = player1.GetComponent<PlayerController>().GetHealth().Value;
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
        if (gameObjectArray.Length == 0)
        {
            GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
            if (gameObjectArray[0].GetComponent<PlayerController>().GetBullet().GetElement() == Element.Water)
            {
                player1 = gameObjectArray[0];
            }
            else
            {
                player1 = gameObjectArray[1];
            }
            if (player1 != null)
            {
                SetMaxHealth(100);
            }

        }
        else
        {
            hp.Value = player1.GetComponent<PlayerController>().GetHealth().Value;
        }

    }
}
