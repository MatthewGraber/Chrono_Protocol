using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public UnitType unitType;
    public string UnitName;

    // Unit stats
    [SerializeField] protected int MAX_HP;
    [SerializeField] protected int SPEED;
    protected int HP;
    public int movement;
    public int actionsRemaining = 1;
    public bool player => unitType == UnitType.Hero || unitType == UnitType.Building;

    public int ID;
    protected static int idIncrement = 0;

    [SerializeField] public BaseAttack attack;


    public List<List<Vector2>> availablePaths;
    public List<Vector2> availableSpaces;

    protected List<Vector2> currentPath;

    public float TIME_BETWEEN_MOVES = 0.1f;

    // True while the character is acting
    public bool acting = false;
    public bool moving = false;

    public virtual void init()
    {
        availablePaths = new List<List<Vector2>>();
        availableSpaces = new List<Vector2>();
        currentPath = new List<Vector2>();
        HP = MAX_HP;
        movement = SPEED;

        idIncrement++;
        ID = idIncrement;
    }

    // Basic interaction functions
    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            switch (unitType) {
                case UnitType.Hero:
                    UnitManager.Instance.AllHeroes.Remove((BaseHero) this);
                    break;
                case UnitType.Building:
                    UnitManager.Instance.AllBuildings.Remove((BaseBuilding) this);
                    break;
                case UnitType.Enemy:
                    UnitManager.Instance.AllEnemies.Remove((BaseEnemy) this);
                    break;
                default:
                    break;
            }
            OccupiedTile.SetUnit(null);
            Destroy(this.gameObject);

            UnitManager.Instance.UpdatePlayerOptions();
        }
    }

    public void Heal(int health)
    {
        HP += health;
        if (HP > MAX_HP) 
            HP = MAX_HP;
    }

    public void FindAvailableSpaces()
    {
        availablePaths.Clear();
        availableSpaces.Clear();

        // If the unit has no movement remaining, return immediately
        if (movement <= 0)
            return;

        List<Vector2> explored = new List<Vector2>();
        List<List<Vector2>> frontier = new List<List<Vector2>>();
        Vector2 currentPos = OccupiedTile.pos;

        // Adds the current space as an option
        List<Vector2> initPath = new List<Vector2>();
        initPath.Add(currentPos);
        explored.Add(currentPos);
        availablePaths.Add(initPath);
        availableSpaces.Add(currentPos);

        List<Vector2> adjacent = GridManager.Instance.AdjacentSpaces(currentPos);
        foreach (Vector2 space in adjacent)
        {
            if (GridManager.Instance.GetTileAtPosition(space).walkable)
            {
                List<Vector2> newPath = new List<Vector2>();
                explored.Add(space);
                newPath.Add(space);
                frontier.Add(newPath);
                availablePaths.Add(newPath);
                availableSpaces.Add(space);
            }
        }


        while (frontier.Count > 0)
        {
            List<Vector2> newSpaces = GridManager.Instance.AdjacentSpaces(frontier[0].Last());
            List<Vector2> oldPath = frontier[0];

            foreach (Vector2 space in newSpaces)
            {
                // Check to see if we've explored this tile already
                if (explored.Contains(space)) { }
                else {

                    // Add the tile to list of explored tiles
                    explored.Add(space);

                    // Check to make sure the tile can be entered
                    if (frontier[0].Count >= movement) { }
                    else if (!GridManager.Instance.GetTileAtPosition(space).walkable) { }
                    else
                    {
                        // The current path is valid
                        // Add it to availablePaths and the frontier
                        List<Vector2> newPath = new List<Vector2>();
                        newPath.AddRange(oldPath);
                        newPath.Add(space);
                        availablePaths.Add(newPath);
                        frontier.Add(newPath);
                    }
                }
            }
            frontier.Remove(oldPath);

        }

        // Add the ends of all paths to availableSpaces
        foreach(List<Vector2> path in availablePaths)
        {
            if (!(availableSpaces.Contains(path.Last())))
                availableSpaces.Add(path.Last());
        }

    }

    public void ShowHighlightMoveTiles(bool select = true)
    {
        foreach(Vector2 pos in availableSpaces)
        {
            Tile space = GridManager.Instance.GetTileAtPosition(pos);
            space.MoveHighlight(select);
        }
    }

    public void ShowHighlightAttackTiles(bool select = true)
    {
        if (attack == null)
            return;

        foreach (var target in attack.availableTargets)
        {
            target.OccupiedTile.AttackHighlight(select);
        }
    }

    public void OldMove(Tile space)
    {
        GridManager.Instance.ClearAllHighlights();

        // If we 'moved' to the same space, immediately return
        if (space == OccupiedTile) return;

        space.SetUnit(this);


        // Find the path the unit took and reduce their movement accordingly
        foreach (List<Vector2> path in availablePaths)
        {
            if (path.Last().Equals(space.pos))
            {
                movement -= path.Count;
                break;
            }
        }
        foreach (var hero in UnitManager.Instance.AllHeroes)
            hero.UpdateSelf();

    }

    public void Move(Tile space)
    {
        moving = true;
        StartCoroutine(coMove(space));

    }

    IEnumerator coMove(Tile space)
    {
        // With this active, only some of the enemies take their turns
        if (!UnitManager.Instance.UnitQueue.Contains(this) && UnitManager.Instance.ActingUnit != ID)
        {
            UnitManager.Instance.UnitQueue.Add(this);
            UnityEngine.Debug.Log("Added unit to queue");
        }
        yield return new WaitUntil(() => UnitManager.Instance.ActingUnit == ID);

        // With this active, all enemies take their turns simultaneously
        // WaitForTurn();

        GridManager.Instance.ClearAllHighlights();

        // If we 'moved' to the same space, immediately return
        if (space != OccupiedTile)
        {
            // Find the path the unit took and reduce their movement accordingly
            foreach (List<Vector2> path in availablePaths)
            {
                if (path.Last().Equals(space.pos))
                {
                    currentPath = path;
                    break;
                }
            }


            if (currentPath != null)
            {

                for (int i = 0; i < currentPath.Count; i++)
                {
                    GridManager.Instance.GetTileAtPosition(currentPath[i]).SetUnit(this);
                    movement--;
                    yield return new WaitForSeconds(0.15f);
                }
            }
        }        

        UnitManager.Instance.UpdatePlayerOptions();
        if (player)
        {
            UnitManager.Instance.DeactivateUnit();
        }
        UnityEngine.Debug.Log(UnitName + " finished moving");
        moving = false;
    }

    public IEnumerator WaitForTurn()
    {
        if (!UnitManager.Instance.UnitQueue.Contains(this) && UnitManager.Instance.ActingUnit != ID)
        {
            UnitManager.Instance.UnitQueue.Add(this);
            UnityEngine.Debug.Log("Added unit to queue");

        }
        yield return new WaitUntil(() => UnitManager.Instance.ActingUnit == ID);
    }

    public IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public void Attack(BaseUnit enemy)
    {
        if (attack != null)
            if (attack.availableTargets.Contains(enemy))
                attack.Attack(enemy);
    }

    public void UpdateSelf()
    {
        FindAvailableSpaces();
        if (attack != null)
        {
            attack.FindTargets();
        }
    }

    virtual public void StartOfTurn()
    {
        movement = SPEED;
        actionsRemaining = 1;
        UpdateSelf();
    }

    virtual public string GetInfo()
    {
        String info = UnitName + "\nHP: " + HP.ToString() + "/" + MAX_HP.ToString() + "\nSpeed: " + SPEED;
        
        return info;
    }

    override public string ToString()
    {
        return GetInfo();
    }


    // Getters

    public int GetHP()
    {
        return HP;
    }
    public int GetMovement()
    {
        return movement;
    }
    public int GetSpeed()
    {
        return SPEED;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
