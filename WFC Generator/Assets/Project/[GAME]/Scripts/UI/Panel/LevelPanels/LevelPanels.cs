using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelPanels : Panel
{
    [HideInInspector] public static UnityEvent OnSuccessTxtCome = new();

    public Panel BonusWinPanel;
    public Panel LevelSuccessPanel;
    public Panel LevelCompletedPanel;
    public Panel BonusLevelPanel;
    public Panel NextLvlPanel;
    public GameObject ChibiInformative;
    public ParticleSystem confetti;

    private void OnEnable()
    {
        EventManager.OnLvlEndPanelFinish.AddListener(InitializeNextLevelPanel);  
        EventManager.OnLevelFinish.AddListener(() => StartCoroutine(InitializeLevelSuccessPanel()));
        SuccessAnimController.OnSuccessWent.AddListener(InitializeLevelCompletedPanel);
        NextLevelButton.OnBonusLevel.AddListener(InitializeBonusLevelPanel);
        CharacterBase.OnFineModuleClick.AddListener(() => StartCoroutine(ShowChibiInformative()));
    }

    private void OnDisable()
    {
        EventManager.OnLvlEndPanelFinish.RemoveListener(InitializeNextLevelPanel);
        EventManager.OnLevelFinish.RemoveListener(() => StartCoroutine(InitializeLevelSuccessPanel()));
        SuccessAnimController.OnSuccessWent.RemoveListener(InitializeLevelCompletedPanel);
        NextLevelButton.OnBonusLevel.RemoveListener(InitializeBonusLevelPanel);
        CharacterBase.OnFineModuleClick.RemoveListener(() => StartCoroutine(ShowChibiInformative()));
    }

    private void Start()
    {
        HideLevelPanels();
        NextLvlPanel.HidePanel();
        ChibiInformative.SetActive(false);
    }

    private IEnumerator InitializeLevelSuccessPanel()
    {
        yield return new WaitForSeconds(1f);

        if (CharacterBase.Instance.isMapSucceed)
        {
            if (!IsBonusLevel())
            {
                LevelSuccessPanel.ShowPanel();
                OnSuccessTxtCome.Invoke();
            }
            else
            {
                InitializeBonusWinPanel();
            }
        }
        else
            InitializeLevelCompletedPanel();

        CharacterBase.Instance.ResetMapSuccess();
    }

    [HideInInspector] public static Action OnBonusShowedUp;

    [HideInInspector] public static Action OnLevelCShowed;
    private void InitializeLevelCompletedPanel()
    {
        confetti.Play();

        LevelSuccessPanel.HidePanel();
        LevelCompletedPanel.ShowPanel();
        OnLevelCShowed.Invoke();
    }

    private void InitializeBonusWinPanel()
    {
        BonusLevelPanel.HidePanel();
        BonusWinPanel.ShowPanel();
        OnBonusShowedUp.Invoke();
    }

    private void InitializeBonusLevelPanel()
    {
        BonusLevelPanel.ShowPanel();
    }

    private void InitializeNextLevelPanel()
    {
        HideLevelPanels();
        NextLvlPanel.ShowPanel();
    }

    private void HideLevelPanels()
    {
        BonusWinPanel.HidePanel();
        LevelSuccessPanel.HidePanel();
        LevelCompletedPanel.HidePanel();
        BonusLevelPanel.HidePanel();
    }

    private bool IsBonusLevel()
    {
        if (LevelManager.Instance.LevelIndex == 0)  // since the panels are initailized after bonus level ended, the index turns to 0 again.
            return true;

        return false;
    }

    private IEnumerator ShowChibiInformative()
    {
        ChibiInformative.SetActive(true);

        yield return new WaitForSeconds(0.8f);

        ChibiInformative.SetActive(false);
    }
}
