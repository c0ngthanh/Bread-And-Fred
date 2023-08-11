using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBtn : MonoBehaviour
{
    private void Start() {
        this.GetComponent<Button>().onClick.AddListener(ActiveTutorialUI);
    }

    private void ActiveTutorialUI()
    {
        Debug.Log("Open tutorial");
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().tutorialUI.gameObject.SetActive(true);
    }
}
