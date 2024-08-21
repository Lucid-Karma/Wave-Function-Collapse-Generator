using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : Button
{
    private static bool isMusicOn = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(MusicOnOff);
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        onClick.RemoveListener(MusicOnOff);
    }

    private void MusicOnOff()
    {
        EventManager.OnButtonClick.Invoke();
        if (isMusicOn)
        {
            EventManager.OnMusicOff.Invoke();
            isMusicOn = false;
        }
        else
        {
            EventManager.OnMusicOn.Invoke();
            isMusicOn = true;
        }
    }
}
