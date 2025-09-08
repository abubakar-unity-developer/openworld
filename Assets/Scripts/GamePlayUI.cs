using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    public GameObject completePanel;
    public GameObject failedPanel;
    public GameObject pausePanel;
    public GameObject loadingPanel;

    public AudioSource[] soundEffect;

    public static GamePlayUI Instance;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StartFunctionForBtnSound();
    }
    private void Update()
    {
        UpdateFunctionForCheckingAndMusicDown();
    }

    // ================= SeatBelt =================

    private bool seatBelt = false;
    public GameObject seatBeltBtn;
    public void SeatBelt()
    {
        seatBelt = !seatBelt;
        AudioSource audioSource = soundEffect[1].transform.GetComponent<AudioSource>();
        if (seatBelt)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                seatBeltBtn.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else
        {
            audioSource.Play();
            seatBeltBtn.transform.GetChild(0).gameObject.SetActive(true);
        }

    }


    // ================= Home/Restart/NextBtn/Resume/PauseBtn =================
    public void HomeBtn() => StartCoroutine(LoadScene(1));
    public void RestartBtn() => StartCoroutine(LoadScene(0));

    public void NextLevelBtn()
    {
        if (GameManger.Instance.currentLevel < GameManger.Instance.levels.Length - 1)
        {
            DataGame.selectedLevel++;
        }
        StartCoroutine(LoadScene(2));
    }
    public void PauseBtn()
    {
        pausePanel.SetActive(true);
        AudioListener.volume = 0;
        Time.timeScale = 0;

    }
    public void ResumeBtn()
    {
        Time.timeScale = 1;
        AudioListener.volume = 1;
        pausePanel.SetActive(false);
    }

    // ================= GamePlayUIBtnSound =================

    [Header("BtnsSoundGamePlay")]
    public Button[] buttons;
    
    void StartFunctionForBtnSound()
    {
        foreach (Button btn in buttons)
            btn.onClick.AddListener(() => PlayButtonSound());
    }
    private void PlayButtonSound()
    {
        if (soundEffect[0] != null)
            soundEffect[0].Play();
    }

    // ================= ControlCHange =================
    public void ChangeControlls()
    {
        int totalModes = 4;
        int current = PlayerPrefs.GetInt("Controlls", 0);
        int newVal = (current + 1) % totalModes;
        PlayerPrefs.SetInt("Controlls", newVal);
        PlayerPrefs.Save();
        RCC.SetMobileController((RCC_Settings.MobileController)newVal);
        Debug.Log("Current Control Mode: " + ((RCC_Settings.MobileController)newVal).ToString());
    }
    public void ChangeControlls2(int m_Index)
    {
        RCC.SetMobileController((RCC_Settings.MobileController)m_Index);
    }



    // ================= SceneLoad =================
    private IEnumerator LoadScene(int index)
    {
        Time.timeScale = 1;
        AudioListener.volume = 1;
        loadingPanel.SetActive(true);
        var asyncLoad = SceneManager.LoadSceneAsync(index);
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSeconds(2f);
        asyncLoad.allowSceneActivation = true;
    }

    // ================= HornSystem =================
    [Header("Horn System")]
    public AudioClip[] hornClips;
    public AudioSource hornAudioSource;
    private int currentHornIndex = 0;

    public void HornBtn()
    {
        if (hornClips.Length == 0)
        {
            Debug.LogWarning("No horn clips assigned!");
            return;
        }
        if (hornAudioSource.isPlaying)
            hornAudioSource.Stop();

        hornAudioSource.clip = hornClips[currentHornIndex];
        hornAudioSource.Play();

        currentHornIndex++;
        if (currentHornIndex >= hornClips.Length)
            currentHornIndex = 0;
    }

    // ================= LevelCompletePrefs =================
    public void LevelComplete()
    {
        completePanel.SetActive(true);
        CompleteLevelPrefs("PlayedLevels_Mode1", GameManger.Instance.currentLevel);
    }

    private void CompleteLevelPrefs(string modeKey, int completedLevel)
    {
        int playedLevels = PlayerPrefs.GetInt(modeKey, 0);

        if (completedLevel >= playedLevels)
        {
            PlayerPrefs.SetInt(modeKey, completedLevel + 1);
            PlayerPrefs.Save();
        }
    }

    // ================= CheckingAndMusicDown =================
    [Header("CutScene CheckingAndMusicDown")]
    public AudioSource timelineAudioSource;

    public float reducedVolume = 0.3f;
    public float normalVolume = 1f;

    public float threshold = 0.01f;
    public float gracePeriod = 0.2f;

    public float fadeSpeed = 2f;

    private float[] samples = new float[256];
    private float silenceTimer = 0f;
    private bool shouldReduce = false;

    void UpdateFunctionForCheckingAndMusicDown()
    {
        if (timelineAudioSource == null || MusicSystemAdvanced.Ins.bgMuscAudioSource == null)
            return;

        timelineAudioSource.GetOutputData(samples, 0);
        float sum = 0f;

        for (int i = 0; i < samples.Length; i++)
            sum += samples[i] * samples[i];

        float rms = Mathf.Sqrt(sum / samples.Length);

        if (rms > threshold)
        {
            silenceTimer = 0f;
            shouldReduce = true;
        }
        else
        {
            silenceTimer += Time.deltaTime;

            if (silenceTimer > gracePeriod)
                shouldReduce = false;
        }

        float targetVolume = shouldReduce ? reducedVolume : normalVolume;
        MusicSystemAdvanced.Ins.bgMuscAudioSource.volume = Mathf.MoveTowards(MusicSystemAdvanced.Ins.bgMuscAudioSource.volume,
            targetVolume, fadeSpeed * Time.deltaTime);
    }

}
