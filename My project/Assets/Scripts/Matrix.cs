using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
public class Matrix : MonoBehaviour
{ 
    public int width;
    public int height;
    public int depth;
    public GameObject entry;
    public GameObject exit;
    public List<GameObject> nodes;

    void Start()
    {
        Debug.Log("Start");
        int idIncrement = 1;
        for (int z = 1; z <= depth; z++)
        {
            for (int y = 1; y <= height; y++)
            {
                for (int x = 1; x <= width; x++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "Node " + idIncrement;
                    cube.AddComponent<Node>();
                    cube.GetComponent<MeshRenderer>().material.color = Color.white;
                    cube.GetComponent<Node>().SetID(idIncrement);
                    cube.SetActive(true);
                    cube.transform.localPosition = new Vector3(x, y, z);
                    cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    nodes.Add(cube);
                    idIncrement++;
                }
            }
        }
        AttachToParent(this.gameObject);
        nodes.Last().name = "Exit";
        
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        sphere.transform.localPosition = Vector3.zero;
        sphere.AddComponent<Searcher>();
        sphere.GetComponent<Searcher>().currentNode = nodes[0];
    }
    void Update()
    {

    }

    public Matrix(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;

        nodes = new List<GameObject>(width * height * depth);


        entry = nodes.First();
        exit = nodes.Last();

    }

    public void AttachToParent(GameObject parent)
    {
        foreach(GameObject node in nodes)
        {
           node.transform.parent = parent.transform;
        }
    }
}
