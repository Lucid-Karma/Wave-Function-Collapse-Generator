using System.Collections;
using UnityEngine;

public class OnBoardingPanel : MonoBehaviour
{
    public GameObject TapInstruction;
    public GameObject CameraInstruction;
    public GameObject ChallengeButton;

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("LevelCount") > 2) return;

        EventManager.OnGameStart.AddListener(() => StartCoroutine(ShowTapInstructions()));
        EventManager.OnLevelStart.AddListener(() => StartCoroutine(ShowCamInstructions()));
        EventManager.OnLevelSuccess.AddListener(DestroyPanel);
        EventManager.OnLevelFinish.AddListener(DeactivateInst);
        StartMatchmakingButton.OnMatchmakingRequest += DeactivateInst;
    }
    private void OnDisable()
    {
        if (PlayerPrefs.GetInt("LevelCount") > 2) return;

        EventManager.OnGameStart.RemoveListener(() => StartCoroutine(ShowTapInstructions()));
        EventManager.OnLevelStart.RemoveListener(() => StartCoroutine(ShowCamInstructions()));
        EventManager.OnLevelSuccess.RemoveListener(DestroyPanel);
        EventManager.OnLevelFinish.RemoveListener(DeactivateInst);
        StartMatchmakingButton.OnMatchmakingRequest -= DeactivateInst;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("LevelCount") > 2) Destroy(gameObject);

        DeactivateInst();

        if (PlayerPrefs.GetInt("LevelCount") <= 2)
            ChallengeButton.SetActive(false);
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

            TapInstruction.SetActive(false);
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
        if(LevelManager.Instance.LevelIndex == 1)
        {
            ChallengeButton.SetActive(true);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            DeactivateInst();
    }
}
