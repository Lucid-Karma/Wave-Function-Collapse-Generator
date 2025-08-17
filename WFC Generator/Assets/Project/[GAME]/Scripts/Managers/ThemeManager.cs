using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ThemeManager : Singleton<ThemeManager>
{
    public Material material; // Shader material
    public Material cityMaterial; // Shader material
    [SerializeField] private Vector4 Grass;
    [SerializeField] private Vector4 Road;
    [SerializeField] private Vector4 RoadBorder;
    private Vector4 roof1, roof2;
    private Vector4 trees;

    [HideInInspector] public Color GrassColor;
    [HideInInspector] public Color RoadColor;
    private Color RoadBorderColor;
    [HideInInspector] public Color CameraBgColor;
    private Color RoofColor;

    private int TextureAtlasSize = 2;
    private float NormalizedBlockTextureSize { get { return 1f / (float)TextureAtlasSize; } }

    private void OnEnable()
    {
        SetRegions();

        EventManager.OnLevelStart.AddListener(SetRegionColors);
        EventManager.OnLevelFinish.AddListener(ChangeMainColor);
        LevelManager.OnLoopComplete.AddListener(() => HexColor = GenerateGhibliColor());
        WfcGenerator.OnMapPreReady.AddListener(CheckDaylight);
    }
    private void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(SetRegionColors);
        EventManager.OnLevelFinish.RemoveListener(ChangeMainColor);
        LevelManager.OnLoopComplete.RemoveListener(() => HexColor = GenerateGhibliColor());
        WfcGenerator.OnMapPreReady.RemoveListener(CheckDaylight);
    }
    private void CheckDaylight()
    {
        IsColorDark(colorScheme.colors[2].hex.value);
    }

    private void Start()
    {
        StartCoroutine(GetColorScheme());
    }

    private void SetRegions()
    {
        TextureAtlasSize = 2;
        Grass = CalculateSquareRegion(2);
        Road = CalculateSquareRegion(0);
        RoadBorder = CalculateSquareRegion(3);

        TextureAtlasSize = 8;
        roof1 = CalculateSquareRegion(18);
        roof2 = CalculateSquareRegion(37);
        trees = CalculateSquareRegion(14);
    }

    private void SetRegionColors()
    {
        directionalLight.intensity = 1;
        SetTileColors();
        SetCityColors();
    }
    private void SetTileColors()
    {
        material.SetVectorArray("_RegionCoords", new Vector4[] {
            Grass, // Coordinations
            Road,
            RoadBorder,
            //new Vector4(0.7f, 0.7f, 0.8f, 0.8f)
            CalculateSquareRegion(1)
        });
        material.SetColorArray("_RegionColors", new Color[] {
            GrassColor, RoadColor, RoadBorderColor, Color.white // Colors
        });
    }
    private void SetCityColors()
    {
        cityMaterial.SetVectorArray("_RegionCoords", new Vector4[] { roof1, roof2, trees });
        cityMaterial.SetColorArray("_RegionColors", new Color[] { RoofColor, RoofColor, RoadColor });
    }

    private Vector4 CalculateSquareRegion(int textureId)
    {
        float y = textureId / TextureAtlasSize;
        float x = textureId - (y * TextureAtlasSize);

        x *= NormalizedBlockTextureSize;
        y *= NormalizedBlockTextureSize;

        y = 1f - y - NormalizedBlockTextureSize;

        return new Vector4(x, y, x + NormalizedBlockTextureSize, y + NormalizedBlockTextureSize);
    }

    public string HexColor
    {
        get
        {
            //return (PlayerPrefs.GetString("HexColor") == null) ? hexColor : PlayerPrefs.GetString("HexColor");
            if(PlayerPrefs.GetString("HexColor") == "")
            {
                return hexColor;
            }
            else
            {
                return PlayerPrefs.GetString("HexColor");
            }
        }
        set
        {
            PlayerPrefs.SetString("HexColor", value);
        }
    }

    private string hexColor = "3197E6";
    private string apiUrl => $"https://www.thecolorapi.com/scheme?hex={HexColor}&mode=triad&count=5";
    ColorScheme colorScheme;

    IEnumerator GetColorScheme()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                //Debug.Log(HexColor);
                colorScheme = JsonUtility.FromJson<ColorScheme>(jsonResponse);

                if (colorScheme?.colors != null && colorScheme.colors.Length >= 4)
                {
                    GrassColor = GetColorFromString(colorScheme.colors[2].hex.value);
                    RoadColor = GetColorFromString(colorScheme.colors[0].hex.value);
                    RoadBorderColor = GetColorFromString(colorScheme.colors[1].hex.value);
                    RoofColor = GetColorFromString(colorScheme.colors[3].hex.value);
                    //HexColor = colorScheme.colors[0].hex.value.Substring(1, 6);

                    CameraBgColor = GetColorFromString("#" + TransformHexColor(colorScheme.colors[0].hex.value));

                }
                else
                {
                    Debug.LogError("Color scheme data is incomplete or null.");
                    FallbackToDefaultColor();
                }
            }
            else
            {
                Debug.LogError("API isteði baþarýsýz: " + request.error);
                //InternetChecker.Instance.ShowNoInternet(noInternetPanel);
            }
        }
    }
    private void ChangeMainColor()
    {
        if (LevelManager.Instance.LevelIndex != 0)
        {
            HexColor = colorScheme.colors[0].hex.value.Substring(1, 6);
            StartCoroutine(GetColorScheme());
        }
        else    
        {
            StartCoroutine(GetColorScheme());
        }
        //print("main color " + HexColor);
    }

    private int HexToDec(string hex)
    {
        int dec = Convert.ToInt32(hex, 16);
        return dec;
    }
    private float HexToFloatNormalized(string hex)
    {
        return HexToDec(hex) / 255f;
    }

    private Color GetColorFromString(string hexString)
    {
        float r = HexToFloatNormalized(hexString.Substring(1, 2));
        float g = HexToFloatNormalized(hexString.Substring(3, 2));
        float b = HexToFloatNormalized(hexString.Substring(5, 2));
        //Debug.Log($"R: {r}, G: {g}, B: {b}");
        return new Color(r, g, b);
    }

    #region new theme
    private System.Random random = new System.Random();
    public string GenerateRandomColor()
    {
        int red = random.Next(0, 256);   
        int green = random.Next(0, 256); 
        int blue = random.Next(0, 256);  
        
        return $"{red:X2}{green:X2}{blue:X2}";
    }

    private string GenerateGhibliColor()
    {
        int division = LevelManager.Instance.LevelCount / 6;    // loop index
        if (division < 18) return StudioGhibliPalette.ghibliPalette[division - 1];
        int remainder = division % 18;
        int result = (remainder == 0)? 18: remainder;
        return StudioGhibliPalette.ghibliPalette[result - 1];
    }
    #endregion

    public string GetRandomPastelColorHex()
    {
        // Generate a random pastel color
        float hue = UnityEngine.Random.Range(0f, 1f);
        float saturation = UnityEngine.Random.Range(0.4f, 0.7f);
        float value = UnityEngine.Random.Range(0.8f, 1f);

        // Convert HSV to RGB
        Color randomColor = Color.HSVToRGB(hue, saturation, value);

        // Convert RGB to Hex
        return ColorToHex(randomColor);
    }
    private  string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255);
        int g = Mathf.RoundToInt(color.g * 255);
        int b = Mathf.RoundToInt(color.b * 255);
        return $"{r:X2}{g:X2}{b:X2}";
    }

    #region Camera
    public static string TransformHexColor(string inputHex)
    {
        // Parse the input hex color to a Color object
        if (!ColorUtility.TryParseHtmlString(inputHex, out Color color))
        {
            Debug.LogError("Invalid hex color input!");
            return inputHex;
        }

        // Convert the color to HSV
        Color.RGBToHSV(color, out float h, out float s, out float v);

        // Transform HSV components
        h = (h + 0.15f) % 1.0f; // Shift hue
        s = Mathf.Clamp01(s * 0.8f); // Adjust saturation
        v = Mathf.Clamp01(v * 1.3f); // Increase brightness

        // Convert back to Color and then to hex
        Color transformedColor = Color.HSVToRGB(h, s, v);
        return ColorUtility.ToHtmlStringRGB(transformedColor);
    }
    #endregion

    #region ColorUtils
    [HideInInspector] public static UnityEvent OnNight = new();
    public Light directionalLight;
    public void IsColorDark(string hex)
    {
        if (!ColorUtility.TryParseHtmlString(hex, out Color color))
            return;

        // RGB bileþenlerini al ve 0-255 arasýna dönüþtür
        Color32 color32 = color;
        int brightness = (int)(0.2126f * color32.r + 0.7152f * color32.g + 0.0722f * color32.b);

        if (brightness < 48) //128, 64
        {
            directionalLight.intensity = 0.5f;
            //print("Night");
            OnNight.Invoke();
        }  
    }

    // public only for button action. Do not use outside
    public void FallbackToDefaultColor()
    {
        Color fallback = GetColorFromString("#D4248A");
        RoadColor = fallback;
        RoadBorderColor = GetColorFromString("#8ADE26");
        GrassColor = GetColorFromString("#2E94E3");
        RoofColor = GetColorFromString("#369AE7");
        CameraBgColor = fallback;
    }
    #endregion

    #region Internet Checker
    [SerializeField] private GameObject noInternetPanel;

    // public only for button action. Do not use outside
    public void TryChangeMainColor()
    {
        if (LevelManager.Instance.LevelIndex != 0)
        {
            HexColor = colorScheme.colors[0].hex.value.Substring(1, 6);
            StartCoroutine(TryGetColorScheme());
        }
        else
        {
            StartCoroutine(TryGetColorScheme());
        }
    }
    IEnumerator TryGetColorScheme()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                colorScheme = JsonUtility.FromJson<ColorScheme>(jsonResponse);

                if (colorScheme?.colors != null && colorScheme.colors.Length >= 4)
                {
                    GrassColor = GetColorFromString(colorScheme.colors[2].hex.value);
                    RoadColor = GetColorFromString(colorScheme.colors[0].hex.value);
                    RoadBorderColor = GetColorFromString(colorScheme.colors[1].hex.value);
                    RoofColor = GetColorFromString(colorScheme.colors[3].hex.value);

                    CameraBgColor = GetColorFromString("#" + TransformHexColor(colorScheme.colors[0].hex.value));
                    InternetChecker.Instance.HideNoInternet(noInternetPanel);
                }
            }
        }
    }
    #endregion
}

[System.Serializable]
public class ColorScheme
{
    public ColorInfo[] colors;
}
[System.Serializable]
public class ColorInfo
{
    public Hex hex;
}

[System.Serializable]
public class Hex
{
    public string value;
}
