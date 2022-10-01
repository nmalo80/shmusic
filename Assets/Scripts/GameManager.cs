using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] Text MsgText = null;

    [Header("Game Outcome")]
    [SerializeField] int Score = 0;

    [Header("Powerup Prefabs")]
    [SerializeField] GameObject MetalPickPrefab = null;
    [SerializeField] GameObject DarbukaPowerupPrefab = null;

    private float screenRightBound;
    private float screenLeftBound;

    // used by Player script to change background
    public GameObject standardBackGround;
    public GameObject electricBackGround;
    public GameObject catDancingAnimation;

    // Animation States
    public int CurrentInstrument;

    // Anim states
    private const int FLAMENCO_GUITAR = 0, ELECTRIC_GUITAR = 1, DARBUKA = 2;

    // Getters
    public int GetScore() { return Score; }
    public GameObject GetMetalPickPrefab() { return MetalPickPrefab; }
    public GameObject GetDarbukaPowerupPrefab() { return DarbukaPowerupPrefab; }

    public float GetScreenRightBound() { return screenRightBound; }
    public float GetScreenLeftBound() { return screenLeftBound; }

    public int GetFLAMENCO_GUITAR() { return FLAMENCO_GUITAR; }
    public int GetELECTRIC_GUITAR() { return ELECTRIC_GUITAR; }
    public int GetDARBUKA() { return DARBUKA; }

    private bool transformationsEnabled;

    public void SetTransformationsEnabled(bool value) { transformationsEnabled = value; }
    public bool GetTransformationsEnabled() { return transformationsEnabled; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        InitiateGameManager();
    }

    public void ResetGame()
    {
        Score = 0;
    }

    public void InitiateGameManager()
    {
        screenLeftBound = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        screenRightBound = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
    }

    public void IncreaseScore(int value)
    {
        Score += value;
    }

    

    public void ActivateElectricBG()
    {
        standardBackGround.SetActive(false);
        electricBackGround.SetActive(true);
        MsgText.text = "Keep the fire pressed";
    }

    public void ActivateStandardBG()
    {
        catDancingAnimation.SetActive(false);
        electricBackGround.SetActive(false);
        standardBackGround.SetActive(true);
        MsgText.text = "";
    }

    public void ActivateBongoBG()
    {
        catDancingAnimation.SetActive(true);
        standardBackGround.SetActive(false);
        MsgText.text = "Invincible!!";
    }
}
