using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero1 : BaseHero
{
    // Start is called before the first frame update
    override public void init()
    {
        base.init();
        attack.init(this, 3, false, 3);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
