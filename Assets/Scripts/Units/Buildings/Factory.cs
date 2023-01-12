using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : BaseBuilding
{
    override public void StartOfTurn()
    {
        base.StartOfTurn();
        foreach (var building in UnitManager.Instance.AllBuildings)
        {
            building.Heal(1);
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
