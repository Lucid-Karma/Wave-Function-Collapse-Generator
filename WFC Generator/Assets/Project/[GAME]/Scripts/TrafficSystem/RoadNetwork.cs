using System.Collections.Generic;
using UnityEngine;

public class RoadNetwork : Singleton<RoadNetwork>
{
    [HideInInspector]
    public List<Transform> waypoints = new List<Transform>();

    private void OnEnable()
    {
        WfcGenerator.OnMapReady.AddListener(SetWaypoints);
    }
    private void OnDisable()
    {
        WfcGenerator.OnMapReady.RemoveListener(SetWaypoints);
    }

    public void SetWaypoints()
    {
        foreach (var waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            waypoints.Add(waypoint.transform);
        }
    }

    //public List<Node> nodes = new List<Node>();
    //public List<ModuleObject> modules = new List<ModuleObject>();

    //void Start()
    //{
    //    // Example: Create a simple circular road network
    //    for (int i = 0; i < 5; i++)
    //    {
    //        Vector3 position = new Vector3(i * 5, 0, 0); // Example positions
    //        nodes.Add(new Node(position));
    //    }

    //    // Connect nodes in a loop
    //    for (int i = 0; i < nodes.Count; i++)
    //    {
    //        Node currentNode = nodes[i];
    //        Node nextNode = nodes[(i + 1) % nodes.Count]; // Loop back to the start
    //        currentNode.neighbours.Add(nextNode);
    //        nextNode.neighbours.Add(currentNode); // Two-way connection
    //    }
    //}
}
