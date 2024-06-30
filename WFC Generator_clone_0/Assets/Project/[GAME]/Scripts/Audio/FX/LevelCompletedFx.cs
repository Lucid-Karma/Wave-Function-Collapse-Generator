using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompletedFx : MonoBehaviour
{
    private AudioSource levelCompletedFx;

    void Start()
    {
        levelCompletedFx = gameObject.GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        LevelPanels.OnLevelCShowed += (() => levelCompletedFx.Play());
    }
    void OnDisable()
    {
        LevelPanels.OnLevelCShowed -= (() => levelCompletedFx.Play());
    }
}
