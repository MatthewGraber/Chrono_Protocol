using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillMonkey : BaseEnemy
{
    // Start is called before the first frame update
    override public void init()
    {
        base.init();
        attack.init(this, 1, false, 4);
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
