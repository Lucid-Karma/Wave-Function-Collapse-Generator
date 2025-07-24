using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;
using Unity.Netcode;
using UnityEngine.EventSystems;

public class CharacterBase : Singleton<CharacterBase> 
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

    private float tapInputDownTime;
    private float tapDuration;

    void OnEnable()
    {
        WfcGenerator.OnMapReady.AddListener(EnsureDraw);
        WfcGenerator.OnMapSolve.AddListener(RestoreMOsToOriginal);
        EventManager.OnClick.AddListener(UpdateAndCheckMap);
    }
    void OnDisable()
    {
        WfcGenerator.OnMapReady.RemoveListener(EnsureDraw);
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
            if (lotTransforms.Count == 0)
                break;

            randomTIndex = Random.Range(0, lotTransforms.Count);
            LiftAndLowerBrokeMO(lotTransforms[randomTIndex]);
            RotatePrefab(lotTransforms[randomTIndex]);
            lotTransforms.RemoveAt(randomTIndex);
        }
    }
    private void EnsureDraw()
    {
        do
        {
            DrawCells();
            CollapseGrid();
        }
        while (!isFail);

        Invoke("AnnounceRotateCompleted", 1f);
    }
    private void LiftAndLowerBrokeMO(Transform rotatableTransform)
    {
        rotatableTransform.DOMove(new Vector3(rotatableTransform.position.x, 1, rotatableTransform.position.z), 1f).SetEase(Ease.Unset)
            .OnComplete(() =>
            {
                rotatableTransform.DOMove(new Vector3(rotatableTransform.position.x, 0, rotatableTransform.position.z), 0.2f).SetEase(Ease.InBack);
            });
    }
    private void LiftAndLowerMOs()
    {
        for (int i = 0; i < _rotatableTransforms.Count; i++)
        {
            Transform rotatableTransform = _rotatableTransforms[i];
            rotatableTransform.DOMove(new Vector3(rotatableTransform.position.x, 1, rotatableTransform.position.z), 1f).SetEase(Ease.Unset)
                .OnComplete(() =>
                {
                    rotatableTransform.DOMove(new Vector3(rotatableTransform.position.x, 0, rotatableTransform.position.z), 0.2f).SetEase(Ease.InBack);
                });
        }
    }
    public bool IsRotatable(Transform t)
    {
        return _rotatableTransforms.Contains(t);
    }

    private void RotatePrefab(Transform moduleTransform)
    {
        if (moduleTransform != null) 
        {
            ModuleObject moForVehicle = moduleTransform.GetComponent<ModuleObject>();
            int randomIndex = Random.Range(0, _desiredAngles.Length - 1);
            int randomRotation = _desiredAngles[randomIndex];

            moForVehicle?.ControlVehicle(false);
            //moduleTransform.DOMove(new Vector3(moduleTransform.position.x, 1, moduleTransform.position.z), 1f).SetEase(Ease.Unset)
            //    .OnComplete(() =>
            //    {
            //        moduleTransform.DOMove(new Vector3(moduleTransform.position.x, 0, moduleTransform.position.z), 0.2f).SetEase(Ease.InBack);
            //    }); 
            moduleTransform.DORotate(new Vector3(0f, 0f, (float)randomRotation), 1f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutQuad).OnComplete(() => { moForVehicle?.ControlVehicle(true); }); 
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
            Transform rotatableTransform = _rotatableTransforms[i];
            ModuleObject moForVehicle = rotatableTransform.GetComponent<ModuleObject>();

            gap = _moduleAngles[i] - rotatableTransform.localEulerAngles.y;
            moForVehicle?.ControlVehicle(false);

            LiftAndLowerMOs();
            rotatableTransform.DORotate(new Vector3(0f, 0f, gap), 1f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutQuad).OnComplete(() => { moForVehicle?.ControlVehicle(true); });

            moduleObject = rotatableTransform.GetComponent<IModuleObject>();
            if (moduleObject != null)
            {
                moduleObject.UpdateMO_Angle(rotatableTransform);
            }
        }

        isMapSucceed = false;
        Invoke("EndMap", 1f);
    }

    void CheckInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    tapInputDownTime = Time.time;
                    break;
                case TouchPhase.Ended:
                    tapDuration = Mathf.Abs(tapInputDownTime - Time.time);

                    if (tapDuration < 0.2f)
                    {
                        Vector3 touchPosition = touch.position;
                        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit))
                        {
                            Transform moduleTransform = hit.transform;
                            if (moduleTransform != null && !isRotating)
                            {
                                AdaptGameMode(moduleTransform);
                            }
                        }
                    }
                    tapDuration = 0;
                    break;
                default:
                    break;
            }
        }
    }

    public void AdaptGameMode(Transform moduleTransform)
    {
        if (_rotatableTransforms.Contains(moduleTransform))
        {
            moduleObject = moduleTransform.GetComponent<IModuleObject>();
            if (moduleObject != null)
            {
                moduleTransform.GetComponent<ModuleObject>().RotateModule();
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
        nextMO = _candidateMOs[Random.Range(0, _candidateMOs.Count - 1)];
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
            isMapSucceed = true;
            EventManager.OnLevelSuccess.Invoke();
            EndMap();
            return;
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
        ResetData();
        LevelManager.Instance.FinishLevel();
    }
    private void ResetData()
    {

        _candidateMOs.Clear();
        _rotatableTransforms.Clear();
        lotTransforms.Clear();
        _moduleAngles.Clear();
    }
    public void ResetMapSuccess()
    {
        isMapSucceed = false;
    }

    int _row;
    int _col;
    private int _length;
    private int _width;
    [HideInInspector] public bool isFail;
    private void FindNeighbors(ModuleObject moduleObject)
    {
        _row = moduleObject.Row;
        _col = moduleObject.Column;

        // North
        if (_col > 0)
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
        if (_col < _length - 1)
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
        if (_row < _width - 1)
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
