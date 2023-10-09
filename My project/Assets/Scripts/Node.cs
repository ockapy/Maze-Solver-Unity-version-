using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;



enum  State
{
    Zero,
    One
};

public class Node : MonoBehaviour
{
    public int id;
    public int state;
    // Start is called before the first frame update
    void Start()
    {
       this.state = (int)State.Zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetID(int id)
    {
        this.id = id;
    }
}
