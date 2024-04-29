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


    void OnEnable()
    {
        RotateCells.OnGridCollapse.AddListener(() => isChecked = false);
        //RotateCells.OnModulesRotate.AddListener(DeactivateCity);
        //EventManager.OnLevelSuccess.AddListener(ActivateCity);
    }
    void OnDisable()
    {
        RotateCells.OnGridCollapse.RemoveListener(() => isChecked = false);
        //RotateCells.OnModulesRotate.RemoveListener(DeactivateCity);
        //EventManager.OnLevelSuccess.RemoveListener(ActivateCity);

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
        // Debug.Log("default: " + moduleTransform.name +
        //     "\nnorth: " + north+
        //     "\nsouth: " + south+
        //     "\neast: " + east+
        //     "\nwest: " + west);
        _north = north;
        _south = south;
        _east = east;
        _west = west;

        north = _west;
        south = _east;
        east = _north;
        west = _south;
        // Debug.Log("current: " + moduleTransform.name +
        //     "\nnorth: " + north+
        //     "\nsouth: " + south+
        //     "\neast: " + east+
        //     "\nwest: " + west);
    }
}
