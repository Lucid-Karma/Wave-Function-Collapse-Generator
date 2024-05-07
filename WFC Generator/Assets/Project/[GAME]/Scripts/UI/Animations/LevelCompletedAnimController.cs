using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompletedAnimController : MonoBehaviour
{
    private Animator levelCompletedAnimator;

    private void OnEnable()
    {
        levelCompletedAnimator = GetComponent<Animator>();

        LevelPanels.OnLevelCShowed += StartLevelCompleted;
    }
    private void OnDisable()
    {
        LevelPanels.OnLevelCShowed -= StartLevelCompleted;
    }

    private void StartLevelCompleted()
    {
        if (levelCompletedAnimator != null)
            levelCompletedAnimator.SetTrigger("popUp");
    }

    public void InitializeNewLevel()
    {
        EventManager.OnLevelInitialize.Invoke();
    }
}
