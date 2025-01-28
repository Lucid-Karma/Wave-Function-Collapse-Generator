using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using NativeGalleryNamespace;

public class Screenshot : MonoBehaviour
{
    private string screenshotFolderName = "Civic Connect Screenshots";

    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private Canvas logoCanvas;
    [HideInInspector] public static UnityEvent OnScreenshotStart = new();
    [HideInInspector] public static UnityEvent OnScreenshotEnd = new();
    //private bool isSsP_Open;

    //private void OnEnable()
    //{
    //    PlayerInputController.OnPlayerTapInput.AddListener(CaptureAndSaveScreenshot);
    //    MenuPanels.OnSSPanel.AddListener(() => isSsP_Open = true);
    //    MenuCloseButton.OnMenuClose.AddListener(() => isSsP_Open = false);
    //}
    //private void OnDisable()
    //{
    //    PlayerInputController.OnPlayerTapInput.RemoveListener(CaptureAndSaveScreenshot);
    //    MenuPanels.OnSSPanel.RemoveListener(() => isSsP_Open = true);
    //    MenuCloseButton.OnMenuClose.RemoveListener(() => isSsP_Open = false);
    //}
    private void Start()
    {
        logoCanvas.enabled = false;
    }

    public void CaptureAndSaveScreenshot()
    {
        //if (isSsP_Open)
            StartCoroutine(CaptureScreenshotCoroutine());
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {
        OnScreenshotStart.Invoke();

        if (uiCanvas != null && logoCanvas != null)
        {
            uiCanvas.enabled = false;
            logoCanvas.enabled = true;
        }
            

        yield return new WaitForEndOfFrame();

        //string fileName = "screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        //string path = Path.Combine(Application.persistentDataPath, fileName);

        //ScreenCapture.CaptureScreenshot(path);

        string folderPath = Path.Combine(Application.persistentDataPath, screenshotFolderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = $"Screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string filePath = Path.Combine(folderPath, fileName);

        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        #if UNITY_ANDROID
                NativeGallery.SaveImageToGallery(filePath, screenshotFolderName, fileName);
        #endif

        Destroy(screenshot);

        Debug.Log($"Screenshot saved to: {filePath}");

        if (uiCanvas != null && logoCanvas != null)
        {
            uiCanvas.enabled = true;
            logoCanvas.enabled = false; 
        }
        EventManager.OnButtonClick.Invoke();
        //isSsP_Open = false;
        OnScreenshotEnd.Invoke();
    }
}
