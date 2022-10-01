using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    private Wave waveConfig;
    private int waypointIndex = 0;
    //private GameObject enemyPrefab;
    private List<Vector3> waypointPositions = new List<Vector3>();

    public Wave GetWaveConfig() { return waveConfig; }
    public void SetWaveConfig(Wave waveConfig) 
    { 
        this.waveConfig = waveConfig; 
    }

    // Start is called before the first frame update
    void Start()
    {
        if(waveConfig != null)
            waypointPositions = waveConfig.GetWayPoints();
    }

    // Update is called once per frame
    void Update()
    {
        MoveFollowingPath();
    }

    private void MoveFollowingPath()
    {
        if (waypointIndex <= waypointPositions.Count - 1)
        {
            var targetPosition = waypointPositions[waypointIndex];
            var movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);

            if (transform.position == targetPosition)
            {
                waypointIndex++;
                //print(targetPosition);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
