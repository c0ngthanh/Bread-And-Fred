using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PauseBtn : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] UnityEngine.UI.Image image;
    private void Start() {
        image =  this.GetComponent<UnityEngine.UI.Image>();
        this.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ActivePauseUI);
    }

    private void ActivePauseUI()
    {
        Debug.Log("PauseGame");
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().pauseMenuUI.gameObject.SetActive(true);
        Time.timeScale =0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = new Color(1,1,1,0.3f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.white;
    }
}
