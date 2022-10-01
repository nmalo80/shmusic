using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip FlamencoTune = null;
    [SerializeField] AudioClip ElectricTune = null;
    [SerializeField] AudioClip PercussiveTune = null;
    [SerializeField] AudioClip BossBattleTune = null;

    private float MainTuneTime = 0f;

    public static MusicPlayer Instance { get; private set; }

    public float GameSpeed = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayFlamencoTune(float startingPoint = -1)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = FlamencoTune;
        
        if (startingPoint == -1)
            audio.time = MainTuneTime;
        else
            audio.time = startingPoint;

        audio.Play();
    }

    public void PlayElectricTune()
    {
        AudioSource audio = GetComponent<AudioSource>();
        MainTuneTime = audio.time;
        audio.clip = ElectricTune;
        audio.time = 0f;
        audio.Play();
    }

    public void PlayDarbukaTune()
    {
        AudioSource audio = GetComponent<AudioSource>();
        MainTuneTime = audio.time;
        audio.clip = PercussiveTune;
        audio.time = 0f;
        audio.Play();
    }

    public void PlayBossBattleTune()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = BossBattleTune;
        audio.time = 0f;
        audio.Play();
    }

}
