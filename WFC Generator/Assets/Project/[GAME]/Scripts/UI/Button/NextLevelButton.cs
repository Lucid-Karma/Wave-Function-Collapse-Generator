using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NextLevelButton : Button
{
    [HideInInspector] public static UnityEvent OnBonusLevel = new();
    private Panel parentPanel;
    private int bonusLevelIndex;

    protected override void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(InitializeNextLevel);
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        onClick.RemoveListener(InitializeNextLevel);
    }

    protected override void Start()
    {
        bonusLevelIndex = LevelManager.Instance.LevelData.Levels.Count - 2;
        parentPanel = GetComponentInParent<Panel>();
    }

    public void InitializeNextLevel()
    {
        EventManager.OnLevelInitialize.Invoke();
        EventManager.OnButtonClick.Invoke();

        if (LevelManager.Instance.LevelIndex >= bonusLevelIndex)
        {
            OnBonusLevel.Invoke();
        }

        parentPanel.HidePanel();
    }
}
