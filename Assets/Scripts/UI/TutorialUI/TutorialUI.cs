using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] Page[] pageHolder;
    [SerializeField] Page currentPage;
    [SerializeField] int currentIndex;
    [SerializeField] Button nextPageBtn;
    [SerializeField] Button previousPageBtn;
    private void OnEnable() {
        for (int i = 0; i < pageHolder.Length; i++)
        {
            if(i==0){
                pageHolder[i].gameObject.SetActive(true);
            }else{
                pageHolder[i].gameObject.SetActive(false);
            }
        }
        currentIndex =0;
        currentPage = pageHolder[0];
    }
    private void Start() {
        nextPageBtn.onClick.AddListener(NextPage);
        previousPageBtn.onClick.AddListener(PreviousPage);
        gameObject.SetActive(false);
    }

    public void PreviousPage()
    {
        if(currentIndex == 0){
            return;
        }
        currentPage.gameObject.SetActive(false);
        currentIndex--;
        currentPage = pageHolder[currentIndex];
        currentPage.gameObject.SetActive(true); 
    }

    public void NextPage()
    {
        if(currentIndex == pageHolder.Length-1){
            return;
        }
        currentPage.gameObject.SetActive(false);
        currentIndex++;
        currentPage = pageHolder[currentIndex];
        currentPage.gameObject.SetActive(true); 
    }
}
