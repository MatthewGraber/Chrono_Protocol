using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;
    public int turn = 0;    // What number turn it currently is
    public int roundsRemaining => 20 - turn;

    public static event Action<GameState> OnGameStateChanged;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateGameState(GameState.GenerateGrid);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnHeroes:
                UnitManager.Instance.SpawnBuildings();
                UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                break;
            case GameState.PlayerTurn:
                UnitManager.Instance.UpkeepHeroes();
                MenuManager.Instance.UpdateRoundCounter(roundsRemaining);
                break;
            case GameState.EnemyTurn:
                UnitManager.Instance.EnemyTurn();
                turn++;
                //VictoryCheck();
                break;
            case GameState.Victory:
                MenuManager.Instance.showVictory(true);
                break;
            case GameState.Lose:
                MenuManager.Instance.showVictory(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    public void PlayerTurn()
    {
        UpdateGameState(GameState.PlayerTurn);
    }

    public void EndTurnButton()
    {
        if (State == GameState.PlayerTurn)
        {
            UpdateGameState(GameState.EnemyTurn);
        }
    }

    public void VictoryCheck()
    {
        if (UnitManager.Instance.AllBuildings.Count == 0)  // TODO: add victory logic
        {
            UpdateGameState(GameState.Lose);
        }
        else if (roundsRemaining == 0) // TODO: add defeat logic
        {
            UpdateGameState(GameState.Victory);
        }
        else if (State == GameState.EnemyTurn)
        {
            UpdateGameState(GameState.SpawnEnemies);
        }
    }

    public IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum GameState
{
    GenerateGrid,
    SpawnHeroes,
    SpawnEnemies,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Lose
}