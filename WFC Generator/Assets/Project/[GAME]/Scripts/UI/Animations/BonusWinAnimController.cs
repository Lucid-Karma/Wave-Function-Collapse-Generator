using DG.Tweening;
using UnityEngine;

public class BonusWinAnimController : MonoBehaviour
{
    private void OnEnable()
    {
        LevelPanels.OnBonusShowedUp += StartBonusPopUp;
    }
    private void OnDisable()
    {
        LevelPanels.OnBonusShowedUp -= StartBonusPopUp;
    }

    public RectTransform amazingText;
    public float showDuration = 1.5f;

    void Start()
    {
        amazingText.localScale = Vector3.zero;
        amazingText.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void StartBonusPopUp()
    {
        Sequence seq = DOTween.Sequence();

        // Ba�lang�� g�r�n�rl��� ve pozisyonu
        amazingText.localScale = Vector3.zero;
        amazingText.anchoredPosition = Vector2.zero;
        CanvasGroup canvasGroup = amazingText.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        // Animasyon s�ras�
        seq.Append(canvasGroup.DOFade(1, 0.2f)); // Fade in
        seq.Join(amazingText.DOScale(1.2f, 0.4f).SetEase(Ease.OutBack)); // Pop effect
        seq.Append(amazingText.DOPunchScale(Vector3.one * 0.1f, 0.3f, 10, 1)); // Hafif z�plama
        seq.AppendInterval(1f); // 1 saniye bekle
        seq.Append(amazingText.DOAnchorPosY(200f, 0.5f).SetEase(Ease.InSine)); // Yukar� do�ru kay
        seq.Join(canvasGroup.DOFade(0, 0.5f)); // Fade out
        seq.OnComplete(() =>
        {
            // �ste�e ba�l�: Reset veya disable
            amazingText.localScale = Vector3.zero;
            amazingText.anchoredPosition = Vector2.zero;

            InitializeNewLevel();
        });
    }
    public void InitializeNewLevel()
    {
        EventManager.OnLvlEndPanelFinish.Invoke();
    }
}
