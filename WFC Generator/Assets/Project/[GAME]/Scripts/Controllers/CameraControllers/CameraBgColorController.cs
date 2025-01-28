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
        //_camera.backgroundColor = LevelManager.Instance.CurrentThemeData.roadColor;
        _camera.backgroundColor = ThemeManager.Instance.CameraBgColor;
        //_camera.backgroundColor = GetToonColor(ThemeManager.Instance.GrassColor);
    }

    public Color GetToonColor(Color inputColor)
    {
        // Toon seviyeleri için eþik deðerler
        float[] thresholds = { 0.25f, 0.5f, 0.75f, 1.0f };

        // Gölgelendirme için temel tonu hesapla (yalnýzca parlaklýk üzerinde çalýþýr)
        float brightness = inputColor.grayscale; // Gri tonaj parlaklýk.

        // En yakýn eþik deðerine yuvarla
        float toonBrightness = 0f;
        foreach (var threshold in thresholds)
        {
            if (brightness <= threshold)
            {
                toonBrightness = threshold;
                break;
            }
        }

        // Toon rengi oluþtur ve geri döndür
        return new Color(toonBrightness * inputColor.r, toonBrightness * inputColor.g, toonBrightness * inputColor.b, inputColor.a);
    }

    //public Material material;
    //void Start()
    //{
    //    if (null == material ||
    //       null == material.shader || !material.shader.isSupported)
    //    {
    //        enabled = false;
    //        return;
    //    }
    //}

    //void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, destination, material);
    //}
}
