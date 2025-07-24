using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

public class WfcGenerator : Singleton<WfcGenerator>
{
    [HideInInspector]
    public List<CellSO> cells = new List<CellSO> ();
    private List<CellSO> candidateCells = new List<CellSO>();
    [SerializeField] private CellSO Cell; //.......................A CellSO reference with modules defined.
    public int _width;
    public int _length;
    [SerializeField] private float _moduleSize;


    private int firstCollapse; //................The index of the first cell to be created in the list.
    Quaternion prefabRotation;

    int _row;   
    int _col;

    int lowestEntropyValue; //...................Lowest usage value of modules among cells to Find_Lowest_Entropy and to Get_Definite_State.
    int randomModule; //.........................Selected cell's random module index to Get_Definite_State.

    [HideInInspector]
    public GameObject gridHolder; //.............Game object for centering the position of the grid.
    [SerializeField] 
    private GameObject emptyObject;
    [HideInInspector] public List<Transform> rotatableObjectTs = new();
    public List<ModuleObject> moduleObjects = new();
    ModuleObject moduleObject;
    [HideInInspector] public List<GameObject> animationSequenceList = new();
    [HideInInspector] public List<GameObject> multiplayerPieces = new();

    [HideInInspector] public static UnityEvent OnMapReady = new();
    [HideInInspector] public static UnityEvent OnMapSolve = new();
    [HideInInspector] public static UnityEvent OnMapAnimationEnd = new();
    [HideInInspector] public static UnityEvent OnMapPreReady = new();

    private void Start()
    {
        moduleObjects.Clear();
    }

    public void OnEnable()
    {
        EventManager.OnLevelInitialize.AddListener(RecreateLevel);
        EventManager.OnLevelStart.AddListener(GenerateWFC);
    }
    public  void OnDisable()
    {
        EventManager.OnLevelInitialize.RemoveListener(RecreateLevel);
        EventManager.OnLevelStart.RemoveListener(GenerateWFC);
    }
    [SerializeField] private GameObject carPrefab;
    private List<GameObject> cars = new List<GameObject>();
    public virtual void RecreateLevel()
    {
        ResetData();
        Destroy(gridHolder);
        LevelManager.Instance.StartLevel();
    }
    private void ResetData()
    {
        cells.Clear();
        candidateCells.Clear();
        rotatableObjectTs.Clear();
        moduleObjects.Clear();
        RotatableRegionIndexList.Clear();

        originalPositions.Clear();
        animationSequenceList.Clear();
    }
    private void DestroyMO_Objects()
    {
        for (int i = 0; i < moduleObjects.Count; i++)
        {
            if (moduleObjects[i] != null)
            {
                Destroy(moduleObjects[i].gameObject);
            }
            //else
            //    Debug.Log(i + " null " + moduleObjects.Count);
        }
    }

    public void GenerateWFC()
    {
        Generate();
        StartWave();
        CollapseGrid();
    }

    private void StartWave()
    {
        firstCollapse = cells.Count / 2;
        
        cells[firstCollapse].isCollapsed = true;
        int starterModule = Random.Range(0, cells[firstCollapse].modules.Count);
        //Debug.Log("Starting wave. Collapsed cell is: " + firstCollapse);

        cells[firstCollapse].modules.RemoveAll(module => module !=cells[firstCollapse].modules[starterModule]); //it was 4.
        cells[firstCollapse].modules[0].moduleUsageCount ++;
        prefabRotation = cells[firstCollapse].modules[0].modulePrefab.transform.rotation;
        GameObject obj = (GameObject)Instantiate(cells[firstCollapse].modules[0].modulePrefab, cells[firstCollapse].cellPos, prefabRotation);

        moduleObject = obj.GetComponent<ModuleObject>();
        if (moduleObject != null)
        {
            moduleObject.Row = cells[firstCollapse].Row;
            moduleObject.Column = cells[firstCollapse].Column;
            moduleObjects.Add(moduleObject);
            moduleObject.isDowntown = true;
        }

        ListRotatableMOTransforms(cells[firstCollapse], cells[firstCollapse].modules[0], obj.transform);

        gridHolder = (GameObject)Instantiate(emptyObject);
        gridHolder.transform.position = obj.transform.position;
        obj.transform.SetParent(gridHolder.transform);
        animationSequenceList.Add(obj);
    }

