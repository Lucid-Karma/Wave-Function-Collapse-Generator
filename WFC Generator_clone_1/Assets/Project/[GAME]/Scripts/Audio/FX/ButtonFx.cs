using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFx : MonoBehaviour
{
    private AudioSource buttonFx;

    void Start()
    {
        buttonFx = gameObject.GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        EventManager.OnButtonClick.AddListener(() => buttonFx.Play());
    }
    void OnDisable()
    {
        EventManager.OnButtonClick.RemoveListener(() => buttonFx.Play());
    }
}
