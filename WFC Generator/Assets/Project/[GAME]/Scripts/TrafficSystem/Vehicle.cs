using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Waypoint;

public interface IVehicleBehavior
{
    void Drive(Vehicle vehicle);
}

public class NormalDriving : IVehicleBehavior
{
    public void Drive(Vehicle vehicle)
    {
        vehicle.MoveForward();
        vehicle.AdjustSteering();
        vehicle.FindIntersectionWaypoint();
        vehicle.CheckOtherVehicles();
    }
}

public class IntersectionDriving : IVehicleBehavior
{
    public void Drive(Vehicle vehicle)
    {
        vehicle.MoveTowardsWaypoint();
        vehicle.CheckOtherVehicles();
    }
}

public class CarefulDriving : IVehicleBehavior
{
    public void Drive(Vehicle vehicle)
    {
        vehicle.CarefulDrive();
    }
}

public class StopForTrafficLight : IVehicleBehavior
{
    public void Drive(Vehicle vehicle)
    {
        if (vehicle.IsAtTrafficLight && !vehicle.CanPassTrafficLight)
            vehicle.Stop();
        //else
            //vehicle.FollowPath();
    }
}

public class StopForDraw : IVehicleBehavior
{
    public void Drive(Vehicle vehicle) { }
}

public class Vehicle : MonoBehaviour
{
    private IVehicleBehavior _behavior;
    public Queue<Vector3> _path = new Queue<Vector3>();

    public bool IsAtTrafficLight { get; private set; }
    public bool CanPassTrafficLight { get; private set; }
    public bool CanMove { get; set; }

    public void SetBehavior(IVehicleBehavior behavior) => _behavior = behavior;

    private void Start()
    {
        IsAtTrafficLight = false;
        CanPassTrafficLight = true;

        SetBehavior(new NormalDriving());
    }

    public void Stop()
    {
        // Logic to stop the vehicle
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;
        _behavior?.Drive(this);
    }




    private float waypointThreshold = 0.3f;

    private Transform targetWaypoint;
    private Transform lastWaypoint;

