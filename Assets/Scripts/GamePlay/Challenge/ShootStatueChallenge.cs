using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootStatueChallenge : NetworkBehaviour
{
    [SerializeField] List<Statue> statues;
    [SerializeField] List<LockedChest> lockedChests;
    // Start is called before the first frame update
    private int numberOfActiveStatues =0;
    private int totalStatues;
    void Start()
    {
        totalStatues = statues.Count;
    }
    public void ActiveStatue(){
        numberOfActiveStatues++;
        if(numberOfActiveStatues == totalStatues){
            Debug.Log("Unlock Chest");
            UnlockChest();
        }
    }

    private void UnlockChest()
    {
        foreach (LockedChest chest in lockedChests)
        {
            if(IsServer){
                chest.SetCoditionOpen(true);
            }
        }
    }
}
