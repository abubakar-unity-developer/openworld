using UnityEngine;
using UnityEngine.UI;

public class MusicSystemAdvanced : MonoBehaviour
{
    public static MusicSystemAdvanced Ins;
    [Header("MainMusicBtn")]
    public Button mainMusicBtn;
    public GameObject mainMusicSetup;
    private bool isMusicSetupOn = false;


    [Header("Audio Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    public AudioClip[] playlist;

    [Header("Player Controls")]
    public Button playPauseButton;
    public Button nextButton;
    public Button prevButton;
    public Button forwardButton;
    public Button backwardButton;

    [Header("Display Elements")]
    public Slider volumeSlider;
    public Text timeDisplayText;
    public Text trackNameText;

    public AudioSource bgMuscAudioSource;
    private int currentTrackIndex = 0;
    private bool isPlaying = false;
    private bool isDragging = false;

    public void Awake()
    {
        Ins = this;
        bgMuscAudioSource = gameObject.AddComponent<AudioSource>();
        bgMuscAudioSource.playOnAwake = false;
        bgMuscAudioSource.volume = masterVolume;

        volumeSlider.value = masterVolume;

        SetupEventListeners();

        if (playlist.Length > 0)
        {
            PlayRandomTrack();
        }
    }

    private void Update()
    {
        if (isPlaying && !isDragging && bgMuscAudioSource.clip != null)
        {
            UpdateProgressUI();

            if (!bgMuscAudioSource.isPlaying && bgMuscAudioSource.time >= bgMuscAudioSource.clip.length - 0.1f)
            {
                NextTrack();
            }
        }
    }

    
    

    void MainMusicSetupToogle()
    {
        isMusicSetupOn = !isMusicSetupOn;
        mainMusicSetup.SetActive(isMusicSetupOn);
    }
    private void PlayRandomTrack()
    {
        currentTrackIndex = Random.Range(0, playlist.Length);
        isPlaying = true;
        LoadTrack();
    }

    private void SetupEventListeners()
    {
        mainMusicBtn.onClick.AddListener(MainMusicSetupToogle);
        playPauseButton.onClick.AddListener(TogglePlayPause);
        nextButton.onClick.AddListener(NextTrack);
        prevButton.onClick.AddListener(PreviousTrack);
        forwardButton.onClick.AddListener(() => Seek(20f));
        backwardButton.onClick.AddListener(() => Seek(-20f));

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

   

    private void TogglePlayPause()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
        {
            if (bgMuscAudioSource.clip == null) LoadTrack();
            if (bgMuscAudioSource.time > 0) bgMuscAudioSource.UnPause();
            else bgMuscAudioSource.Play();
            playPauseButton.transform.GetChild(0).gameObject.SetActive(true);
            playPauseButton.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            bgMuscAudioSource.Pause();
            playPauseButton.transform.GetChild(0).gameObject.SetActive(false);
            playPauseButton.transform.GetChild(1).gameObject.SetActive(true);
        }


    }

    private void LoadTrack()
    {
        if (playlist.Length == 0) return;
        bgMuscAudioSource.clip = playlist[currentTrackIndex];
        bgMuscAudioSource.Play();
        UpdateTrackName();
        UpdateProgressUI();
    }

    private void NextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % playlist.Length;
        LoadTrack();
    }

    private void PreviousTrack()
    {
        if (bgMuscAudioSource.time > 0.01f)
        {
            bgMuscAudioSource.time = 0;
            bgMuscAudioSource.Play();
        }
        else
        {
            currentTrackIndex = (currentTrackIndex - 1 + playlist.Length) % playlist.Length;
            LoadTrack();
        }
    }


    private void Seek(float seconds)
    {
        if (bgMuscAudioSource.clip == null) return;

        float newTime = bgMuscAudioSource.time + seconds;

        if (newTime >= bgMuscAudioSource.clip.length - 1f)
        {
            NextTrack();
            return;
        }

        bgMuscAudioSource.time = Mathf.Clamp(newTime, 0, bgMuscAudioSource.clip.length);
        UpdateProgressUI();
    }

    private void SetVolume(float volume)
    {
        masterVolume = volume;
        bgMuscAudioSource.volume = masterVolume;
    }

 
    private void UpdateProgressUI()
    {
        if (bgMuscAudioSource.clip == null) return;
        UpdateTimeDisplay();
    }

    private void UpdateTimeDisplay()
    {
        if (bgMuscAudioSource.clip == null) return;

        string totalTime = FormatTime(bgMuscAudioSource.clip.length);
        string remainingTime = FormatTime(bgMuscAudioSource.clip.length - bgMuscAudioSource.time);

        timeDisplayText.text = $" {totalTime}/{remainingTime}";
    }

    private void UpdateTrackName()
    {
        if (bgMuscAudioSource.clip != null)
        {
            trackNameText.text = bgMuscAudioSource.clip.name;
        }
        else
        {
            trackNameText.text = "No track loaded";
        }
    }
    private string FormatTime(float seconds)
    {
        int minutes = (int)(seconds / 60);
        int secs = (int)(seconds % 60);
        return $"{minutes}:{secs:00}";
    }
}
