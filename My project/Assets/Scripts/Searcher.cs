using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Searcher : MonoBehaviour
{
    public GameObject currentNode;
    private List<GameObject> visited = new();
    public Dictionary<GameObject,int> internalData = new();
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<MeshRenderer>().material.color = Color.blue;

    }

    // Update is called once per frame
    void Update()
    {

            if (transform.position == currentNode.transform.position)
            {
                if(currentNode.name == "Exit")
                {
                    return;
                }
                Search();
            }
           
            Move(currentNode);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, 1.5f);

    }

    private Dictionary<GameObject, int> RetrieveData()
    {
        Dictionary<GameObject, int> data = new Dictionary<GameObject, int>();

        for (int i = 0; i < 1000; i++)
        {
            try
            {
                var streamreader = new StreamReader($"Assets/Data/data{i}.json");

                try
                {
                    string json = File.ReadAllText($"Assets/Data/data{i}.json");

                    List<PathData> rawData = JsonConvert.DeserializeObject<List<PathData>>(json);

                    foreach (PathData path in rawData)
                    {
                        data.Add(path.node, path.score);
                    }
                }
                catch (Exception exception)
                {

                    streamreader.Close();
                }

                streamreader.Close();

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Aucun fichier de données trouvé.");
                break;
            }
            
        }
        return data;
    }
    private void Search()
    {
        RetrieveData();
        Debug.Log(Application.dataPath);
        Collider[] adjacentNodes = GetAdjacentNodes();
        currentNode.GetComponent<MeshRenderer>().material.color = Color.green;
        GameObject selected = SelectCanditate(adjacentNodes);
        visited.Add(selected.gameObject);
        currentNode = selected;
        if (currentNode.name == "Exit")
        {
            GetPath();
        }
    }

    private GameObject SelectCanditate(Collider[] adjacentNodes)
    {
        List<GameObject> candidates = new List<GameObject>();
        foreach (Collider c in adjacentNodes)
        {
            if (c.gameObject.GetComponent<MeshRenderer>().material.color != Color.green)
            {
                candidates.Add(c.gameObject);
                if(c.gameObject.name == "Exit")
                {
                    GameObject exit = c.gameObject;
                    exit.GetComponent<MeshRenderer>().material.color = Color.blue;
                    return exit;
                }
            }
        }
        if(candidates.Count == 0)
        {
            Override();
        }
        
        System.Random rand = new System.Random();
        GameObject selected =  candidates[rand.Next(candidates.Count - 1)];
        selected.GetComponent<MeshRenderer>().material.color = Color.blue;
        return selected;
    }

    private void Override()
    {
        for (float i = 2f; i <= 10f; i++)
        {
            Gizmos.DrawWireSphere(transform.position, i);
            Collider[] scanResults = GetAdjacentNodes(i);
            foreach(Collider node in scanResults)
            {
                if(node.GetComponent<MeshRenderer>().material.color == Color.red)
                {
                    currentNode = node.gameObject;
                    return;
                }
            }
        }
    }

    private Collider[] GetAdjacentNodes(float i)
    {
        Collider[] hitboxes = Physics.OverlapSphere(currentNode.transform.position, i);
        ChangeColor(hitboxes, Color.red);
        return hitboxes;
    }

    private Collider[] GetAdjacentNodes()
    {
        Collider[] hitboxes = Physics.OverlapSphere(currentNode.transform.position, 1.5f);
        ChangeColor(hitboxes,Color.red);
        return hitboxes;
    }

    private void ChangeColor(Collider[] hitboxes, Color color)
    {
        foreach (Collider hitbox in hitboxes)
        {
            if(hitbox.gameObject.GetComponent<MeshRenderer>().material.color == Color.white)
            {
                hitbox.gameObject.GetComponent<MeshRenderer>().material.color = color;
            }
        }
    }

    public void Move(GameObject node)
    {
        transform.position = Vector3.MoveTowards(transform.position, node.transform.position,10 * Time.deltaTime);
    }

    public void GetPath()
    {
        List<PathData> paths = new List<PathData>();
        List<PathData> mappedNodes = new List<PathData>();

        float x, y, z;
        x = visited[0].transform.position.x;
        y = visited[0].transform.position.y;
        z = visited[0].transform.position.z;

        foreach (GameObject node in visited)
        {
            int score = 0;
            float X, Y, Z;
            X = node.transform.position.x;
            Y = node.transform.position.y;
            Z = node.transform.position.z;
            
            if(X==Y && Y == Z)
            {
                score = 3;
                x = X; y = Y; z = Z;
            }
            else if((X == x+1 && Y==y+1) || (X==x+1 && Z==z+1) || (Y==y+1 && Z == z + 1))
            {
                score = 2;
            }else if(X==x+1 || Y==y+1 || Z == z + 1)
            {
                score = 1;
            }
            
            paths.Add(new(node,score));

            if (!internalData.ContainsKey(node))
            {
                mappedNodes.Add(new(node, score));
            }
            
        }
        try
        {
            string json = JsonConvert.SerializeObject(paths, Formatting.Indented);
            File.WriteAllText("Asset/Data/path.json", json);

            int fileName = 0;

            for (int i = 0; i < 1000; i++)
            {
                if (!File.Exists($"Assets/Data/data{i}.json")) { fileName = i; break; }
            }

            string data = JsonConvert.SerializeObject(mappedNodes, Formatting.Indented);
            using (var stream = new FileStream($"Assets/Data/data{fileName}.json", FileMode.Append, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(data);
                writer.Close();
            }



        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        };

    }
}
