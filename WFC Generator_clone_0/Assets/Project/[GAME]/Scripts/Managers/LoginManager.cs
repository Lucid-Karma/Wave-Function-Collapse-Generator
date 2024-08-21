using UnityEngine;
using UnityEngine.Events;

public class LoginManager : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnFirstLogin = new();
    public int LoginCount
    {
        get
        {
            return PlayerPrefs.GetInt("LoginCount", 0);
        }
        set
        {
            PlayerPrefs.SetInt("LoginCount", value);
        }
    }

    void Start()
    {
        if (LoginCount == 0)
        {
            LevelManager.Instance.LevelCount++;
            PlayerPrefs.SetInt("LevelCount", LevelManager.Instance.LevelCount);
            OnFirstLogin.Invoke();
        }

        IncreaseLoginCount();
        //Debug.Log("Current LoginCount: " + LoginCount);
    }

    public void IncreaseLoginCount()
    {
        LoginCount++;
        PlayerPrefs.SetInt("LoginCount", LoginCount);
    }

    public void ResetLoginCount()
    {
        PlayerPrefs.SetInt("LoginCount", 0);
    }

}
