using TMPro;
using UnityEngine;

public class BrokenRoadTextController : MonoBehaviour
{
    private TextMeshProUGUI brknRoadText;
    public TextMeshProUGUI BrknRoadText
    {
        get
        {
            if (brknRoadText == null)
                brknRoadText = GetComponent<TextMeshProUGUI>();

            return brknRoadText;
        }
    }

    private void OnEnable()
    {
        CharacterBase.OnSingleModuleMove.AddListener(UpdateSolutionText);
        LevelSolve.OnSolveBtnUse.AddListener(ResetBrknRoadCount);
    }

    private void OnDisable()
    {
        CharacterBase.OnSingleModuleMove.RemoveListener(UpdateSolutionText);
        LevelSolve.OnSolveBtnUse.RemoveListener(ResetBrknRoadCount);
    }

    private void Start()
    {
        ResetBrknRoadCount();
    }

    private void UpdateSolutionText(int brokenRoadCount)
    {
        BrknRoadText.text = brokenRoadCount.ToString();
    }

    private void ResetBrknRoadCount()
    {
        BrknRoadText.text = 0.ToString();
    }
}