    private void Generate()
    {
        CellSO originalCell = Cell;

        for (int row = 0; row < _width; row++)
        {
            for (int col = 0; col < _length; col++)
            {
                CellSO cell = ScriptableObject.CreateInstance<CellSO>();
                cell.modules = new List<ModuleSO>(originalCell.modules);
                cell.cellPos = new Vector3(row * _moduleSize, 0, col * _moduleSize * -1);   
                cell.Row = row;
                cell.Column = col;

                cells.Add(cell);
            }
        }

        centerCellIndex = cells.Count / 2;
        starterIndex = centerCellIndex - 4 - (_length * 2);
        //CalculateRotatableRegion();
        GetDiagonalAreaIndexes(9, 5);
        //Debug.Log("Grid generated. Cell count: " + cells.Count);
    }
    
    private void FindNeighbors(CellSO cell)
    {
        _row = cell.Row;
        _col = cell.Column;

        // North
        if (_col > 0 )
        {
            CellSO northNeighbor = cells.Find(c => c.Column == _col - 1 && c.Row == _row && !c.isCollapsed);
            if (northNeighbor != null)
            {
                UpdateCell(0, northNeighbor, cell);

                if (!candidateCells.Contains(northNeighbor))
                {
                    candidateCells.Add(northNeighbor);
                }
            }
        }

        // South
        if (_col < _length - 1 )
        {
            CellSO southNeighbor = cells.Find(c => c.Column == _col + 1 && c.Row == _row && !c.isCollapsed);
            if (southNeighbor != null)
            {
                UpdateCell(1, southNeighbor, cell);

                if (!candidateCells.Contains(southNeighbor))
                {
                    candidateCells.Add(southNeighbor);
                }
            }
        }

        // East
        if (_row < _width - 1 )
        {
            CellSO eastNeighbor = cells.Find(c => c.Column == _col && c.Row == _row + 1 && !c.isCollapsed);
            if (eastNeighbor != null)
            {
                UpdateCell(2, eastNeighbor, cell);

                if (!candidateCells.Contains(eastNeighbor))
                {
                    candidateCells.Add(eastNeighbor);
                }
            }
        }

        // West
        if (_row > 0)
        {
            CellSO westNeighbor = cells.Find(c => c.Column == _col && c.Row == _row - 1 && !c.isCollapsed);
            if (westNeighbor != null)
            {
                UpdateCell(3, westNeighbor, cell);

                if (!candidateCells.Contains(westNeighbor))
                {
                    candidateCells.Add(westNeighbor);
                }
            }
        }
    }

    private CellSO FindLowestEntropy()
    {
        int lowestModuleCount = candidateCells.Min(list => list.modules.Count); 

        var lowestEntropies = candidateCells.Where(num => num.modules.Count == lowestModuleCount).ToList();

        if(lowestEntropies.Count > 1)
        {
            lowestEntropyValue = lowestEntropies.Min(x => x.entropy);
            var lowestEntropyValues = lowestEntropies.Where(entropyValue => entropyValue.entropy == lowestEntropyValue).ToList();

            if(lowestEntropyValues.Count > 1)    return lowestEntropyValues[Random.Range(0, lowestEntropyValues.Count)];
            else    return lowestEntropies.Find(y => y.entropy == lowestEntropyValue);  // or lowestEntropyValues[0] ...first and only element.
        }
        else
        {
            return lowestEntropies[0];
        }
    }

    private void UpdateCell(int direction, CellSO neighborCell, CellSO cell)
    {
        // 0=north, 1=south, 2=east, 3=west

        neighborCell.modules.RemoveAll(possibleModule => !IsMatching(direction, possibleModule, cell.modules[0]));  //cell.modules[0] represents the last remaining cell.

        neighborCell.entropy = neighborCell.modules.Sum(x => x.moduleUsageCount);
    }

