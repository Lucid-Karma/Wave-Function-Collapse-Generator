using DG.Tweening;
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
        originalScale = transform.localScale;
        base.OnEnable();
        onClick.AddListener(InitializeNextLevel);
        EventManager.OnLvlEndPanelFinish.AddListener(StartPulseAnimation);
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        onClick.RemoveListener(InitializeNextLevel);
        EventManager.OnLvlEndPanelFinish.RemoveListener(StartPulseAnimation);
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
        DisableAnimation();
    }

    [SerializeField] private float scaleMultiplier = 1.2f; // Büyüme oraný (örneðin 1.2 = %20 büyüt)
    [SerializeField] private float pulseDuration = 1f;    // Animasyon süresi (saniye)
    private Vector3 originalScale;
    void StartPulseAnimation()
    {
        // Sonsuz döngüde büyüyüp küçülme animasyonu
        transform.DOScale(originalScale * scaleMultiplier, pulseDuration)
                 .SetLoops(-1, LoopType.Yoyo) // -1 = sonsuz döngü, Yoyo = büyüyüp küçülme
                 .SetEase(Ease.InOutSine);   // Yumuþak geçiþ efekti
    }
    private void DisableAnimation()
    {
        transform.DOKill(); // Tween'i durdur
        transform.localScale = originalScale; // Orijinal boyutuna dön
    }
}
