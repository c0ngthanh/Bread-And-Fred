using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Helper;

public class PlayerDie : NetworkBehaviour
{
    [SerializeField] GameObject[] players;
    [SerializeField] TeleportWaypoint teleportWaypoint;
    [SerializeField] bossAction boss;
    [SerializeField] SetUpRoom grid;
    [SerializeField] GameObject cage;
    [SerializeField] LosePanel losePanel;
    private int countDown = 5;
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
                    {
                        foreach (var deadplayer in players)
                        {
                            deadplayer.gameObject.SetActive(false);
                            deadplayer.GetComponent<PlayerController>().SetCurrentHealthServerRpc(deadplayer.GetComponent<PlayerController>().GetMaxHealth().Value);
                            if (deadplayer.GetComponent<PlayerController>().GetSkillState().Value == SkillState.Unlocked)
                            {
                                deadplayer.GetComponent<PlayerController>().SetCanSkill(true);
                            }

                        }
                        losePanel.countDownText.text = countDown.ToString();
                        losePanel.gameObject.SetActive(true);
                        StartCoroutine(ResetCoroutine());
                    }
                }
            }
        }
    }

    private IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(1);
        countDown -= 1;
        losePanel.countDownText.text = countDown.ToString();
        if (countDown <= 0)
        {
            Reset();
            countDown = 5;
            yield return null;
        }
        else
        {
            StartCoroutine(ResetCoroutine());
        }
    }

    // [ServerRpc(RequireOwnership = false)]
    private void Reset()
    {
        losePanel.gameObject.SetActive(false);
        foreach (var player in players)
        {
            player.gameObject.SetActive(true);
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

