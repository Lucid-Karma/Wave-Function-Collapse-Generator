using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    private static bool isMusicOn;

    public static bool IsMusicOn { get => isMusicOn; private set => isMusicOn = value; }

    private Image btnImg;
    [SerializeField] private Sprite SoundOn;
    [SerializeField] private Sprite SoundOff;

    private void OnEnable()
    {
        btnImg = GetComponent<Image>();
        btnImg.sprite = SoundOn;

        //base.OnEnable();
        //onClick.AddListener(MusicOnOff);
    }

    //protected override void OnDisable()
    //{
    //    base.OnEnable();
    //    onClick.RemoveListener(MusicOnOff);
    //}

    private void Start()
    {
        IsMusicOn = true;
    }

    public void MusicOnOff()
    {
        EventManager.OnButtonClick.Invoke();
        if (IsMusicOn)
        {
            EventManager.OnMusicOff.Invoke();
            IsMusicOn = false;
            btnImg.sprite = SoundOff;
        }
        else
        {
            EventManager.OnMusicOn.Invoke();
            IsMusicOn = true;
            btnImg.sprite = SoundOn;
        }
    }
}
