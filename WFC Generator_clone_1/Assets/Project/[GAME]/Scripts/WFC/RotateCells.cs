using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;
using Unity.Netcode;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class RotateCells : MultiplayerSingleton<RotateCells>
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
    [HideInInspector] public bool isRotating = false;///////
    [HideInInspector] public bool isDrawCompleted;
    [HideInInspector] public int rotatableCount;

    [HideInInspector] public static UnityEvent OnGridCollapse = new();
    [HideInInspector] public static UnityEvent OnModulesRotate = new();
    [HideInInspector] public static UnityEvent OnNetworkShutdown = new();
    [HideInInspector] public bool isMapSucceed { get; private set; }
    [HideInInspector] public bool isMismatch = false;

    void OnEnable()
    {
        WfcGenerator.OnMapReady.AddListener(DrawCells);
        WfcGenerator.OnMapSolve.AddListener(RestoreMOsToOriginal);
        EventManager.OnClick.AddListener(UpdateAndCheckMap);
        EventManager.OnCollapseEnd.AddListener(() => MultiplayerTurnManager.Instance.NumberOfMoves++ );
        LobbyManager.OnClientDisconnect.AddListener(EndMatchDueToDisconnect);
    }
    void OnDisable()
    {
        WfcGenerator.OnMapReady.RemoveListener(DrawCells);
        WfcGenerator.OnMapSolve.RemoveListener(RestoreMOsToOriginal);
        EventManager.OnClick.RemoveListener(UpdateAndCheckMap);
        EventManager.OnCollapseEnd.RemoveListener(() => MultiplayerTurnManager.Instance.NumberOfMoves++ );
        LobbyManager.OnClientDisconnect.RemoveListener(EndMatchDueToDisconnect);
    }

    void Start()
    {
        generator = GetComponent<WfcGenerator>();
        isRotating = false;
    }

    void Update()
    {
        if (!isDrawCompleted) return;

        if (GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.SinglePlayer)
        {
            CheckInput();
        }
        else
        {
            if (MultiplayerTurnManager.Instance.CanPlay && !isRotating)
            {
                CheckInput();
            }
        }
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

            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform moduleTransform = hit.transform;
                if (moduleTransform != null && !isRotating)
                {
                    //Debug.Log($"CanPlay={DrawController.Instance.canPlay}, \nIsRotating={isRotating}");
                    AdaptGameMode(moduleTransform);
                }
            }
        }
    }
    public void AdaptGameMode(Transform moduleTransform)
    {
        if(GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.SinglePlayer)
        {
            if (_rotatableTransforms.Contains(moduleTransform))
            {
                moduleObject = moduleTransform.GetComponent<IModuleObject>();
                if (moduleObject != null)
                {
                    moduleTransform.GetComponent<ModuleObject>().RotateModule(false);
                }
            }
        }
        else
        {  
            if (IsServer || IsHost)
            {
                if (_rotatableTransforms.Contains(moduleTransform))
                {
                    moduleObject = moduleTransform.GetComponent<IModuleObject>();
                    if (moduleObject != null)
                    {
                        moduleTransform.GetComponent<ModuleObject>().RotateModule(true);
                    }
                }
            }
            else if (IsClient)
            {
                if (moduleTransform.GetComponent<ModuleObject>().isRotatable)
                {
                    moduleObject = moduleTransform.GetComponent<IModuleObject>();
                    if (moduleObject != null)
                    {
                        isRotating = true;
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
            if (GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.Multiplayer)
            {
                EndMultiplayerMatch();
            }
            else
            {
                isMapSucceed = true;
                EventManager.OnLevelSuccess.Invoke();
                EndMap();
                return;
            }
        }
    }
    private void EndMultiplayerMatch()
    {
        if (MultiplayerTurnManager.Instance.currentPlayer == MultiplayerTurnManager.Turn.HostTurn)
        {
            isMapSucceed = true;
            EventManager.OnLevelSuccess.Invoke();
            EndChallengeClientRpc();
            return;
        }
        else if (MultiplayerTurnManager.Instance.currentPlayer == MultiplayerTurnManager.Turn.ClientTurn)
        {

            EndChallengeWithWinClientRpc();
            return;
        }
    }

    [ClientRpc]
    private void EndChallengeClientRpc()
    {
        RecreateLevel();

        GameManager.OnMultiplayerGameFinish.Invoke();
        EndMultiplayerSession();

        generator.executingWfcGeneratorState = ExecutingWfcGeneratorState.Singleplayer;
        generator.SwitchState(generator.singleplayerPuzzleGenerator);
    }
    [ClientRpc]
    public void EndChallengeWithWinClientRpc()
    {
        isMapSucceed = false;

        if (!IsHost)
        {
            isMapSucceed = true;
            EventManager.OnLevelSuccess.Invoke();
        }
        
        EndChallengeClientRpc();
    }
    [ClientRpc]
    public void EndMatchClientRpc() // GiveUp...makes host win when client give up.
    {
        if (IsHost)
        {
            isMapSucceed = true;
            EventManager.OnLevelSuccess.Invoke();
        }

        EndChallengeClientRpc();
    }
    public void EndMatchDueToDisconnect()
    {
        if (isMismatch)
        {
            isMismatch = false;
            return;
        }  

        isMapSucceed = true;
        EventManager.OnLevelSuccess.Invoke(); 

        RecreateLevel();

        GameManager.OnMultiplayerGameFinish.Invoke();
        EndMultiplayerSession();

        generator.executingWfcGeneratorState = ExecutingWfcGeneratorState.Singleplayer;
        generator.SwitchState(generator.singleplayerPuzzleGenerator);
    }
    public void EndMatchDueToMismatch()
    {
        isMismatch = true;
        RecreateLevel();
        EndMultiplayerSession();
    }
    
    public void EndMultiplayerSession()
    {
        if (IsServer || IsHost)
        {
            if (isMismatch)
                isMismatch = false;

            OnNetworkShutdown.Invoke();
            NetworkManager.Singleton.Shutdown();
        }
    }

    private void EndMap()
    {
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
    public void ResetMapSuccess()
    {
        isMapSucceed = false;
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
