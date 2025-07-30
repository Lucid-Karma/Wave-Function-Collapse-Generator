using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FxMuteButton : MonoBehaviour
{
    private static bool isFxOn;

    public static bool IsFxOn { get => isFxOn; private set => isFxOn = value; }

    private Image btnImg;
    [SerializeField] private Sprite SoundOn;
    [SerializeField] private Sprite SoundOff;

    [SerializeField] private List<AudioSource> FxObjects = new();

    private void OnEnable()
    {
        btnImg = GetComponent<Image>();
        btnImg.sprite = SoundOn;
    }

    private void Start()
    {
        IsFxOn = true;
    }

    public void MusicOnOff()
    {
        EventManager.OnButtonClick.Invoke();
        if (IsFxOn)
        {
            for (int i = 0; i < FxObjects.Count; i++)
            {
                FxObjects[i].mute = true;
            }
            IsFxOn = false;
            btnImg.sprite = SoundOff;
        }
        else
        {
            for (int i = 0; i < FxObjects.Count; i++)
            {
                FxObjects[i].mute = false;
            }
            IsFxOn = true;
            btnImg.sprite = SoundOn;
        }
    }
}
