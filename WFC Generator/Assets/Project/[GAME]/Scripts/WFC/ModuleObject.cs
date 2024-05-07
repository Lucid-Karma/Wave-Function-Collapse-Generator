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


    void OnEnable()
    {
        
        if (gameObject.transform.childCount > 0)
           _cityPart = gameObject.transform.GetChild(0).gameObject;
        ActivateCity();

        RotateCells.OnGridCollapse.AddListener(() => isChecked = false);
        RotateCells.OnModulesRotate.AddListener(DeactivateCity);
        EventManager.OnLevelFinish.AddListener(ActivateCity);
    }
    void OnDisable()
    {
        RotateCells.OnGridCollapse.RemoveListener(() => isChecked = false);
        RotateCells.OnModulesRotate.RemoveListener(DeactivateCity);
        EventManager.OnLevelFinish.RemoveListener(ActivateCity);

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
            _cityPart.SetActive(true);
        }
    }

    private void DeactivateCity()
    {
        if (_cityPart != null)
        {
            _cityPart.SetActive(false);
        }
    }
}
