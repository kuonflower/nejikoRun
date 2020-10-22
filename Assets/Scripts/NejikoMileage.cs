using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [System.Serializable]
public class NejikoMileage
{
    public string name;
    public int score;

    public NejikoMileage(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}
