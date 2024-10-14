using UnityEngine;

public class Audio : MonoBehaviour
{
    public static Audio Instance { get; private set; }
    public static Audio audioObject = null;

    private AudioSource background;
    [SerializeField] private AudioClip singlePlayerBackground;
    [SerializeField] private AudioClip multiplayerBackground;

    void Awake()
    {
        background = gameObject.GetComponent<AudioSource>();

        if( audioObject == null )
        {
            audioObject = this;
            DontDestroyOnLoad( this );
        }
        else if( this != audioObject )
        {
            Destroy( gameObject );
        }
    }

    void OnEnable()
    {
        EventManager.OnMusicOn.AddListener(PlayMusic);
        EventManager.OnMusicOff.AddListener(PauseMusic);

        LobbyManager.OnPlayersReady.AddListener(PlayMultiplayer);
        GameManager.OnMultiplayerGameFinish.AddListener(PlaySinglePlayer);
    }
    void OnDisable()
    {
        EventManager.OnMusicOn.RemoveListener(PlayMusic);
        EventManager.OnMusicOff.RemoveListener(PauseMusic);

        LobbyManager.OnPlayersReady.RemoveListener(PlayMultiplayer);
        GameManager.OnMultiplayerGameFinish.RemoveListener(PlaySinglePlayer);
    }

    public void PlayMusic()
    {
        background.Play();
    }
    public void PauseMusic()
    {
        background.Pause();
    }

    private void PlayMultiplayer()
    {
        background.clip = multiplayerBackground;

        if (MuteButton.IsMusicOn)
        {
            background.volume = 0.5f;
            background.Play();
        } 
    }

    private void PlaySinglePlayer()
    {
        background.clip = singlePlayerBackground;

        if (MuteButton.IsMusicOn)
        {
            background.volume = 1f;
            background.Play();
        } 
    }
}