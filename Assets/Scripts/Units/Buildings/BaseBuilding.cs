using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuilding : BaseUnit
{

    override public void init()
    {
        SPEED = 0;
        base.init();
        unitType = UnitType.Building;
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
