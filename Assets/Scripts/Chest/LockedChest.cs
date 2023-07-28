using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LockedChest : NormalChest
{
    [SerializeField] private NetworkVariable<bool> coditionOpen = new NetworkVariable<bool>(false);
    public override void Openchest()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (IsServer)
            {
                coditionOpen.Value = !coditionOpen.Value;
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && canOpen.Value == true)
        {
            if (coditionOpen.Value)
            {
                openServerRpc();
            }
            else
            {
                notificationUI.SetText("Locked");
                StartCoroutine(SetTextAgain());
            }
        }
    }
    private IEnumerator SetTextAgain()
    {
        yield return new WaitForSeconds(2);
        notificationUI.SetText("Press E to open");
    }
    public void SetCoditionOpen(bool value){
        coditionOpen.Value = value;
    }

}
