using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BaseAttack : MonoBehaviour
{
    // Attack stats
    protected int range;
    protected bool lob;
    protected int damage;

    // Attacking unit
    protected BaseUnit attacker;

    public List<Vector2> cardinalDirections;
    public List<BaseUnit> availableTargets;

    public void FindTargets()
    {
        availableTargets.Clear();

        if (attacker.actionsRemaining > 0)
        {
            foreach (var direction in cardinalDirections)
            {
                for (int i = 1; i <= range; i++)
                {
                    var pos = attacker.OccupiedTile.pos + direction * i;
                    var tile = GridManager.Instance.GetTileAtPosition(pos);
                    if (tile != null)
                    {
                        if (tile.OccupiedUnit != null)
                        {
                            availableTargets.Add(tile.OccupiedUnit);
                        }
                        if (tile.blocked && !lob)
                            break;
                    }
                }
            }
        }
    }

    public void FindTargets(Vector2 space)
    {
        availableTargets.Clear();

        if (attacker.actionsRemaining > 0)
        {
            foreach (var direction in cardinalDirections)
            {
                for (int i = 1; i <= range; i++)
                {
                    var pos = space + direction * i;
                    var tile = GridManager.Instance.GetTileAtPosition(pos);
                    if (tile != null)
                    {
                        if (tile.OccupiedUnit != null)
                        {
                            availableTargets.Add(tile.OccupiedUnit);
                        }
                        if (tile.blocked && !lob)
                            break;
                    }
                }
            }
        }
    }

    public virtual void Attack(BaseUnit target)
    {
        attacker.actionsRemaining -= 1;
        target.TakeDamage(damage);
        GridManager.Instance.ClearAllHighlights();
        FindTargets();
    }

    public void init(BaseUnit a, int r, bool l, int d)
    {
        attacker = a;
        range = r;
        lob = l;
        damage = d;

        cardinalDirections = new List<Vector2>();
        cardinalDirections.Add(new Vector2(0, 1));
        cardinalDirections.Add(new Vector2(0, -1));
        cardinalDirections.Add(new Vector2(1, 0));
        cardinalDirections.Add(new Vector2(-1, 0));

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
}
