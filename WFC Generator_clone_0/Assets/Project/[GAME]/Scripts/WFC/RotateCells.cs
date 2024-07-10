using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;
using Unity.Netcode;
using UnityEngine.EventSystems;

public class RotateCells : NetworkBehaviour
{
    private List<ModuleObject> _candidateMOs = new();
    WfcGenerator generator;
    IModuleObject moduleObject;
    int cellCountToRotate;  // DifficultyManager
    int randomTIndex;
    private List<Transform> lotTransforms = new();
    private List<float> _moduleAngles = new();
    private List<Transform> _rotatableTransforms = new();
    private int[] _desiredAngles = new int[3] {90, 180, 270};
    [HideInInspector] public static bool isRotating = false;///////
    [HideInInspector] public static bool isDrawCompleted;
    [HideInInspector] public static int rotatableCount;

    [HideInInspector] public static UnityEvent OnGridCollapse = new();
    [HideInInspector] public static UnityEvent OnModulesRotate = new();
    [HideInInspector] public static bool isMapSucceed { get; private set; }

    void OnEnable()
    {
        WfcGenerator.OnMapReady.AddListener(DrawCells);
        WfcGenerator.OnMapSolve.AddListener(RestoreMOsToOriginal);
        EventManager.OnClick.AddListener(UpdateAndCheckMap);
    }
    void OnDisable()
    {
        WfcGenerator.OnMapReady.RemoveListener(DrawCells);
        WfcGenerator.OnMapSolve.RemoveListener(RestoreMOsToOriginal);
        EventManager.OnClick.RemoveListener(UpdateAndCheckMap);
    }

    void Start()
    {
        generator = GetComponent<WfcGenerator>();
        isRotating = false;
    }

    void Update()
    {
        if (!isDrawCompleted) return;

        CheckInput();
    }

    
    

    void DrawCells()
    {
        _length = generator._length;
        _width = generator._width;
        _candidateMOs.AddRange(generator.moduleObjects);
        _rotatableTransforms.AddRange(generator.rotatableObjectTs);
        for (int i = 0; i < _rotatableTransforms.Count; i++)
        {
            if(_rotatableTransforms[i] != null)
                _moduleAngles.Add(_rotatableTransforms[i].rotation.eulerAngles.y);
        }
        lotTransforms.AddRange(_rotatableTransforms);
        rotatableCount = _rotatableTransforms.Count;
        
        cellCountToRotate = LevelManager.Instance.DifficultyData.MO_CountToRotate;

        for (int i = 0; i < cellCountToRotate; i++)
        {
            randomTIndex = Random.Range(0, lotTransforms.Count - 1);
            RotatePrefab(lotTransforms[randomTIndex]);
            lotTransforms.RemoveAt(randomTIndex);
        }
        Invoke("AnnounceRotateCompleted", 1f);
    }

    private void RotatePrefab(Transform moduleTransform)
    {
        if (moduleTransform != null) 
        {
            int randomIndex = Random.Range(0, _desiredAngles.Length - 1);
            int randomRotation = _desiredAngles[randomIndex];

            moduleTransform.DORotate(new Vector3(0f, (float)randomRotation, 0f), 1f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutQuad);
            for (int i = 0; i < randomIndex + 1; i++)
            {
                moduleObject = moduleTransform.GetComponent<IModuleObject>();
                if (moduleObject != null)
                {
                    moduleObject.UpdateMO_Angle(moduleTransform);
                }
            }

            //moduleTransform.GetComponent<ModuleObject>().RotateModuleForDrawn(randomIndex, randomRotation);
        }
    }

    private void AnnounceRotateCompleted()
    {
        OnModulesRotate.Invoke();
        isDrawCompleted = true;
    }

    float gap;
    private void RestoreMOsToOriginal()
    {
        for (int i = 0; i < _rotatableTransforms.Count; i++)
        {
            gap = _moduleAngles[i] - _rotatableTransforms[i].localEulerAngles.y;
            _rotatableTransforms[i].DORotate(new Vector3(0f, gap, 0f), 1f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutQuad);

            moduleObject = _rotatableTransforms[i].GetComponent<IModuleObject>();
            if (moduleObject != null)
            {
                moduleObject.UpdateMO_Angle(_rotatableTransforms[i]);
            }
        }

        isMapSucceed = false;
        Invoke("EndMap", 1f);
    }

