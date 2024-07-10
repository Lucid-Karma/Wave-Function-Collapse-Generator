using UnityEngine;

public class SingleplayerPuzzleGenerator : WfcGeneratorStates
{
    public override void CreatePuzzle(WfcGenerator fsm)
    {
        Debug.Log("single player..");
        GameModeManager.Instance.StartSinglePlayer();
        fsm.RecreateLevel();

        LevelManager.Instance.StartLevel();
    }

    public override void EnterState(WfcGenerator fsm)
    {
        Debug.Log("Entered Singleplayer");
    }

    public override void ExitState(WfcGenerator fsm)
    {
        throw new System.NotImplementedException();
    }
}
