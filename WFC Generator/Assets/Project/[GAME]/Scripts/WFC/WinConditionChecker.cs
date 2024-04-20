using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinConditionChecker : MonoBehaviour
{
    WfcGenerator wfcGenerator;
    RotateCells rotateCells;

    List<CellSO> originalCells;

    private bool isGameWon = false;

    void Start()
    {
        wfcGenerator = GetComponent<WfcGenerator>();
        rotateCells = GetComponent<RotateCells>();

        // Subscribe to an event after cell rotations are complete (if available)
        // Alternatively, call CheckWinCondition after a delay
    }

    // void CheckWinCondition()
    // {
    //     // Get the original (unrotated) cell states
    //     originalCells.AddRange(wfcGenerator.cells);

    //     // Get the current cell states after rotations
    //     List<Transform> currentCellTransforms = rotateCells._grid.transform.GetComponentsInChildren<Transform>().ToList();
    //     List<CellSO> currentCells = currentCellTransforms.Select(GetCellSOFromTransform).ToList();

    //     // Check if any rotations broke connections or created intersections
    //     isGameWon = AreCellsValid(originalCells, currentCells);

    //     if (isGameWon)
    //     {
    //         // Display win message
    //         Debug.Log("You Win!");
    //         // Stop the game or perform other win actions
    //     }
    //     else
    //     {
    //         // Display instructions or hints (optional)
    //     }
    // }

    private bool AreCellsValid(List<CellSO> originalCells, List<CellSO> currentCells)
    {
        // Implement logic to check if current cells have valid connections and no intersections
        // This might involve iterating through cells and checking their modules/states
        // You'll need to adapt this logic based on your specific CellSO and ModuleSO definitions
        
        // Example (replace with your actual logic):
        for (int i = 0; i < originalCells.Count; i++)
        {
            CellSO originalCell = originalCells[i];
            CellSO currentCell = currentCells[i];

            // Check if current cell's modules violate connections or create intersections with neighbors
            // based on original cell information and current cell rotation
            // if ()
            // {
            //     return false;
            // }
        }

        return true; // All cells are valid
    }

    private CellSO GetCellSOFromTransform(Transform transform)
    {
        // Implement logic to find the CellSO associated with the given transform (assuming there's a way to identify it)
        // This might involve searching the WfcGenerator script's data structures
        return null; // Replace with actual implementation
    }
}

