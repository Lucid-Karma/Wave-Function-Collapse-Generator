using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;

public class RotateCells : MonoBehaviour
{
    private List<ModuleObject> _rotatableModuleObjects = new();
    private List<ModuleObject> _candidateMOs = new();
    WfcGenerator generator;
    int cellCountToRotate;  // DifficultyManager
    int randomTIndex;
    private List<ModuleObject> MOs = new();
    private List<Transform> lotTransforms = new();
    private List<float> _moduleAngles = new();
    private List<Transform> _transforms = new();
    private int[] _desiredAngles = new int[3] {90, 180, 270};
    private bool isRotating = false;

    void Start()
    {
        generator = GetComponent<WfcGenerator>();

        Invoke("DrawCells", 3f);
    }

    void Update()
    {
        CheckInput();
    }

    void DrawCells()
    {
        _length = generator._length;
        _width = generator._width;
        MOs.AddRange(generator.moduleObjects);
        _candidateMOs.AddRange(MOs);
        _rotatableModuleObjects.AddRange(generator.rotatableModuleObjects);
        _transforms.AddRange(generator.rotatableObjectTs);
        for (int i = 0; i < _transforms.Count; i++)
        {
            _moduleAngles.Add(_transforms[i].rotation.eulerAngles.y);
            _rotatableModuleObjects[i].moduleTransform = _transforms[i];    // !!!!
        }
        lotTransforms.AddRange(_transforms);
        
        cellCountToRotate = 1;///*lotTransforms.Count;*/ lotTransforms.Count / 2;    //gonna be deleted..

        for (int i = 0; i < cellCountToRotate; i++)
        {
            randomTIndex = Random.Range(0, lotTransforms.Count - 1);
            RotatePrefab(lotTransforms[randomTIndex]);
            lotTransforms.RemoveAt(randomTIndex);
        }
    }

    void RotatePrefab(Transform moduleTransform)
    {
        if (moduleTransform != null) 
        {
            int randomIndex = Random.Range(0, _desiredAngles.Length - 1);
            int randomRotation = _desiredAngles[randomIndex];

            Vector3 currentRotation = moduleTransform.rotation.eulerAngles;
            moduleTransform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y + randomRotation, currentRotation.z);
            //moduleTransform.DORotate(new Vector3(0f, (float)randomRotation, 0f), 1f, RotateMode.LocalAxisAdd)
            //    .SetEase(Ease.OutQuad);
            for (int i = 0; i < randomIndex+1; i++)
            {
                UpdateMO_Angle(_transforms.IndexOf(moduleTransform));
            }
        }
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
        //         }
        //     }
        // }
        #endregion

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform moduleTransform = hit.transform;

