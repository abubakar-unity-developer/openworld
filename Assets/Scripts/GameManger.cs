using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class GameManger : MonoBehaviour
{
    public static GameManger Instance;

    public GameObject trafficObject;
    public GameObject border;
    public GameObject blackScreen;

    [Header("UI Buttons")]
    public Button Button1;
    public Button ExitBtn;
    public Button ExitBike;

    [Header("Controls / UI")]
    public GameObject RccControl;
    public GameObject BikeControl, BikeCamera;
    public GameObject PlaneControl;
    public GameObject ThirdPersonCntrol;
    public GameObject[] RCCControls;

    [Header("TestGame")]
    public bool testGame;
    public int levelIndex;
    public int playerIndex;

    [Header("LevelData")]
    public DataGame[] levels;
    public int currentLevel;

    [Header("PlayerData")]
    public int currentPlayer;
    public GameObject player;

    [Header("UI")]
    public GameObject startEngineBtn;
    public GameObject skipBtn;

    public void Awake()
    {
        Instance = this;
        currentLevel = testGame ? levelIndex : DataGame.selectedLevel;
        currentPlayer = testGame ? playerIndex : DataGame.selectedPlayer;
    }
    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        trafficObject.transform.SetParent(player.transform);
        trafficObject.transform.localPosition = Vector3.zero;
        trafficObject.transform.localRotation = Quaternion.identity;
    }
    void StartLevel()
    {
        player.transform.SetPositionAndRotation(levels[currentLevel].startPos.position,
            levels[currentLevel].startPos.rotation);
        player.SetActive(true);
      
        StartCoroutine(StartSceneFunction());
    }
    IEnumerator StartSceneFunction()
    {
        levels[currentLevel].startScene.SetActive(true);
        skipBtn.SetActive(true);
        border.SetActive(true);
        yield return new WaitForSeconds(levels[currentLevel].startSceneDuration - 2);
        levels[currentLevel].startScene.SetActive(false);
        skipBtn.SetActive(false);
        border.SetActive(false);
        blackScreen.SetActive(false);
        levels[currentLevel].instructionPanel.SetActive(true);
    }
    public void SkipBtn()
    {
        StopAllCoroutines();
        levels[currentLevel].startScene.SetActive(false);
        skipBtn.SetActive(false);
        border.SetActive(false);
        blackScreen.SetActive(false);
        levels[currentLevel].instructionPanel.SetActive(true);
    }
    public void OkBtn()
    {
        startEngineBtn.SetActive(true);
        levels[currentLevel].instructionPanel.SetActive(false);
    }
    public void StartEngineBtn()
    {
        GamePlayUI.Instance.soundEffect[0].Play();
        RccControl.SetActive(true);
        levels[currentLevel].trigger.SetActive(true);
        startEngineBtn.SetActive(false);
    }
    public void EndScene()
    {
        StartCoroutine(EndSceneFunction());
    }
    IEnumerator EndSceneFunction()
    {
        player.GetComponent<Rigidbody>().drag = 10;
        border.SetActive(true);
        levels[currentLevel].EndScene.SetActive(true);
        yield return new WaitForSeconds(Mathf.Max(0, levels[currentLevel].EndSceneDuration - 2));

        levels[currentLevel].EndScene.SetActive(false);
        border.SetActive(false);
        GamePlayUI.Instance.LevelComplete();
    }
}