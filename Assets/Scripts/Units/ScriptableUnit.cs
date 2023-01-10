using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]

public class ScriptableUnit : ScriptableObject
{
    public UnitType unitType;
    public BaseUnit UnitPrefab;
}

public enum UnitType
{
    Hero,
    Enemy,
    Building
}
