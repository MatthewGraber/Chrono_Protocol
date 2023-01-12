using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital : BaseBuilding
{
    // Heals all heroes 1 HP at the start of its turn
    override public void StartOfTurn()
    {
        base.StartOfTurn();
        foreach (var hero in UnitManager.Instance.AllHeroes)
        {
            hero.Heal(1);
        }
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
