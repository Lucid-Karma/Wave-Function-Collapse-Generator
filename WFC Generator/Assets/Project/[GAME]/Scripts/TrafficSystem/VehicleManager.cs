using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VehicleManager : Singleton<VehicleManager>
{
    [HideInInspector] public static UnityEvent OnVehiclesStopped = new();

    [HideInInspector]
    public int vehiclePriority;
    
    private int stoppedVehicleCount;
    public int StoppedVehicleCount { get => stoppedVehicleCount; 
        set  
        {
            stoppedVehicleCount = value;
            //if (stoppedVehicleCount >= 3) 
            //{
            //    CharacterBase.Instance.isDrawCompleted = true;
            //    OnVehiclesStopped.Invoke();
            //}    
        } 
    }

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
        print(vehiclePriority);
        vehiclePriority = 0;
    }
}
