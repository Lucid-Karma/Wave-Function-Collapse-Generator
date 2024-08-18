using UnityEngine;

public class CameraBgColorController : MonoBehaviour
{
    Camera _camera;
    [SerializeField] private Color _multiplayerColor;

    void Awake()
    {
        _camera = GetComponent<Camera>(); 
    }

    void OnEnable()
    {
        EventManager.OnLevelStart.AddListener(UpdateCameraColor);
        GameManager.OnMultiplayerGameStart.AddListener(() => _camera.backgroundColor = _multiplayerColor);
    }
    void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(UpdateCameraColor);
        GameManager.OnMultiplayerGameStart.RemoveListener(() => _camera.backgroundColor = _multiplayerColor);
    }

    private void UpdateCameraColor()
    {
        _camera.backgroundColor = LevelManager.Instance.CurrentThemeData.roadColor;
    }
}
