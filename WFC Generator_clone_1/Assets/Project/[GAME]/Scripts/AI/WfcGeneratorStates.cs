using UnityEngine;

public abstract class WfcGeneratorStates : IStates<WfcGenerator>
{
    public abstract void EnterState(WfcGenerator fsm);
    public abstract void CreatePuzzle(WfcGenerator fsm);
    //public abstract void CollapseCell(WfcGenerator fsm);
    public abstract void ExitState(WfcGenerator fsm);

    public void OnNetworkSpawn(WfcGenerator fsm)
    {
        fsm?.OnNetworkSpawn();
    }

    public void OnDestroy(WfcGenerator fsm)
    {
        fsm?.OnDestroy();
    }
}
