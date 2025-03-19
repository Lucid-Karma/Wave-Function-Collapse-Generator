using TMPro;
using UnityEngine;

public class ClaimTimerTextController : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    public TextMeshProUGUI TimerText
    {
        get
        {
            if (timerText == null)
                timerText = GetComponent<TextMeshProUGUI>();

            return timerText;
        }
    }

    private float maxTime = 60f;
    private float currentTime;
    private int minutes;
    private int seconds;

    [HideInInspector] public static bool isActive;
    [SerializeField] private GameObject parent;

    private void OnEnable()
    {
        EventManager.OnGameStart.AddListener(() => isActive = true);

        if(GameManager.Instance.IsGameStarted)
            isActive = true;
    }

    private void Start()
    {
        currentTime = PlayerPrefs.GetFloat("CurrentTime");
        if (currentTime <= 0)
        {
            currentTime = maxTime;
            DeactivateTimer();
        }
    }

    private void Update()
    {
        if (isActive)
        {
            currentTime -= Time.deltaTime;
            minutes = Mathf.FloorToInt(currentTime / 60);
            seconds = Mathf.FloorToInt(currentTime % 60);
            TimerText.text = string.Format("{0}:{1:00}", minutes, seconds);

            if (currentTime <= 0)
            {
                currentTime = maxTime;
                DeactivateTimer();
            }
        }
    }

    private void DeactivateTimer()
    {
        isActive = false;
        parent.SetActive(false);
    }

    private void OnDisable()
    {
        EventManager.OnGameStart.RemoveListener(() => isActive = true);
        PlayerPrefs.SetFloat("CurrentTime", currentTime);
    }

    //private void OnApplicationQuit()
    //{
    //    PlayerPrefs.SetFloat("CurrentTime", currentTime);
    //}
}
