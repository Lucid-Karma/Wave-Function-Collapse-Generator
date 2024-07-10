using UnityEngine;

public class GameModeManager : Singleton<GameModeManager>
{
    public enum GameMode { SinglePlayer, Multiplayer }

    public GameMode CurrentGameMode { get; private set; }

    [HideInInspector] public bool IsMultiplayer { get; private set; }

    private void OnEnable()
    {
        EventManager.OnGameStart.AddListener(StartSinglePlayer);
        //GameManager.OnMultiplayerGameStart.AddListener(StartMultiplayer);
    }
    private void OnDisable()
    {
        EventManager.OnGameStart.RemoveListener(StartSinglePlayer);
        //GameManager.OnMultiplayerGameStart.RemoveListener(StartMultiplayer);
    }

    public void StartSinglePlayer()
    {
        CurrentGameMode = GameMode.SinglePlayer;

        IsMultiplayer = false;
    }

    public void StartMultiplayer()
    {
        CurrentGameMode = GameMode.Multiplayer;
        GameManager.OnMultiplayerGameStart.Invoke();    //?????

        IsMultiplayer = true;
    }
}
