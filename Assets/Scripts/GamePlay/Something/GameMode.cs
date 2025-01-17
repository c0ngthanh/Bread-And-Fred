using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameMode : NetworkBehaviour
{
    public static GameMode Instance {get; private set;}
    NetworkVariable<Mode> gameMode = new NetworkVariable<Mode>(Mode.Single);
    NetworkVariable<bool> cheat = new NetworkVariable<bool>(false);
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public enum Mode{
        Single,
        Multi
    }
    public NetworkVariable<bool> GetCheat(){
        return cheat;
    }
    public void SetCheat(bool state) {
        cheat.Value = state;
    }
    public NetworkVariable<Mode> GetGameMode(){
        return gameMode;
    }
    public void SetGameMode(Mode state) {
        gameMode.Value = state;
    }
}
