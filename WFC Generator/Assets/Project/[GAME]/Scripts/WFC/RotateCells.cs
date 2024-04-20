using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateCells : MonoBehaviour
{
    public GameObject _grid;
    private List<Transform> _moduleTransforms = new();
    WfcGenerator generator;
    int cellCountToRotate;  // DifficultyManager
    int randomTIndex;
    //private List<CellSO> cells = new();
    private List<Transform> lotTransforms = new();
    private List<float> transforms = new();
    private int[] angles = new int[3] {90, 180, 270};
    private bool isRotating = false;

    void Start()
    {
        generator = GetComponent<WfcGenerator>();
        //_grid = generator.gridHolder;

        Invoke("DrawCells", 3f);
    }

    void Update()
    {
        CheckInput();
    }

    void DrawCells()
    {
        foreach (Transform moduleTransform in generator.rotatableObjectTs)
        {
            _moduleTransforms.Add(moduleTransform);
        }
        //cells.AddRange(generator.cells);
        lotTransforms.AddRange(_moduleTransforms);
        for (int i = 0; i < _moduleTransforms.Count; i++)
        {
            transforms.Add(_moduleTransforms[i].rotation.eulerAngles.y);
        }

        cellCountToRotate = 3;///*lotTransforms.Count;*/ lotTransforms.Count / 2;    //gonna be deleted..

        for (int i = 0; i < cellCountToRotate; i++)
        {
            randomTIndex = Random.Range(0, lotTransforms.Count);
            RotatePrefab(_moduleTransforms[randomTIndex]);
            lotTransforms.RemoveAt(randomTIndex);
        }
    }

    void RotatePrefab(Transform moduleTransform)
    {
        if (moduleTransform != null) 
        {
            int randomIndex = Random.Range(0, angles.Length);
            int randomRotation = angles[randomIndex];

            //Debug.Log(randomRotation);
            Vector3 currentRotation = moduleTransform.rotation.eulerAngles;
            moduleTransform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y + randomRotation, currentRotation.z);
            //moduleTransform.DORotate(new Vector3(0f, (float)randomRotation, 0f), 1f, RotateMode.LocalAxisAdd)
            //    .SetEase(Ease.OutQuad);
        }
        else
            Debug.Log("moduleTransform is null");
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
                Transform moduleTransform = hit.transform.GetComponent<Transform>();

                if (moduleTransform != null && !isRotating)
                {
                    if(_moduleTransforms.Contains(moduleTransform))
                        RotateModule(moduleTransform);
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
        
        //CheckGrid();
    }

    void CheckGrid()
    {
        for (int i = 0; i < _grid.transform.childCount; i++)
        {
            if(_grid.transform.GetChild(i).rotation.eulerAngles.y == transforms[i])
            {
                //Debug.Log("_grid: " + _grid.transform.GetChild(i).rotation.eulerAngles.y + " transforms: " + transforms[i]);
                continue;
            }
            else
            {
                Debug.Log("Incorrect");
                return;
            }
        }

        Debug.Log("WIN");
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
}
