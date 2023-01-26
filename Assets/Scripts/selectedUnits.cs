using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectedUnits : MonoBehaviour
{
    public List<BaseHero> SpawnMe;

    public static selectedUnits Instance;

    public void importUnits(BaseHero x)
    {
        SpawnMe.Add(x);
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);
            Debug.Log("New instance of selectedUnits");
        }
        else if (Instance != this)
        {
            //Destroy(this);
            Debug.Log("Old instance of selectedUnits");
        }
    }

}
