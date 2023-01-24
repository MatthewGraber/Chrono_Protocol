using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    public Behaviour behaviour;
    public bool boss;

    bool attacking = false;

    private List<BaseUnit> availableTargets;

    override public void init()
    {
        base.init();
        unitType = UnitType.Enemy;
        availableTargets = new List<BaseUnit>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Turn()
    {
        acting = true;
        StartCoroutine(ieTurn());
    }

    public IEnumerator ieTurn()
    {
        /*if (!(UnitManager.Instance.UnitQueue.Contains(this)) && UnitManager.Instance.ActingUnit != this)
        {

        }*/
        while (UnitManager.Instance.ActingUnit != ID) yield return null;
        // WaitForTurn();

        // Flash the tile to indicate to the player that this unit is about to activate
        OccupiedTile.MoveHighlight(true);
        yield return new WaitForSeconds(0.5f);
        OccupiedTile.MoveHighlight(false);
        yield return new WaitForSeconds(0.5f);

        Debug.Log(UnitName + " continuing turn");

        FindAvailableSpaces();
        FindTargets();
        string targetList = "";
        foreach (var target in availableTargets)
        {
            targetList += "\n" + target.ToString();
        }
        Debug.Log("Available targets: " + targetList);

        switch (behaviour)
        {
            case Behaviour.Aggressive:
                StartCoroutine(Aggressive()); break;

            case Behaviour.Challenger:
                StartCoroutine(Challenger()); break;

            case Behaviour.BuildingEater:
                StartCoroutine(BuildingEater()); break;
            default: break;
        }

        // Wait until the unit has finished moving before ending the turn
        while (acting) yield return null;

        Debug.Log(UnitName + " ending turn");

        movement = SPEED;
        actionsRemaining = 1;
        UnitManager.Instance.EnemyTurnsFinished++;
        UnitManager.Instance.DeactivateUnit();
    }


    // Searches through all possible available spaces and finds available targets
    public void FindTargets()
    {
        // Clears the current list of available targets
        availableTargets.Clear();

        // Looks through all available spaces it can move to
        foreach (var space in availableSpaces)
        {
            // Checks to see what targets it can reach from that space
            attack.FindTargets(space);
            foreach (var target in attack.availableTargets)
            {
                // Adds each target to the enemy's list if it isn't already there
                if (!availableTargets.Contains(target))
                {
                    availableTargets.Add(target);
                }
            }
        }
    }


    /*
     * *******************
     * BEHAVIOUR FUNCTIONS
     * *******************
     */

    // Dumb behaviour
    // TODO: Describe dumb behaviour


    // Challenger behaviour
    // Finds the target with the highest HP and attacks it
    private IEnumerator Challenger()
    {
        List<BaseUnit> preferedTargets = new List<BaseUnit>();

        foreach (var target in availableTargets)
        {
            if (target.player)
                preferedTargets.Add(target);
        }

        if (preferedTargets.Count > 0)
        {
            preferedTargets = Prioritize<BaseUnit>(preferedTargets, Priorities.HighHP);
            StartCoroutine(MoveAndAttack(preferedTargets[0]));
            while (moving || attacking) yield return null;
        }

        // after attacking, move towards the center of the map
        FindAvailableSpaces();
        if (availableSpaces.Count > 0)
        {
            availablePaths = Prioritize<Vector2>(availablePaths, Priorities.CloseToCenter);
            availablePaths = Prioritize<Vector2>(availablePaths, Priorities.Close);
            Move(GridManager.Instance.GetTileAtPosition(availablePaths[0].Last()));
        }

        while (moving) yield return null;
        acting = false;
    }

    // Aggressive behaviour
    // Finds the target with the lowest HP and attacks it
    private IEnumerator Aggressive()
    {
        List<BaseUnit> preferedTargets = new List<BaseUnit>();

        foreach (var target in availableTargets)
        {
            if (target.player)
                preferedTargets.Add(target);
        }

        if (preferedTargets.Count > 0)
        {
            preferedTargets = Prioritize<BaseUnit>(preferedTargets, Priorities.LowHP);
            StartCoroutine(MoveAndAttack(preferedTargets[0]));
            while (moving || attacking) yield return null;
        }

        // after attacking, move towards the center of the map
        FindAvailableSpaces();
        if (availableSpaces.Count > 0)
        {
            availablePaths = Prioritize<Vector2>(availablePaths, Priorities.CloseToCenter);
            availablePaths = Prioritize<Vector2>(availablePaths, Priorities.Close);
            Move(GridManager.Instance.GetTileAtPosition(availablePaths[0].Last()));
        }

        while (moving || attacking) yield return null;
        acting = false;
    }


    // BuildingEater behaviour
    // BuildingEaters will EXCLUSIVELY target buildings
    private IEnumerator BuildingEater()
    {
        List<BaseUnit> preferedTargets = new List<BaseUnit>();

        foreach (BaseUnit target in availableTargets)
        {
            if (target.unitType == UnitType.Building)
            {
                preferedTargets.Add(target);
                Debug.Log("Added " + target.UnitName + " to preferedTargets");
            }
        }

        // If it could find any buildings in range, attack on of them
        if (preferedTargets.Count > 0)
        {
            preferedTargets = Prioritize<BaseUnit>(preferedTargets, Priorities.LowHP);
            // This makes us decide which unit to attack BEFORE we decide where the best place to go is
            // TODO: FIX THIS
            string targetList = "";
            foreach (var target in preferedTargets)
            {
                targetList += "\n" + target.ToString();
            }
            Debug.Log("Prefered targets after prioritizing: " + targetList);
            StartCoroutine(MoveAndAttack(preferedTargets[0]));
            while (moving || attacking) yield return null;
        }

        // after attacking, move towards the center of the map
        FindAvailableSpaces();
        if (availableSpaces.Count > 0)
        {
            availablePaths = Prioritize<Vector2>(availablePaths, Priorities.CloseToCenter);
            availablePaths = Prioritize<Vector2>(availablePaths, Priorities.Close);
            Move(GridManager.Instance.GetTileAtPosition(availablePaths[0].Last()));
        }

        while (moving) yield return null;
        acting = false;
    }



    // Attacks a target
    private IEnumerator MoveAndAttack(BaseUnit target)
    {
        List<List<Vector2>> possilbePaths = new List<List<Vector2>>();
        foreach (var path in availablePaths)
        {
            attack.FindTargets(path.Last());
            if (attack.availableTargets.Contains(target))
            {
                possilbePaths.Add(path);
            }
        }

        // WaitSeconds(1.0f);

        if (possilbePaths.Count > 0)
        {
            attacking = true;

            possilbePaths = Prioritize<Vector2>(possilbePaths, Priorities.CloseToCenter);
            possilbePaths = Prioritize<Vector2>(possilbePaths, Priorities.Close);
            Move(GridManager.Instance.GetTileAtPosition(possilbePaths[0].Last()));
            
            // Wait until we finish moving for the attack animation
            while (moving) yield return null;

            target.OccupiedTile.AttackHighlight(true);
            yield return new WaitForSeconds(0.5f);
            target.OccupiedTile.AttackHighlight(false);
            attack.Attack(target);
            yield return new WaitForSeconds(0.5f);
            attacking = false;
        }
        else
        {
            yield break;
        }
    }

    private List<T> Prioritize<T>(List<T> things, Priorities priority) where T : notnull
    {
        List<T> highPriority = new List<T>();
        List<int> scores = new List<int>();
        object t = typeof(T);

        for (int i = 0; i < things.Count; i++)
        {
            object thingi = things[i];

            switch (priority)
            {
                // Close to center
                // Uses a list of Vecor2's or a list of paths
                case Priorities.CloseToCenter:        
                    scores.Add(GridManager.Instance.DistanceFromCenter((Vector2) thingi));
                    break;

                // Low HP
                // Uses a list of units
                case Priorities.LowHP:
                    scores.Add(((BaseUnit) thingi).GetHP()); break;

                // Building
                // Uses a list of units
                case Priorities.Building:
                    if (((BaseUnit) thingi).unitType == UnitType.Building)
                        scores.Add(1);
                    else
                        scores.Add(0);
                    break;

                // Hero
                // Uses a list of units
                case Priorities.Hero:
                    if (((BaseUnit) thingi).unitType == UnitType.Hero)
                        scores.Add(1);
                    else
                        scores.Add(0);
                    break;

                default:
                    scores.Add(0);
                    break;
            }
        }


        // Now that we've scored everything, determine which things are best
        int best = scores[0];
        for (int i = 0; i < things.Count; i++)
        {
            // Use a priority switch to determine if high or low values are good
            switch (priority)
            {
                // If low value is good
                case Priorities.CloseToCenter:
                case Priorities.Close:
                case Priorities.LowHP:
                    if (scores[i] == best)
                    {
                        highPriority.Add(things[i]);
                    }
                    else if (scores[i] < best) {
                        highPriority.Clear();
                        best = scores[i];
                        highPriority.Add(things[i]);
                    }
                    break;

                // If high value is good
                default:
                    if (scores[i] == best)
                    {
                        highPriority.Add(things[i]);
                    }
                    else if (scores[i] > best)
                    {
                        highPriority.Clear();
                        best = scores[i];
                        highPriority.Add(things[i]);
                    }
                    break;
            }
        }

        return highPriority;
    }

    private List<List<T>> Prioritize<T>(List<List<T>> things, Priorities priority) where T : notnull
    {
        List<List<T>> highPriority = new List<List<T>>();
        List<int> scores = new List<int>();
        object t = typeof(T);

        for (int i = 0; i < things.Count; i++)
        {
            List<object> thingi = things[i].Cast<object>().ToList();

            switch (priority)
            {
                // Close to center
                // Uses a list of Vecor2's or a list of paths
                case Priorities.CloseToCenter:
                    List<Vector2> stuff = thingi.Cast<Vector2>().ToList();
                    scores.Add(GridManager.Instance.DistanceFromCenter(stuff.Last()));
                    break;

                // Close to the attacker
                // Uses a list of paths
                case Priorities.Close:
                    List<Vector2> coolstuff = thingi.Cast<Vector2>().ToList();
                    scores.Add(coolstuff.Count); break;

                default:
                    scores.Add(0);
                    break;
            }
        }

        // Now that we've scored everything, determine which things are best
        int best = scores[0];
        for (int i = 0; i < things.Count; i++)
        {
            // Use a priority switch to determine if high or low values are good
            switch (priority)
            {
                // If low value is good
                case Priorities.CloseToCenter:
                case Priorities.Close:
                case Priorities.LowHP:
                    if (scores[i] == best)
                    {
                        highPriority.Add(things[i]);
                    }
                    else if (scores[i] < best)
                    {
                        highPriority.Clear();
                        best = scores[i];
                        highPriority.Add(things[i]);
                    }
                    break;

                // If high value is good
                default:
                    if (scores[i] == best)
                    {
                        highPriority.Add(things[i]);
                    }
                    else if (scores[i] > best)
                    {
                        highPriority.Clear();
                        best = scores[i];
                        highPriority.Add(things[i]);
                    }
                    break;
            }
        }

        return highPriority;
    }

}

public enum Priorities
{
    CloseToCenter,
    Close,
    LowHP,
    HighHP,
    Building,
    Hero,
}

public enum Behaviour
{
    Dumb,
    Aggressive,
    Challenger,
    BuildingEater,
}