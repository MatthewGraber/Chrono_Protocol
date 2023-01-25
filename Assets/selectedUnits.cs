using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectedUnits : MonoBehaviour
{
    public List<BaseHero> SpawnMe;

    public void importUnits(BaseHero x)
    {
        SpawnMe.Add(x);
    }

}
