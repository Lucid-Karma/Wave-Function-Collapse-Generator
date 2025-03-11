using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ThemeManager : Singleton<ThemeManager>
{
    public Material material; // Shader material
    public Material cityMaterial; // Shader material
    [SerializeField] private Vector4 Grass;
    [SerializeField] private Vector4 Road;
    [SerializeField] private Vector4 RoadBorder;
    private Vector4 roof1;
    private Vector4 roof2;

    [HideInInspector] public Color GrassColor;
    [HideInInspector] public Color RoadColor;
    private Color RoadBorderColor;
    [HideInInspector] public Color CameraBgColor;

    private int TextureAtlasSize = 2;
    private float NormalizedBlockTextureSize { get { return 1f / (float)TextureAtlasSize; } }

    private void OnEnable()
    {
        SetRegions();

        EventManager.OnLevelStart.AddListener(SetRegionColors);
        EventManager.OnLevelFinish.AddListener(ChangeMainColor);
        //LevelManager.OnLoopComplete.AddListener(() => HexColor = GenerateGhibliColor());
    }
    private void OnDisable()
    {
        EventManager.OnLevelStart.RemoveListener(SetRegionColors);
        EventManager.OnLevelFinish.RemoveListener(ChangeMainColor);
        //LevelManager.OnLoopComplete.RemoveListener(() => HexColor = GenerateGhibliColor());
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
    }

    private void SetRegionColors()
    {
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

        //print($"GrassColor {ColorToHex(GrassColor)}");
        //print($"RoadColor {ColorToHex(RoadColor)}");
        //print($"RoadBorderColor {ColorToHex(RoadBorderColor)}");
    }
    private void SetCityColors()
    {
        cityMaterial.SetVectorArray("_RegionCoords", new Vector4[] { roof1, roof2 });
        cityMaterial.SetColorArray("_RegionColors", new Color[] { GrassColor, RoadColor });
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

                if (colorScheme?.colors != null && colorScheme.colors.Length >= 3)
                {
                    GrassColor = GetColorFromString(colorScheme.colors[2].hex.value);
                    RoadColor = GetColorFromString(colorScheme.colors[0].hex.value);
                    RoadBorderColor = GetColorFromString(colorScheme.colors[1].hex.value);
                    //HexColor = colorScheme.colors[0].hex.value.Substring(1, 6);

                    CameraBgColor = GetColorFromString("#" + TransformHexColor(colorScheme.colors[0].hex.value));

                }
                else
                {
                    Debug.LogError("Color scheme data is incomplete or null.");
                }
            }
            else
            {
                Debug.LogError("API isteði baþarýsýz: " + request.error);
            }
        }
    }
    private void ChangeMainColor()
    {
        if (LevelManager.Instance.LevelIndex != 0)
        {
            print("regular level");
            HexColor = colorScheme.colors[0].hex.value.Substring(1, 6);
            StartCoroutine(GetColorScheme());
        }
        else    //LevelIndex >= LevelData.Levels.Count - 1
        {
            print("loop ended");
            GenerateGhibliColor();
            StartCoroutine(GetColorScheme());
        }
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
        int mononoke = random.Next(StudioGhibliPalette.ghibliPalette.Length);
        Debug.Log(mononoke);
        return StudioGhibliPalette.ghibliPalette[mononoke];
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
