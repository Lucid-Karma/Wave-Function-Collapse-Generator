using Unity.Netcode;
using UnityEngine;

public abstract class FSMBase<T> : NetworkBehaviour where T : FSMBase<T>
{
    protected IStates<T> currentState;

    protected void StartState(IStates<T> starterState)
    {
        currentState = starterState;
        currentState.EnterState((T)this);
    }

    protected void CreatePuzzle(IStates<T> _currentState)
    {
        currentState = _currentState;
        currentState.CreatePuzzle((T)this);
    }

    public void SwitchState(IStates<T> nextState)
    {
        currentState = nextState;
        currentState.EnterState((T)this);
    }
}
