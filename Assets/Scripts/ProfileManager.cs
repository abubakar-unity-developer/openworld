using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance;
    [Header("Panels")]
    public GameObject createdProfilePanel;
    public GameObject countryPanel;
    public GameObject profilePanel;

    [Header("Country")]
    public Button openCountryPanelButton;
    public Button[] countryButtons;
    public Image mainCountryImage;
    public Image[] selectedCountryFlags;
    public Button countrySaveButton;

    [Header("Profile")]
    public Button openProfilePanelButton;
    public Button[] profileButtons;
    public Image mainProfileImage;
    public Image[] selectedProfileFlags;
    public Button profileSaveButton;

    [Header("Gender")]
    public Button maleButton;
    public Button femaleButton;
    public Image[] displayGenderImages;

    [Header("Name Input")]
    public InputField nameInputField;
    public Button saveProfileButton;
    public Text[] displayNameTexts;

    private GameObject selectedCountryObject = null;
    private GameObject selectedProfileObject = null;

    private string selectedGender = "";
    private bool isGenderSelected = false;
    private bool isNameEntered = false;


    private void Awake()
    {
        Instance = this;
    }
    public void StartFunctionForProfileManager()
    {
        openCountryPanelButton.onClick.AddListener(OpenCountryPanel);
        openProfilePanelButton.onClick.AddListener(OpenProfilePanel);

        SetupCountryButtons();
        SetupProfileButtons();

        countrySaveButton.onClick.AddListener(OnCountrySave);
        countrySaveButton.interactable = false;

        profileSaveButton.onClick.AddListener(OnProfileSave);
        profileSaveButton.interactable = false;

        maleButton.onClick.AddListener(() => OnGenderSelected("Male"));
        femaleButton.onClick.AddListener(() => OnGenderSelected("Female"));

        nameInputField.onValueChanged.AddListener(delegate { OnNameChanged(); });
        saveProfileButton.onClick.AddListener(OnProfileSaveFinal);
        saveProfileButton.interactable = false;

        LoadSavedCountryFlag();
        LoadSavedProfileFlag();
        LoadSavedNameAndGender();
    }

    // ===================== COUNTRY =========================

    void OpenCountryPanel()
    {
        createdProfilePanel.SetActive(false);
        countryPanel.SetActive(true);
    }

    void SetupCountryButtons()
    {
        foreach (Button btn in countryButtons)
        {
            Button capturedBtn = btn;
            capturedBtn.onClick.AddListener(() => OnCountrySelected(capturedBtn.gameObject));
        }
    }

    void OnCountrySelected(GameObject selected)
    {
        foreach (Button btn in countryButtons)
        {
            btn.transform.GetChild(0).gameObject.SetActive(false);
        }

        selected.transform.GetChild(0).gameObject.SetActive(true);
        selectedCountryObject = selected;

        Image flagImage = selected.GetComponent<Image>();
        if (flagImage != null)
        {
            foreach (Image img in selectedCountryFlags)
            {
                img.sprite = flagImage.sprite;
            }
        }

        countrySaveButton.interactable = true;
    }

    void OnCountrySave()
    {
        if (selectedCountryObject == null) return;

        Image flagImage = selectedCountryObject.GetComponent<Image>();
        if (flagImage != null)
        {
            PlayerPrefs.SetString("SelectedCountryFlag", flagImage.sprite.name);
            PlayerPrefs.Save();

            mainCountryImage.sprite = flagImage.sprite;
            foreach (Image img in selectedCountryFlags)
            {
                img.sprite = flagImage.sprite;
            }

            countryPanel.SetActive(false);
            createdProfilePanel.SetActive(true);
        }
    }

    void LoadSavedCountryFlag()
    {
        string savedFlagName = PlayerPrefs.GetString("SelectedCountryFlag", "");
        if (!string.IsNullOrEmpty(savedFlagName))
        {
            foreach (Button btn in countryButtons)
            {
                Image img = btn.GetComponent<Image>();
                if (img != null && img.sprite.name == savedFlagName)
                {
                    mainCountryImage.sprite = img.sprite;
                    foreach (Image display in selectedCountryFlags)
                    {
                        display.sprite = img.sprite;
                    }

                    btn.transform.GetChild(0).gameObject.SetActive(true);
                    selectedCountryObject = btn.gameObject;
                    break;
                }
            }
        }
    }

    // ===================== PROFILE =========================

    void OpenProfilePanel()
    {
        createdProfilePanel.SetActive(false);
        profilePanel.SetActive(true);
    }

    void SetupProfileButtons()
    {
        foreach (Button btn in profileButtons)
        {
            Button capturedBtn = btn;
            capturedBtn.onClick.AddListener(() => OnProfileSelected(capturedBtn.gameObject));
        }
    }

    void OnProfileSelected(GameObject selected)
    {
        foreach (Button btn in profileButtons)
        {
            btn.transform.GetChild(0).gameObject.SetActive(false);
        }

        selected.transform.GetChild(0).gameObject.SetActive(true);
        selectedProfileObject = selected;

        Image profileImage = selected.GetComponent<Image>();
        if (profileImage != null)
        {
            foreach (Image img in selectedProfileFlags)
            {
                img.sprite = profileImage.sprite;
            }
        }

        profileSaveButton.interactable = true;
    }

    void OnProfileSave()
    {
        if (selectedProfileObject == null) return;

        Image profileImage = selectedProfileObject.GetComponent<Image>();
        if (profileImage != null)
        {
            PlayerPrefs.SetString("SelectedProfileFlag", profileImage.sprite.name);
            PlayerPrefs.Save();

            mainProfileImage.sprite = profileImage.sprite;
            foreach (Image display in selectedProfileFlags)
            {
                display.sprite = profileImage.sprite;
            }

            profilePanel.SetActive(false);
            createdProfilePanel.SetActive(true);
        }
    }

    void LoadSavedProfileFlag()
    {
        string savedProfileName = PlayerPrefs.GetString("SelectedProfileFlag", "");
        if (!string.IsNullOrEmpty(savedProfileName))
        {
            foreach (Button btn in profileButtons)
            {
                Image img = btn.GetComponent<Image>();
                if (img != null && img.sprite.name == savedProfileName)
                {
                    mainProfileImage.sprite = img.sprite;
                    foreach (Image display in selectedProfileFlags)
                    {
                        display.sprite = img.sprite;
                    }

                    btn.transform.GetChild(0).gameObject.SetActive(true);
                    selectedProfileObject = btn.gameObject;
                    break;
                }
            }
        }
    }

    // ===================== GENDER =========================

    void OnGenderSelected(string gender)
    {
        selectedGender = gender;
        isGenderSelected = true;

        maleButton.transform.GetChild(0).gameObject.SetActive(gender == "Male");
        femaleButton.transform.GetChild(0).gameObject.SetActive(gender == "Female");

        Image selectedImage = gender == "Male" ? maleButton.GetComponent<Image>() : femaleButton.GetComponent<Image>();
        if (selectedImage != null)
        {
            foreach (Image img in displayGenderImages)
            {
                img.sprite = selectedImage.sprite;
            }

            PlayerPrefs.SetString("ProfileGenderSprite", selectedImage.sprite.name);
            PlayerPrefs.SetString("ProfileGender", gender); 
            PlayerPrefs.Save();
        }

        CheckIfSaveShouldBeEnabled();
    }

    // ===================== NAME =========================

    void OnNameChanged()
    {
        isNameEntered = !string.IsNullOrEmpty(nameInputField.text.Trim());
        CheckIfSaveShouldBeEnabled();
    }

    void CheckIfSaveShouldBeEnabled()
    {
        saveProfileButton.interactable = isNameEntered && isGenderSelected;
    }

    void OnProfileSaveFinal()
    {
        string playerName = nameInputField.text.Trim();

        PlayerPrefs.SetString("ProfileName", playerName);
        PlayerPrefs.Save();

        foreach (Text txt in displayNameTexts)
        {
            txt.text = playerName;
        }

        Debug.Log(" Profile Saved: " + playerName + " | Gender: " + selectedGender);

        StartCoroutine(OnSaveProfileFunction());
    }

    IEnumerator OnSaveProfileFunction()
    {
        MainMenuManager.Instance.loadingPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        MainMenuManager.Instance.mainMenu.SetActive(true);
        MainMenuManager.Instance.loadingPanel.SetActive(false);
        
    }

    void LoadSavedNameAndGender()
    {
        // Load name
        string savedName = PlayerPrefs.GetString("ProfileName", "");
        if (!string.IsNullOrEmpty(savedName))
        {
            nameInputField.text = savedName;
            foreach (Text txt in displayNameTexts)
            {
                txt.text = savedName;
            }
            isNameEntered = true;
        }

        // Load gender image
        string savedGenderSprite = PlayerPrefs.GetString("ProfileGenderSprite", "");
        string savedGender = PlayerPrefs.GetString("ProfileGender", "");

        if (!string.IsNullOrEmpty(savedGenderSprite))
        {
            Sprite maleSprite = maleButton.GetComponent<Image>().sprite;
            Sprite femaleSprite = femaleButton.GetComponent<Image>().sprite;

            if (maleSprite.name == savedGenderSprite)
            {
                selectedGender = "Male";
                isGenderSelected = true;
                maleButton.transform.GetChild(0).gameObject.SetActive(true);
                foreach (Image img in displayGenderImages)
                    img.sprite = maleSprite;
            }
            else if (femaleSprite.name == savedGenderSprite)
            {
                selectedGender = "Female";
                isGenderSelected = true;
                femaleButton.transform.GetChild(0).gameObject.SetActive(true);
                foreach (Image img in displayGenderImages)
                    img.sprite = femaleSprite;
            }
        }

        CheckIfSaveShouldBeEnabled();
    }
}
