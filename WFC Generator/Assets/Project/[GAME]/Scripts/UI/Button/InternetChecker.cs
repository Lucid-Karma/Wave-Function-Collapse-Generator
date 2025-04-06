using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InternetChecker : MonoBehaviour
{
    [SerializeField] private GameObject noInternetPanel; // Assign in inspector
    [SerializeField] private Button retryButton; // Optional: Retry button
    [SerializeField] private Button continueButton;

    private void Start()
    {
        noInternetPanel?.SetActive(false);
        retryButton?.onClick.AddListener(RetryConnection);
        continueButton?.onClick.AddListener(HideNoInternet);
        StartCoroutine(CheckConnectionCoroutine());
    }

    public void RetryConnection()
    {
        StartCoroutine(CheckConnectionCoroutine());
    }

    IEnumerator CheckConnectionCoroutine()
    {
        // First: Quick reachability check
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowNoInternet();
            yield break;
        }

        // Second: Try to access a known lightweight URL
        UnityWebRequest request = new UnityWebRequest("https://clients3.google.com/generate_204");
        request.method = UnityWebRequest.kHttpVerbHEAD;
        request.timeout = 5;

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        bool hasInternet = !(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError);
#else
        bool hasInternet = !(request.isNetworkError || request.isHttpError);
#endif

        if (hasInternet)
            HideNoInternet();
        else
            ShowNoInternet();
    }

    void ShowNoInternet()
    {
        noInternetPanel?.SetActive(true);
        Time.timeScale = 0f; // Optional: pause the game
    }

    void HideNoInternet()
    {
        noInternetPanel?.SetActive(false);
        Time.timeScale = 1f;
    }
}
