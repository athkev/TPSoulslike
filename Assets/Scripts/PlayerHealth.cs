using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    private static PlayerHealth instance;
    public static PlayerHealth Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
    }
}
