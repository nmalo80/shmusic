using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<WaveGroup> WaveGroups = null;
    [SerializeField] GameObject FinalBoss = null;

    // bool that shows if the next wave can start
    private bool readyForNextWave;
    // bool that indicates if a wave group coroutine is processing
    private bool waveGroupCoroutineExecuting = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.standardBackGround = GameObject.Find("BG_WaterFall");
        GameManager.Instance.electricBackGround = GameObject.Find("CloudBG");
        if (GameManager.Instance.electricBackGround)
            GameManager.Instance.electricBackGround.SetActive(false);

        GameManager.Instance.catDancingAnimation = GameObject.Find("CatAnimationObject");
        if (GameManager.Instance.catDancingAnimation)
            GameManager.Instance.catDancingAnimation.SetActive(false);

        MusicPlayer.Instance.PlayFlamencoTune(0f);

        GameManager.Instance.SetTransformationsEnabled(true);

        StartCoroutine(StartWaveGroups());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator StartWaveGroups()
    {
        //List<WaveGroup> tempList = new List<WaveGroup>();
        //tempList.Add(WaveGroups[0]);

        foreach (var waveGroup in WaveGroups)
        //foreach (var waveGroup in tempList)
        {
            waveGroupCoroutineExecuting = true;
            StartCoroutine(LaunchWaves(waveGroup));

            // I have to initially wait a couple of seconds. If not, the second wave will start before any enemies has been created
            yield return new WaitForSeconds(2);
            // Then I'll wait for the previous wave to be cleared
            yield return new WaitUntil(() => IsScreenClear());
        }

        GameManager.Instance.CurrentInstrument = GameManager.Instance.GetFLAMENCO_GUITAR();
        Player.Instance.ResetTimer();
        GameManager.Instance.SetTransformationsEnabled(false);
        Player.Instance.TransitionToInstrument(GameManager.Instance.GetFLAMENCO_GUITAR());

        StartFinalBoss();
    }

    private IEnumerator LaunchWaves(WaveGroup waveGroup)
    {
        foreach (var wave in waveGroup.Waves)
        {
            wave.SetWaveOffset();
            
            StartCoroutine(SpawnAllEnemiesInWave(wave));

            yield return new WaitForSeconds(waveGroup.TimeBetweenWaves);   
        }
        waveGroupCoroutineExecuting = false;
    }

    private IEnumerator SpawnAllEnemiesInWave(Wave wave)
    {
        int index = 0;
        for (int i = 0; i < wave.GetNumberOfEnemies(); i++)
        {
            if(wave.RandomizeEnemies)
            {
                index = UnityEngine.Random.Range(0, wave.GetEnemyPrefabList().Count - 1);
            }
            
            // instantiate a variable of "type" Global Object, and assign a wave to its script
            var enemy = Instantiate(wave.GetEnemyPrefab(index), wave.GetWayPoints()[0], Quaternion.identity);

            if(wave.EnemyBulletSpeed != 0)
            {
                enemy.GetComponent<Enemy>().SetBulletSpeed(wave.EnemyBulletSpeed);
            }

            enemy.GetComponent<EnemyPathing>().SetWaveConfig(wave);
            //Debug.Log("Spawn Enemy: " + enemy.name);

            yield return new WaitForSeconds(wave.GetTimeBetweenSpawns());
        }
    }

    private void StartFinalBoss()
    {
        var enemy = Instantiate(FinalBoss, new Vector3(0,0,0) , Quaternion.identity);
        MusicPlayer.Instance.PlayBossBattleTune();
    }

    private bool IsScreenClear()
    {
        bool returnValue = (!waveGroupCoroutineExecuting && FindObjectOfType<Enemy>() == null);
        //print("condition is: " + returnValue);
        return(returnValue);
    }
}
    
