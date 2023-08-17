using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitBtn : MonoBehaviour
{
    [SerializeField] private GameObject TutorialUI;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ExitTutorial);
        gameObject.SetActive(false);
    }

    private void ExitTutorial()
    {
        TutorialUI.SetActive(false);
    }
}
