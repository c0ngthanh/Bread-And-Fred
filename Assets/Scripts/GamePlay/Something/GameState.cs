using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameState : MonoBehaviour
{

    public enum State{
        Normal,
        Rotate
    }
    public static State gameState = State.Normal;

    public static State GetGameState(){
        return gameState;
    }
    public static void SetGameState(State value){
        gameState = value;
    }
}
