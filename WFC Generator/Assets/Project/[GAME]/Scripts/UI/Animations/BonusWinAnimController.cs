using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusWinAnimController : MonoBehaviour
{
    private Animator bonusAnimator;
    private void OnEnable()
    {
        bonusAnimator = GetComponent<Animator>();

        LevelPanels.OnBonusShowedUp += StartBonusPopUp;
        CollectButton.OnCoinCollect.AddListener(() => bonusAnimator.SetTrigger("close"));
    }
    private void OnDisable()
    {
        LevelPanels.OnBonusShowedUp -= StartBonusPopUp;
        CollectButton.OnCoinCollect.RemoveListener(() => bonusAnimator.SetTrigger("close"));
    }

    private void StartBonusPopUp()
    {
        bonusAnimator.SetTrigger("popUp");
    }
    public void InitializeNewLevel()
    {
        EventManager.OnLvlEndPanelFinish.Invoke();
    }
}
