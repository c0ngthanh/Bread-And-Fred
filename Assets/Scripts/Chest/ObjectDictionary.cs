using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Object Dictionary")]
public class ObjectDictionary : ScriptableObject
{
    [SerializeField] public List<NetworkObject> objectDictionary;
}
