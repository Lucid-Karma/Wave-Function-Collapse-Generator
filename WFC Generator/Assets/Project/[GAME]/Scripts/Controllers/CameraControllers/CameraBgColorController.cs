using UnityEngine;

public class CameraBgColorController : MonoBehaviour
{
    Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>(); 
    }

    void OnEnable()
    {
        EventManager.OnLevelStart.AddListener(UpdateCameraColor);
    }
    void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(UpdateCameraColor);
    }

    private void UpdateCameraColor()
    {
        _camera.backgroundColor = LevelManager.Instance.CurrentThemeData.ThemeColor;
    }
}
