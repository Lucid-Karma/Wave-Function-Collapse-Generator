using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : Button
{
    private static bool isMusicOn;

    public static bool IsMusicOn { get => isMusicOn; private set => isMusicOn = value; }

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

    protected override void Start()
    {
        IsMusicOn = true;
    }

    private void MusicOnOff()
    {
        EventManager.OnButtonClick.Invoke();
        if (IsMusicOn)
        {
            EventManager.OnMusicOff.Invoke();
            IsMusicOn = false;
        }
        else
        {
            EventManager.OnMusicOn.Invoke();
            IsMusicOn = true;
        }
    }
}
