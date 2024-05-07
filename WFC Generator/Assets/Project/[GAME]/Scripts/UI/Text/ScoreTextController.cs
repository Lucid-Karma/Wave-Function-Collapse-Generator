using UnityEngine;
using TMPro;

public class ScoreTextController : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    public TextMeshProUGUI ScoreText
    {
        get
        {
            if(scoreText == null)
            scoreText = GetComponent<TextMeshProUGUI>();

            return scoreText;
        }
    }

    private void OnEnable()
    {
        EventManager.OnLevelFinish.AddListener(UpdateScoreText);
        BuyButton.OnSolutionBuy.AddListener(UpdateScoreText);
    }

    private void OnDisable()
    {
        EventManager.OnLevelFinish.RemoveListener(UpdateScoreText);
        BuyButton.OnSolutionBuy.RemoveListener(UpdateScoreText);
    }

    private void Start()
    {
        UpdateScoreText();
    }

    int point;
    private void UpdateScoreText()
    {
        point = PlayerCoinController.RewardAmount;
        ScoreText.text = point.ToString();
    }
}
