using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMP_Text textMeshPro;
    void Start()
    {
        gameObject.SetActive(false);
    }
    public void SetText(string text){
        textMeshPro.text = text;
    }
    public void SetActive(bool value){
        gameObject.SetActive(value);
    }
}
