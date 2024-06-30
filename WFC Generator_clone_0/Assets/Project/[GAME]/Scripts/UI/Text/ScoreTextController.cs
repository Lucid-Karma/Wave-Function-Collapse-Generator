using UnityEngine;
using TMPro;
using System.Collections;

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
        BuyButton.OnSolutionBuy.AddListener(() => StartCoroutine(DecreaseScoreCoroutine()));
        PlayerCoinController.OnBonusAdded.AddListener(UpdateScoreText);
    }

    private void OnDisable()
    {
        EventManager.OnLevelFinish.RemoveListener(UpdateScoreText);
        BuyButton.OnSolutionBuy.RemoveListener(() => StartCoroutine(DecreaseScoreCoroutine()));
        PlayerCoinController.OnBonusAdded.RemoveListener(UpdateScoreText);
    }

    private void Start()
    {
        point = PlayerCoinController.RewardAmount;
        ScoreText.text = point.ToString();
    }

    int point;
    private void UpdateScoreText()
    {
        StartCoroutine(IncreaseScoreCoroutine());
    }

    private float increaseSpeed = 0.07f;
    private IEnumerator IncreaseScoreCoroutine()
    {
        while (true)
        {
            if (point < PlayerCoinController.RewardAmount)
            {
                point++;
                ScoreText.text = point.ToString();
            }

            yield return new WaitForSeconds(increaseSpeed);
        }
    }

    private float decreaseSpeed = 0.02f;
    private IEnumerator DecreaseScoreCoroutine()
    {
        while (true)
        {
            if (point > PlayerCoinController.RewardAmount)
            {
                point--;
                ScoreText.text = point.ToString();
            }

            yield return new WaitForSeconds(decreaseSpeed);
        }
    }
}