    private bool IsMatching(int direction, ModuleSO neighborModule, ModuleSO cellModule)
    {
        if (direction == 0) // North
            return neighborModule.south == cellModule.north;

        if (direction == 1) // South
            return neighborModule.north == cellModule.south;

        if (direction == 2) // East
            return neighborModule.west == cellModule.east;

        if (direction == 3) // West
            return neighborModule.east == cellModule.west;

        return false;
    }

    private GameObject GetDefiniteState(CellSO currentCell)
    {
        if (currentCell.modules.Count > 0)
        {   
            ModuleSO selectedModule;
            if (currentCell.modules.Where(x => x.moduleUsageCount == lowestEntropyValue).Any())
            {
                selectedModule = currentCell.modules.Find(x => x.moduleUsageCount == lowestEntropyValue);
            }
            else
            {
                randomModule = Random.Range(0, currentCell.modules.Count);
                selectedModule = currentCell.modules[randomModule];
            }

            currentCell.modules.RemoveAll(module => module !=selectedModule);
            
            if (selectedModule != null)
            {
                GameObject modulePrefab = selectedModule.modulePrefab;

                if (modulePrefab != null)
                {
                    selectedModule.moduleUsageCount ++;
                    prefabRotation = modulePrefab.transform.rotation;
                    return modulePrefab;
                }
                else
                {
                    Debug.LogWarning("modulePrefab is not assigned in the selected ModuleSO.");
                    return null; // or return a default GameObject if you have one.
                }
            }
            else
            {
                Debug.LogWarning("Selected ModuleSO is null.");
                return null; // or return a default GameObject if you have one.
            }
        }
        else
        {
            Debug.LogWarning("No modules attached to the current cell.");
            return null; // or return a default GameObject if you have one.
        }
    }

    private void CollapseCell()
    {
        CellSO nextCell;
        nextCell = FindLowestEntropy();
        nextCell.isCollapsed = true;
        GameObject obj = Instantiate(GetDefiniteState(nextCell), nextCell.cellPos, prefabRotation);

        moduleObject = obj.GetComponent<ModuleObject>();
        if (moduleObject != null)
        {
            moduleObject.Row = nextCell.Row;
            moduleObject.Column = nextCell.Column;
            moduleObjects.Add(moduleObject);
            //Debug.Log(moduleObject.name + moduleObjects.Count);
        }
        
        ListRotatableMOTransforms(nextCell, nextCell.modules[0], obj.transform);

        obj.transform.SetParent(gridHolder.transform);
        SetModulesStatic(nextCell, moduleObject, obj);
        animationSequenceList.Add(obj);
        //Debug.Log("cell index is: " + candidateCells.IndexOf(nextCell));
        candidateCells.Remove(nextCell);
        
        //Debug.Log("Cell collapsed. CandidateCells count: " + candidateCells.Count);

        FindNeighbors(nextCell);
    }
    private void CollapseGrid()
    {
        if(cells.Where(x => x.isCollapsed).Any())
        {
            var cell = cells.Find(x => x.isCollapsed);

            FindNeighbors(cell);
        } 

        while (cells.Where(x => !x.isCollapsed).Any())
        {
            if (candidateCells.Count > 0)
            {
                CollapseCell();
            }
            else
            {
                break;
            }
        }

        gridHolder.transform.position = new Vector3(0, 0, 28);//Vector3.zero;
        //Invoke("AnnounceMapPreReady", 3.5f);
        //print("R COUNT: " + rotatableObjectTs.Count);
        //Invoke("AnnounceMapReady", 4f); // 2f
        AnimateModulesEnter();
    }

