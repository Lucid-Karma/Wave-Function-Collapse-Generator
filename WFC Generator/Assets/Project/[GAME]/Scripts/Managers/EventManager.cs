using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent OnGameStart = new();
    public static UnityEvent OnGameEnd = new();

    public static UnityEvent OnLevelInitialize = new();
    public static UnityEvent OnLevelStart = new();
    public static UnityEvent OnLevelContine = new();
    public static UnityEvent OnLevelFinish = new();

    public static UnityEvent OnLevelSuccess = new();
    public static UnityEvent OnLevelFail = new();

    public static UnityEvent OnRestart = new();

    public static UnityEvent OnMusicOn = new();
    public static UnityEvent OnMusicOff = new();

    public static UnityEvent OnClick = new();
    public static UnityEvent OnButtonClick = new();
}
