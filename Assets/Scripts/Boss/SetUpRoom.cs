using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;
using Unity.Netcode;
public class SetUpRoom : NetworkBehaviour
{
    //man che 
    [SerializeField] private GameObject bossRoomPanel;

    // Start is called before the first frame update 
    GameObject player1, player2;
    GameObject boss;
    private GameObject finalDoor;
    private GameObject bossStand;
    private NetworkVariable<bool> closeDoor = new NetworkVariable<bool>();
    private NetworkVariable<bool> bossAppear = new NetworkVariable<bool>();


    private GameObject[] gameObjectArray;

    void Start()
    {
        gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
        boss = GameObject.FindGameObjectWithTag("StoneBoss");
        if (gameObjectArray.Length == 2)
        {
            player1 = gameObjectArray[1];
            player2 = gameObjectArray[0];
        }
        else if (gameObjectArray.Length == 1)
        {
            player1 = gameObjectArray[0];
            player2 = gameObjectArray[0];
        }
        bossRoomPanel.SetActive(true);

        closeDoor.Value = false;
        closeDoor.OnValueChanged += setUpDoor;
        finalDoor = GameObject.FindGameObjectWithTag("FinalDoor");
        finalDoor.SetActive(false);

        bossAppear.Value = false;
        bossAppear.OnValueChanged += setUpBossAppear;
        bossStand = GameObject.FindGameObjectWithTag("BossStand");
        bossStand.SetActive(true);

        boss.GetComponent<bossAction>().GetDie().OnValueChanged += BossDeath;
    }

    // Update is called once per frame 

    private void BossDeath(bool oldVal, bool newVal)
    {
        if (newVal)
        {
            SetBossAppearServerRpc(false);
            Debug.Log("trang thai boss _ boss die" + bossAppear.Value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCloseDoorServerRpc(bool val)
    {
        if (IsServer)
        {
            this.closeDoor.Value = val;
        }
    }
    public NetworkVariable<bool> GetCloseDoor()
    {
        return this.closeDoor;
    }



    [ServerRpc(RequireOwnership = false)]
    public void SetBossAppearServerRpc(bool val)
    {
        if (IsServer)
        {
            this.bossAppear.Value = val;
        }
    }
    public NetworkVariable<bool> GetBossAppear()
    {
        return this.bossAppear;
    }


    void Update()
    {

        Debug.Log("close door " + closeDoor.Value);
        if (player1.transform.position.x < 78 && player1.transform.position.y > 143 && player2.transform.position.x < 78 && player2.transform.position.y > 143)
        {
            SetCloseDoorServerRpc(true);
        }
        if (player1.transform.position.x < 40 && player1.transform.position.y > 143 && player2.transform.position.x < 50 && player2.transform.position.y > 143)
        {
            // bossAction boss = Instantiate(bossPrefab, bossSpawnPoint);
            // boss.GetComponent<NetworkObject>().Spawn();
            SetBossAppearServerRpc(true);
        }
        Debug.Log("trang thai boss _ boss appear" + bossAppear.Value);
    }

    private void setUpDoor(bool oldVal, bool newVal)
    {
        finalDoor.SetActive(newVal);
        // MapPanel.SetActive(newVal);
        bossRoomPanel.SetActive(oldVal);
    }


    private void setUpBossAppear(bool oldVal, bool newVal)
    {
        if (!newVal)
        {
            bossStand.SetActive(true);
        }
        else
        {
            bossStand.SetActive(false);
            BackgroundAudio.Instance.SetBossBackgroundAudioServerRpc();
        }
    }
}