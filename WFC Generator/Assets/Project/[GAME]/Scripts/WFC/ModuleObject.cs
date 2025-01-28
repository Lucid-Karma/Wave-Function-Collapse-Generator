using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleObject: MonoBehaviour, IModuleObject
{
    public bool isChecked = false;
    public int Row;
    public int Column;

    public int north;
    public int south;
    public int east;
    public int west;

    int _north, _south, _east, _west;

    GameObject _cityPart;

    public bool isRotatable;
    public bool isRoad;

    [SerializeField] private GameObject smokeParticlePrefab;


    void OnEnable()
    {
        if (gameObject.transform.childCount > 0)
           _cityPart = gameObject.transform.GetChild(0).gameObject;
        ActivateCity();

        CharacterBase.OnGridCollapse.AddListener(() => isChecked = false);
        CharacterBase.OnModulesRotate.AddListener(DeactivateCity);
        EventManager.OnLevelFinish.AddListener(ActivateCity);
        WfcGenerator.OnMapReady.AddListener(SpawnCar);
    }
    void OnDisable()
    {
        CharacterBase.OnGridCollapse.RemoveListener(() => isChecked = false);
        CharacterBase.OnModulesRotate.RemoveListener(DeactivateCity);
        EventManager.OnLevelFinish.RemoveListener(ActivateCity);
        WfcGenerator.OnMapReady.RemoveListener(SpawnCar);

        isChecked = false;

        // ModuleSO already updates these variables on OnEnable..!!!
        north = _north;
        south = _south;
        east = _east;
        west = _west;

        Row = 0;
        Column = 0;
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
        if (_cityPart != null)
        {
            ShowCity();
        }
    }
    private void DeactivateCity()
    {
        if (_cityPart != null)
        {
            HideCity();
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

        //GameObject particle = Instantiate(smokeParticlePrefab, transform.position + new Vector3(0, -1f, 0), Quaternion.identity);
        //Destroy(particle, 1.5f);

        transform.DOMove(new Vector3(transform.position.x, 1, transform.position.z), 0.2f).SetEase(Ease.Unset/*OutBounce*/)
            .OnComplete(() =>
            {
                transform.DOMove(new Vector3(transform.position.x, 0, transform.position.z), 0.2f).SetEase(Ease.InBack);
            });
        transform.DORotate(new Vector3(0f, 0f, 90f), 0.4f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Unset)
                .OnComplete(() =>
                {
                    CharacterBase.Instance.isRotating = false; UpdateMO_Angle(transform);
                    EventManager.OnClick.Invoke();
                });

        
        //modulePrefab.Rotate(Vector3.up, 90f);
    }
    #endregion
    #region Traffic
    [SerializeField] private Transform[] back;
    [SerializeField] private Transform[] forward;
    
    [HideInInspector] public List<Transform> Waypoints = new();
    public void SpawnCar()
    {
        if(isRotatable)
        {
            var direction = (Random.Range(0, 2) == 1)? back: forward;
            var drawResult = direction[0];//direction[Random.Range(0, 2)];
            GameObject obj = Instantiate(GameManager.Instance.carPrefabs[Random.Range(0, GameManager.Instance.carPrefabs.Length)], drawResult.position, Quaternion.identity);
            Vehicle vehicle = obj.GetComponent<Vehicle>();
            if(vehicle != null)
            {
                var list = WfcGenerator.Instance.destinationsList.Find(x => x.Contains(this));
                for (int i = 0; i < list.Count; i++)
                {
                    vehicle.SetPath(list[i].Waypoints);
                }
                
            }
            obj.SetActive(true);
            //vehicle._path.Dequeue();
            vehicle.CanMove = true;
            print(obj.transform.position);
            // call car pool to get one.
        }
    }
    #endregion
    #region Animation
    public void ShowCity()
    {
        _cityPart.SetActive(true);
        _cityPart.transform.localScale = Vector3.zero;
        _cityPart.transform.DOScale(new Vector3(0.0066f, 0.0066f, 0.0066f), 1f).SetEase(Ease.OutBounce);
    }

    public void HideCity()
    {
        _cityPart.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).OnComplete(() => _cityPart.SetActive(false));
    }
    #endregion
}