    public void FindIntersectionWaypoint()
    {
        if (Physics.Raycast(transform.position + Vector3.up * rayHeightOffset, transform.forward, out RaycastHit waypointHit, 0.5f, WaypointLayer))
        {
            lastWaypoint = waypointHit.transform;

            if (lastWaypoint.GetComponent<Waypoint>().waypointType != WaypointType.Intersection) return;

            List<Transform> possibleWaypoints = new List<Transform>();

            possibleWaypoints = lastWaypoint.GetComponent<Waypoint>().connectedWaypoints;
            //Debug.Log(lastWaypoint.name + $" Found {possibleWaypoints.Count} possible waypoints.");

            if (possibleWaypoints.Count > 0)
            {
                //for (int i = 0; i < possibleWaypoints.Count; i++)
                //{
                //    Debug.Log($"No {i} is: " + possibleWaypoints[i].name);
                //}

                Transform newWaypoint;
                newWaypoint = possibleWaypoints[Random.Range(0, possibleWaypoints.Count)];

                targetWaypoint = newWaypoint;
                transform.rotation = Quaternion.Euler(0, GetClosestRotation(transform.eulerAngles.y), 0);
                SetBehavior(new IntersectionDriving());
                //Debug.Log("Yeni waypoint se�ildi: " + targetWaypoint.name);
            }
            else
            {
                Debug.LogWarning("Uygun waypoint bulunamad�.");
            }
        }
    }
    private readonly float[] possibleAngles = { -90f, 0f, 90f, 180f };
    private float GetClosestRotation(float currentRotation)
    {
        return possibleAngles.OrderBy(angle => Mathf.Abs(Mathf.DeltaAngle(currentRotation, angle))).First();
    }
    public void MoveTowardsWaypoint()
    {
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        float currentAngle = transform.eulerAngles.y;

        //if (Vector3.Distance(transform.position, targetWaypoint.position) > 1.7f)
        //{
        //    float ngleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
        //    float otationStep = 100f * Time.fixedDeltaTime;
        //    float otationAmount = Mathf.Clamp(ngleDifference, -otationStep, otationStep);
        //    if (Mathf.Abs(ngleDifference) > 1f) // K���k farklar i�in d�nmeyi engelle
        //    {
        //        transform.Rotate(0, otationAmount, 0);
        //    }
        //}

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 1.5f)
        {
            float nearestAngle = GetClosestRotation(targetAngle);
            float angleDifference = Mathf.DeltaAngle(currentAngle, nearestAngle);

            float rotationStep = 80f * Time.fixedDeltaTime;
            float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
            transform.Rotate(0, rotationAmount, 0);
            if (Vector3.Distance(transform.position, targetWaypoint.position) < waypointThreshold)
            {
                SetBehavior(new NormalDriving());
                //Debug.Log("intersectiondan ��k�l�yor");
            }

        }
        MoveForward();
    }




    public LayerMask trackLayer;
    public float speed = 5f;
    public float rayDistance = 0.5f;
    public float steeringSensitivity = 2.5f;
    public float rayHeightOffset = 0.2f; // Raises the raycast to detect guardrails
    public float frontOffset = 2.5f; // Arac�n �n�ne do�ru kayd�rma mesafesi

    private float targetDistance;

    public LayerMask carLayer; // Arabalar� alg�lamak i�in katman
    public LayerMask trafficLightLayer; // Trafik ���klar� i�in katman
    public LayerMask WaypointLayer;

    void OnEnable()
    {
        CanMove = true;
        IdentifyGuardrailDistance();

        rayDirections = new Vector3[3];
        hits = new RaycastHit[3];
        //WfcGenerator.OnMapReady.AddListener(WaitForPlay);
        EventManager.OnLevelFinish.AddListener(MoveAfterWin);
    }
    private void OnDisable()
    {
        //WfcGenerator.OnMapReady.RemoveListener(WaitForPlay);
        EventManager.OnLevelFinish.RemoveListener(MoveAfterWin);
    }

    Vector3 rayOrigin;
    void IdentifyGuardrailDistance()
    {
        rayOrigin = transform.position + transform.forward * frontOffset + Vector3.up * rayHeightOffset;
        RaycastHit hitRight;

        if (Physics.Raycast(rayOrigin, transform.right, out hitRight, rayDistance, trackLayer))
            targetDistance = hitRight.distance;
    }

    public void MoveForward()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }
    RaycastHit hit;
    public void AdjustSteering()
    {
        rayOrigin = transform.position + transform.forward * frontOffset + Vector3.up * rayHeightOffset;

        if (Physics.Raycast(rayOrigin, transform.right, out hit, rayDistance, trackLayer))
        {
            float correction = targetDistance - hit.distance;

            // K���k d�zeltmeleri s�f�rla
            if (Mathf.Abs(correction) < 0.07f)
            {
                correction = 0f;
            }

            // D�zeltme de�erini s�n�rla
            correction = Mathf.Clamp(correction, -1f, 1f);
            transform.Rotate(0, -correction * steeringSensitivity, 0);
            noTrackDetectedTime = 0;
        }
        else
            DestroyVehicle();
    }

    private IVehicleBehavior _previousbehavior;
    #region CarefulDriving
    private Vector3[] rayDirections;
    private RaycastHit[] hits;
    public void CheckOtherVehicles()
    {
        UpdateSensors();
        // Analyze sensor hits
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                _previousbehavior = _behavior;
                SetBehavior(new CarefulDriving());
            }
        }
        //if (Physics.Raycast(transform.position + Vector3.up * rayHeightOffset, transform.forward, out RaycastHit carHit, 0.5f, carLayer))
        //{
        //    _previousbehavior = _behavior;
        //    rayDirections = new Vector3[3];
        //    hits = new RaycastHit[3];
        //    SetBehavior(new CarefulDriving());
        //}
    }
    public void CarefulDrive()
    {
        UpdateSensors();
        // Analyze sensor hits
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null) { }
            else SetBehavior(_previousbehavior);
        }
        //if (Physics.Raycast(transform.position + Vector3.up * rayHeightOffset, transform.forward, out RaycastHit carHit, 0.5f, carLayer))
        //{

        //}
        //else
        //    SetBehavior(_previousbehavior);
    }
    void UpdateSensors()
    {
        // Main forward ray
        rayDirections[0] = transform.forward;

        // Angled rays for corner detection
        rayDirections[1] = (transform.forward + transform.right * 0.5f).normalized;
        rayDirections[2] = (transform.forward - transform.right * 0.5f).normalized;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Physics.Raycast(transform.position + Vector3.up * rayHeightOffset, rayDirections[i], out hits[i], 0.5f, carLayer);
        }
    }

    #endregion

    private void WaitForPlay()
    {
        transform.SetParent(hit.transform.parent);
        _previousbehavior = _behavior;
        SetBehavior(new StopForDraw());
    }
    private void MoveAfterWin()
    {
        transform.SetParent(null);
        SetBehavior(_previousbehavior);
    }

    private float noTrackDetectedTime = 0f;
    private float disappearTime = 1f;
    private void DestroyVehicle()
    {
        noTrackDetectedTime += Time.fixedDeltaTime; 

        if (noTrackDetectedTime >= disappearTime)
        {
            Destroy(gameObject);
            print("whaaa");
        }
    }

    private void OnBecameVisible()
    {
        gameObject.SetActive(true);
        //Debug.Log("boringgg");
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
        //Debug.Log("You CAN'T see me, my time is NOW..!");
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + Vector3.up * rayHeightOffset, rayOrigin + transform.forward * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayOrigin, rayOrigin + transform.right * rayDistance);
    }
}
