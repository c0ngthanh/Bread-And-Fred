using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatMode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Toggle>().onValueChanged.AddListener(CheatOnValueChanged);
    }

    private void CheatOnValueChanged(bool arg0)
    {
        GameMode.Instance.SetCheat(arg0);
    }
}
