using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    NotStarted,
    Preparing,
    Idle,
    Running
}

public class GameController : Subject
{
    private GameState _gameState = GameState.NotStarted;

    public GameState GameState
    {
        get => _gameState;
        set
        {
            _gameState = value;

            for(int i = 0; i < Observers.Count; i++)
                Observers[i].Notify(value);
        }
    }

    public GameState GetState()
    {
        return GameState;
    }

    public void SetState(GameState state)
    {
        GameState = state;
    }
}
