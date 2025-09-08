using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DataGame : MonoBehaviour
{
    public GameObject Player;
    public GameObject instructionPanel;
    public Transform startPos;
    public GameObject trigger;
    [Header("StartScene")]
    public GameObject startScene;
    public float startSceneDuration;
    [Header("EndScene")]
    public GameObject EndScene;
    public float EndSceneDuration;

    public static int selectedLevel;
    public static int selectedPlayer;
}
