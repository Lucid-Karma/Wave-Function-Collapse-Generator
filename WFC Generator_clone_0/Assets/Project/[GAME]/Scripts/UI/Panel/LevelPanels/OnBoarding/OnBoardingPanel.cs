using System.Collections;
using UnityEngine;

public class OnBoardingPanel : MonoBehaviour
{
    public GameObject TapInstruction;
    public GameObject CameraInstruction;

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("LoginCount") > 1) return;

        EventManager.OnGameStart.AddListener(() => StartCoroutine(ShowTapInstructions()));
        EventManager.OnLevelStart.AddListener(() => StartCoroutine(ShowCamInstructions()));
        EventManager.OnLevelSuccess.AddListener(DestroyPanel);
        EventManager.OnLevelFinish.AddListener(DeactivateInst);
    }
    private void OnDisable()
    {
        if (PlayerPrefs.GetInt("LoginCount") > 1) return;

        EventManager.OnGameStart.RemoveListener(() => StartCoroutine(ShowTapInstructions()));
        EventManager.OnLevelStart.RemoveListener(() => StartCoroutine(ShowCamInstructions()));
        EventManager.OnLevelSuccess.RemoveListener(DestroyPanel);
        EventManager.OnLevelFinish.RemoveListener(DeactivateInst);
    }

    private void Start()
    {
        DeactivateInst();
    }

    private IEnumerator ShowTapInstructions()
    {
        yield return new WaitForSeconds(3.2f);

        TapInstruction.SetActive(true);
    }

    private IEnumerator ShowCamInstructions()
    {
        if (LevelManager.Instance.LevelIndex == 1)
        {
            yield return new WaitForSeconds(3.2f);

            CameraInstruction.SetActive(true);
        } 
    }

    private void DeactivateInst()
    {
        TapInstruction.SetActive(false);
        CameraInstruction.SetActive(false);
    }

    private void DestroyPanel()
    {
        if(LevelManager.Instance.LevelIndex == 2)
            Destroy(gameObject);
    }
}
