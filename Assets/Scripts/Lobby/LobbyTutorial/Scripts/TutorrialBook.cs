using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorrialBook : MonoBehaviour
{

    [SerializeField] private GameObject tutorialUI;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OpenUI);
    }
    public void OpenUI()
    {
        tutorialUI.gameObject.SetActive(true);
    }
}