    void CheckInput()
    {
        #region Mobile
        // if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        // {
        //     Vector3 touchPosition = Input.GetTouch(0).position;
        //     Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        //     RaycastHit hit;

        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         Transform moduleTransform = hit.transform.GetComponent<Transform>();

        //         if (moduleTransform != null)
        //         {
        //             RotateModule(moduleTransform);
        //             EventManager.OnClick.Invoke();
        //         }
        //     }
        // }
        #endregion

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            //Debug.Log("mouse button down");
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("raycast hit");
                Transform moduleTransform = hit.transform;
                //Debug.Log(isRotating);
                if (moduleTransform != null && !isRotating)
                {
                    //Debug.Log(moduleTransform.name + " module is not null");
                    AdaptGameMode(moduleTransform);
                }
            }
        }
    }
    private void AdaptGameMode(Transform moduleTransform)
    {
        if(GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.SinglePlayer)
        {
            if (_rotatableTransforms.Contains(moduleTransform))
            {
                //Debug.Log("rotatable contains the module");
                moduleObject = moduleTransform.GetComponent<IModuleObject>();
                if (moduleObject != null)
                {
                    //Debug.Log("m object is not null");
                    moduleTransform.GetComponent<ModuleObject>().RotateModule();
                }
            }
        }
        else
        {
            //Debug.Log("rotate multiplayer mode");
            //if (!IsOwner) return;

            if (IsServer || IsHost)
            {
                if (_rotatableTransforms.Contains(moduleTransform))
                {
                    //Debug.Log("rotatable contains the module");
                    moduleObject = moduleTransform.GetComponent<IModuleObject>();
                    if (moduleObject != null)
                    {
                        //Debug.Log("m object is not null");
                        moduleTransform.GetComponent<ModuleObject>().RotateModule();
                    }
                }
            }
            else if (IsClient)
            {
                //Debug.Log("is client true");
                if (moduleTransform.GetComponent<ModuleObject>().isRotatable)
                {
                    //Debug.Log("moduletransfrom is rotatable");
                    moduleObject = moduleTransform.GetComponent<IModuleObject>();
                    if (moduleObject != null)
                    {
                        //Debug.Log("m object is not null");
                        moduleTransform.GetComponent<ModuleObject>().RotatePuzzlePieceServerRpc();
                    }
                }
            }
            
        }
    }
    public NetworkVariable<bool> isRotatableTransformsContains = new NetworkVariable<bool>(false);

    private void UpdateAndCheckMap()
    {
        CollapseGrid();
        _candidateMOs.Clear();
        _candidateMOs.AddRange(generator.moduleObjects);
        isFail = false;
    }

    private void CollapseMO()
    {
        ModuleObject nextMO;
        nextMO = _candidateMOs[Random.Range(0, _candidateMOs.Count-1)];
        nextMO.isChecked = true;
        _candidateMOs.Remove(nextMO);

        FindNeighbors(nextMO);
    }
    
    private void CollapseGrid()
    {
        OnGridCollapse.Invoke();

        if (isFail) return;
        while (generator.moduleObjects.Where(x => !x.isChecked).Any())
        {
            if (_candidateMOs.Count > 0)
            {
                CollapseMO();
            }
            else
                break;

            if (isFail) 
                break;
        }

        if (!isFail)
        {
            //isMapSucceed = true;

            if (GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.Multiplayer)
            {
                if (IsHost)
                {
                    //EndMultiplayerSession();
                    //GameManager.OnMultiplayerGameFinish.Invoke();
                    EndChallengeClientRpc();
                }
            }
            else
            {
                isMapSucceed = true;
                EventManager.OnLevelSuccess.Invoke();
                EndMap();
            }
        }
    }
    [ClientRpc]
    private void EndChallengeClientRpc()
    {
        GameManager.OnMultiplayerGameFinish.Invoke();
        EndMultiplayerSession();
        
        generator.executingWfcGeneratorState = ExecutingWfcGeneratorState.Singleplayer;
        generator.SwitchState(generator.singleplayerPuzzleGenerator);
        isMapSucceed = true;
        EventManager.OnLevelSuccess.Invoke();
        EndMap();
    }
    public void EndMultiplayerSession()
    {
        if (IsServer || IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("session ended");
        }
    }

    private void EndMap()
    {
        Debug.Log("WIN !!!");
        LevelManager.Instance.LevelIndex++;
        PlayerPrefs.SetInt("LastLevel", LevelManager.Instance.LevelIndex);
        RecreateLevel();
    }
    
    private void RecreateLevel()
    {
        isDrawCompleted = false;
        _candidateMOs.Clear();
        _rotatableTransforms.Clear();
        lotTransforms.Clear();
        _moduleAngles.Clear();
        LevelManager.Instance.FinishLevel();
    }

    int _row;
    int _col;
    private int _length;
    private int _width;
    private bool isFail;
    private void FindNeighbors(ModuleObject moduleObject)
    {
        _row = moduleObject.Row;
        _col = moduleObject.Column;

        // North
        if (_col > 0 )
        {
            ModuleObject northNeighbor = generator.moduleObjects.Find(c => c.Column == _col - 1 && c.Row == _row && !c.isChecked);
            if (northNeighbor != null)
            {
                if (!IsMatching(0, northNeighbor, moduleObject))
                {
                    isFail = true;
                    return;
                }
            }
        }
        
        // South
        if (_col < _length - 1 )
        {
            ModuleObject southNeighbor = generator.moduleObjects.Find(c => c.Column == _col + 1 && c.Row == _row && !c.isChecked);
            if (southNeighbor != null)
            {
                if (!IsMatching(1, southNeighbor, moduleObject))
                {
                    isFail = true;
                    return;
                }
            }
        }

        // East
        if (_row < _width - 1 )
        {
            ModuleObject eastNeighbor = generator.moduleObjects.Find(c => c.Column == _col && c.Row == _row + 1 && !c.isChecked);
            if (eastNeighbor != null)
            {
                if (!IsMatching(2, eastNeighbor, moduleObject))
                {
                    isFail = true;
                    return;
                }
            }
        }

        // West
        if (_row > 0)
        {
            ModuleObject westNeighbor = generator.moduleObjects.Find(c => c.Column == _col && c.Row == _row - 1 && !c.isChecked);
            if (westNeighbor != null)
            {
                if (!IsMatching(3, westNeighbor, moduleObject))
                {
                    isFail = true;
                    return;
                }
            }
        }

        isFail = false;
    }

    private bool IsMatching(int direction, ModuleObject neighborModule, ModuleObject currentModule)
    {
        if (direction == 0) // North
            return neighborModule.south == currentModule.north;

        if (direction == 1) // South
            return neighborModule.north == currentModule.south;

        if (direction == 2) // East
            return neighborModule.west == currentModule.east;

        if (direction == 3) // West
            return neighborModule.east == currentModule.west;

        return false;
    }
}
