
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Helper;
using Unity.Netcode;

public class HealthBarPlayer2 : NetworkBehaviour
{
    // Start is called before the first frame update
    private GameObject player1;
    public Slider slider;
    private NetworkVariable<float> hp = new NetworkVariable<float>();
    private GameObject[] gameObjectArray;
    [SerializeField] public GameObject grid;
    void Start()
    {
        gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
        if (gameObjectArray.Length == 2)
        {
            if (gameObjectArray[0].GetComponent<PlayerController>().GetBullet().GetElement() == Element.Fire)
            {
                player1 = gameObjectArray[0];
            }
            else
            {
                player1 = gameObjectArray[1];
            }
        }
        else if (gameObjectArray.Length == 1)
        {
            player1 = gameObjectArray[0];

        }
        if (player1 != null)
        {
            SetMaxHealth(100);
        }
        player1.GetComponent<PlayerController>().GetHealth().OnValueChanged += UpdateHealthBar;
        grid.GetComponent<SetUpRoom>().GetBossAppear().OnValueChanged += UpdateHealthBarStatus;
        Hide();

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

    private void UpdateHealthBarStatus(bool previousValue, bool newValue)
    {
        if (newValue)
        {
            Show();
        }
        else
        {
            DoDelayAction(2);
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (gameObjectArray.Length == 0)
        {
            GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
            if (gameObjectArray.Length == 2)
            {
                if (gameObjectArray[0].GetComponent<PlayerController>().GetBullet().GetElement() == Element.Fire)
                {
                    player1 = gameObjectArray[0];
                }
                else
                {
                    player1 = gameObjectArray[1];
                }
            }
            else if (gameObjectArray.Length == 1)
            {
                player1 = gameObjectArray[0];

            }

        }
        else
        {
            hp.Value = player1.GetComponent<PlayerController>().GetHealth().Value;
        }
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void Show()
    {
        this.gameObject.SetActive(true);
    }


    void DoDelayAction(float delayTime)
    {
        StartCoroutine(DelayAction(delayTime));
    }

    IEnumerator DelayAction(float delayTime)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);
        Hide();
    }

}