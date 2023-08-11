using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NormalChest : NetworkBehaviour
{
    [SerializeField] protected Transform spawnPoint;
    [SerializeField] protected ObjectDictionary dictionary;
    [Header("List of objects in chest")]
    [SerializeField] protected NetworkObject[] objectsList;
    [Header("Range of objects in chest")]
    [SerializeField] protected Vector2[] objectsCountRange;
    [SerializeField] protected List<Vector2> idAndNumber;
    protected UIManager uIManager;
    protected NotificationUI notificationUI;
    protected NetworkObject temp;
    protected NetworkVariable<bool> canOpen = new NetworkVariable<bool>(false);
    // public struct ObjectInChest
    // {
    //     GameObject chestObject;
    //     int number;
    // }
    protected void Start()
    {
        for (int i = 0; i < objectsList.Length; i++)
        {
            FindObjectinDictionary(objectsList[i], i);
        }
        uIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        notificationUI = uIManager.notificationUI;
    }
    protected void FindObjectinDictionary(NetworkObject obj, int index)
    {
        for (int i = 0; i < dictionary.objectDictionary.Count; i++)
        {
            if (dictionary.objectDictionary[i] == obj)
            {
                idAndNumber.Add(new Vector2(i, Random.Range((int)objectsCountRange[index].x, (int)objectsCountRange[index].y + 1)));
            }
        }
    }
    protected void Update()
    {
        Openchest();
    }
    public virtual void Openchest()
    {
        if (Input.GetKeyDown(KeyCode.E) && canOpen.Value == true)
        {
            openServerRpc();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer)
        {
            canOpen.Value = true;
        }
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerController>().IsLocalPlayer)
        {
            notificationUI.SetText("Press E to open");
            notificationUI.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsServer)
        {
            canOpen.Value = false;
        }
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerController>().IsLocalPlayer)
        {
            notificationUI.SetActive(false);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    protected void openServerRpc()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Open");
    }
    public void OnOpenAction()
    {
        if(!IsServer){
            return;
        }
        for (int i = 0; i < idAndNumber.Count; i++)
        {
            for (int j = 0; j < idAndNumber[i].y; j++)
            {
                temp = Instantiate(dictionary.objectDictionary[(int)idAndNumber[i].x], spawnPoint.position, Quaternion.identity);
                temp.Spawn();
                temp.gameObject.AddComponent<Rigidbody2D>();
                temp.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5f, 5f), 5), ForceMode2D.Impulse);
                temp.gameObject.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                temp.gameObject.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
            }
        }
        setActiveClientRpc(false);
    }
    [ClientRpc]
    private void setActiveClientRpc(bool value){
        gameObject.SetActive(value);
    }
}
