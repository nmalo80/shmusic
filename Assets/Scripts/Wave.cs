using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Enemy Wave")]
public class Wave : ScriptableObject // ScriptableObject creates a reusable class
{
    [SerializeField] List<GameObject> EnemyPrefab = null;
    public bool RandomizeEnemies = false;
    [SerializeField] GameObject PathPrefab = null;
    [SerializeField] float TimeBetweenSpawns = 0.5f;
    [SerializeField] float SpawnRandomFactor = 0.3f;
    [SerializeField] int NumberOfEnemies = 5;
    [SerializeField] float MoveSpeed = 2f;
    
    // if 0, the default enemy bullet speed is used
    public float EnemyBulletSpeed;

    public float WaveOffsetXMax = 2f;

    [SerializeField] float waveOffsetX = 0f;

    [SerializeField] float waveOffsetY = 0f;

    public List<GameObject> GetEnemyPrefabList() {return EnemyPrefab;}

    public GameObject GetEnemyPrefab(int i) { return EnemyPrefab[i]; }

    public GameObject GetPathPrefab() { return PathPrefab; }

    public List<Vector3> GetWayPoints() 
    {
        var waveWaypointPosition = new List<Vector3>();

        foreach (Transform t in PathPrefab.transform)
        {
            waveWaypointPosition.Add(t.position + new Vector3(waveOffsetX, waveOffsetY, 0));
        }
        
        return waveWaypointPosition;
    }

    public float GetTimeBetweenSpawns() { return TimeBetweenSpawns;  }
    public float GetSpawnRandomFactor() { return SpawnRandomFactor; }
    public int GetNumberOfEnemies() { return NumberOfEnemies; }
    public float GetMoveSpeed() { return MoveSpeed; }

    public void SetWaveOffset() 
    {
        if(WaveOffsetXMax != 0)
            waveOffsetX = Random.Range(0f, WaveOffsetXMax); 
    }
    public float GetWaveOffset() { return waveOffsetX; }


}