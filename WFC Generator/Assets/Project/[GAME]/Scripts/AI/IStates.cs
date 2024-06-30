using UnityEngine;

public interface IStates<T> where T : MonoBehaviour
{
    void EnterState(T fsm);
    void OnNetworkSpawn(T fsm);
    void OnDestroy(T fsm);
    void CreatePuzzle(T fsm);
    void ExitState(T fsm);
}
