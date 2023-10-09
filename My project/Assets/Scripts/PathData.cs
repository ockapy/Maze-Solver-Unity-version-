using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class PathData
{
    public GameObject node;
    public int score;

    [JsonConstructor]
    public PathData(GameObject node, int score)
    {
        this.node = node;
        this.score = score;
    }
    
}
