# Wave Function Collapse Generator (For Procedural Level Generation)
WFC generator is created as a solid example of a fully architectured system. Below, you can see how each method contributes to the overall functionality of the wave function collapse algorithm.
This approach ensures a modular, efficient, and expandable system for procedural level generation.

## Table of Contents
- [Key Characteristics of The WFC System](#KeyCharacteristicsofTheWFCSystem)
- [Code Explanation (WfcGenerator)](#CodeExplanation(WfcGenerator))
  - [Class and Variables Setup](#ClassandVariablesSetup)
  - [Event Listeners Setup](#EventListenersSetup)
  - [Level Recreation](#LevelRecreation)
  - [Grid Generation and Initialization](#GridGenerationandInitialization)
  - [Starting the Wave Function Collapse](#StartingtheWaveFunctionCollapse)
  - [Finding and Collapsing the Grid Cells](#FindingandCollapsingtheGridCells)
  - [Updating Cells and Matching Modules](#UpdatingCellsandMatchingModules)
  - [Finalizing the Collapse](#FinalizingtheCollapse)
- [Other Scripts](#OtherScripts)
  - [CellSO](#CellSO)
  - [ModuleSO](#ModuleSO)
  - [EventManager](#EventManager)
- [Gallery](#Gallery)

## Key Characteristics of The WFC System

1. **Separation of Concerns**:
   - Distinct classes (`WfcGenerator`, `CellSO`, `ModuleSO`) each serving a specific purpose:
     - **`WfcGenerator`**: Manages the overall process of generating the grid and collapsing cells.
     - **`CellSO`**: Represents individual cells in the grid, each with its potential modules and state.
     - **`ModuleSO`**: Represents the modules that can occupy cells, including their connections and properties.

2. **Data Management**:
   - As a architectural decision, using ScriptableObjects (`CellSO`, `ModuleSO`) to store cell and module data makes the system modular and allows easy configuration and extension without changing the code.

3. **Modularity and Extensibility**:
   - The system is modular, allowing new modules and cells to be added by creating new ScriptableObjects. This is a sign of good software architecture, making the system easily extensible and maintainable.

4. **Encapsulation and Reusability**:
   - The generator encapsulates the WFC algorithm logic, including initialization, neighbor checking, entropy calculation, and cell collapsing. This makes the code reusable for generating different types of levels or maps.

5. **Event-Driven Architecture**:
   - Using `UnityEvent` for events like `OnMapReady` shows an event-driven architecture, which is helpful in decoupling different parts of the system.

6. **Clear Flow and Management**:
   - The script has a clear flow: initializing the grid (`Generate`), starting the collapse (`StartWave`), managing the collapse (`CollapseCell`, `CollapseGrid`), and finding neighbors (`FindNeighbors`). This clear flow helps in maintaining and understanding the system.


## Code Explanation (WfcGenerator)

### 1. **Class and Variables Setup**

```csharp
public class WfcGenerator : MonoBehaviour
{
    [HideInInspector]
    public List<CellSO> cells = new List<CellSO>();
    private List<CellSO> candidateCells = new List<CellSO>();
    public CellSO Cell; // A reference to a CellSO ScriptableObject, which defines possible modules.
    public int _width;  // Width of the grid.
    public int _length; // Length of the grid.
    [SerializeField] private int _moduleSize; // Size of each module.

    private int firstCollapse; // Index of the first cell to be collapsed.
    
    // Other variables...
}
```



**Explanation:**
- **Purpose**: The `WfcGenerator` class is the main driver for the WFC algorithm in Unity. It controls the grid's generation, the collapse process, and the interaction between different cells and modules.
- **Variables**:
  - `cells` and `candidateCells` are lists that store all cells and candidate cells for collapse.
  - `Cell` is a `CellSO` (ScriptableObject) that defines possible modules for each cell. [see](#cellSo)
  - `_width` and `_length` define the dimensions of the grid.
  - `firstCollapse` tracks the first cell to be collapsed to start the WFC process.

### 2. **Event Listeners Setup**

```csharp
void OnEnable()
{
    EventManager.OnLevelStart.AddListener(GenerateWFC);
    EventManager.OnLevelInitialize.AddListener(RecreateLevel);
}

void OnDisable()
{
    EventManager.OnLevelStart.RemoveListener(GenerateWFC);
    EventManager.OnLevelInitialize.RemoveListener(RecreateLevel);
}
```

**Explanation:**
- **Purpose**: These methods manage event listeners, ensuring the appropriate methods are called when specific events occur (like starting or initializing a level).
- **`OnEnable` and `OnDisable`**:
  - `OnEnable`: Registers methods (`GenerateWFC` and `RecreateLevel`) to respond to events (`OnLevelStart` and `OnLevelInitialize`).
  - `OnDisable`: Unregisters these methods to prevent memory leaks or unintended behavior when the object is disabled or destroyed.

### 3. **Level Recreation**

```csharp
private void RecreateLevel()
{
    cells.Clear();
    candidateCells.Clear();
    rotatableObjectTs.Clear();
    moduleObjects.Clear();

    Destroy(gridHolder);
    LevelManager.Instance.StartLevel();
}
```

**Explanation:**
- **Purpose**: Resets the grid and clears all data to start fresh. This function is useful when restarting or initializing a new level.
- **Details**:
  - Clears all lists to remove previous data and reset the state.
  - Destroys `gridHolder`, which is a GameObject that contains all the grid cells.
  - Calls `StartLevel` from `LevelManager` to begin the level setup process anew.

### 4. **Grid Generation and Initialization**

```csharp
public void GenerateWFC()
{
    Generate();
    StartWave();
    CollapseGrid();
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
}
```

**Explanation:**
- **Purpose**: These methods are responsible for generating the grid of cells and initializing the WFC process.
- **`GenerateWFC`**: Orchestrates the generation, starting the wave, and collapsing the grid.
- **`Generate`**:
  - Iterates over a grid size defined by `_width` and `_length`.
  - Creates new `CellSO` instances for each position in the grid.
  - Copies modules from the `originalCell` (a predefined `CellSO`) to each new cell, setting its position and grid coordinates.

### 5. **Starting the Wave Function Collapse**

```csharp
private void StartWave()
{
    firstCollapse = cells.Count / 2;
    
    cells[firstCollapse].isCollapsed = true;
    int starterModule = Random.Range(0, cells[firstCollapse].modules.Count);

    cells[firstCollapse].modules.RemoveAll(module => module != cells[firstCollapse].modules[starterModule]);
    cells[firstCollapse].modules[0].moduleUsageCount++;
    prefabRotation = cells[firstCollapse].modules[0].modulePrefab.transform.rotation;
    GameObject obj = Instantiate(cells[firstCollapse].modules[0].modulePrefab, cells[firstCollapse].cellPos, prefabRotation);

    moduleObject = obj.GetComponent<ModuleObject>();
    if (moduleObject != null)
    {
        moduleObject.Row = cells[firstCollapse].Row;
        moduleObject.Column = cells[firstCollapse].Column;
        moduleObjects.Add(moduleObject);
    }

    ListRotatableMOTransforms(cells[firstCollapse].modules[0], obj.transform);

    gridHolder = Instantiate(emptyObject);
    gridHolder.transform.position = obj.transform.position;
    obj.transform.SetParent(gridHolder.transform);
}
```

**Explanation:**
- **Purpose**: Initializes the wave collapse by randomly selecting a module for the first cell and setting it up in the grid.
- **Details**:
  - **`firstCollapse`**: Starts collapsing from the middle of the grid (`cells.Count / 2`).
  - **Random Module Selection**: Randomly selects a module for the first cell and removes all other modules that are not selected.
  - **Instantiation**: Instantiates the selected module's prefab at the cell's position with its rotation.
  - **Grid Holder Setup**: Creates a `gridHolder` GameObject to manage the positioning of all grid cells collectively.

### 6. **Finding and Collapsing the Grid Cells**

```csharp
private void FindNeighbors(CellSO cell)
{
    _row = cell.Row;
    _col = cell.Column;

    // Check each direction (North, South, East, West) and add valid neighbors to candidateCells...
}

private CellSO FindLowestEntropy()
{
    int lowestModuleCount = candidateCells.Min(list => list.modules.Count); 

    var lowestEntropies = candidateCells.Where(num => num.modules.Count == lowestModuleCount).ToList();

    if (lowestEntropies.Count > 1)
    {
        lowestEntropyValue = lowestEntropies.Min(x => x.entropy);
        var lowestEntropyValues = lowestEntropies.Where(entropyValue => entropyValue.entropy == lowestEntropyValue).ToList();

        return lowestEntropyValues.Count > 1 
            ? lowestEntropyValues[Random.Range(0, lowestEntropyValues.Count)] 
            : lowestEntropyValues[0];
    }
    else
    {
        return lowestEntropies[0];
    }
}
```

**Explanation:**
- **Purpose**: These methods are essential for determining the next cell to collapse based on their entropy and updating their states accordingly.
- **`FindNeighbors`**:
  - Searches for valid neighbor cells around a given cell (`North`, `South`, `East`, `West`) that have not yet collapsed.
  - Adds valid neighbors to the `candidateCells` list for potential collapse.
- **`FindLowestEntropy`**:
  - Finds the cell with the lowest entropy (the fewest possible modules) among the `candidateCells`.
  - If multiple cells have the same lowest entropy, it randomly selects one to add variety and avoid deterministic patterns in the grid generation.

### 7. **Updating Cells and Matching Modules**

```csharp
private void UpdateCell(int direction, CellSO neighborCell, CellSO cell)
{
    // direction: 0 = North, 1 = South, 2 = East, 3 = West

    neighborCell.modules.RemoveAll(possibleModule => !IsMatching(direction, possibleModule, cell.modules[0]));

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
```

**Explanation:**
- **Purpose**: These methods are used to refine the possible modules for each cell based on its neighbors, ensuring that the modules match correctly according to predefined rules.
- **`UpdateCell`**:
  - Filters out modules in a neighboring cell that do not match the module in the currently collapsed cell.
  - Recalculates the entropy of the neighboring cell after possible modules have been removed.
- **`IsMatching`**:
  - Checks if a module in a neighboring cell matches the module in the current cell based on their edges (`north`, `south`, `east`, `west`).

### 8. **Finalizing the Collapse**

```csharp
public void CollapseGrid()
{
    while (candidateCells.Any(cell => !cell.isCollapsed))
    {
        var cellToCollapse = FindLowestEntropy();
        cellToCollapse.isCollapsed = true;

        // Continue the collapse process by selecting and instantiating modules as in StartWave...
    }

    AnnounceMapReady();
}

private void AnnounceMapReady()
{
    EventManager.OnMapReady.Invoke();
}
```
**Explanation:**

- **Purpose**: Manages the collapse of all cells in the grid until no more cells are left uncollapsed, finalizing the procedural generation process.
- **Details**:
  - **CollapseGrid**: Continuously collapses cells with the lowest entropy, reducing the possible states in each step.
  - **AnnounceMapReady**: Invokes an event to signal that the map generation is complete, allowing other systems to proceed.
 

## Other Scripts

### **CellSO**
```csharp
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WFC/CellSO")]
public class CellSO : ScriptableObject 
{
    public List<ModuleSO> modules = new List<ModuleSO>();
    [HideInInspector] public Vector3 cellPos;
    public bool isCollapsed = false;
    [HideInInspector] public int entropy = 0;

    public int Row { get; set; }
    public int Column { get; set; }

    private void OnDisable()
    {
        isCollapsed = false;
        entropy = 0;
    }
}
```

### **ModuleSO**
```csharp
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WFC/ModuleSO")]
public class ModuleSO : ScriptableObject 
{
    public GameObject modulePrefab;
    [HideInInspector] public ModuleObject moduleObject;
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

        moduleObject = modulePrefab.GetComponent<ModuleObject>();
        if (moduleObject != null)
        {
            moduleObject.north = north;
            moduleObject.south = south;
            moduleObject.east = east;
            moduleObject.west = west;
        }
    }

    void OnDisable()
    {
        moduleUsageCount = 0;
    }
}
```

### **EventManager**
```csharp
using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent OnGameStart = new();
    public static UnityEvent OnGameEnd = new();

    public static UnityEvent OnLevelInitialize = new();
    public static UnityEvent OnLevelStart = new();
    public static UnityEvent OnLevelContine = new();
    public static UnityEvent OnLevelFinish = new();

    public static UnityEvent OnLevelSuccess = new();
    public static UnityEvent OnLevelFail = new();

    public static UnityEvent OnRestart = new();
}
```

## Gallery

<img src="https://github.com/user-attachments/assets/46449fa5-4200-437f-b8f7-6556bcd8ca57" alt="Açıklama"  width="600"/>
<img src="https://github.com/user-attachments/assets/2fc713ad-6c9c-4913-bddf-094dc0a8ac37" alt="Açıklama"  width="600"/>
<img src="https://github.com/user-attachments/assets/29e0bb2c-1c37-427c-8984-265f64f99a8a" alt="Açıklama"  height="400" id="cellSo"/>
<img src="https://github.com/user-attachments/assets/7ece6916-bde7-449f-b4fc-4e0d8256adcc" alt="Açıklama"  height="400"/>
<img src="https://github.com/user-attachments/assets/5c9fd447-007a-4a32-b96e-e5576710bc1e" alt="Açıklama"  height="400"/>
