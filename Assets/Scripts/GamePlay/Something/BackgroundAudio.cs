using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BackgroundAudio : NetworkBehaviour
{
    [SerializeField] AudioData data;
    [SerializeField] AudioSource source;
    public static BackgroundAudio Instance {get;private set;}
    private void Awake() {
        Instance = this;
        source = GetComponent<AudioSource>();
        source.clip = data.defaultAudio;
        source.Play();
    }
    [ServerRpc(RequireOwnership =false)]
    public void SetDefaultBackgroundAudioServerRpc(){
        SetDefaultBackgroundAudioClientRpc();
    }
    [ClientRpc]
    public void SetDefaultBackgroundAudioClientRpc(){
        source.clip = data.defaultAudio;
        source.Play();
    }
    [ServerRpc(RequireOwnership =false)]
    public void SetBossBackgroundAudioServerRpc(){
        SetBossBackgroundAudioClientRpc();
    }
    [ClientRpc]
    public void SetBossBackgroundAudioClientRpc(){
        source.clip = data.bossAudio;
        source.Play();
    }
}
