using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Helper;

public class PlayerHolder : NetworkBehaviour
{
    public static PlayerHolder Instance { get; set; }

    public NetworkVariable<float> money = new NetworkVariable<float>(0);
    public NetworkVariable<float> gems = new NetworkVariable<float>(0);



    [SerializeField] PlayerController player1;
    [SerializeField] PlayerController player2;
    // Update is called once per frame
    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        player1.SetDirXServerRpc(0);
        player2.SetDirXServerRpc(0);
        if (Input.GetKeyDown(KeyCode.W))
        {
            player1.OnJumpInput();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            player2.OnJumpInput();
        }
        if (Input.GetKey(KeyCode.D))
        {
            player1.SetDirXServerRpc(1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            player1.SetDirXServerRpc(-1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (player1.IsGrounded())
            {
                player1.SetPlayerStateServerRpc(PlayerController.PlayerState.Sitting);
            }
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (player1.IsGrounded())
            {
                player1.SetPlayerStateServerRpc(PlayerController.PlayerState.Idle);
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            player2.SetDirXServerRpc(1);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            player2.SetDirXServerRpc(-1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (player2.IsGrounded())
            {
                player2.SetPlayerStateServerRpc(PlayerController.PlayerState.Sitting);
            }
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (player2.IsGrounded())
            {
                player2.SetPlayerStateServerRpc(PlayerController.PlayerState.Idle);
            }
        }
        if (Input.GetKeyDown(KeyCode.J) && player1.GetCanShoot())
        {
            player1.Attack();
        }
        if (Input.GetKeyDown(KeyCode.P) && player2.GetCanShoot())
        {
            player2.Attack();
        }
        if (Input.GetKeyDown(KeyCode.Q) && player1.GetCanSkill())
        {
            if (player1.GetSkillState().Value == SkillState.Locked)
            {

            }
            else
            {
                player1.UseSkill();
            }
        }
        if (Input.GetKeyDown(KeyCode.O) && player2.GetCanSkill())
        {
            if (player2.GetSkillState().Value == SkillState.Locked)
            {

            }
            else
            {
                Debug.Log("Player 2 skill");
                player2.UseSkill();
            }
        }
    }
    public NetworkVariable<float> GetGems()
    {
        return gems;
    }
    public NetworkVariable<float> GetMoney()
    {
        return money;
    }
    public void SetGems(float value)
    {
        gems.Value = value;
    }
    public void SetMoney(float value)
    {
        money.Value = value;
    }
    public PlayerController[] GetPlayerList()
    {
        return new PlayerController[] { player1, player2 };
    }
}

