using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillBurstHolder : MonoBehaviour
{
    [SerializeField] GameObject nameSkill;
    [SerializeField] float countDown;
    public float GetCountDown(){
        return countDown;
    }
    public GameObject GetNameSkill(){
        return nameSkill;
    }
}
