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
        // textMeshPro.text = "Press E to open chest";
    }
    public void SetText(string text){
        textMeshPro.text = text;
    }
    public void OnPlayerTriggerEnterWithChest(){
        // textMeshPro.text = "Press E to open chest";
        gameObject.SetActive(true);
    }
    public void OnPlayerTriggerExitWithChest(){
        // textMeshPro.text = "Exit with chest";
        gameObject.SetActive(false);
    }
}
