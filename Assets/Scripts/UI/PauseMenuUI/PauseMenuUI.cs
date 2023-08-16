using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : NetworkBehaviour
{
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Toggle cheatMode;
    [SerializeField] private Button homeBtn;
    // Start is called before the first frame update
    void Start()
    {
        resumeBtn.onClick.AddListener(ResumeGame);
        homeBtn.onClick.AddListener(ReturnHome);
        gameObject.SetActive(false);
    }

    private void ReturnHome()
    {
        NetworkManager.Singleton.Shutdown();
        NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
        Time.timeScale = 1;
    }

    private void ResumeGame()
    {
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().pauseMenuUI.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
