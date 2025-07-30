using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleObject: MonoBehaviour, IModuleObject
{
    [HideInInspector] public bool isDowntown;
    public bool isChecked = false;
    public int Row;
    public int Column;

    public int north;
    public int south;
    public int east;
    public int west;

    int _north, _south, _east, _west;

    GameObject _cityPart;

    public bool isVehicleSpawnPoint;
    public bool isStraightRoad;
    public bool isRoad;

    [SerializeField] private GameObject smokeParticlePrefab;

    void OnEnable()
    {
        if (gameObject.transform.childCount > 0)
           _cityPart = gameObject.transform.GetChild(0).gameObject;
        ActivateCity();
        SpawnCar();

        CharacterBase.OnGridCollapse.AddListener(() => isChecked = false);
        CharacterBase.OnModulesRotate.AddListener(DeactivateCity);
        //VehicleManager.OnVehiclesStopped.AddListener(DeactivateCity);
        EventManager.OnLevelFinish.AddListener(ActivateCity);
    }
    void OnDisable()
    {
        CharacterBase.OnGridCollapse.RemoveListener(() => isChecked = false);
        CharacterBase.OnModulesRotate.RemoveListener(DeactivateCity);
        //VehicleManager.OnVehiclesStopped.RemoveListener(DeactivateCity);
        EventManager.OnLevelFinish.RemoveListener(ActivateCity);

        isChecked = false;

        // ModuleSO already updates these variables on OnEnable..!!!
        north = _north;
        south = _south;
        east = _east;
        west = _west;

        Row = 0;
        Column = 0;

        gameObject.isStatic = false;
    }

    public void UpdateMO_Angle(Transform moduleTransform)
    {
        _north = north;
        _south = south;
        _east = east;
        _west = west;

        north = _west;
        south = _east;
        east = _north;
        west = _south;
    }

    private void ActivateCity()
    {
        if (!isDowntown) return;
        if (_cityPart != null)
        {
            ShowCity();
            //ShowVehicle();
        }
    }
    private void DeactivateCity()
    {
        if (!isDowntown) return;
        if (_cityPart != null)
        {
            HideCity();
            //HideVehicle();
        }
    }

    #region RotateProcess
    //public void RotateModuleForDrawn(int randomIndex, int randomRotation)
    //{
    //    transform.DORotate(new Vector3(0f, 0f, (float)randomRotation), 1f, RotateMode.LocalAxisAdd)
    //            .SetEase(Ease.OutQuad);
    //    for (int i = 0; i < randomIndex + 1; i++)
    //    {
    //        UpdateMO_Angle(transform);
    //    }
    //}
    public void RotateModule()
    {
        CharacterBase.Instance.isRotating = true;
        ControlVehicle(false);

        //GameObject particle = Instantiate(smokeParticlePrefab, transform.position + new Vector3(0, -1f, 0), Quaternion.identity);
        //Destroy(particle, 1.5f);

        //transform.DOMove(new Vector3(transform.position.x, 1, transform.position.z), 0.2f).SetEase(Ease.Unset/*OutBounce*/)
        //    .OnComplete(() =>
        //    {
        //        transform.DOMove(new Vector3(transform.position.x, 0, transform.position.z), 0.2f).SetEase(Ease.InBack);
        //    });
        transform.DORotate(new Vector3(0f, 0f, 90f), 0.4f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Unset)
                .OnComplete(() =>
                {
                    CharacterBase.Instance.isRotating = false; UpdateMO_Angle(transform);
                    EventManager.OnClick.Invoke();
                    ControlVehicle(true);
                });

        
        //modulePrefab.Rotate(Vector3.up, 90f);
    }
    #endregion

    #region Traffic
    [SerializeField] private Vector3 spawnPoint;
    [SerializeField] private Quaternion spawnRotation;
    public void SpawnCar()
    {
        if (isVehicleSpawnPoint)
        {
            Vector3 spawnPosition = gameObject.transform.position + spawnPoint;
            GameObject obj = Instantiate(GameManager.Instance.carPrefabs[Random.Range(0, GameManager.Instance.carPrefabs.Length)], spawnPosition, spawnRotation);
            obj.transform.parent = transform;
            obj.SetActive(true);
        }
    }
    public void ControlVehicle(bool vehiclePresence)
    {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.CompareTag("Car"))
            {
                Vehicle vehicle = child.GetComponent<Vehicle>();
                vehicle.CanMove = vehiclePresence;
                //Debug.Log(" the vehicle has stopped by " + name + " " + child.position);
                return;
            }
        }
    }
    //private Queue<Vehicle> _childVehicles = new Queue<Vehicle>();
    private List<Vehicle> _childVehicles = new();
    public void EnqueueVehicle(Vehicle vehicle)
    {
        //_childVehicles.Enqueue(vehicle);
        _childVehicles.Add(vehicle);
    }
    public void DequeueVehicle(Vehicle vehicle)
    {
        //if( _childVehicles.Count > 0 )
        //    _childVehicles.Dequeue();

        if (_childVehicles.Count > 0)
            _childVehicles.Remove(vehicle);
    }
    public bool IsPriorVehicle(Vehicle vehicle)
    {
        //return vehicle == _childVehicles.Peek();
        return vehicle == _childVehicles.First();
    }
    public bool IsCityActive()
    {
        return _cityPart.activeSelf;
    }
    #endregion

    #region Animation
    public void ShowCity()
    {
        _cityPart.SetActive(true);
        _cityPart.transform.localScale = Vector3.zero;
        _cityPart.transform.DOScale(new Vector3(0.0066f, 0.0066f, 0.0066f), 1f).SetEase(Ease.OutBounce).OnComplete(() => ActivateVehicle());
    }

    public void HideCity()
    {
        //DeactivateVehicle();
        _cityPart.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).OnComplete(() => {
            _cityPart.SetActive(false);
            DeactivateVehicle();
        });
    }

    private void DeactivateVehicle()
    {
        foreach (Vehicle vehicle in _childVehicles)
        {
            if(vehicle != null)
            {
                vehicle.Stop();
                vehicle.gameObject.SetActive(false);
            }
                //vehicle.gameObject.SetActive(false);
        }
    }
    private void ActivateVehicle()
    {
        foreach (Vehicle vehicle in _childVehicles)
        {
            if (vehicle != null)
            {
                vehicle.gameObject.SetActive(true);
                vehicle.CanMove = true;
            }
                //vehicle.gameObject.SetActive(true);
        }
    }
    #endregion
}
