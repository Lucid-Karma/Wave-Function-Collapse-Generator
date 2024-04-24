using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WFC/ModuleSO")]
public class ModuleSO : ScriptableObject 
{
    public GameObject modulePrefab;
    [Space]
    public int north;
    public int south;
    public int east;
    public int west;

    [HideInInspector] public int moduleUsageCount = 0;

    [HideInInspector] public List<int> moduleType = new List<int>();

    private void OnEnable()
    {
        moduleType.Add(north);
        moduleType.Add(south);
        moduleType.Add(east);
        moduleType.Add(west);

        moduleObject = new(){

            isChecked = false,

            north = north,
            south = south,
            east = east,
            west = west
        };
        RotateCells.OnGridCollapse.AddListener(() => moduleObject.isChecked = false);
    }

    void OnDisable()
    {
        moduleUsageCount = 0;

        RotateCells.OnGridCollapse.RemoveListener(() => moduleObject.isChecked = false);
    }

    [HideInInspector] public ModuleObject moduleObject;
}

public class ModuleObject
{
    public Transform moduleTransform;

    public bool isChecked;

    public int Row;
    public int Column;

    public int north;
    public int south;
    public int east;
    public int west;
}

