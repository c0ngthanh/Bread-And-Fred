using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitBtn : MonoBehaviour
{

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ExitTutorial);
    }

    private void ExitTutorial()
    {
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().tutorialUI.gameObject.SetActive(false);
    }
}
