using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDie : NetworkBehaviour
{
    [SerializeField] GameObject[] players;
    [SerializeField] TeleportWaypoint teleportWaypoint;
    [SerializeField] bossAction boss;
    [SerializeField] SetUpRoom grid;
    [SerializeField] GameObject cage;
    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        boss = GameObject.FindGameObjectWithTag("StoneBoss").GetComponent<bossAction>();
        boss.currentHealth.OnValueChanged += BossDieAction;
    }

    private void BossDieAction(float previousValue, float newValue)
    {
        if (newValue <= 0)
        {
            cage.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var player in players)
        {
            if (player.GetComponent<PlayerController>() != null)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController.GetCurrentHealth().Value <= 0)
                {
                    if (IsServer)
                        Reset();
                }
            }
        }
    }
    // [ServerRpc(RequireOwnership = false)]
    private void Reset()
    {
        Debug.Log("Reset");
        foreach (var player in players)
        {
            player.GetComponent<PlayerController>().SetCurrentHealthServerRpc(player.GetComponent<PlayerController>().GetMaxHealth().Value);
            player.transform.position = teleportWaypoint.transform.position + Vector3.up * 2;
            // GameObject.FindGameObjectWithTag("BossStand").gameObject.SetActive(true);
        }
        Camera.main.GetComponent<CameraFollowPlayer>().flag = true;
        boss.SetHealthServerRpc(boss.maxHealth);
        boss.SetAngryStatusServerRpc(false);
        boss.SetAngrySpeedServerRpc(2f);
        boss.SetTypeActionServerRpc(2);
        boss.SetFollowServerRpc(false);
        boss.CallStatus(2);
        boss.SetFlagServerRpc(true);
        boss.transform.position = transform.position;
        grid.SetCloseDoorServerRpc(false);
        grid.SetBossAppearServerRpc(false);
        boss.transform.localScale = new Vector2(10, 10);
        boss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        boss.SetGrowServerRpc(false);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().flag = true;
        ResetClientRpc();
    }
    [ClientRpc]
    private void ResetClientRpc()
    {
        BackgroundAudio.Instance.SetDefaultBackgroundAudioServerRpc();
    }
}