    #region Utility Methods
    void ListRotatableMOTransforms(CellSO cell, ModuleSO module, Transform moduleTransform)
    {
        if (!RotatableRegionIndexList.Contains(cells.IndexOf(cell))) return;

        if (module.north == module.south && module.south == module.east && module.east == module.west)
        {
            InnerIntersectionList.Add(moduleTransform);
        }
        else
        {
            rotatableObjectTs.Add(moduleTransform);
            //print("CANDIDATE NO: " + cells.IndexOf(cell));
        }
    }
    private void SetModulesStatic(CellSO cell, ModuleObject module, GameObject moduleObj)
    {
        if (!RotatableRegionIndexList.Contains(cells.IndexOf(cell)))
        {
            module.isDowntown = false;
            moduleObj.isStatic = true;
        }
        else
            module.isDowntown = true;
    }
    List<Transform> InnerIntersectionList = new List<Transform>();
    List<int> RotatableRegionIndexList = new List<int>();
    int centerCellIndex;
    int starterIndex;
    private void CalculateRotatableRegion()
    {
        int cellIndex = starterIndex;
       
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                RotatableRegionIndexList.Add(cellIndex);
                cellIndex ++;

            }
            starterIndex += _length;
            cellIndex = starterIndex;
        }
    }
    private void GetDiagonalAreaIndexes(int length, int thickness)
    {
        // Start from bottom-left
        int startX = 5;
        int startZ = 13;

        for (int i = 0; i < length; i++)
        {
            int x = startX + i;
            int z = startZ - i;

            if (x >= _width || z < 0) break;

            // For each point along the diagonal, add thickness vertically along z-axis
            int halfThickness = thickness / 2;
            for (int dz = -halfThickness; dz <= halfThickness; dz++)
            {
                int thickZ = z + dz;
                if (thickZ >= 0 && thickZ < _length)
                {
                    int index = thickZ * _width + x;
                    RotatableRegionIndexList.Add(index);
                }
            }
        }
    }

    [SerializeField] private float delayBetweenBlocks = 0.1f;
    [SerializeField] private float riseDuration = 0.5f;
    [SerializeField] private float startYOffset = -49f;

    private List<Vector3> originalPositions = new List<Vector3>();
    private void AnimateModulesEnter()
    {
        foreach (var block in animationSequenceList)
        {
            Vector3 originalPos = block.transform.position;
            originalPositions.Add(originalPos);
            block.transform.position = originalPos + Vector3.up * startYOffset;
        }

        // 2. Animasyonu baþlat
        StartCoroutine(RevealCity());
    }
    [SerializeField] private AudioSource cityPartAnimFx;
    [SerializeField] private AudioClip cityPartClip;
    [SerializeField] private AudioSource restOfTheCityAnimFx;
    IEnumerator RevealCity()
    {
        for (int i = 0; i < InnerIntersectionList.Count; i++)
        {
            InnerIntersectionList[i].DOMoveY(originalPositions[i].y, riseDuration)
                .SetEase(Ease.OutBack);

            cityPartAnimFx.PlayOneShot(cityPartClip);
            animationSequenceList.Remove(rotatableObjectTs[i].gameObject);
            yield return new WaitForSeconds(delayBetweenBlocks);
        }
        for (int i = 0; i < rotatableObjectTs.Count; i++)
        {
            rotatableObjectTs[i].DOMoveY(originalPositions[i].y, riseDuration)
                .SetEase(Ease.OutBack);

            cityPartAnimFx.PlayOneShot(cityPartClip);
            animationSequenceList.Remove(rotatableObjectTs[i].gameObject);
            yield return new WaitForSeconds(delayBetweenBlocks);
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < animationSequenceList.Count; i++)
        {
            animationSequenceList[i].transform.DOMoveY(originalPositions[i].y, riseDuration)
                .SetEase(Ease.OutBack);
        }
        restOfTheCityAnimFx.Play() ;

        InnerIntersectionList.Clear();
        OnMapAnimationEnd.Invoke();

        Invoke("AnnounceMapPreReady", 3.5f);
        print("R COUNT: " + rotatableObjectTs.Count);
        Invoke("AnnounceMapReady", 4f); // 2f
    }

    //private int MapSize = 15;
    //private float NormalizedMapSize { get { return 1f / (float)MapSize; } }
    //private Vector4 CalculateSquareRegion(int squareId)
    //{
    //    float y = squareId / MapSize;
    //    float x = squareId - (y * MapSize);

    //    x *= NormalizedMapSize;
    //    y *= NormalizedMapSize;

    //    y = 1f - y - NormalizedMapSize;

    //    return new Vector4(x, y, x + NormalizedMapSize, y + NormalizedMapSize);
    //}

    void AnnounceMapReady()
    {
        OnMapReady?.Invoke();
    }
    void AnnounceMapPreReady()
    {
        OnMapPreReady.Invoke();
    }
    #endregion        
}
