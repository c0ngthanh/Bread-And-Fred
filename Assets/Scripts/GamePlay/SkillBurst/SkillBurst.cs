using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillBurst : MonoBehaviour
{
    [SerializeField] private SkillBurstHolder skillBurstHolder;
    private void Start() {
    }
    public void EndAnimation(){
        Destroy(gameObject);
        Destroy(skillBurstHolder.gameObject);
    }
}
