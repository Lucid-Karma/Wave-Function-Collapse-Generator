
using UnityEngine;

public interface IPlayer 
{
    void GenerateMap();
    void DoMove(Transform moduleTransform);
    void CollapseGrid();
}
