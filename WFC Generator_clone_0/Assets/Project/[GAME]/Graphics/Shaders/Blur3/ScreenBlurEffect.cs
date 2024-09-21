using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenBlurEffect : MonoBehaviour
{
    public Material blurMaterial; // Assign the material with the custom shader
    [Range(0.001f, 0.01f)] public float blurSize = 0.005f; // Control the blur intensity

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Set the blur size in the shader
        blurMaterial.SetFloat("_BlurSize", blurSize);

        // Apply the blur shader
        Graphics.Blit(src, dest, blurMaterial);
    }

    private void OnEnable()
    {
        EventManager.OnLevelFinish.AddListener(() => StartCoroutine(UpdateLevelFinishBlurSize()));
        EventManager.OnLevelStart.AddListener(DecreaseBlurSize);
    }
    private void OnDisable()
    {
        EventManager.OnLevelFinish.RemoveListener(() => StartCoroutine(UpdateLevelFinishBlurSize()));
        EventManager.OnLevelStart.RemoveListener(DecreaseBlurSize);
    }

    private IEnumerator UpdateLevelFinishBlurSize()
    {
        yield return new WaitForSeconds(1f);

        blurSize = 0.01f;
    }

    private void DecreaseBlurSize()
    {
        blurSize = 0f;
    }
}
