using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SuccessAnimController : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnSuccessWent = new();

    private Animator succesAnimator;

    private void OnEnable()
    {
        succesAnimator = GetComponent<Animator>();

        LevelPanels.OnSuccessTxtCome.AddListener(() => succesAnimator.SetTrigger("come"));
    }
    private void OnDisable()
    {
        LevelPanels.OnSuccessTxtCome.RemoveListener(() => succesAnimator.SetTrigger("come"));
    }

    public void EndSuccessGo()
    {
        OnSuccessWent.Invoke();
    }
}
