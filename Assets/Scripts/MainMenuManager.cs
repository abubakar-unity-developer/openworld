using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    public bool peofileSetup;
    public GameObject[] profileDiplayOnOff;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StartFunctionForSwitchPanels();
        StartFunctionForURl();
        StartFUnctionForPtofile();
        Time.timeScale = 1;
        AudioListener.volume = 1f;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        LevelSystem(levelsBtns, "PlayedLevels");
    }

    private void Update()
    {
        /*UpdateFunctionForButtonSoundOnClick();*/
    }

    //=================ProfileSytemYesOrNot============================================================================================
    void StartFUnctionForPtofile()
    {
        if (peofileSetup)
        {
            foreach (GameObject profileDisplaySetup in profileDiplayOnOff)
            {
                profileDisplaySetup.SetActive(true);
            }
            ProfileManager.Instance.StartFunctionForProfileManager();
            ProfileManager.Instance.createdProfilePanel.SetActive(true);
        }
        else
        {
            foreach (GameObject profileDisplaySetup in profileDiplayOnOff)
            {
                profileDisplaySetup.SetActive(false);
            }
            mainMenu.SetActive(true);
        }
    }


    //=================SwitchPanels============================================================================================

    [Header("Panels")]
    public GameObject mainMenu;
    public GameObject modeSelection;
    public GameObject settingPnl;
    public GameObject exitPnl;
    public GameObject levelSelection;
    public GameObject loadingPanel;

    [Header("Buttons")]
    public Button playBtn;
    public Button settingBtn;
    public Button exitBtn;
    public Button settingSaveBtn;
    public Button exitNoBtn;
    public Button gameYesBtn;
    public Button modeBtn;
    public Button modeBackBtn;
    public Button levelSelectionBackBtn;

    void StartFunctionForSwitchPanels()
    {
        playBtn.onClick.AddListener(() => StartCoroutine(SwitchPanel(mainMenu, modeSelection, true)));
        settingBtn.onClick.AddListener(() => StartCoroutine(SwitchPanel(mainMenu, settingPnl, false)));
        exitBtn.onClick.AddListener(() => StartCoroutine(SwitchPanel(mainMenu, exitPnl, false)));
        settingSaveBtn.onClick.AddListener(() => StartCoroutine(SwitchPanel(settingPnl, mainMenu, false)));
        exitNoBtn.onClick.AddListener(() => StartCoroutine(SwitchPanel(exitPnl, mainMenu, false)));
        gameYesBtn.onClick.AddListener(() => Application.Quit());
        modeBtn.onClick.AddListener(() => StartCoroutine(SwitchPanel(modeSelection, levelSelection, true)));
        modeBackBtn.onClick.AddListener(() => StartCoroutine(SwitchPanel(modeSelection, mainMenu, false)));
        levelSelectionBackBtn.onClick.AddListener(() => StartCoroutine(SwitchPanel(levelSelection, modeSelection, false)));
    }

    IEnumerator SwitchPanel(GameObject from, GameObject to, bool useLoading)
    {
        from.SetActive(false);

        if (useLoading)
        {
            loadingPanel.SetActive(true);
            yield return new WaitForSeconds(2f);
            loadingPanel.SetActive(false);
        }

        to.SetActive(true);
    }




    //=================LevelSystem============================================================================================
    [Header("LevelsBtn")]
    public GameObject[] levelsBtns;

    public void LevelSelection(int levelIndex)
    {
        DataGame.selectedLevel = levelIndex;
        StartCoroutine(SceneLoad());
    }
    private void LevelSystem(GameObject[] levelButtons, string modeKey)
    {
        int playedLevels = PlayerPrefs.GetInt(modeKey, 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool isUnlocked = playedLevels >= i;
            Button button = levelButtons[i].GetComponent<Button>();

            button.interactable = isUnlocked;
            levelButtons[i].transform.GetChild(1).gameObject.SetActive(!isUnlocked);
        }
    }


    //=================SceneLoad============================================================================================

    IEnumerator SceneLoad()
    {
        loadingPanel.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);
        asyncLoad.allowSceneActivation = false;
        float waitTime = 2f;
        float timer = 0f;
        while (timer < waitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;
    }


    //=================ButtonSound============================================================================================
    [Header("BtnSound")]
    public GameObject Btnsound;
    /*void UpdateFunctionForButtonSoundOnClick()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => SoundOnClick());
        }
    }*/
   /* void SoundOnClick()
    {
        Btnsound.GetComponent<AudioSource>().Play();
    }*/



    //=================ControlChange============================================================================================
    /* public void ChangeControlls2(int m_Index)
     {
         if (m_Index == 0)
         {
             RCC.SetMobileController(RCC_Settings.MobileController.TouchScreen);
         }
         else if (m_Index == 1)
         {
             RCC.SetMobileController(RCC_Settings.MobileController.SteeringWheel);
         }
         else if (m_Index == 2)
         {
             RCC.SetMobileController(RCC_Settings.MobileController.SteeringWheel);
         }
     }*/


    //=================Links============================================================================================


    [Header("URL")]
    public Button privacyPolicyBtn;
    public string privacyPolicyURL;
    public Button moreGamesURLBtn;
    public string moreGamesURL;
    public Button mainMenuURLBtn;
    public string mainMenuURL;
    public Button modeSelectionURLBtn;
    public string modeSelectionURL;
    public Button rateUsURLBtn;
    public string rateUsURL;

    private void StartFunctionForURl()
    {
        privacyPolicyBtn.onClick.AddListener(() => OpenURL(privacyPolicyURL));
        moreGamesURLBtn.onClick.AddListener(() => OpenURL(moreGamesURL));
        mainMenuURLBtn.onClick.AddListener(() => OpenURL(mainMenuURL));
        modeSelectionURLBtn.onClick.AddListener(() => OpenURL(modeSelectionURL));
        rateUsURLBtn.onClick.AddListener(() => OpenURL(rateUsURL));
    }

    void OpenURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogWarning("URL is empty or null.");
        }
    }

}