                if (moduleTransform != null && !isRotating)
                {
                    if(_transforms.Contains(moduleTransform))
                    {
                        RotateModule(moduleTransform);
                        UpdateMO_Angle(_transforms.IndexOf(moduleTransform));
                        FindNeighbors(_rotatableModuleObjects[_transforms.IndexOf(moduleTransform)]);
                        //CollapseGrid();
                        // _candidateMOs.Clear();
                        // _candidateMOs.AddRange(MOs);
                        // isFail = false;
                    } 
                }
            }
        }
    }
    private void RotateModule(Transform modulePrefab)
    {
        isRotating = true;
        modulePrefab.DORotate(new Vector3(0f, 90f, 0f), 1f, RotateMode.LocalAxisAdd) 
            .SetEase(Ease.OutQuad) 
            .OnComplete(() => isRotating = false);
        //modulePrefab.Rotate(Vector3.up, 90f);
    }
    int _north, _south, _east, _west;
    private void UpdateMO_Angle(int index)
    {
        ModuleObject moduleObject = _rotatableModuleObjects[index];
        // Debug.Log("default: " + moduleObject.moduleTransform.name +
        //     "\nnorth: " + moduleObject.north+
        //     "\nsouth: " + moduleObject.south+
        //     "\neast: " + moduleObject.east+
        //     "\nwest: " + moduleObject.west);
        _north = moduleObject.north;
        _south = moduleObject.south;
        _east = moduleObject.east;
        _west = moduleObject.west;
        moduleObject.north = _west;
        moduleObject.south = _east;
        moduleObject.east = _north;
        moduleObject.west = _south;
        // Debug.Log("current: " + moduleObject.moduleTransform.name +
        //     "\nnorth: " + moduleObject.north+
        //     "\nsouth: " + moduleObject.south+
        //     "\neast: " + moduleObject.east+
        //     "\nwest: " + moduleObject.west);
    }

    private void CollapseMO()
    {
        ModuleObject nextMO;
        nextMO = _candidateMOs[Random.Range(0, _candidateMOs.Count-1)];
        nextMO.isChecked = true;
        _candidateMOs.Remove(nextMO);

        FindNeighbors(nextMO);
    }
    [HideInInspector] public static UnityEvent OnGridCollapse = new();
    private void CollapseGrid()
    {
        OnGridCollapse.Invoke();
        Debug.Log("enter collapse grid");

        if (isFail) return;
        while (MOs.Where(x => !x.isChecked).Any())
        {
            if (_candidateMOs.Count > 0)
            {
                CollapseMO();
            }
            else
            {
                break;
            }
            if (isFail) 
            {
                Debug.Log("failed");
                break;
            }
        }

        if (!isFail)
        {
            Debug.Log("WIN !!!");
        }
        Debug.Log("exit collapse grid"); 
    }
    int _row;
    int _col;
    private int _length;
    private int _width;
    private bool isFail;
    private void FindNeighbors(ModuleObject currentModuleObject)
    {
        _row = currentModuleObject.Row;
        _col = currentModuleObject.Column;
        Debug.Log("satir: " + _row + " sütun: " + _col);

        // North
        if (_col > 0 )
        {
            ModuleObject northNeighbor = MOs.Find(c => c.Column == _col - 1 && c.Row == _row && !c.isChecked);
            if (northNeighbor != null)
            {
                Debug.Log("North N: " + northNeighbor.moduleTransform.name +
            "\nnorth: " + northNeighbor.north+
            "\nsouth: " + northNeighbor.south+
            "\neast: " + northNeighbor.east+
            "\nwest: " + northNeighbor.west);
                if (!IsMatching(0, northNeighbor, currentModuleObject))
                {
                    Debug.Log("false");
                    isFail = true;
                    return;
                }
            }
        }

        // South
        if (_col < _length - 1 )
        {
            ModuleObject southNeighbor = MOs.Find(c => c.Column == _col + 1 && c.Row == _row && !c.isChecked);
            if (southNeighbor != null)
            {
                Debug.Log("South N: " + southNeighbor.moduleTransform.name +
            "\nnorth: " + southNeighbor.north+
            "\nsouth: " + southNeighbor.south+
            "\neast: " + southNeighbor.east+
            "\nwest: " + southNeighbor.west);
                if (!IsMatching(1, southNeighbor, currentModuleObject))
                {
                    Debug.Log("false");
                    isFail = true;
                    return;
                }
            }
        }

        // East
        if (_row < _width - 1 )
        {
            ModuleObject eastNeighbor = MOs.Find(c => c.Column == _col && c.Row == _row + 1 && !c.isChecked);
            if (eastNeighbor != null)
            {
                Debug.Log("East N: " + eastNeighbor.moduleTransform.name +
            "\nnorth: " + eastNeighbor.north+
            "\nsouth: " + eastNeighbor.south+
            "\neast: " + eastNeighbor.east+
            "\nwest: " + eastNeighbor.west);
                if (!IsMatching(2, eastNeighbor, currentModuleObject))
                {
                    Debug.Log("false");
                    isFail = true;
                    return;
                }
            }
        }

        // West
        if (_row > 0)
        {
            ModuleObject westNeighbor = MOs.Find(c => c.Column == _col && c.Row == _row - 1 && !c.isChecked);
            if (westNeighbor != null)
            {
                Debug.Log("West N: " + westNeighbor.moduleTransform.name +
            "\nnorth: " + westNeighbor.north+
            "\nsouth: " + westNeighbor.south+
            "\neast: " + westNeighbor.east+
            "\nwest: " + westNeighbor.west);
                if (!IsMatching(3, westNeighbor, currentModuleObject))
                {
                    Debug.Log("false");
                    isFail = true;
                    return;
                }
            }
        }

        isFail = false;
    }

    private bool IsMatching(int direction, ModuleObject neighborModule, ModuleObject currentModuleObject)
    {
        if (direction == 0) // North
            return neighborModule.south == currentModuleObject.north;

        if (direction == 1) // South
            return neighborModule.north == currentModuleObject.south;

        if (direction == 2) // East
            return neighborModule.west == currentModuleObject.east;

        if (direction == 3) // West
            return neighborModule.east == currentModuleObject.west;

        return false;
    }
}
