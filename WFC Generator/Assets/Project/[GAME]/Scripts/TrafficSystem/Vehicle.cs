using System.Collections.Generic;
using UnityEngine;

public interface IVehicleBehavior
{
    void Drive(Vehicle vehicle);
}

public class NormalDriving : IVehicleBehavior
{
    public void Drive(Vehicle vehicle)
    {
        Debug.Log("following");
        vehicle.FollowPath();
    }
}

public class StopForTrafficLight : IVehicleBehavior
{
    public void Drive(Vehicle vehicle)
    {
        if (vehicle.IsAtTrafficLight && !vehicle.CanPassTrafficLight)
            vehicle.Stop();
        else
            vehicle.FollowPath();
    }
}

public class Vehicle : MonoBehaviour
{
    private IVehicleBehavior _behavior;
    public Queue<Vector3> _path = new Queue<Vector3>();
    public float speed = 5f;

    public bool IsAtTrafficLight { get; private set; }
    public bool CanPassTrafficLight { get; private set; }
    public bool CanMove { get; set; }

    public void SetBehavior(IVehicleBehavior behavior) => _behavior = behavior;

    private void OnEnable()
    {
        CanMove = true;
    }
    private void Start()
    {
        IsAtTrafficLight = false;
        CanPassTrafficLight = true;
        //CanMove = false;

        SetBehavior(new NormalDriving());
    }
    public void SetPath(List<Transform> waypoints)
    {
        _path.Clear();
        for (int i = 0; i < waypoints.Count; i++)
        {
            _path.Enqueue(waypoints[i].position);
        }
    }

    public void FollowPath()
    {
        if (_path == null) return;
        if (_path.Count > 0)
        {
            // Move toward next waypoint
            Vector3 nextWaypoint = _path.Peek();
            transform.position = Vector3.MoveTowards(transform.position, nextWaypoint, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, nextWaypoint) < 0.1f) _path.Dequeue();
        }
        else
        {
            Destroy(gameObject); // Path complete
            print(name + " path complete");
        }
    }

    public void Stop()
    {
        // Logic to stop the vehicle
    }

    private void Update()
    {
        if (!CanMove) return;
        _behavior?.Drive(this);
    }
}
