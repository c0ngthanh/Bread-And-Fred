
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
    // private NetworkVariable<float> hp = new NetworkVariable<float>();
    [SerializeField] public GameObject grid;
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("StoneBoss");
        if (boss != null)
        {
            SetMaxHealth(100);
        }

        boss.GetComponent<bossAction>().GetHealth().OnValueChanged += UpdateHealthBar;
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
        // if (boss == null)
        // {
        //     boss = GameObject.FindGameObjectWithTag("StoneBoss");
        //     if (boss != null)
        //     {
        //         SetMaxHealth(100);
        //     }
        // }
        // else
        // {
        //     hp.Value = boss.GetComponent<bossAction>().GetHealth().Value;
        // }
    }

    private void Hide()
    {
        Debug.Log("Tat HB boss");
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
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


