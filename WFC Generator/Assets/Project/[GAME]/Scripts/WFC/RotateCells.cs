using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;

public class RotateCells : MonoBehaviour
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
    private bool isRotating = false;

    [HideInInspector] public static UnityEvent OnGridCollapse = new();

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
        _candidateMOs.AddRange(generator.moduleObjects);
        _rotatableTransforms.AddRange(generator.rotatableObjectTs);
        for (int i = 0; i < _rotatableTransforms.Count; i++)
        {
            _moduleAngles.Add(_rotatableTransforms[i].rotation.eulerAngles.y);
        }
        lotTransforms.AddRange(_rotatableTransforms);
        
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
                moduleObject = moduleTransform.GetComponent<IModuleObject>();
                if (moduleObject != null)
                {
                    moduleObject.UpdateMO_Angle(moduleTransform);
                }
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
                    if(_rotatableTransforms.Contains(moduleTransform))
                    {
                        moduleObject = moduleTransform.GetComponent<IModuleObject>();
                        if (moduleObject != null)
                        {
                            RotateModule(moduleTransform);
                            moduleObject.UpdateMO_Angle(moduleTransform);
                            CollapseGrid();
                            _candidateMOs.Clear();
                            _candidateMOs.AddRange(generator.moduleObjects);
                            isFail = false;
                        }  
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
