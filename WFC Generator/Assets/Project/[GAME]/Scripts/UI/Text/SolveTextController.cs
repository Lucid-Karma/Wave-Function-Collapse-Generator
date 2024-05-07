using TMPro;
using UnityEngine;

public class SolveTextController : MonoBehaviour
{
    private TextMeshProUGUI solutionText;
    public TextMeshProUGUI SolutionText
    {
        get
        {
            if (solutionText == null)
                solutionText = GetComponent<TextMeshProUGUI>();

            return solutionText;
        }
    }

    public static int SolveCount
    {
        get
        {
            return PlayerPrefs.GetInt("SolveCount", 0);
        }
        set
        {
            if(value < 0)
                value = 0;

            PlayerPrefs.SetInt("SolveCount", value);
        }
    }

    private void OnEnable()
    {
        BuyButton.OnSolutionBuy.AddListener(IncreaseAndUpdateSolutionText);
        SolveButton.OnSolveBtnUse.AddListener(DecreaseAndUpdateSolutionText);
    }

    private void OnDisable()
    {
        BuyButton.OnSolutionBuy.RemoveListener(IncreaseAndUpdateSolutionText);
        SolveButton.OnSolveBtnUse.RemoveListener(DecreaseAndUpdateSolutionText);
    }

    private void Start()
    {
        SolutionText.text = SolveCount.ToString();
    }

    private void IncreaseAndUpdateSolutionText()
    {
        SolveCount++;
        PlayerPrefs.SetInt("SolveCount", SolveCount);
        SolutionText.text = SolveCount.ToString();
    }

    private void DecreaseAndUpdateSolutionText()
    {
        SolveCount --;
        PlayerPrefs.SetInt("SolveCount", SolveCount);
        SolutionText.text = SolveCount.ToString();
    }
}
