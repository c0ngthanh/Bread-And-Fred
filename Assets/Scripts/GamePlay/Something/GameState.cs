using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameState : NetworkBehaviour
{
    public static GameState Instance {get; private set;}
    NetworkVariable<State> gameState = new NetworkVariable<State>(State.Normal);
    public enum State{
        Normal,
        Rotate
    }
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
    public NetworkVariable<State> GetGameState(){
        return gameState;
    }
    public void SetGameState(State state) {
        gameState.Value = state;
    }
}
