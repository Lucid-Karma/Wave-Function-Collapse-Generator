using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public enum WaypointType { Regular, Intersection }

    public WaypointType waypointType;
    public List<Transform> connectedWaypoints;
}
