using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Linq;
using UnityEngine;
using System;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;

    public BaseUnit SelectedUnit;
    public List<BaseHero> AllHeroes;
    public List<BaseEnemy> AllEnemies;
    public List<BaseBuilding> AllBuildings;

    void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        AllHeroes = new List<BaseHero>();
        AllEnemies = new List<BaseEnemy>();
        AllBuildings = new List<BaseBuilding>();
    }


    // Spawns the initial set of heroes
    public void SpawnHeroes()
    {
        var heroCount = 3;

        for (int i = 0; i < heroCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseHero>(UnitType.Hero);
            var spawnedHero = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            AllHeroes.Add(spawnedHero);
            spawnedHero.init();
            randomSpawnTile.SetUnit(spawnedHero);
        }

        GameManager.Instance.UpdateGameState(GameState.SpawnEnemies);
    }


    // Spawns the initial wave of enemies
    public void SpawnEnemies()
    {
        int enemyCount = 1;
        int bosses = 0;

        switch (GameManager.Instance.turn)
        {
            case 0:
                enemyCount = 6;
                break;
            case 5:
                enemyCount = 3;
                break;
            case 10:
                bosses = 1;
                break;
            case 15:
                bosses = 1;
                enemyCount = 3;
                break;
            case 18:
            case 19:
                enemyCount = 5;
                break;
            default:
                break;
        }

        // Spawn regular enemies
        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomEnemy(false);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            AllEnemies.Add(spawnedEnemy);
            spawnedEnemy.init();
            randomSpawnTile.SetUnit(spawnedEnemy);
        }

        // Spawn bosses
        for (int i = 0; i < bosses; i++)
        {
            var randomPrefab = GetRandomEnemy(true);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            AllEnemies.Add(spawnedEnemy);
            spawnedEnemy.init();
            randomSpawnTile.SetUnit(spawnedEnemy);
        }

        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);

    }


    // Spawns the initial set of buildings
    public void SpawnBuildings()
    {
        List<Vector2> locations = new List<Vector2>();
        locations.Add(new Vector2((GridManager.Instance.width / 2) - 1, (GridManager.Instance.height / 2) - 1));
        locations.Add(new Vector2((GridManager.Instance.width / 2) + 1, (GridManager.Instance.height / 2) - 1));
        locations.Add(new Vector2((GridManager.Instance.width / 2) - 1, (GridManager.Instance.height / 2) + 1));
        locations.Add(new Vector2((GridManager.Instance.width / 2) + 1, (GridManager.Instance.height / 2) + 1));

        while (locations.Count > 0)
        {
            var randomPrefab = GetRandomUnit<BaseBuilding>(UnitType.Building);
            var spawnedBuilding = Instantiate(randomPrefab);
            int n = UnityEngine.Random.Range(0, locations.Count);
            var randomSpawnTile = GridManager.Instance.GetTileAtPosition(locations[n]);

            AllBuildings.Add(spawnedBuilding);
            spawnedBuilding.init();
            randomSpawnTile.SetUnit(spawnedBuilding);
            locations.RemoveAt(n);
        }
    }

    private T GetRandomUnit<T>(UnitType unitType) where T : BaseUnit
    {
        return (T) _units.Where(u => u.unitType == unitType).OrderBy(o => UnityEngine.Random.value).First().UnitPrefab;
    }

    private BaseEnemy GetRandomEnemy(bool boss)
    {
        BaseEnemy enemy = (BaseEnemy)_units.Where(u => u.unitType == UnitType.Enemy).OrderBy(o => UnityEngine.Random.value).First().UnitPrefab;
        if (enemy.boss == boss)
        {
            return enemy;
        }
        else
        {
            return GetRandomEnemy(boss);
        }
    }

    public void SetSelectedUnit(BaseUnit hero)
    {
        if (SelectedUnit != null)
        {
            // De-highlight previous hero's available movement options
            SelectedUnit.ShowHighlightMoveTiles(false);
            SelectedUnit.ShowHighlightAttackTiles(false);

        }
        SelectedUnit = hero;
        MenuManager.Instance.ShowSelectedUnit(hero);
        
        // If we selected a new hero, show their attack and move options
        if (hero != null)
        {
            hero.ShowHighlightMoveTiles(true);
            hero.ShowHighlightAttackTiles(true);
        }
    }

    public void UpkeepHeroes()
    {
        foreach(var hero in AllHeroes)
        {
            hero.StartOfTurn();
        }
    }

    public void UpdatePlayerOptions()
    {
        foreach (var hero in AllHeroes)
        {
            hero.UpdateSelf();
        }
        foreach (var building in AllBuildings)
        {
            building.UpdateSelf();
        }
    }

    public void EnemyTurn()
    {
        foreach (var enemy in AllEnemies)
        {
            enemy.Turn();
        }
    }

}
