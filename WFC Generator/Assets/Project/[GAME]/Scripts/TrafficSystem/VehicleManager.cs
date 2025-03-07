using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : Singleton<VehicleManager>
{
    [HideInInspector]
    public int vehiclePriority;

    private void OnEnable()
    {
        WfcGenerator.OnMapReady.AddListener(MakeDefaultPriority);
    }
    private void OnDisable()
    {
        WfcGenerator.OnMapReady.RemoveListener(MakeDefaultPriority);
    }

    private void MakeDefaultPriority()
    {
        vehiclePriority = 0;
    }
}
